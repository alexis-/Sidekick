using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mnemophile.Attributes.SpacedRepetition
{
  [AttributeUsage(AttributeTargets.Class)]
  public class SpacedRepetitionSettingsDescriptorAttribute : Attribute
  {
    public SpacedRepetitionSettingsDescriptorAttribute() {}
  }
}
