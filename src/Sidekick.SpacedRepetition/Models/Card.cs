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

namespace Sidekick.SpacedRepetition.Models
{
  using System;
  using System.Diagnostics;
  using System.Text;

  using AgnosticDatabase.Attributes;

  using Catel.ComponentModel;
  using Catel.Data;

  using Newtonsoft.Json;

  using Sidekick.Shared.Extensions;

  /// <summary>
  ///   Card model for SpacedRepetition system. Keeps track of individual cards progress
  ///   (ease, interval, ...) and datas.
  /// </summary>
  /// <seealso cref="Catel.Data.ModelBase" />
  [Table("Cards")]
  public partial class Card : ModelBase
  {
    #region Fields

    private float _eFactor;
    private int _interval;

    #endregion



    #region Constructors

    /// <summary>Initializes a new instance of the <see cref="Card"/> class.</summary>
    public Card() { }

    /// <summary>Initializes a new instance of the <see cref="Card"/> class.</summary>
    /// <param name="config">The configuration.</param>
    /// <param name="noteId">The note identifier.</param>
    /// <param name="data">The data.</param>
    public Card(CollectionConfig config, int noteId = -1, string data = null)
    {
      Id = DateTime.Now.UnixTimestamp();
      Config = config;
      NoteId = noteId;
      LastModified = Id;

      Due = 0;
      PracticeState = Models.PracticeState.New;
      MiscState = CardMiscStateFlag.None;

      _eFactor = 0;
      _interval = 0;

      Reviews = 0;
      Lapses = 0;

      Data = data;
    }

    #endregion



    #region Properties

    /// <summary>Database field. Database identifier, date time of creation time - Unix timestamp.</summary>
    [PrimaryKey]
    [DisplayName("SpacedRepetition_Card_Header_Id")]
    public int Id { get; set; }

    /// <summary>Database Field. Associated note id to which this card belong.</summary>
    [Indexed]
    public int NoteId { get; set; }

    /// <summary>
    ///   Database Field. Last time this card was modified, either by reviewing, or by editing
    ///   content - Unix timestamp.
    /// </summary>
    [DisplayName("SpacedRepetition_Card_Header_LastModified")]
    public int LastModified { get; set; }

    /// <summary>Database Field. Next review time - Unix timestamp.</summary>
    [Indexed]
    [DisplayName("SpacedRepetition_Card_Header_Due")]
    public int Due { get; set; }

    /// <summary>Database Field. Main card state (new, learning, due, ...). Also see
    ///   <see cref="Models.PracticeState" />
    /// </summary>
    [Indexed]
    [DisplayName("SpacedRepetition_Card_Header_PracticeState")]
    public short PracticeState { get; set; }

    /// <summary>
    ///   Database Field. Other card states (suspended, dismissed, ...). See
    ///   <see cref="CardMiscStateFlag" />
    /// </summary>
    [Indexed]
    [DisplayName("SpacedRepetition_Card_Header_MiscState")]
    public CardMiscStateFlag MiscState { get; set; }

    /// <summary>Database Field. Ease factor which relates how easily this card was remembered.</summary>
    [DisplayName("SpacedRepetition_Card_Header_EFactor")]
    public float EFactor { get { return _eFactor; } set { _eFactor = SanitizeEFactor(value); } }

    /// <summary>Database Field. Current interval between reviews in days.</summary>
    [DisplayName("SpacedRepetition_Card_Header_Interval")]
    public int Interval
    {
      get { return _interval; }
      set { _interval = SanitizeInterval(value); }
    }

    /// <summary>Database Field. Number of time this card was reviewed.</summary>
    [DisplayName("SpacedRepetition_Card_Header_Reviews")]
    public int Reviews { get; set; }

    /// <summary>Database Field. How many time this card has lapsed.</summary>
    [DisplayName("SpacedRepetition_Card_Header_Lapses")]
    public int Lapses { get; set; }

    /// <summary>Data access property.</summary>
    [Ignore]
    [DisplayName("SpacedRepetition_Card_Header_Data")]
    public string Data { get; set; }

    /// <summary>
    ///   Database Field. Actual data of the card (what to display). Wrapper property which
    ///   serializes and deserializes the content.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Column("Data")]
    public byte[] DataBackingField
    {
      get { return Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(Data)); }
      set
      {
        string uniStr = Encoding.Unicode.GetString(value, 0, value.Length);
        Data = JsonConvert.DeserializeObject<string>(uniStr);
      }
    }

    #endregion
  }
}