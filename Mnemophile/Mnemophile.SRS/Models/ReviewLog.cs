using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mnemophile.Attributes.DB;
using Mnemophile.Const.SRS;

namespace Mnemophile.SRS.Models
{
  [Table("ReviewsLogs")]
  public class ReviewLog
  {
    public ReviewLog() { }

    public ReviewLog(
      int id, int cardId,
      int lastDue, ConstSRS.CardPracticeState lastState,
      int lastInterval, float lastEFactor)
    {
      Id = id;
      CardId = cardId;
      LastDue = lastDue;
      LastState = lastState;
      LastInterval = lastInterval;
      LastEFactor = lastEFactor;
    }

    public ReviewLog(
      int id, int cardId,
      ConstSRS.Grade grade,
      int lastDue, int newDue,
      ConstSRS.CardPracticeState lastState,
      ConstSRS.CardPracticeState newState,
      int lastInterval, int newInterval,
      float lastEFactor, float newEFactor,
      int evalTime)
    {
      Id = id;
      CardId = cardId;
      Grade = grade;
      LastDue = lastDue;
      NewDue = newDue;
      LastState = lastState;
      NewState = newState;
      LastInterval = lastInterval;
      NewInterval = newInterval;
      LastEFactor = lastEFactor;
      NewEFactor = newEFactor;
      EvalTime = evalTime;
    }

    public void CompleteReview(
      ConstSRS.Grade grade,
      int newDue, ConstSRS.CardPracticeState newState, int newInterval,
      float newEFactor, int evalTime)
    {
      Grade = grade;
      NewDue = newDue;
      NewState = newState;
      NewInterval = newInterval;
      NewEFactor = newEFactor;
      EvalTime = evalTime;
    }

    public int Id { get; set; }
    public int CardId { get; set; }

    public ConstSRS.Grade Grade { get; set; }
    
    public int LastDue { get; set; }
    public int NewDue { get; set; }
    public ConstSRS.CardPracticeState LastState { get; set; }
    public ConstSRS.CardPracticeState NewState { get; set; }
    public int LastInterval { get; set; }
    public int NewInterval { get; set; }
    public float LastEFactor { get; set; }
    public float NewEFactor { get; set; }

    public int EvalTime { get; set; }
  }
}
