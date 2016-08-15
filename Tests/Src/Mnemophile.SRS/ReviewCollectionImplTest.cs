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
        db, config, true);

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
        db, config, true);
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

      reviewCollection = new ReviewCollectionImpl(db, config, true);
      @continue = await reviewCollection.Initialized;

      @continue.Should().Be(false);
      reviewCollection.Current.Should().BeNull();
    }

    [Theory]
    [InlineData(1, ConstSRS.Grade.Good), InlineData(2, ConstSRS.Grade.Good),
      InlineData(5, ConstSRS.Grade.Good), InlineData(10, ConstSRS.Grade.Good),
      InlineData(20, ConstSRS.Grade.Good),
      InlineData(50, ConstSRS.Grade.Good)]
    [InlineData(1, ConstSRS.Grade.Easy), InlineData(10, ConstSRS.Grade.Easy),
      InlineData(20, ConstSRS.Grade.Easy),
      InlineData(50, ConstSRS.Grade.Easy)]
    public async void AnswerCard(int cardCount, ConstSRS.Grade grade)
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
      int itExpected = ComputeTotalReviewCount(
        notes, config, grade);

      // Create review collection and answer all
      ReviewCollectionImpl reviewCollection = new ReviewCollectionImpl(
        db, config, true);
      bool @continue = await reviewCollection.Initialized;

      @continue.Should().Be(true);

      while (@continue)
      {
        itCount++;

        ICard card = reviewCollection.Current;
        card.Should().NotBeNull();

        @continue = await reviewCollection.Answer(grade);
      }

      itCount.Should().Be(itExpected);

      reviewCollection = new ReviewCollectionImpl(db, config, true);
      @continue = await reviewCollection.Initialized;

      @continue.Should().Be(false);
      reviewCollection.Current.Should().BeNull();
    }

    [Fact]
    public async void ReviewCount()
    {
      // Create context
      CardTestDb db = new CardTestDb();
      CollectionConfig config = CollectionConfig.Default;

      int newCardCount = config.NewCardPerDay * 2;
      int dueCardCount = config.DueCardPerDay * 2;
      int cardCount = newCardCount + dueCardCount;

      IEnumerable<Note> notes = new CollectionGenerator(
        new CardGenerator(
          new Fixture(),
          new TimeGenerator(),
          config,
          cardCount,
          newCardCount, 0, dueCardCount, 0),
        db, cardCount, 1).Generate();

      // Ensure context
      notes.Count().Should().Be(cardCount);
      notes.Sum(n => n.Cards.Count).Should().Be(cardCount);
      notes.Max(n => n.Cards.Count).Should().Be(1);
      notes.Min(n => n.Cards.Count).Should().Be(1);
      notes.Any(n => n.Cards.Any(c => c.Lapses > 0)).Should().Be(false);
      notes.Sum(n => n.Cards.Count(
        c => c.PracticeState == ConstSRS.CardPracticeState.New))
           .Should().Be(newCardCount);
      notes.Sum(n => n.Cards.Count(
        c => c.PracticeState == ConstSRS.CardPracticeState.Due))
           .Should().Be(dueCardCount);

      // Compute expected results
      int itCount = 0;
      int itDueCount = 0;
      int itNewCount = 0;
      int itLearnCount = 0;

      HashSet<int> learnIds = new HashSet<int>();

      int itExpected = ComputeTotalReviewCount(
        notes, config, ConstSRS.Grade.Good);

      int newCardGoal1 = config.NewCardPerDay / 2;
      int dueCardGoal1 = config.DueCardPerDay / 2;


      // 1
      // Create review collection and answer first batch
      ReviewCollectionImpl reviewCollection = new ReviewCollectionImpl(
        db, config, true);
      bool @continue = await reviewCollection.Initialized;

      @continue.Should().Be(true);

      while (@continue)
      {
        // Sanity card check
        ICard card = reviewCollection.Current;
        card.Should().NotBeNull();

        // Check learn cards
        if (card.IsLearning())
        {
          learnIds.Contains(((Card)card).Id).Should().Be(false);
          learnIds.Add(((Card)card).Id);
        }

        // Update counters
        itCount++;

        if (card.IsNew())
          itNewCount++;
        else if (card.IsDue())
          itDueCount++;
        else
          itLearnCount++;

        // Answer
        @continue = await reviewCollection.Answer(ConstSRS.Grade.Good);

        int dueLeft = reviewCollection.CountByState(
          ConstSRS.CardPracticeStateFilterFlag.Due);
        int newLeft = reviewCollection.CountByState(
          ConstSRS.CardPracticeStateFilterFlag.New);

        @continue = @continue && dueLeft > dueCardGoal1;
        @continue = @continue && newLeft > newCardGoal1;

        // Assert
        newLeft.Should().Be(config.NewCardPerDay - itNewCount);
        dueLeft.Should().Be(config.DueCardPerDay - itDueCount);
      }

      itCount.Should().BeLessThan(itExpected);
      
      // 2
      // Create review collection and answer first batch
      reviewCollection = new ReviewCollectionImpl(db, config, true);
      @continue = await reviewCollection.Initialized;

      @continue.Should().Be(true);

      while (@continue)
      {
        // Sanity card check
        ICard card = reviewCollection.Current;
        card.Should().NotBeNull();

        // Check learn cards
        if (card.IsLearning())
        {
          learnIds.Contains(((Card)card).Id).Should().Be(false);
          learnIds.Add(((Card)card).Id);
        }

        // Update counters
        itCount++;

        bool learn = false;

        if (card.IsNew())
          itNewCount++;
        else if (card.IsDue())
          itDueCount++;
        else
        {
          itLearnCount++;
          learn = true;
        }

        // Answer
        @continue = await reviewCollection.Answer(ConstSRS.Grade.Good);

        int dueLeft = reviewCollection.CountByState(
          ConstSRS.CardPracticeStateFilterFlag.Due);
        int newLeft = reviewCollection.CountByState(
          ConstSRS.CardPracticeStateFilterFlag.New);


        if (learn)
          card.IsLearning().Should().Be(false);
        // Assert
        newLeft.Should().Be(config.NewCardPerDay - itNewCount);
        dueLeft.Should().Be(config.DueCardPerDay - itDueCount);
      }

      itNewCount.Should().Be(config.NewCardPerDay);
      itDueCount.Should().Be(config.DueCardPerDay);
      itLearnCount.Should().Be(config.NewCardPerDay);
      itCount.Should().Be(itExpected);
    }

    private void GetCollectionPracticeStates(
      IEnumerable<Note> notes, out int newCards, out int learnCards,
      out int lapsingCards, out int dueCards)
    {
      newCards = notes.Sum(n => n.Cards.Count(c => c.IsNew()));
      learnCards = notes.Sum(
        n => n.Cards.Count(c => c.IsLearning() && c.Lapses == 0));
      lapsingCards = notes.Sum(
        n => n.Cards.Count(c => c.IsLearning() && c.Lapses > 0));
      dueCards = notes.Sum(
        n => n.Cards.Count(c => c.IsDue()));
    }

    private int ComputeTotalReviewCount(
      IEnumerable<Note> notes, CollectionConfig config, ConstSRS.Grade grade)
    {
      notes.Max(n => n.Cards.Count).Should().Be(1);

      int newCards, learnCards, lapsingCards, dueCards;
      GetCollectionPracticeStates(notes,
        out newCards, out learnCards, out lapsingCards, out dueCards);

      newCards = Math.Min(newCards, config.NewCardPerDay);
      dueCards = Math.Min(dueCards, config.DueCardPerDay);

      return
        (grade == ConstSRS.Grade.Easy
        ? newCards : newCards * config.LearningSteps.Length)
        + (grade == ConstSRS.Grade.Easy
        ? learnCards : learnCards * config.LearningSteps.Length)
        + (grade == ConstSRS.Grade.Easy
        ? lapsingCards : lapsingCards * config.LapseSteps.Length)
        + dueCards;
    }
  }
}
