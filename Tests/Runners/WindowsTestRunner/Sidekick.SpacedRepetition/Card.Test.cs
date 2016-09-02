﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Ploeh.AutoFixture;
using Sidekick.Shared.Extensions;
using Sidekick.Shared.Utils;
using Sidekick.SpacedRepetition;
using Sidekick.SpacedRepetition.Const;
using Sidekick.SpacedRepetition.Models;
using Sidekick.SpacedRepetition.Tests;
using Xunit;

namespace WindowsTestRunner.Sidekick.SpacedRepetition
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
      gradesCount[(short)CardPracticeState.Due] = 4;
      gradesCount[(short)CardPracticeState.Learning] = 3;
      gradesCount[(short)CardPracticeState.New] = 3;

      for (int i = 0; i < cardCount; i++)
      {
        Card card = cardGenerator.Generate();

        GradeInfo[] grades = card.ComputeGrades();

        grades.Should()
              .NotBeNull()
              .And
              .HaveCount(gradesCount[card.PracticeState]);
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

      Card card = new Card(Config);

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
        card.IsDue().Should().Be(true);
      }
    }

    [Fact]
    public void FailCard()
    {
      Card card = CardGenerator.MakeCard(
        Config,
        CardPracticeState.New,
        DateTime.Now.AddSeconds(60).ToUnixTimestamp(),
        2.5f, 1);

      // Ensure setting
      int learningSteps = Config.LearningSteps.Length;

      learningSteps.Should().BeGreaterThan(1);
      card.IsNew().Should().Be(true);

      // Correct answer
      card.Answer(Grade.Good);

      card.IsLearning().Should().Be(true);
      card.GetCurrentLearningIndex().Should().Be(1);
      card.GetLearningStepsLeft().Should().Be(learningSteps - 1);

      // Fail
      card.Answer(Grade.Fail);

      // Answer until graduation
      for (int i = 0; i < learningSteps; i++)
      {
        card.IsLearning().Should().Be(true);
        card.GetCurrentLearningIndex().Should().Be(i);
        card.GetLearningStepsLeft().Should().Be(learningSteps - i);

        card.Answer(Grade.Good);
      }

      card.IsDue().Should().Be(true);
      card.Due.Should().BeGreaterOrEqualTo(
        DateTime.Today.AddDays(1).ToUnixTimestamp());
    }

    [Fact]
    public void LapseCard()
    {
      Card card = CardGenerator.MakeCard(
        Config,
        CardPracticeState.Due,
        DateTime.Now.AddSeconds(60).ToUnixTimestamp(),
        2.5f, 1);
      
      card.IsDue().Should().Be(true);

      card.Answer(Grade.Fail);
      card.IsLearning().Should().Be(true);
      card.Lapses.Should().Be(1);
      card.IsLeech().Should().Be(false);
    }

    [Fact]
    public void ReviewInterval()
    {
      // In time, 250% ease, 1d ivl
      Card card = CardGenerator.MakeCard(
        Config,
        CardPracticeState.Due,
        DateTime.Now.AddSeconds(60).ToUnixTimestamp(),
        2.5f, 1);

      ReviewIntervalExpectedIntervals(card, 2, 3, 2, 3, 2, 4);

      // In time, 250% ease, 2d ivl
      card = CardGenerator.MakeCard(
        Config,
        CardPracticeState.Due,
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
        card.ComputeReviewInterval(Grade.Hard)
            .Should().BeInRange(hardMin, hardMax);
        card.ComputeReviewInterval(Grade.Good)
            .Should().BeInRange(goodMin, goodMax);
        card.ComputeReviewInterval(Grade.Easy)
            .Should().BeInRange(easyMin, easyMax);
      }
    }
  }
}