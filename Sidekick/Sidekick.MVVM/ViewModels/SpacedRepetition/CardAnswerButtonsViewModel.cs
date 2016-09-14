// 
// The MIT License (MIT)
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

namespace Sidekick.MVVM.ViewModels.SpacedRepetition
{
  using Catel.Fody;
  using Catel.MVVM;

  using Sidekick.SpacedRepetition.Const;

  /// <summary>
  ///   Review buttons handling View Model
  /// </summary>
  /// <seealso cref="Catel.MVVM.ViewModelBase" />
  public class CardAnswerButtonsViewModel : ViewModelBase
  {
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="CardAnswerButtonsViewModel"/> class.
    /// </summary>
    /// <param name="gradeInfos">The grade infos.</param>
    public CardAnswerButtonsViewModel([NotNull] GradeInfo[] gradeInfos)
      : base(false)
    {
      GradeInfos = gradeInfos;

      AnswerCommand = new Command(OnAnswerCommandExecute);
    }

    #endregion



    #region Properties

    /// <summary>
    /// Gets or sets the grade infos.
    /// </summary>
    public GradeInfo[] GradeInfos { get; set; }

    /// <summary>
    /// Gets or sets the answer command.
    /// </summary>
    public ICatelCommand AnswerCommand { get; set; }

    #endregion



    #region Methods

    private void OnAnswerCommandExecute() { }

    #endregion
  }
}