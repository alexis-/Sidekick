using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Mnemophile.Utils;

namespace Mnemophile.Utils.Tests
{
    /// <summary>This class contains parameterized unit tests for Delay</summary>
    [PexClass(typeof(Delay))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class DelayTest
    {

    /// <summary>Test stub for FromDays(Int32)</summary>
    [PexMethod]
    public Delay FromDaysTest(int days)
    {
      Delay result = Delay.FromDays(days);

      PexAssert.AreEqual((int)result, days * 60 * 60 * 24);

      return result;
      // TODO: add assertions to method DelayTest.FromDaysTest(Int32)
    }

    /// <summary>Test stub for FromHours(Int32)</summary>
    [PexMethod]
    public Delay FromHoursTest(int hours)
    {
      Delay result = Delay.FromHours(hours);

      PexAssert.AreEqual((int)result, hours * 60 * 60);

      return result;
      // TODO: add assertions to method DelayTest.FromHoursTest(Int32)
    }

    /// <summary>Test stub for FromMinutes(Int32)</summary>
    [PexMethod]
    public Delay FromMinutesTest(int minutes)
    {
      Delay result = Delay.FromMinutes(minutes);

      PexAssert.AreEqual((int)result, minutes * 60);

      return result;
      // TODO: add assertions to method DelayTest.FromMinutesTest(Int32)
    }

    /// <summary>Test stub for get_Days()</summary>
    [PexMethod]
    public float DaysGetTest([PexAssumeUnderTest]Delay target)
    {
      float result = target.Days;

      PexAssert.AreEqual(result, ((float)target) / (60 * 60 * 24));

      return result;
      // TODO: add assertions to method DelayTest.DaysGetTest(Delay)
    }

    /// <summary>Test stub for get_Hours()</summary>
    [PexMethod]
    public float HoursGetTest([PexAssumeUnderTest]Delay target)
    {
      float result = target.Hours;

      PexAssert.AreEqual(result, ((float)target) / (60 * 60));

      return result;
      // TODO: add assertions to method DelayTest.HoursGetTest(Delay)
    }

    /// <summary>Test stub for get_Minutes()</summary>
    [PexMethod]
    public float MinutesGetTest([PexAssumeUnderTest]Delay target)
    {
      float result = target.Minutes;

      PexAssert.AreEqual(result, ((float)target) / 60);

      return result;
      // TODO: add assertions to method DelayTest.MinutesGetTest(Delay)
    }

    /// <summary>Test stub for get_Seconds()</summary>
    [PexMethod]
    public int SecondsGetTest([PexAssumeUnderTest]Delay target)
    {
      int result = target.Seconds;

      PexAssert.AreEqual(result, (int)target);

      return result;
      // TODO: add assertions to method DelayTest.SecondsGetTest(Delay)
    }

    /// <summary>Test stub for op_Implicit(Int32)</summary>
    [PexMethod]
    public Delay op_ImplicitTest(int delay)
    {
      Delay result = (Delay)delay;
      return result;
      // TODO: add assertions to method DelayTest.op_ImplicitTest(Int32)
    }

    /// <summary>Test stub for op_Implicit(Delay)</summary>
    [PexMethod]
    public int op_ImplicitTest01(Delay delay)
    {
      PexAssume.IsNotNull(delay);

      int result = (int)delay;
      return result;
      // TODO: add assertions to method DelayTest.op_ImplicitTest01(Delay)
    }
  }
}
