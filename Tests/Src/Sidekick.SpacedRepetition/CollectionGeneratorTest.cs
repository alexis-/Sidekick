// 
// The MIT License (MIT)
// Copyright (c) 2016 Incogito
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

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Sidekick.SpacedRepetition.Generators;
using Sidekick.SpacedRepetition.Models;
using Xunit;

namespace Sidekick.SpacedRepetition.Tests
{
  public class CollectionGeneratorTest
  {
    #region Methods

    //
    // Tests

    [Theory]
    [InlineData(10, 10, 3)]
    [InlineData(5, 10, 3)]
    [InlineData(2, 10, 5)]
    [InlineData(100, 200, 4)]
    public void CollectionGeneratorTests(
      int noteCount, int cardCount, int maxCardPerNote)
    {
      IEnumerable<Note> notes = new CollectionGenerator(
        new CardGenerator(new TimeGenerator(),
          CollectionConfig.Default,
          cardCount),
        noteCount, maxCardPerNote).Generate();

      notes.Count().Should().Be(noteCount);
      notes.Sum(n => n.Cards.Count).Should().Be(cardCount);
      notes.Max(n => n.Cards.Count).Should().BeLessOrEqualTo(maxCardPerNote);
      notes.Min(n => n.Cards.Count).Should().BeGreaterOrEqualTo(1);
      notes.Average(n => n.Cards.Count).Should().Be(
        (double)cardCount / (double)noteCount);
    }

    #endregion
  }
}