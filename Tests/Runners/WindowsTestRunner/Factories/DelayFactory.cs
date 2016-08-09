using Mnemophile.Utils;
// <copyright file="DelayFactory.cs">Copyright ©  2016</copyright>

using System;
using Microsoft.Pex.Framework;

namespace Mnemophile.Utils
{
  /// <summary>A factory for Mnemophile.Utils.Delay instances</summary>
  public static partial class DelayFactory
  {

    /// <summary>A factory for Mnemophile.Utils.Delay instances</summary>
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
