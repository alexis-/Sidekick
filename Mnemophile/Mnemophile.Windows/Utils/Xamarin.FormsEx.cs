using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Mnemophile.Windows.Utils
{
  public static class XamarinFormsEx
  {
    public static Color ToWPFColor(this Xamarin.Forms.Color color)
    {
      return Color.FromArgb(
        (byte)(color.A * 255),
        (byte)(color.R * 255),
        (byte)(color.G * 255),
        (byte)(color.B * 255));
    }
  }
}
