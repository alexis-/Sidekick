using System;
using Mnemophile.Interfaces.SRS;
using Mnemophile.Utils;

namespace Mnemophile.SRS.Models
{
  public partial class Card : ICard
  {
    private Card() {}

    public Card(int noteId, int eFactor)
    {
      Id = DateTime.Now.UnixTimestamp();
      NoteId = noteId;
      LastModified = Id;

      Due = 0;
      State = CardStateFlag.New;

      EFactor = eFactor;
      Interval = 0;

      Reviews = 0;
      Lapses = 0;
    }

    public int Id { get; private set;  }
    public int NoteId { get; set; }
    public int LastModified { get; set; }

    public int Due { get; set; }
    public CardStateFlag State { get; private set; }

    private float _eFactor;
    public float EFactor
    {
      get { return _eFactor; }
      set { _eFactor = SanitizeEFactor(value); }
    }
    private int _interval;
    public int Interval {
      get { return _interval; }
      set { _interval = SanitizeInterval(value); }
    }

    public int Reviews { get; set; }
    public int Lapses { get; set; }
  }
}
