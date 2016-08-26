using System;
using System.Collections.Generic;
using System.Linq;
using Mnemophile.Const.SpacedRepetition;
using Mnemophile.SpacedRepetition.Impl;
using Mnemophile.SpacedRepetition.Models;
using Mnemophile.Utils;
using Ploeh.AutoFixture;

namespace Mnemophile.SpacedRepetition.Tests
{
  public class CardGenerator
  {
    // Config
    private int NewCount { get; set; }
    private int LearnCount { get; set; }
    private int DueCount { get; set; }
    private int LapseCount { get; set; }

    // Misc
    private HashSet<int> CardIds { get; }

    internal Fixture Fixture { get; }
    internal TimeGenerator TimeGenerator { get; }
    internal CollectionConfig Config { get; }

    private static readonly Random Random = new Random();


    //
    // Constructor

    public CardGenerator(Fixture fixture,
      TimeGenerator timeGenerator, CollectionConfig config,
      int count = 100, float newRatio = 3, float learnRatio = 2,
      float dueRatio = 5, float lapsePercent = 10)
    {
      Fixture = fixture;
      TimeGenerator = timeGenerator;
      Config = config;

      float totalRatio = newRatio + learnRatio + dueRatio;

      NewCount = (int)(newRatio / totalRatio * count);
      LearnCount = (int)(learnRatio / totalRatio * count);
      DueCount = count - NewCount - LearnCount;
      LapseCount = (int)Math.Min(
        lapsePercent / 100.0f * (DueCount + LearnCount),
        DueCount + LearnCount);

      CardIds = new HashSet<int>();
    }


    //
    // Core methods

    public int Left() => NewCount + LearnCount + DueCount;

    public Card Generate(int noteId = -1)
    {
      Card card = new Card(Config, noteId, Fixture.Create<string>());

      return Generate(card);
    }

    public Card Generate(Card card)
    {
      card.Id = TimeGenerator.RandomId(CardIds);
      card.LastModified = Math.Max(card.Id, TimeGenerator.RandomTime());

      bool lapsing;
      ConstSpacedRepetition.CardPracticeState state;

      ComputeNextCardType(out state, out lapsing);

      switch (state)
      {
        case ConstSpacedRepetition.CardPracticeState.Due:
          DueCount--;
          return SetupDueCard(card, lapsing);

        case ConstSpacedRepetition.CardPracticeState.Learning:
          LearnCount--;
          return SetupLearningCard(card, lapsing);

        case ConstSpacedRepetition.CardPracticeState.New:
          NewCount--;
          return SetupNewCard(card);
      }

      throw new InvalidOperationException();
    }

    private void ComputeNextCardType(
      out ConstSpacedRepetition.CardPracticeState state,
      out bool lapsing)
    {
      int total = DueCount + LearnCount + NewCount;

      if (total == 0)
        throw new InvalidOperationException("Card count exhausted");

      int rnd = Random.Next(0, total);

      if (rnd < DueCount)
        state = ConstSpacedRepetition.CardPracticeState.Due;
      
      else if (rnd < DueCount + LearnCount)
        state = ConstSpacedRepetition.CardPracticeState.Learning;

      else
        state = ConstSpacedRepetition.CardPracticeState.New;

      if (LapseCount > 0
          && (state == ConstSpacedRepetition.CardPracticeState.Due
              || state == ConstSpacedRepetition.CardPracticeState.Learning))
      {
        int lapseTotal = DueCount + LearnCount;

        lapsing = Random.Next(0, lapseTotal) < LapseCount;

        if (lapsing)
          LapseCount--;
      }
      else
        lapsing = false;
    }

    private Card SetupDueCard(Card card, bool lapsing)
    {
      card.PracticeState = ConstSpacedRepetition.CardPracticeState.Due;
      card.Due = Math.Max(card.Id, TimeGenerator.RandomTime());

      if (lapsing)
        SetupLapse(card);
      
      SetupSpacedRepetitionDatas(card);

      return card;
    }

    private Card SetupLearningCard(Card card, bool lapsing)
    {
      card.PracticeState = ConstSpacedRepetition.CardPracticeState.Learning;
      card.Due = TimeGenerator.RandomTime(card.LearningOrLapsingSteps.Last());

      if (lapsing)
      {
        SetupLapse(card);
        SetupSpacedRepetitionDatas(card);
      }

      return card;
    }

    private Card SetupNewCard(Card card)
    {
      card.PracticeState = ConstSpacedRepetition.CardPracticeState.New;
      card.Due = 0; //Math.Max(card.Id, TimeGenerator.RandomTime());

      return card;
    }

    private void SetupSpacedRepetitionDatas(Card card)
    {
      card.EFactor = RandomEFactor();
      card.Interval = Math.Max(
        1, Math.Abs(DateTime.Now.ToUnixTimestamp() - card.Due)
           / (3600 * 24));
      card.Interval = Random.Next(card.Interval, card.Interval * 3);
      card.Reviews = (int)Math.Ceiling(Math.Sqrt(card.Interval));
    }

    private void SetupLapse(Card card)
    {
      card.Lapses = Random.Next(1, Config.LeechThreshold * 2);
    }

    private float RandomEFactor()
    {
      return Random.Next(
        (int)(Config.ReviewMinEase * 100),
        300) / 100.0f;
    }

    
    //
    // Misc helper

    public static Card MakeCard(
      CollectionConfig config,
      ConstSpacedRepetition.CardPracticeState state,
      int due = -1,
      float eFactor = 2.5f,
      int interval = 1,
      int reviews = 0,
      int lapses = 0)
    {
      return new Card(config)
      {
        PracticeState = state,
        Due = due == -1 ? DateTime.Now.AddSeconds(60).ToUnixTimestamp() : due,
        EFactor = eFactor,
        Interval = interval,
        Reviews = reviews,
        Lapses = lapses
      };
    }
  }
}