using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mnemophile.Attributes.SRS
{
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class SRSSettingsItem : Attribute
  {
    public SRSSettingsItem(int categoryIndex)
    {
      CategoryIndex = categoryIndex;
    }

    public int CategoryIndex { get; }
    public string CategoryLabel { get; set; }

    // TODO: Description, constraints, ...
  }
}
