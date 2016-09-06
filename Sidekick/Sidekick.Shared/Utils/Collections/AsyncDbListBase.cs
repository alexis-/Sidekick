// 
// The MIT License (MIT)
// 
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the 
// Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

namespace Sidekick.Shared.Utils.Collections
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;

  using Catel.Logging;

  using Sidekick.Shared.Interfaces.Database;
  using Sidekick.Shared.Utils.LazyLoad;

  /// <summary>
  ///   Base collection class for handling card async lazy loading, caching
  ///   and sorting.
  /// </summary>
  /// <typeparam name="T">Collection type</typeparam>
  public abstract class AsyncDbListBase<T>
    where T : class
  {
    #region Fields

    private T _current;
    private int _index;

    #endregion



    #region Constructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="AsyncDbListBase{T}" /> class.
    /// </summary>
    /// <param name="db">Database instance</param>
    /// <param name="lazyLoad">Whether to use lazy loading</param>
    protected AsyncDbListBase(IDatabase db, bool lazyLoad = false)
    {
      Status = ReviewStatus.New;
      LockObject = new object();
      LoadCompletionSource = null;

      Log = LogManager.GetCurrentClassLogger();

      Db = db;

      if (lazyLoad)
        LazyLoader = DbLazyLoad<T>.GetOrCreateInstance(db);

      _index = -1;
      _current = null;
      Objects = null;
    }

    #endregion



    #region Enums

    /// <summary>
    ///   Enumeration status
    /// </summary>
    public enum ReviewStatus
    {
      /// <summary>
      ///   Uninitialized AsyncDbList, Objects and Current is null, and Index is -1.
      /// </summary>
      New,

      /// <summary>
      ///   Initialized AsyncDbList, Current may or may not be null, and Index is >= -1.
      /// </summary>
      MoveNext,

      /// <summary>
      ///   Initialized AsyncDbList, Current may or may not be null, Index is >= -1, and end of
      ///   available item store is reached (i.e. no more item can be loaded).
      /// </summary>
      MoveNextEndOfStore,

      /// <summary>
      ///   AsyncDbList reached end of collection, Current is null and Index equals collection
      ///   size.
      /// </summary>
      Complete,
    }

    #endregion



    #region Properties

    /// <summary>
    ///   Current iterated object.
    /// </summary>
    public T Current
    {
      get { return _current; }
      protected set { _current = OnCurrentChange(_current, value); }
    }

    /// <summary>
    ///   Iteration index.
    /// </summary>
    public int Index
    {
      get { return _index; }
      protected set { _index = OnIndexChange(_index, value); }
    }

    /// <summary>
    ///   Current state of AsyncDbList, i.e. whether new, iterating, end of store or completed.
    /// </summary>
    public ReviewStatus Status { get; set; }

    /// <summary>
    ///   Backing-property collection for loaded objects.
    /// </summary>
    protected List<T> Objects { get; private set; }

    /// <summary>
    ///   Provides synchronization context for <see cref="Objects" />,
    ///   <see cref="LoadCompletionSource" /> and <see cref="FurtherLoadCompletionSource" />
    ///   accesses.
    /// </summary>
    protected object LockObject { get; }

    /// <summary>
    ///   Completion source used in scheduling asynchronous loading and pre-loading.
    /// </summary>
    protected TaskCompletionSource<object> LoadCompletionSource { get; set; }

    /// <summary>
    ///   Completion source used in scheduling asynchronous further (opposed to lazy) loading and
    ///   pre-loading.
    /// </summary>
    protected TaskCompletionSource<object> FurtherLoadCompletionSource { get; set; }

    /// <summary>
    ///   Database instance used for all loading operations.
    /// </summary>
    protected IDatabase Db { get; }

    /// <summary>
    ///   Lazy loading provider (optional).
    /// </summary>
    protected DbLazyLoad<T> LazyLoader { get; }

    /// <summary>
    ///   Logger instance.
    /// </summary>
    protected ILog Log { get; }

    #endregion



    #region Methods

    /// <summary>
    ///   Manually add an item.
    /// </summary>
    /// <param name="item">Object to add</param>
    /// <param name="sort">
    ///   Whether to sort list following item addition.
    /// </param>
    public virtual void AddManual(T item, bool sort = true)
    {
      lock (LockObject)
      {
        Objects.Add(item);

        if (sort)
          Sort();
      }
    }

    /// <summary>
    ///   Manually add items.
    /// </summary>
    /// <param name="items">Objects to add</param>
    /// <param name="sort">
    ///   Whether to sort list following items addition.
    /// </param>
    public virtual void AddManual(IEnumerable<T> items, bool sort = true)
    {
      lock (LockObject)
      {
        Objects.AddRange(items);

        if (sort)
          Sort();
      }
    }

    /// <summary>
    ///   Fetch next item if available.
    ///   Load more items asynchronously if needed.
    /// </summary>
    /// <returns>Whether any item is available</returns>
    public virtual Task<bool> MoveNextAsync()
    {
      if (!CheckState())
        return TaskConstants.BooleanFalse;

      Task<bool> ret;

      lock (LockObject)
      {
        // Items are available and index load threshold is undefined, or has
        // not been reached yet.
        if (Index < Objects.Count - 1
            && (GetMaxIndexLoadThreshold() < 0 || Index < GetMaxIndexLoadThreshold()
                || (Index >= GetMaxIndexLoadThreshold()
                    && Status == ReviewStatus.MoveNextEndOfStore)))
        {
          // If new item is not yet fully loaded, return a Task to be awaited
          if (LazyLoader != null && GetFurtherLoadedIndex() <= Index)
          {
            CallFurtherLoad();

            ret = MoveNextFurtherLoadAsync(FurtherLoadCompletionSource.Task);
          }

          // Everything's fine, move on to next object
          else
          {
            Current = Objects[++Index];

            return TaskConstants.BooleanTrue;
          }
        }

        // No more items, have we reached end of item store ?
        else if (Status == ReviewStatus.MoveNextEndOfStore)
        {
          OnComplete();

          return TaskConstants.BooleanFalse;
        }

        // No more loaded items, or item load threshold reached.
        // Status not marked as end of store. Although this does not guarantee
        // more items are available, try to fetch them.
        else
        {
          CallLoadMore();

          ret = MoveNextLoadItemsAsync(LoadCompletionSource.Task);
        }

        // We need not retain old item's reference anymore
        Current = null;
      }

      return ret;
    }

    /// <summary>
    ///   Helper methods which simply adding items to current collection by dealing with
    ///   synchronization and asynchronous operations.
    /// </summary>
    /// <param name="dbQueryFunc">The DB query provider function.</param>
    /// <returns>Added item count</returns>
    protected async Task<int> AddItemsAsync(Func<ITableQuery<T>> dbQueryFunc)
    {
      // Fully load up to IncrementalFurtherLoadMax items
      var items = await Task.Run(
        () =>
        {
          using (Db.Lock())
            return dbQueryFunc().ToList();
        }).ConfigureAwait(false);

      // Add to collection
      lock (LockObject)
        Objects.AddRange(items);

      return items.Count;
    }

    /// <summary>
    ///   Must be called before any other operation.
    ///   Initialize <see cref="Objects"/> store, set <see cref="Status"/> to
    ///   <see cref="ReviewStatus.MoveNext"/> and load content if <see cref="load"/> is true.
    /// </summary>
    /// <param name="load">if set to <c>true</c> load content asynchronously.</param>
    protected void Initialize(bool load = false)
    {
      Objects = new List<T>();
      Index = -1;

      Status = ReviewStatus.MoveNext;

      if (load)
        CallLoadMore();
    }

    /// <summary>
    ///   Called when end of collection is reached.
    /// </summary>
    protected void OnComplete()
    {
      Status = ReviewStatus.Complete;
      Current = null;
    }

    /// <summary>
    ///   Called when Current item changes (null or new value).
    /// </summary>
    /// <param name="oldValue">The old value.</param>
    /// <param name="newValue">The new value.</param>
    /// <returns>New value</returns>
    protected virtual T OnCurrentChange(T oldValue, T newValue)
    {
      if (LazyLoader != null && oldValue != null)
        LazyLoader.LazyUnload(oldValue);

      return newValue;
    }

    /// <summary>
    ///   Called when index changes.
    /// </summary>
    /// <param name="oldIndex">The old index.</param>
    /// <param name="newIndex">The new index.</param>
    /// <returns>New index</returns>
    protected virtual int OnIndexChange(int oldIndex, int newIndex)
    {
      lock (LockObject)
      {
        if (GetNextLoadThreshold() >= 0 && newIndex >= GetNextLoadThreshold())
          CallLoadMore();

        if (GetNextFurtherLoadThreshold() >= 0 && newIndex >= GetNextFurtherLoadThreshold())
          CallFurtherLoad();
      }

      return newIndex;
    }

    /// <summary>
    ///   Checks whether current <see cref="Status"/> is valid.
    /// </summary>
    /// <param name="allowNew">if set to <c>true</c> [allow new].</param>
    /// <returns>Whether valid</returns>
    /// <exception cref="System.InvalidOperationException">Invalid state</exception>
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

        // End of list reached, no further MoveNextAsync call are necessary.
        case ReviewStatus.Complete:
          return false;
      }

      return true;
    }

    //
    // Abstract methods

    /// <summary>
    ///   Gets the maximum reachable index until awaiting item load to complete. If
    ///   <see cref="AsyncDbListBase{T}.ReviewStatus.MoveNextEndOfStore" /> is set, this is
    ///   ignored.
    /// </summary>
    /// <returns>Maximum reachable index.</returns>
    protected abstract int GetMaxIndexLoadThreshold();

    /// <summary>
    ///   Index threshold used in determining when asynchronous item loading should be initiated.
    /// </summary>
    /// <returns>Next load index threshold.</returns>
    protected abstract int GetNextLoadThreshold();

    /// <summary>
    ///   Index threshold used in determining when asynchronous further item loading should be
    ///   initiated. Only required when using lazy loading.
    /// </summary>
    /// <returns>Next load index threshold.</returns>
    protected abstract int GetNextFurtherLoadThreshold();

    /// <summary>
    ///   Gets the index of last fully loaded item. Only required when using lazy loading.
    /// </summary>
    /// <returns>Index of last fully loaded item.</returns>
    protected abstract int GetFurtherLoadedIndex();

    /// <summary>
    ///   Does the further load.
    /// </summary>
    /// <returns></returns>
    protected abstract Task DoFurtherLoadAsync();

    /// <summary>
    ///   Does the load more.
    /// </summary>
    /// <param name="fullLoad">if set to <c>true</c> [full load].</param>
    /// <returns></returns>
    protected abstract Task<bool> DoLoadMoreAsync(bool fullLoad);

    /// <summary>
    ///   Sort cards upon adding new cards. Object lock should already be in place when
    ///   calling this.
    /// </summary>
    protected abstract void Sort();

    //
    // Async loading methods

    /// <summary>
    ///   Called if asynchronous further item loading is needed.
    ///   Wait for async load task to finish.
    /// </summary>
    /// <param name="loadCompletionTask">The load task</param>
    /// <returns>Always true</returns>
    private async Task<bool> MoveNextFurtherLoadAsync(Task loadCompletionTask)
    {
      await loadCompletionTask.ConfigureAwait(false);

      lock (LockObject)
        Current = Objects[++Index];

      return true;
    }

    /// <summary>
    ///   Called if asynchronous item loading is needed.
    ///   Wait for async load task to finish.
    /// </summary>
    /// <param name="loadCompletionTask">The load task</param>
    /// <returns>Whether any item is available</returns>
    private async Task<bool> MoveNextLoadItemsAsync(Task loadCompletionTask)
    {
      await loadCompletionTask.ConfigureAwait(false);

      lock (LockObject)
      {
        // If no item available, set complete state
        if (Index == Objects.Count - 1)
        {
          OnComplete();

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
        await loadCompletionTask.ConfigureAwait(false);

        lock (LockObject)
          Current = Objects[++Index];
      }

      return true;
    }

    /// <summary>
    ///   Initiate asynchronous item loading if necessary
    /// </summary>
    private void CallLoadMore()
    {
      if (LoadCompletionSource == null && Status != ReviewStatus.MoveNextEndOfStore)
      {
        LoadCompletionSource = new TaskCompletionSource<object>();

#pragma warning disable 4014
        LoadMoreAsync();
#pragma warning restore 4014
      }
    }

    /// <summary>
    ///   Initiate asynchronous further item loading if necessary.
    ///   Only required when using lazy loading.
    /// </summary>
    private void CallFurtherLoad()
    {
      if (FurtherLoadCompletionSource == null)
      {
        FurtherLoadCompletionSource = new TaskCompletionSource<object>();

#pragma warning disable 4014
        FurtherLoadAsync();
#pragma warning restore 4014
      }
    }

    private async Task LoadMoreAsync()
    {
      bool ret = false;

      try
      {
        bool fullLoad = Index == Objects.Count - 1;
        ret = await DoLoadMoreAsync(fullLoad).ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Exception caught while running LoadMoreAsync.");
      }
      finally
      {
        lock (LockObject)
        {
          if (ret)
            Sort();

          LoadCompletionSource.SetResult(null);
          LoadCompletionSource = null;
        }
      }
    }

    private async Task FurtherLoadAsync()
    {
      try
      {
        await DoFurtherLoadAsync().ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Exception caught while running FurtherLoadAsync.");
      }
      finally
      {
        lock (LockObject)
        {
          FurtherLoadCompletionSource.SetResult(null);
          FurtherLoadCompletionSource = null;
        }
      }
    }

    #endregion
  }
}