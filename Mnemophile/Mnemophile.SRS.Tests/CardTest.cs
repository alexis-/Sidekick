// <copyright file="CardTest.cs">Copyright ©  2016</copyright>
using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Mnemophile.SRS.Models;

namespace Mnemophile.SRS.Models.Tests
{
    /// <summary>This class contains parameterized unit tests for Card</summary>
    [PexClass(typeof(Card))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    public partial class CardTest
    {
        /// <summary>Test stub for UpdateLearningStep(Boolean)</summary>
        [PexMethod]
        internal void UpdateLearningStepTest([PexAssumeUnderTest]Card target, bool reset)
        {
            target.UpdateLearningStep(reset);
            // TODO: add assertions to method CardTest.UpdateLearningStepTest(Card, Boolean)
        }
    }
}
