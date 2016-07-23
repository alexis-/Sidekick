using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mnemophile.Interfaces.DB;
using Mnemophile.Utils.LazyLoad;

namespace Mnemophile.Utils.Collections
{
  /// <summary>
  ///     Base collection class for handling card async lazy loading, caching
  ///     and sorting.
  /// </summary>
  public abstract class AsyncDbListBase<T> where T : class
  {
    protected enum ReviewStatus
    {
      New,
      MoveNext,
      MoveNextEndOfStore,
      Complete,
    }

    protected ReviewStatus Status { get; set; }
    protected object LockObject { get; }
    protected TaskCompletionSource<object> LoadCompletionSource { get; set; }
    protected TaskCompletionSource<object> 
      FurtherLoadCompletionSource { get; set; }


    protected List<T> Objects { get; private set; }

    private T _current;
    public T Current
    {
      get { return _current; }
      private set { _current = OnCurrentChange(_current, value); }
    }

    private int _index;
    public int Index
    {
      get { return _index; }
      private set { _index = OnIndexChange(_index, value); }
    }

    protected IDatabase Db { get; }
    protected DbLazyLoad<T> LazyLoader { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="AsyncDbListBase{T}"/>
    ///     class.
    /// </summary>
    /// <param name="db">Database</param>
    /// <param name="lazyLoad">Whether to use lazy loading</param>
    protected AsyncDbListBase(IDatabase db, bool lazyLoad = false)
    {
      Status = ReviewStatus.New;
      LockObject = new object();
      LoadCompletionSource = null;

      Db = db;

      if (lazyLoad)
        LazyLoader = DbLazyLoad<T>.GetOrCreateInstance(db);

      Index = -1;
      Objects = null;
      Current = null;
    }

    /// <summary>
    ///     Fetch next item if available.
    ///     Load more items asynchronously if needed.
    /// </summary>
    /// <returns>Whether any item is available</returns>
    public Task<bool> MoveNext()
    {
      if (!CheckState())
        return TaskConstants.BooleanFalse;

      Task<bool> ret;

      lock (LockObject)
      {
        //
        // Items are available and index load threshold is undefined, or has
        // not been reached yet.
        if (Index < Objects.Count - 1
          && (GetMaxIndexLoadThreshold() < 0
              || (Index < GetMaxIndexLoadThreshold()
                  && Status != ReviewStatus.MoveNextEndOfStore)
              ))
        {
          // If new item is not yet fully loaded, return a Task to be awaited
          if (LazyLoader != null && GetFurtherLoadedIndex() <= Index)
          {
            CallFurtherLoad();

            ret = AsyncMoveNextFurtherLoad(FurtherLoadCompletionSource.Task);
          }

          // Everything's fine, move on to next object
          else
          {
            Current = Objects[++Index];

            return TaskConstants.BooleanTrue;
          }
        }

        //
        // No more items, have we reached end of item store ?
        else if (Status == ReviewStatus.MoveNextEndOfStore)
        {
          Complete();

          return TaskConstants.BooleanFalse;
        }

        //
        // No more loaded items, or item load threshold reached.
        // Status not marked as end of store. Although this does not guarantee
        // more items are available, try to fetch them.
        else
        {
          CallLoadMore();

          ret = AsyncMoveNextLoadItems(LoadCompletionSource.Task);
        }
      }

      // We need not retain old item's reference anymore
      Current = null;

      return ret;
    }

    /// <summary>
    ///     Called if asynchronous further item loading is needed.
    ///     Wait for async load task to finish.
    /// </summary>
    /// <param name="loadCompletionTask">The load task</param>
    /// <returns>Always true</returns>
    private async Task<bool> AsyncMoveNextFurtherLoad(Task loadCompletionTask)
    {
      await loadCompletionTask;

      lock (LockObject)
        Current = Objects[++Index];

      return true;
    }

    /// <summary>
    ///     Called if asynchronous item loading is needed.
    ///     Wait for async load task to finish.
    /// </summary>
    /// <param name="loadCompletionTask">The load task</param>
    /// <returns>Whether any item is available</returns>
    private async Task<bool> AsyncMoveNextLoadItems(Task loadCompletionTask)
    {
      await loadCompletionTask;

      lock (LockObject)
      {
        // If no item available, set complete state
        if (Index == Objects.Count - 1)
        {
          Complete();

          return false;
        }
        
        // If new item is not yet fully loaded, await Task
        if (LazyLoader != null && GetFurtherLoadedIndex() < Index)
        {
          CallFurtherLoad();

          loadCompletionTask = FurtherLoadCompletionSource.Task;
        }

        else
        {
          loadCompletionTask = null;

          Current = Objects[++Index];
        }
      }

      // Await item further load task if necessary
      if (loadCompletionTask != null)
      {
        await loadCompletionTask;

        lock (LockObject)
          Current = Objects[++Index];
      }

      return true;
    }

    /// <summary>
    ///     Initiate asynchronous item loading if necessary
    /// </summary>
    private void CallLoadMore()
    {
      if (LoadCompletionSource == null)
      {
        LoadCompletionSource = new TaskCompletionSource<object>();

        LoadMore();
      }
    }

    /// <summary>
    ///     Initiate asynchronous further item loading if necessary.
    ///     Only required when using lazy loading.
    /// </summary>
    private void CallFurtherLoad()
    {
      if (FurtherLoadCompletionSource == null)
      {
        FurtherLoadCompletionSource = new TaskCompletionSource<object>();

        FurtherLoad();
      }
    }

    protected virtual async Task LoadMore()
    {
      bool fullLoad = Index == Objects.Count - 1;
      bool ret = await Task.Run(() => DoLoadMore(fullLoad));

      lock (LockObject)
      {
        if (ret)
          Sort();

        LoadCompletionSource.SetResult(null);
        LoadCompletionSource = null;
      }
    }

    protected virtual async Task FurtherLoad()
    {
      await Task.Run(() => DoFurtherLoad());

      lock (LockObject)
      {
        FurtherLoadCompletionSource.SetResult(null);
        FurtherLoadCompletionSource = null;
      }
    }

    protected bool CheckState(bool allowNew = true)
    {
      //
      // Check for 'New' or 'Complete' status
      switch (Status)
      {
        // Set-up object list, index, and move to 'MoveNext' status
        case ReviewStatus.New:
          if (!allowNew)
            throw new InvalidOperationException("Invalid state: New");

          Initialize();
          break;

        // End of list reached, no further MoveNext call are necessary.
        case ReviewStatus.Complete:
          return false;
      }

      return true;
    }

    protected void Initialize(bool load = false)
    {
      Objects = new List<T>();
      Index = -1;

      Status = ReviewStatus.MoveNext;

      if (load)
        CallLoadMore();
    }

    protected void Complete()
    {
      Status = ReviewStatus.Complete;
      Current = null;
    }

    protected virtual T OnCurrentChange(T oldValue, T newValue)
    {
      if (LazyLoader != null && oldValue != null)
        LazyLoader.LazyUnload(oldValue);

      return newValue;
    }

    protected virtual int OnIndexChange(int oldIndex, int newIndex)
    {
      lock (LockObject)
      {
        if (GetNextLoadThreshold() >= 0 && newIndex >= GetNextLoadThreshold())
          CallLoadMore();

        if (GetNextFurtherLoadThreshold() >= 0
            && newIndex >= GetNextFurtherLoadThreshold())
          CallFurtherLoad();
      }

      return newIndex;
    }


    //
    // Abstract methods

    /// <summary>
    ///     Gets the maximum reachable index until awaiting item load to
    ///     complete. If <see cref="ReviewStatus.MoveNextEndOfStore "/> is
    ///     set, this is ignored.
    /// </summary>
    /// <returns>Maximum reachable index</returns>
    protected abstract int GetMaxIndexLoadThreshold();
    /// <summary>
    ///     Index threshold used in determining when asynchronous item
    ///     loading should be initiated.
    /// </summary>
    /// <returns>Next load index threshold</returns>
    protected abstract int GetNextLoadThreshold();
    /// <summary>
    ///     Index threshold used in determining when asynchronous further item
    ///     loading should be initiated.
    ///     Only required when using lazy loading.
    /// </summary>
    /// <returns>Next load index threshold</returns>
    protected abstract int GetNextFurtherLoadThreshold();
    /// <summary>
    ///     Gets the index of last fully loaded item.
    ///     Only required when using lazy loading.
    /// </summary>
    /// <returns>Index of last fully loaded item</returns>
    protected abstract int GetFurtherLoadedIndex();

    /// <summary>
    ///     Does the further load.
    /// </summary>
    /// <returns></returns>
    protected abstract void DoFurtherLoad();
    /// <summary>
    ///     Does the load more.
    /// </summary>
    /// <param name="fullLoad">if set to <c>true</c> [full load].</param>
    /// <returns></returns>
    protected abstract bool DoLoadMore(bool fullLoad);

    /// <summary>
    ///     Sort cards upon adding new cards.
    /// </summary>
    protected abstract void Sort();
  }
}