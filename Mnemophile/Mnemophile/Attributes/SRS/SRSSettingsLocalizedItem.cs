using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mnemophile.Attributes.SRS
{
  // TODO: Allow localized textual descriptions through Resource items
  // http://stackoverflow.com/questions/356464/localization-of-displaynameattribute
  public class SRSSettingsLocalizedItem : SRSSettingsItem
  {
    public SRSSettingsLocalizedItem(int categoryIndex) : base(categoryIndex)
    {
      throw new NotImplementedException();
    }
  }
}
