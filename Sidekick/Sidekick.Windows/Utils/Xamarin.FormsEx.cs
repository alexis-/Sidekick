using System.Windows.Media;

namespace Sidekick.Windows.Utils
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
