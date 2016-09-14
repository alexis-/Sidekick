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

namespace Sidekick.Windows.Models
{
  using System;

  using Catel.Data;

  /// <summary>Menu Item. Deprecated.</summary>
  /// <seealso cref="Catel.Data.ModelBase" />
  /// <seealso cref="System.IComparable{MenuItem}" />
  public class MenuItem : ModelBase, IComparable<MenuItem>
  {
    #region Constructors

    /// <summary>Initializes a new instance of the <see cref="MenuItem" /> class.</summary>
    /// <param name="localizableTitle">The localizable title.</param>
    /// <param name="iconResourceName">Name of the icon resource.</param>
    /// <param name="position">The position.</param>
    /// <param name="viewModelType">Type of the view model.</param>
    public MenuItem(
      string localizableTitle, string iconResourceName, int position, Type viewModelType)
    {
      Title = localizableTitle;
      IconResourceName = iconResourceName;
      Position = position;
      ViewModelType = viewModelType;
    }

    #endregion



    #region Properties

    /// <summary>Gets or sets the title.</summary>
    public string Title { get; set; }

    /// <summary>Gets or sets the name of the icon resource.</summary>
    public string IconResourceName { get; set; }

    /// <summary>Gets or sets the position.</summary>
    public int Position { get; set; }

    /// <summary>Gets or sets the type of the view model.</summary>
    public Type ViewModelType { get; set; }

    #endregion



    #region Methods

    /// <summary>Compares the current object with another object of the same type.</summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    ///   A value that indicates the relative order of the objects being compared. The
    ///   return value has the following meanings: Value Meaning Less than zero This object is
    ///   less than the <paramref name="other" /> parameter.Zero This object is equal to
    ///   <paramref name="other" />. Greater than zero This object is greater than
    ///   <paramref name="other" />.
    /// </returns>
    public int CompareTo(MenuItem other)
    {
      return Position.CompareTo(other.Position);
    }

    #endregion
  }
}