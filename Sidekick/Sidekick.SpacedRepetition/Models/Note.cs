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
using System.Collections.Generic;
using Catel.Data;
using Sidekick.Shared.Attributes.Database;
using Sidekick.Shared.Extensions;

namespace Sidekick.SpacedRepetition.Models
{
  [Table("Notes")]
  public class Note : ModelBase
  {
    #region Constructors

    public Note()
    {
      Id = DateTime.Now.UnixTimestamp();
      LastModified = Id;
      Cards = new List<Card>();
    }

    #endregion

    #region Properties

    [PrimaryKey]
    public int Id { get; set; }

    public int LastModified { get; set; }

    [Ignore]
    public List<Card> Cards { get; set; }

    #endregion

    #region Methods

    public Card CreateCard(CollectionConfig config, string data)
    {
      Card card = new Card(config, Id, data);

      Cards.Add(card);

      return card;
    }

    #endregion
  }
}