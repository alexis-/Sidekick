using Microsoft.Pex.Framework;
using Sidekick.Shared.Utils;
// <copyright file="DelayFactory.cs">Copyright ©  2016</copyright>

namespace WindowsTestRunner.Factories
{
  /// <summary>A factory for Sidekick.Utils.Delay instances</summary>
  public static partial class DelayFactory
  {

    /// <summary>A factory for Sidekick.Utils.Delay instances</summary>
    [PexFactoryMethod(typeof(Delay))]
    public static Delay Create(int sec)
    {
      Delay delay = sec;
      return delay;

      // TODO: Edit factory method of Delay
      // This method should be able to configure the object in all possible ways.
      // Add as many parameters as needed,
      // and assign their values to each field by using the API.
    }
  }
}
