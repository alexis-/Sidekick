using System;
using System.Collections.Generic;
using Sidekick.Shared.Attributes.DB;
using Sidekick.Shared.Interfaces.SpacedRepetition;
using Sidekick.Shared.Utils;
using Sidekick.SpacedRepetition.Impl;

namespace Sidekick.SpacedRepetition.Models
{
  [Table("Notes")]
  public class Note : INote
  {
    [PrimaryKey]
    public int Id { get; set;  }
    public int LastModified { get; set; }

    [Ignore]
    public List<Card> Cards { get; set; }

    public Note()
    {
      Id = DateTime.Now.UnixTimestamp();
      LastModified = Id;
      Cards = new List<Card>();
    }

    public ICard CreateCard(CollectionConfig config, string data)
    {
      Card card = new Card(config, Id, data);

      Cards.Add(card);

      return card;
    }
  }
}
