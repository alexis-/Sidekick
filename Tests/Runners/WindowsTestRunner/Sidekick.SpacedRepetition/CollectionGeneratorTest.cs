using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Ploeh.AutoFixture;
using Sidekick.SpacedRepetition;
using Sidekick.SpacedRepetition.Models;
using Sidekick.SpacedRepetition.Tests;
using Xunit;

namespace WindowsTestRunner.Sidekick.SpacedRepetition
{
  public class CollectionGeneratorTest
  {
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
        new CardGenerator(
          new Fixture(),
          new TimeGenerator(),
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
  }
}