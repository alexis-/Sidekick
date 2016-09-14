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

namespace Sidekick.Windows.Services
{
  using System.Collections.Specialized;
  using System.Linq;
  using System.Threading.Tasks;

  using Catel.Collections;
  using Catel.IoC;

  using Orc.FilterBuilder;
  using Orc.FilterBuilder.Services;

  using Sidekick.Shared.Interfaces.Database;
  using Sidekick.Windows.Models;

  /// <summary>Handles CollectionQuery operations (saving, loading, adding, ...).</summary>
  public class CollectionQueryManagerService
  {
    #region Fields

    private readonly IDatabaseAsync _db;
    private readonly IReflectionService _reflectionService;

    #endregion



    #region Constructors

    /// <summary>Initializes a new instance of the <see cref="CollectionQueryManagerService" /> class.</summary>
    /// <param name="db">Database instance.</param>
    /// <param name="reflectionService">The reflection service.</param>
    public CollectionQueryManagerService(
      IDatabaseAsync db, IReflectionService reflectionService)
    {
      _db = db;
      _reflectionService = reflectionService;
    }

    #endregion



    #region Properties

    /// <summary>All created CollectionQuery.</summary>
    /// TODO: Thread-safety (insert, update, ...)
    public CollectionQueries CollectionQueries { get; private set; }

    #endregion



    #region Methods

    /// <summary>Updates database values for the specified query.</summary>
    /// <param name="query">The query.</param>
    /// <returns></returns>
    public async Task UpdateAsync(CollectionQuery query)
    {
      await _db.UpdateAsync(query).ConfigureAwait(false);
    }

    /// <summary>Creates the specified current query.</summary>
    /// <param name="query">The current query.</param>
    /// <returns></returns>
    public async Task CreateAsync(CollectionQuery query)
    {
      await _db.InsertAsync(query).ConfigureAwait(false);

      CollectionQueries.Queries.Add(query);
    }

    /// <summary>Initialize manager asynchronously. Load all filters.</summary>
    /// <returns>Waitable task</returns>
    public async Task InitializeAsync()
    {
      if (CollectionQueries != null)
        return;

      CollectionQueries = new CollectionQueries();
      CollectionQueries.Queries.CollectionChanged += QueriesOnCollectionChanged;

      // Create default queries
      AddDefaultQueries();

      // Load user-defined queries from DB
      var userQueries = await _db.Table<CollectionQuery>().ToListAsync().ConfigureAwait(false);

      CollectionQueries.Queries.AddItems(userQueries, SuspensionMode.Adding);
    }

    private void AddDefaultQueries()
    {
      CollectionQuery query;

      // "All" query
      query =
        TypeFactory.Default.CreateInstanceWithParametersAndAutoCompletion(
                     typeof(CollectionQuery), "All", true) as CollectionQuery;

      CollectionQueries.Queries.Add(query);
    }

    private void QueriesOnCollectionChanged(
      object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
    {
      if (notifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Add)
        notifyCollectionChangedEventArgs.NewItems.OfType<CollectionQuery>()
                                        .ForEach(i => i.EnsureIntegrity(_reflectionService));
    }

    #endregion
  }
}