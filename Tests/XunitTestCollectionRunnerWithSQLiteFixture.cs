using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SQLite.Net.Interop;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Mnemophile.Tests
{
  public class XunitTestCollectionRunnerWithSQLiteFixture
    : XunitTestCollectionRunner
  {
    public XunitTestCollectionRunnerWithSQLiteFixture(
      ITestCollection testCollection,
      IEnumerable<IXunitTestCase> testCases,
      IMessageSink diagnosticMessageSink,
      IMessageBus messageBus,
      ITestCaseOrderer testCaseOrderer,
      ExceptionAggregator aggregator,
      CancellationTokenSource cancellationTokenSource)
      : base(testCollection, testCases, diagnosticMessageSink, messageBus,
          testCaseOrderer, aggregator, cancellationTokenSource)
    {
    }

    protected override Task<RunSummary> RunTestClassAsync(
      ITestClass testClass,
      IReflectionTypeInfo @class,
      IEnumerable<IXunitTestCase> testCases)
    {
      CollectionFixtureMappings[typeof(Func<ISQLitePlatform>)] =
        new Func<ISQLitePlatform>(() => new SQLitePlatformTest());
      
      return base.RunTestClassAsync(testClass, @class, testCases);
    }
  }
}