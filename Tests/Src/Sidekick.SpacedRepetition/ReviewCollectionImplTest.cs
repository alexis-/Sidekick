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

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Sidekick.SpacedRepetition.Const;
using Sidekick.SpacedRepetition.Generators;
using Sidekick.SpacedRepetition.Models;
using Sidekick.SpacedRepetition.Review;
using Xunit;

namespace Sidekick.SpacedRepetition.Tests
{
  public class ReviewCollectionImplTest
  {
    [Theory, InlineData(0), InlineData(1), InlineData(5), InlineData(10),
     InlineData(20), InlineData(50), InlineData(100), InlineData(500)]
    public async void RandomCollectionReview(int noteCount)
    {
      // Setup DB & LazyLoader
      CardDb db = new CardDb();

      // Setup collection
      CollectionConfig config = CollectionConfig.Default;
      CollectionGenerator generator = new CollectionGenerator(
        new CardGenerator(new TimeGenerator(3 * 30, true),
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
        Card currentCard = reviewCollection.Current;
        count++;

        currentCard.Should().NotBeNull();
        currentCard.Data.Should().NotBeNull();

        @continue = await reviewCollection.Answer(Grade.Good);
      }
    }

    [Theory]
    [InlineData(1), InlineData(2), InlineData(3), InlineData(4),
     InlineData(5), InlineData(10), InlineData(20), InlineData(50)]
    public async void DismissCard(int cardCount)
    {
      // Create context
      CardDb db = new CardDb();
      CollectionConfig config = CollectionConfig.Default;
      IEnumerable<Note> notes = new CollectionGenerator(
        new CardGenerator(new TimeGenerator(),
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

        Card card = reviewCollection.Current;
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
    [InlineData(1, Grade.Good), InlineData(2, Grade.Good),
     InlineData(5, Grade.Good), InlineData(10, Grade.Good),
     InlineData(20, Grade.Good),
     InlineData(50, Grade.Good)]
    [InlineData(1, Grade.Easy), InlineData(10, Grade.Easy),
     InlineData(20, Grade.Easy),
     InlineData(50, Grade.Easy)]
    public async void AnswerCard(int cardCount, int gradeValue)
    {
      Grade grade = gradeValue;

      // Create context
      CardDb db = new CardDb();
      CollectionConfig config = CollectionConfig.Default;
      IEnumerable<Note> notes = new CollectionGenerator(
        new CardGenerator(new TimeGenerator(),
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

        Card card = reviewCollection.Current;
        card.Should().NotBeNull();

        @continue = await reviewCollection.Answer(grade);
      }

      itCount.Should().Be(itExpected);

      reviewCollection = new ReviewCollectionImpl(db, config, true);
      @continue = await reviewCollection.Initialized;

      @continue.Should().Be(false);
      reviewCollection.Current.Should().BeNull();
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
      IEnumerable<Note> notes, CollectionConfig config, Grade grade)
    {
      notes.Max(n => n.Cards.Count).Should().Be(1);

      int newCards, learnCards, lapsingCards, dueCards;
      GetCollectionPracticeStates(notes,
        out newCards, out learnCards, out lapsingCards, out dueCards);

      newCards = Math.Min(newCards, config.NewCardPerDay);
      dueCards = Math.Min(dueCards, config.DueCardPerDay);

      return
        (grade == Grade.Easy
           ? newCards
           : newCards * config.LearningSteps.Length)
        + (grade == Grade.Easy
             ? learnCards
             : learnCards * config.LearningSteps.Length)
        + (grade == Grade.Easy
             ? lapsingCards
             : lapsingCards * config.LapseSteps.Length)
        + dueCards;
    }

    [Fact]
    public void FixedCollectionReview()
    {
      Assert.True(true); // TODO: Implement
    }

    [Fact]
    public async void ReviewCount()
    {
      // Create context
      CardDb db = new CardDb();
      CollectionConfig config = CollectionConfig.Default;

      int newCardCount = config.NewCardPerDay * 2;
      int dueCardCount = config.DueCardPerDay * 2;
      int cardCount = newCardCount + dueCardCount;

      IEnumerable<Note> notes = new CollectionGenerator(
        new CardGenerator(new TimeGenerator(),
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
        c => c.PracticeState == CardPracticeState.New))
           .Should().Be(newCardCount);
      notes.Sum(n => n.Cards.Count(
        c => c.PracticeState == CardPracticeState.Due))
           .Should().Be(dueCardCount);

      // Compute expected results
      int itCount = 0;
      int itDueCount = 0;
      int itNewCount = 0;
      int itLearnCount = 0;

      HashSet<int> learnIds = new HashSet<int>();

      int itExpected = ComputeTotalReviewCount(
        notes, config, Grade.Good);

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
        Card card = reviewCollection.Current;
        card.Should().NotBeNull();

        // Check learn cards
        if (card.IsLearning())
        {
          learnIds.Contains(card.Id).Should().Be(false);
          learnIds.Add(card.Id);
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
        @continue = await reviewCollection.Answer(Grade.Good);

        int dueLeft = reviewCollection.CountByState(
          CardPracticeStateFilterFlag.Due);
        int newLeft = reviewCollection.CountByState(
          CardPracticeStateFilterFlag.New);

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
        Card card = reviewCollection.Current;
        card.Should().NotBeNull();

        // Check learn cards
        if (card.IsLearning())
        {
          learnIds.Contains(card.Id).Should().Be(false);
          learnIds.Add(card.Id);
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
        @continue = await reviewCollection.Answer(Grade.Good);

        int dueLeft = reviewCollection.CountByState(
          CardPracticeStateFilterFlag.Due);
        int newLeft = reviewCollection.CountByState(
          CardPracticeStateFilterFlag.New);


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
  }
}