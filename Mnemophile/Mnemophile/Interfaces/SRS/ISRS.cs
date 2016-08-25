using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catel.Services;

namespace Mnemophile.Interfaces.SRS
{
  public interface ISRS
  {
    INote CreateNote();

    ILanguageSource GetLanguageSource();
  }
}
