using System;
using System.Text;
using Mnemophile.Attributes.DB;
using Mnemophile.Const.SRS;
using Mnemophile.Interfaces.SRS;
using Mnemophile.Utils;
using Newtonsoft.Json;

namespace Mnemophile.SRS.Models
{
  [Table("Cards")]
  public partial class Card : ICard
  {
    public Card()
    {
    }

    public Card(int noteId, string data)
    {
      Id = DateTime.Now.UnixTimestamp();
      NoteId = noteId;
      LastModified = Id;

      Due = 0;
      PracticeState = ConstSRS.CardPracticeState.New;
      MiscState = ConstSRS.CardMiscStateFlag.None;

      _eFactor = 0;
      _interval = 0;

      Reviews = 0;
      Lapses = 0;

      Data = data;
    }

    [PrimaryKey]
    public int Id { get; set; }

    [Indexed]
    public int NoteId { get; set; }

    public int LastModified { get; set; }

    [Indexed]
    public int Due { get; set; }

    private short _practiceState;
    [Indexed]
    public ConstSRS.CardPracticeState PracticeState
    {
      get
      {
        return _practiceState >= (short)ConstSRS.CardPracticeState.Learning
                 ? ConstSRS.CardPracticeState.Learning
                 : (ConstSRS.CardPracticeState)_practiceState;
      }
      set { _practiceState = (short)value; }
    }

    [Indexed]
    public ConstSRS.CardMiscStateFlag MiscState { get; private set; }

    private float _eFactor;
    public float EFactor
    {
      get { return _eFactor; }
      set { _eFactor = SanitizeEFactor(value); }
    }

    private int _interval;
    public int Interval
    {
      get { return _interval; }
      set { _interval = SanitizeInterval(value); }
    }

    public int Reviews { get; set; }
    public int Lapses { get; set; }
    
    [Column("Data")]
    public byte[] DataBackingField
    {
      get
      {
        return Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(Data));
      }
      set
      {
        string uniStr = Encoding.Unicode.GetString(value, 0, value.Length);
        Data = JsonConvert.DeserializeObject<string>(uniStr);
      }
    }

    [Ignore]
    public string Data { get; set; }
  }
}
