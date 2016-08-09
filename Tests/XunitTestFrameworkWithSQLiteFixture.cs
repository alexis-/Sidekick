using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Mnemophile.Tests
{
  public class XunitTestFrameworkWithSQLiteFixture : XunitTestFramework
  {
    public XunitTestFrameworkWithSQLiteFixture(IMessageSink messageSink)
      : base(messageSink)
    {
    }

    protected override ITestFrameworkExecutor CreateExecutor(
      AssemblyName assemblyName)
      =>
        new XunitTestFrameworkExecutorWithSQLiteFixture(
          assemblyName,
          SourceInformationProvider,
          DiagnosticMessageSink);
  }
}
