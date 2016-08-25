using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Catel.IoC;
using Catel.Services;
using Mnemophile.Const.SRS;
using Mnemophile.MVVM.SpacedRepetition;
using Mnemophile.MVVM.ViewModels.SpacedRepetition;
using Mnemophile.Windows.Utils;
using UserControl = Catel.Windows.Controls.UserControl;

namespace Mnemophile.Windows.Views.SpacedRepetition
{
  /// <summary>
  /// Interaction logic for CardAnswerButtonsView.xaml
  /// </summary>
  public partial class CardAnswerButtonsView : UserControl
  {
    private readonly ILanguageService _languageService;
    private object _lastBuiltView = null;

    public CardAnswerButtonsView()
    {
      IServiceLocator serviceLocator = ServiceLocator.Default;

      _languageService = serviceLocator.ResolveType<ILanguageService>();

      InitializeComponent();
    }

    /// <summary>
    /// Called a new card is displayed, thus resulting in a new set of
    /// <see cref="ConstSRS.GradingInfo" /> and thus changing ViewModel.
    /// </summary>
    protected override void OnViewModelChanged()
    {
      if (ViewModel == null)
        return;

      // Remove last buttons if necessary
      if (_lastBuiltView != null)
        RemoveLogicalChild(_lastBuiltView);

      // Build and add new buttons
      _lastBuiltView = BuildView();
      AddChild(_lastBuiltView);
    }

    /// <summary>
    /// Builds grading buttons and associated controls.
    /// </summary>
    private UIElement BuildView()
    {
      //
      // Get ViewModel

      CardAnswerButtonsViewModel viewModel =
        ViewModel as CardAnswerButtonsViewModel;

      if (viewModel == null)
        throw new InvalidOperationException("ViewModel is NULL.");

      // {Grade Count} Buttons
      // {Grade Count} - 1 Buttons Separator
      int elementCount = viewModel.GradingInfos.Length * 2 - 1;


      //
      // Build Grid

      Grid grid = BuildGrid(elementCount);


      //
      // Build buttons view

      for (int i = 0; i < elementCount; i++)
      {
        ConstSRS.GradingInfo gradingInfo = viewModel.GradingInfos[i];

        UIElement element = i % 2 == 0
          ? (UIElement)BuildButton(gradingInfo)   // Button (Even)
          : BuildSeparator();                     // Separator (Odd)

        Grid.SetColumn(element, i);               // Grid.Column="{i}"
        grid.Children.Add(element);               // Add UIElement to Grid
      }

      return grid;
    }

    /// <summary>
    /// Builds Grid UI control with appropriate ColumnDefinition for buttons
    /// and separators
    /// </summary>
    /// <param name="elementCount">Button count</param>
    /// <returns>Setup Grid</returns>
    private Grid BuildGrid(int elementCount)
    {
      Grid grid = new Grid();

      for (int i = 0; i < elementCount; i++)
        grid.ColumnDefinitions.Add(
          new ColumnDefinition()
          {
            Width = (i % 2 == 0)
              ? new GridLength(1, GridUnitType.Star)
              : new GridLength(1)
          });

      return grid;
    }

    /// <summary>
    /// Builds a button based on GradingInfo's color and text
    /// </summary>
    /// <param name="gradingInfo">Grading info</param>
    /// <returns>Setup Button</returns>
    private Button BuildButton(ConstSRS.GradingInfo gradingInfo)
    {
      ConstSRS.Grade grade = gradingInfo.Grade;

      Button button = new Button();

      SolidColorBrush buttonColorBrush = new SolidColorBrush(
        grade.GetColor().ToWPFColor());
      string buttonText = gradingInfo.LocalizableText;

      button.Content = _languageService.GetString(buttonText);
      button.BorderBrush = buttonColorBrush;
      button.Foreground = buttonColorBrush;
      button.Command = null;
      button.CommandParameter = grade;

      return button;
    }

    /// <summary>
    /// Builds a button separator
    /// </summary>
    /// <returns>Separator</returns>
    private Rectangle BuildSeparator()
    {
      Rectangle rectangle = new Rectangle();

      rectangle.SetResourceReference(Shape.FillProperty, "GrayBrush9");
      rectangle.Width = 1;

      return rectangle;
    }
  }
}
