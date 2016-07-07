using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mnemophile.Const.SRS;
using Mnemophile.Utils;

namespace Mnemophile.SRS.Models
{
  // TODO: Create attributes for UI preferences
  internal class CollectionConfig
  {
    public bool BroadGradingMode { get; set; }
    public Delay[] LearningSteps { get; set; }
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
    public float ReviewHardEaseModifier { get; set; }
    public float ReviewGoodEaseModifier { get; set; }
    public float ReviewEasyEaseModifier { get; set; }
    public Delay[] LapseSteps { get; set; }
    public float LapseEaseMalus { get; set; }
    public float LapseIntervalFactor { get; set; }
    public int LapseMinInterval { get; set; }
    public int LeechThreshold { get; set; }
    public ConstSRS.CardLeechAction LeechAction { get; set; }

    public static CollectionConfig Default = new CollectionConfig
    {
      // Default grading mode is set to four grading options
      BroadGradingMode = false,
      // Default delays upon review of new card: 1 then 10 minutes (2 reviews)
      LearningSteps = new Delay[] { 60, 600 },
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
      // Default reviewing ease modifier from "hard" answer: -15%
      ReviewHardEaseModifier = -0.15f,
      // Default reviewing ease modifier from "good" answer: 0%
      ReviewGoodEaseModifier = 0.0f,
      // Default reviewing ease modifier from "easy" answer: 15%
      ReviewEasyEaseModifier = 0.15f,
      // Default delays upon card lapse: 10 minutes (1 review)
      LapseSteps = new Delay[] { 600 },
      // Default value substracted from card's ease when lapsing: -20%
      LapseEaseMalus = -0.2f,
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
