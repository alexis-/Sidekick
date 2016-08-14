namespace Mnemophile.SRS.Models
{
  public partial class Card
  {
    /// <summary>
    /// Performs a deep copy of current Card
    /// </summary>
    /// <returns>Deep copied clone</returns>
    public Card Clone()
    {
      return new Card
      {
        Id = this.Id,
        NoteId = this.NoteId,
        Config = this.Config,
        PracticeState = this.PracticeState,
        MiscState = this.MiscState,
        _eFactor = this.EFactor,
        _interval = this.Interval,
        CurrentReviewTime = this.CurrentReviewTime,
        Due = this.Due,
        Lapses = this.Lapses,
        LastModified = this.LastModified
      };
    }
  }
}