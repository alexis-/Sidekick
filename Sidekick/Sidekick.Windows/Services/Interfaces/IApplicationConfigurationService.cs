using MahApps.Metro;

namespace Sidekick.Windows.Services.Interfaces
{
  public interface IApplicationConfigurationService
  {
    //
    // Theme

    /// <summary>
    /// Gets or sets the application theme
    /// </summary>
    AppTheme Theme { get; set; }
    /// <summary>
    /// Gets or sets the application theme accent
    /// </summary>
    Accent Accent { get; set; }
  }
}
