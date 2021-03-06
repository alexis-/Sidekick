// <copyright file="DelayTest.cs">joe</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sidekick.Shared.Utils;

namespace Sidekick.Shared.Utils.Tests
{
    [TestClass]
    [PexClass(typeof(Delay))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class DelayTest
    {
    }
}
