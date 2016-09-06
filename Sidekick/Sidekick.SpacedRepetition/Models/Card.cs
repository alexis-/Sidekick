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

using System;
using System.Text;
using Catel.ComponentModel;
using Catel.Data;
using Newtonsoft.Json;
using Sidekick.Shared.Attributes.Database;
using Sidekick.Shared.Extensions;
using Sidekick.SpacedRepetition.Const;

namespace Sidekick.SpacedRepetition.Models
{
  [Table("Cards")]
  public partial class Card : ModelBase
  {
    #region Fields

    private float _eFactor;
    private int _interval;

    #endregion

    #region Constructors

    public Card()
    {
    }

    public Card(CollectionConfig config, int noteId = -1, string data = null)
    {
      Id = DateTime.Now.UnixTimestamp();
      Config = config;
      NoteId = noteId;
      LastModified = Id;

      Due = 0;
      PracticeState = CardPracticeState.New;
      MiscState = CardMiscStateFlag.None;

      _eFactor = 0;
      _interval = 0;

      Reviews = 0;
      Lapses = 0;

      Data = data;
    }

    #endregion

    #region Properties

    [PrimaryKey]
    [DisplayName("SpacedRepetition_Card_Header_Id")]
    public int Id { get; set; }

    [Indexed]
    public int NoteId { get; set; }

    [DisplayName("SpacedRepetition_Card_Header_LastModified")]
    public int LastModified { get; set; }

    [Indexed]
    [DisplayName("SpacedRepetition_Card_Header_Due")]
    public int Due { get; set; }

    [Indexed]
    [DisplayName("SpacedRepetition_Card_Header_PracticeState")]
    public short PracticeState { get; set; }

    [Indexed]
    [DisplayName("SpacedRepetition_Card_Header_MiscState")]
    public CardMiscStateFlag MiscState
    { get; private set; }

    [DisplayName("SpacedRepetition_Card_Header_EFactor")]
    public float EFactor
    {
      get { return _eFactor; }
      set { _eFactor = SanitizeEFactor(value); }
    }

    [DisplayName("SpacedRepetition_Card_Header_Interval")]
    public int Interval
    {
      get { return _interval; }
      set { _interval = SanitizeInterval(value); }
    }

    [DisplayName("SpacedRepetition_Card_Header_Reviews")]
    public int Reviews { get; set; }
    [DisplayName("SpacedRepetition_Card_Header_Lapses")]
    public int Lapses { get; set; }

    [Ignore]
    public string Data { get; set; }

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

    #endregion
  }
}