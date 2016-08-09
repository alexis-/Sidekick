using System.Collections.Generic;
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Mnemophile.Tests
{
  public class XunitTestFrameworkExecutorWithSQLiteFixture
    : XunitTestFrameworkExecutor
  {
    public XunitTestFrameworkExecutorWithSQLiteFixture(
      AssemblyName assemblyName,
      ISourceInformationProvider sourceInformationProvider,
      IMessageSink diagnosticMessageSink)
      : base(assemblyName, sourceInformationProvider, diagnosticMessageSink)
    {
    }

    protected override async void RunTestCases(
      IEnumerable<IXunitTestCase> testCases,
      IMessageSink executionMessageSink,
      ITestFrameworkExecutionOptions executionOptions)
    {
      using (var assemblyRunner =
        new XunitTestAssemblyRunnerWithSQLiteFixture(
          TestAssembly,
          testCases,
          DiagnosticMessageSink,
          executionMessageSink,
          executionOptions))
        await assemblyRunner.RunAsync();
    }
  }
}
