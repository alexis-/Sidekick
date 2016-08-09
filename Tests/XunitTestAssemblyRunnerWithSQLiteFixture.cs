using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Mnemophile.Tests
{
  public class XunitTestAssemblyRunnerWithSQLiteFixture
    : XunitTestAssemblyRunner
  {
    public XunitTestAssemblyRunnerWithSQLiteFixture(
      ITestAssembly testAssembly,
      IEnumerable<IXunitTestCase> testCases,
      IMessageSink diagnosticMessageSink,
      IMessageSink executionMessageSink,
      ITestFrameworkExecutionOptions executionOptions)
      : base(testAssembly, testCases, diagnosticMessageSink,
          executionMessageSink, executionOptions)
    {
    }

    protected override Task<RunSummary> RunTestCollectionAsync(
      IMessageBus messageBus,
      ITestCollection testCollection,
      IEnumerable<IXunitTestCase> testCases,
      CancellationTokenSource cancellationTokenSource)
      =>
        new XunitTestCollectionRunnerWithSQLiteFixture(
          testCollection,
          testCases,
          DiagnosticMessageSink,
          messageBus,
          TestCaseOrderer,
          new ExceptionAggregator(Aggregator),
          cancellationTokenSource)
      .RunAsync();
  }
}
