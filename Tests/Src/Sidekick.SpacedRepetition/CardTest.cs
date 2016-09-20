// 
// The MIT License (MIT)
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

namespace Sidekick.SpacedRepetition.Tests
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  using FluentAssertions;

  using Sidekick.Shared.Extensions;
  using Sidekick.SpacedRepetition.Extensions;
  using Sidekick.SpacedRepetition.Generators;
  using Sidekick.SpacedRepetition.Models;

  using Xunit;

  public class CardTest
  {
    private CollectionConfig Config => CollectionConfig.Default;

    private void ReviewIntervalExpectedIntervals(
      Card card, int hardMin, int hardMax, int goodMin, int goodMax, int easyMin, int easyMax)
    {
      for (int i = 0; i < 20; i++)
      {
        card.ComputeReviewInterval(Grade.Hard).Should().BeInRange(hardMin, hardMax);
        card.ComputeReviewInterval(Grade.Good).Should().BeInRange(goodMin, goodMax);
        card.ComputeReviewInterval(Grade.Easy).Should().BeInRange(easyMin, easyMax);
      }
    }

    [Fact]
    public void ComputeRandomCardsGrades()
    {
      const int CardCount = 20;

      CardGenerator cardGenerator = new CardGenerator(new TimeGenerator(), Config, CardCount);

      Dictionary<short, int> gradesCount = new Dictionary<short, int>
      {
        [PracticeState.Due] = 4,
        [PracticeState.Learning] = 3,
        [PracticeState.New] = 3
      };

      for (int i = 0; i < CardCount; i++)
      {
        Card card = cardGenerator.Generate();

        ReviewAnswerInfo[] reviewAnswerInfos = card.ComputeGrades();

        reviewAnswerInfos.Should().NotBeNull().And.HaveCount(gradesCount[card.PracticeState]);
      }
    }

    [Fact]
    public void FailCard()
    {
      Card card = CardGenerator.MakeCard(
        Config, PracticeState.New, DateTime.Now.AddSeconds(60).ToUnixTimestamp(), 2.5f, 1);

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
      card.Due.Should().BeGreaterOrEqualTo(DateTimeExtensions.Tomorrow.ToUnixTimestamp());
    }

    [Fact]
    public void LapseCard()
    {
      Card card = CardGenerator.MakeCard(
        Config, PracticeState.Due, DateTime.Now.AddSeconds(60).ToUnixTimestamp(), 2.5f, 1);

      card.IsDue().Should().Be(true);

      card.Answer(Grade.Fail);
      card.IsLearning().Should().Be(true);
      card.Lapses.Should().Be(1);
      card.IsLeech().Should().Be(false);
    }

    [Fact]
    public void LearningCard()
    {
      const int CardCount = 20;

      CardGenerator cardGenerator = new CardGenerator(
        new TimeGenerator(), Config, CardCount, 0, 1, 0, 30);

      Card card = new Card(Config);

      Config.LearningSteps.Length.Should().BeGreaterThan(1);

      for (int i = 0; i < CardCount; i++)
      {
        cardGenerator.Generate(card);
        
        int toGraduation = card.GetLearningStepsLeft();

        card.CurrentReviewTime = DateTime.Now.ToUnixTimestamp();

        card.GetCurrentLearningIndex().Should().Be(0);
        card.GetLearningStepsLeft().Should().Be(card.LearningOrLapsingSteps.Length);

        // Loop until graduation step
        for (int step = 1; step < toGraduation; step++)
        {
          card.UpdateLearningStep();

          card.GetCurrentLearningIndex().Should().Be(step);
          card.GetLearningStepsLeft().Should().Be(toGraduation - step);
          card.Due.Should().Be(card.CurrentReviewTime + card.LearningOrLapsingSteps[step]);
        }

        card.UpdateLearningStep();
        card.IsDue().Should().Be(true);
      }
    }

    [Fact]
    public void ReviewInterval()
    {
      // In time, 250% ease, 1d ivl
      Card card = CardGenerator.MakeCard(
        Config, PracticeState.Due, DateTime.Now.AddSeconds(60).ToUnixTimestamp(), 2.5f, 1);

      ReviewIntervalExpectedIntervals(card, 2, 3, 2, 3, 2, 4);

      // In time, 250% ease, 2d ivl
      card = CardGenerator.MakeCard(
        Config, PracticeState.Due, DateTime.Now.AddSeconds(60).ToUnixTimestamp(), 2.5f, 2);

      ReviewIntervalExpectedIntervals(card, 3, 4, 4, 6, 5, 9);
    }
  }
}