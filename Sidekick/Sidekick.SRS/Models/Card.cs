using System;
using System.Text;
using Newtonsoft.Json;
using Sidekick.Shared.Attributes.DB;
using Sidekick.Shared.Base.SRS;
using Sidekick.Shared.Const.SpacedRepetition;
using Sidekick.Shared.Utils;
using Sidekick.SpacedRepetition.Impl;

namespace Sidekick.SpacedRepetition.Models
{
  [Table("Cards")]
  public partial class Card : BaseCard
  {
    public Card()
    {
    }

    public Card(CollectionConfig config)
      : this(config, -1, null)
    {
    }

    public Card(CollectionConfig config, int noteId, string data)
    {
      Id = DateTime.Now.UnixTimestamp();
      Config = config;
      NoteId = noteId;
      LastModified = Id;

      Due = 0;
      PracticeState = ConstSpacedRepetition.CardPracticeState.New;
      MiscState = ConstSpacedRepetition.CardMiscStateFlag.None;

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

    [Indexed]
    public short PracticeState { get; set; }

    [Indexed]
    public ConstSpacedRepetition.CardMiscStateFlag MiscState { get; private set; }

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
