using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mnemophile.Attributes.DB;
using Mnemophile.Interfaces.SRS;
using Mnemophile.Utils;

namespace Mnemophile.SRS.Models
{
  [Table("Notes")]
  class Note : INote
  {
    [PrimaryKey]
    public int Id { get; }
    public int LastModified { get; set; }

    public Note()
    {
      Id = DateTime.Now.UnixTimestamp();
      LastModified = Id;
    }

    public ICard CreateCard(string[] data)
    {
      return new Card(Id, data);
    }
  }
}
