using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Mnemophile.Const.SRS;
using Mnemophile.Interfaces.SRS;
using Mnemophile.SRS.Impl;
using Mnemophile.SRS.Impl.Review;
using Mnemophile.SRS.Models;
using Mnemophile.SRS.Tests;
using Mnemophile.SRS.Tests.Helpers;
using Mnemophile.Tests;
using Mnemophile.Utils;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace Mnemophile.SRS.Tests
{
  public class CardTest
  {
    public CollectionConfig Config => CollectionConfig.Default;

    [Fact]
    public void ComputeRandomCardsGrades()
    {
      const int cardCount = 20;

      CardGenerator cardGenerator = new CardGenerator(
        new Fixture(),
        new TimeGenerator(),
        Config, cardCount);

      Dictionary<short, int> gradesCount = new Dictionary<short, int>();
      gradesCount[(short)ConstSRS.CardPracticeState.Due] = 4;
      gradesCount[(short)ConstSRS.CardPracticeState.Learning] = 3;
      gradesCount[(short)ConstSRS.CardPracticeState.New] = 3;

      for (int i = 0; i < cardCount; i++)
      {
        Card card = cardGenerator.Generate();

        ConstSRS.GradingInfo[] grades = card.ComputeGrades();

        grades.Should()
              .NotBeNull()
              .And
              .HaveCount(gradesCount[(short)card.PracticeState]);
      }
    }

    [Fact]
    public void LearningCard()
    {
      const int cardCount = 20;

      CardGenerator cardGenerator = new CardGenerator(
        new Fixture(),
        new TimeGenerator(),
        Config,
        cardCount, 0, 1, 0, 30);

      Card card = new Card();

      Config.LearningSteps.Count().Should().BeGreaterThan(1);

      for (int i = 0; i < cardCount; i++)
      {
        cardGenerator.Generate(card);

        //int leftToday = card.ReviewLeftToday();
        int toGraduation = card.GetLearningStepsLeft();

        card.CurrentReviewTime = DateTime.Now.ToUnixTimestamp();

        card.GetCurrentLearningIndex().Should().Be(0);
        card.GetLearningStepsLeft().Should().Be(
          card.LearningOrLapsingSteps.Length);

        // Loop until graduation step
        for (int step = 1; step < toGraduation; step++)
        {
          card.UpdateLearningStep();

          card.GetCurrentLearningIndex().Should().Be(step);
          card.GetLearningStepsLeft().Should().Be(toGraduation - step);
          card.Due.Should().Be(
            card.CurrentReviewTime
            + card.LearningOrLapsingSteps[step]);
        }

        card.UpdateLearningStep();
        card.PracticeState.Should().Be(ConstSRS.CardPracticeState.Due);
      }
    }

    [Fact]
    public void ReviewInterval()
    {
      // In time, 250% ease, 1d ivl
      Card card = CardGenerator.MakeCard(
        ConstSRS.CardPracticeState.Due,
        DateTime.Now.AddSeconds(60).ToUnixTimestamp(),
        2.5f, 1);

      ReviewIntervalExpectedIntervals(card, 2, 3, 2, 3, 2, 4);

      // In time, 250% ease, 2d ivl
      card = CardGenerator.MakeCard(
        ConstSRS.CardPracticeState.Due,
        DateTime.Now.AddSeconds(60).ToUnixTimestamp(),
        2.5f, 2);

      ReviewIntervalExpectedIntervals(card, 3, 4, 4, 6, 5, 9);
    }

    private void ReviewIntervalExpectedIntervals(
      Card card,
      int hardMin, int hardMax,
      int goodMin, int goodMax,
      int easyMin, int easyMax)
    {
      for (int i = 0; i < 20; i++)
      {
        card.ComputeReviewInterval(ConstSRS.Grade.Hard)
            .Should().BeInRange(hardMin, hardMax);
        card.ComputeReviewInterval(ConstSRS.Grade.Good)
            .Should().BeInRange(goodMin, goodMax);
        card.ComputeReviewInterval(ConstSRS.Grade.Easy)
            .Should().BeInRange(easyMin, easyMax);
      }
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
      IEnumerable<Note> notes, CollectionConfig config)
    {
      notes.Max(n => n.Cards.Count).Should().Be(1);

      int newCards, learnCards, lapsingCards, dueCards;
      GetCollectionPracticeStates(notes,
        out newCards, out learnCards, out lapsingCards, out dueCards);

      return
        newCards + newCards * config.LearningSteps.Length
        + learnCards * config.LearningSteps.Length
        + lapsingCards * config.LapseSteps.Length
        + dueCards;
    }
  }
}