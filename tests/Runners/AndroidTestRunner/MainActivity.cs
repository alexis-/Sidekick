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

namespace Sidekick.Tests.Runner.Android
{
  using System.Reflection;

  using global::Android.App;
  using global::Android.OS;

  using Xunit.Runners.UI;
  using Xunit.Sdk;

  /// <summary>
  ///   Main activity test runner.
  /// </summary>
  /// <seealso cref="Xunit.Runners.UI.RunnerActivity" />
  [Activity(Label = "xUnit Android Runner", MainLauncher = true,
     Theme = "@android:style/Theme.Material.Light")]
  public class MainActivity : RunnerActivity
  {
    #region Methods

    /// <summary>
    ///   OnCreate lifecycle.
    /// </summary>
    /// <param name="bundle">The bundle.</param>
    protected override void OnCreate(Bundle bundle)
    {
      // tests can be inside the main assembly
      AddTestAssembly(Assembly.GetExecutingAssembly());

      AddExecutionAssembly(typeof(ExtensibilityPointFactory).Assembly);

      // or in any reference assemblies
      // AddTestAssembly(typeof(SQLiteBasicTest).Assembly);
      // AddTestAssembly(typeof(CardTest).Assembly);
      // or in any assembly that you load (since JIT is available)

      // start running the test suites as soon as the application is loaded
      AutoStart = true;

#if false
// you can use the default or set your own custom writer (e.g. save to web site and tweet it ;-)
			Writer = new TcpTextWriter ("10.0.1.2", 16384);
			// crash the application (to ensure it's ended) and return to springboard
			TerminateAfterExecution = true;
#endif
      // you cannot add more assemblies once calling base
      base.OnCreate(bundle);
    }

    #endregion
  }
}