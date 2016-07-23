using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mnemophile.Interfaces.SRS
{
  public interface INote
  {
     ICard CreateCard(string[] data);
  }
}
