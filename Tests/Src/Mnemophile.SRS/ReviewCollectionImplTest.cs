using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Mnemophile.Attributes.DB;
using Mnemophile.Const.SRS;
using Mnemophile.Interfaces.DB;
using Mnemophile.Interfaces.SRS;
using Mnemophile.SRS.Impl;
using Mnemophile.SRS.Impl.Review;
using Mnemophile.SRS.Models;
using Mnemophile.SRS.Tests.Helpers;
using Mnemophile.Tests;
using Mnemophile.Utils;
using Mnemophile.Utils.LazyLoad;
using Mnemophile.Utils.LazyLoad.Attributes;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace Mnemophile.SRS.Tests
{
  public class ReviewCollectionImplTest
  {

    [Theory, InlineData(0), InlineData(1), InlineData(5), InlineData(10),
      InlineData(20), InlineData(50), InlineData(100), InlineData(500)]
    public async void RandomCollectionReview(int noteCount)
    {
      // Setup DB & LazyLoader
      CardTestDb db = new CardTestDb();

      // Setup collection
      CollectionConfig config = CollectionConfig.Default;
      CollectionGenerator generator = new CollectionGenerator(
        new CardGenerator(
          new Fixture(),
          new TimeGenerator(3 * 30, true),
          config, noteCount * 2),
        db,
        noteCount,
        3);

      IEnumerable<Note> notes = generator.Generate();

      // Collection review testing

      ReviewCollectionImpl reviewCollection = new ReviewCollectionImpl(
        db, config);

      int dueToday = notes.Sum(n => n.Cards.Count(
        c =>
        c.Due < DateTime.Today.AddDays(1).ToUnixTimestamp()
        && c.IsDue()));
      int learnToday = notes.Sum(n => n.Cards.Count(
        c =>
        c.Due < DateTime.Today.AddDays(1).ToUnixTimestamp()
        && c.IsLearning()));
      int newToday = notes.Sum(n => n.Cards.Count(
        c =>
        c.Due < DateTime.Today.AddDays(1).ToUnixTimestamp()
        && c.IsNew()));

      int count = 0;

      bool @continue = await reviewCollection.Initialized;

      while (@continue)
      {
        Card currentCard = (Card)reviewCollection.Current;
        count++;

        currentCard.Should().NotBeNull();
        currentCard.Data.Should().NotBeNull();

        @continue = await reviewCollection.Answer(ConstSRS.Grade.Good);
      }
      
    }

    [Fact]
    public void FixedCollectionReview()
    {
      Assert.True(false);
    }

    [Theory]
    [InlineData(1), InlineData(2), InlineData(3), InlineData(4),
      InlineData(5), InlineData(10), InlineData(20), InlineData(50)]
    public async void DismissCard(int cardCount)
    {
      // Create context
      CardTestDb db = new CardTestDb();
      CollectionConfig config = CollectionConfig.Default;
      IEnumerable<Note> notes = new CollectionGenerator(
        new CardGenerator(
          new Fixture(),
          new TimeGenerator(),
          config,
          cardCount),
        db, cardCount, 1).Generate();

      // Ensure context
      notes.Count().Should().Be(cardCount);
      notes.Sum(n => n.Cards.Count).Should().Be(cardCount);
      notes.Max(n => n.Cards.Count).Should().Be(1);
      notes.Min(n => n.Cards.Count).Should().Be(1);

      // Compute expected results
      int itCount = 0;

      // Create revie collection and dismiss all
      ReviewCollectionImpl reviewCollection = new ReviewCollectionImpl(
        db, config);
      bool @continue = await reviewCollection.Initialized;

      @continue.Should().Be(true);

      while (@continue)
      {
        itCount++;

        ICard card = reviewCollection.Current;
        card.Should().NotBeNull();

        @continue = await reviewCollection.Dismiss();
      }

      itCount.Should().Be(cardCount);

      reviewCollection = new ReviewCollectionImpl(db, config);
      @continue = await reviewCollection.Initialized;

      @continue.Should().Be(false);
      reviewCollection.Current.Should().BeNull();
    }
  }
}
