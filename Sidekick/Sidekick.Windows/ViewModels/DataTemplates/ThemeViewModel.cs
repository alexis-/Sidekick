using System.Windows.Media;
using Catel.Fody;
using Catel.MVVM;
using MahApps.Metro;

namespace Sidekick.Windows.ViewModels.DataTemplates
{
  public class ThemeViewModel : ViewModelBase
  {
    public ThemeViewModel([NotNull]Accent accent)
    {
      ColorName = accent.Name;
      ColorBrush = accent.Resources["AccentColorBrush"] as Brush;
      BorderColorBrush = ColorBrush;
    }

    public ThemeViewModel([NotNull]AppTheme theme)
    {
      ColorName = theme.Name;
      ColorBrush = theme.Resources["WhiteColorBrush"] as Brush;
      BorderColorBrush = theme.Resources["BlackColorBrush"] as Brush;
    }


    
    public string ColorName { get; set; }
    public Brush ColorBrush { get; set; }
    public Brush BorderColorBrush { get; set; }
  }
}
