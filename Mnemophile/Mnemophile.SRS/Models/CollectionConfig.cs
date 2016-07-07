using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mnemophile.Const.SRS;

namespace Mnemophile.SRS.Models
{
  internal class CollectionConfig
  {
    public int[] LearningSteps { get; set; }
    public int LearningCardPerDay { get; set; }
    public ConstSRS.CardInsertionOption InsertionOption { get; set; }
    public int GraduationEasyInterval { get; set; }
    public int GraduationInterval { get; set; }
    public float GraduationStartingEase { get; set; }
    public int ReviewCardPerDay { get; set; }
    public float ReviewEasyBonus { get; set; }
    public float ReviewIntervalModifier { get; set; }
    public int ReviewMaxInterval { get; set; }
    public float ReviewMinEase { get; set; }
    public int[] LapseSteps { get; set; }
    public float LapseIntervalFactor { get; set; }
    public int LapseMinInterval { get; set; }
    public int LeechThreshold { get; set; }
    public ConstSRS.CardLeechAction LeechAction { get; set; }

    public static CollectionConfig Default = new CollectionConfig
    {
      // Default delays upon review of new card: 1 then 10 minutes (2 reviews)
      LearningSteps = new[] { 1, 10 },
      // Default maximum number of new card to learn per day
      LearningCardPerDay = 20,
      // Default insertion order of new card
      InsertionOption = ConstSRS.CardInsertionOption.Linear,
      // Default interval after graduation from "easy" answer
      GraduationEasyInterval = 4,
      // Default interval after graduation
      GraduationInterval = 1,
      // Default ease (EFactor) after graduation: 250%
      GraduationStartingEase = 2.5f,
      // Default number of review per day
      ReviewCardPerDay = 100,
      // Default interval bonus factor from "easy" answer: 130%
      ReviewEasyBonus = 1.3f,
      // Default global interval modifier: 100%
      ReviewIntervalModifier = 1.0f,
      // Default maximum interval: 20 years
      ReviewMaxInterval = 365 * 20,
      // Default minimum ease: 130%
      ReviewMinEase = 1.3f,
      // Default delays upon card lapse: 10 minutes (1 review)
      LapseSteps = new[] { 10 },
      // Default interval modifier upon card lapse: 0%
      LapseIntervalFactor = 0.0f,
      // Default minimum interval upon graduation from card lapse
      LapseMinInterval = 1,
      // Default lapse threshold upon which card fall into leech state
      LeechThreshold = 8,
      // Default leech action
      LeechAction = ConstSRS.CardLeechAction.Suspend
    };
  }
}
