using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mnemophile.Attributes.SRS
{
  [AttributeUsage(AttributeTargets.Class)]
  public class SRSSettingsDescriptorAttribute : Attribute
  {
    public SRSSettingsDescriptorAttribute() {}
  }
}
