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
  using System.Collections.Generic;

  using Catel.Data;

  using Sidekick.Shared.Attributes.Database;
  using Sidekick.Shared.Extensions;

  /// <summary>Note model for Spaced Repetition system.</summary>
  /// <seealso cref="Catel.Data.ModelBase" />
  [Table("Notes")]
  public class Note : ModelBase
  {
    #region Constructors

    /// <summary>Initializes a new instance of the <see cref="Note" /> class.</summary>
    public Note()
    {
      Id = DateTime.Now.UnixTimestamp();
      LastModified = Id;
      Cards = new List<Card>();
    }

    #endregion



    #region Properties

    /// <summary>
    ///   Database identifier, date time of creation time - Unix timestamp. Database
    ///   field.
    /// </summary>
    [PrimaryKey]
    public int Id { get; set; }

    /// <summary>Last modified date time - Unix timestamp Database field.</summary>
    public int LastModified { get; set; }

    /// <summary>Associated cards.</summary>
    [Ignore]
    public List<Card> Cards { get; set; }

    #endregion



    #region Methods

    /// <summary>Creates a card associated with this note.</summary>
    /// <param name="config">The configuration.</param>
    /// <param name="data">The data.</param>
    /// <returns></returns>
    public Card CreateCard(CollectionConfig config, string data)
    {
      Card card = new Card(config, Id, data);

      Cards.Add(card);

      return card;
    }

    #endregion
  }
}