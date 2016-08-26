// 
// The MIT License (MIT)
// Copyright (c) 2016 Incogito
// 
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the 
// Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Catel.IoC;
using Catel.Services;
using Sidekick.MVVM.SpacedRepetition;
using Sidekick.MVVM.ViewModels.SpacedRepetition;
using Sidekick.Shared.Const.SpacedRepetition;
using Sidekick.Windows.Utils;
using UserControl = Catel.Windows.Controls.UserControl;

namespace Sidekick.Windows.Views.SpacedRepetition
{
  /// <summary>
  /// Interaction logic for CardAnswerButtonsView.xaml
  /// </summary>
  public partial class CardAnswerButtonsView : UserControl
  {
    #region Fields

    //
    // Fields

    private readonly ILanguageService _languageService;

    #endregion

    #region Constructors

    //
    // Constructor

    public CardAnswerButtonsView()
    {
      IServiceLocator serviceLocator = ServiceLocator.Default;

      _languageService = serviceLocator.ResolveType<ILanguageService>();

      InitializeComponent();
    }

    #endregion

    #region Methods

    //
    // Methods

    /// <summary>
    /// Called a new card is displayed, thus resulting in a new set of
    /// <see cref="ConstSpacedRepetition.GradingInfo" /> and thus changing ViewModel.
    /// </summary>
    protected override void OnViewModelChanged()
    {
      // Remove last buttons if necessary
      Content = null;

      if (ViewModel == null)
        return;

      // Build and add new buttons
      AddChild(BuildView());
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
        UIElement element =
          (i % 2 == 0)
          // Button (Even)
          ? (UIElement)BuildButton(
            viewModel.GradingInfos[i / 2],
            viewModel.AnswerCommand)
          // Separator (Odd)
          : BuildSeparator();

        Grid.SetColumn(element, i); // Grid.Column="{i}"
        grid.Children.Add(element); // Add UIElement to Grid
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
    private Button BuildButton(
      ConstSpacedRepetition.GradingInfo gradingInfo,
      ICommand command)
    {
      ConstSpacedRepetition.Grade grade = gradingInfo.Grade;

      Button button = new Button();

      SolidColorBrush buttonColorBrush = new SolidColorBrush(
        grade.GetColor().ToWPFColor());
      string buttonText = gradingInfo.LocalizableText;

      button.Content = _languageService.GetString(buttonText);
      button.BorderBrush = buttonColorBrush;
      button.Foreground = buttonColorBrush;
      button.Command = command;
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

    #endregion
  }
}