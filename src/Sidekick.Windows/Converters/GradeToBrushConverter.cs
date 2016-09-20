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

namespace Sidekick.Windows.Converters
{
  using System;
  using System.Globalization;
  using System.Windows.Data;
  using System.Windows.Media;

  using Catel.MVVM.Converters;

  using Sidekick.MVVM.Extensions.SpacedRepetition;
  using Sidekick.SpacedRepetition.Models;
  using Sidekick.Windows.Extensions;

  /// <summary>Converts <see cref="Grade"/> to <see cref="SolidColorBrush" />.</summary>
  /// <seealso cref="System.Windows.Data.IMultiValueConverter" />
  public class GradeToBrushConverter : ValueConverterBase<Grade>
  {
    #region Methods

    /// <inheritdoc />
    protected override object Convert(Grade grade, Type targetType, object parameter)
    {
      return new SolidColorBrush(grade.GetColor().ToWPFColor());
    }

    #endregion
  }
}