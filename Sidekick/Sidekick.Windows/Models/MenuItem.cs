﻿// 
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

using System;
using Catel.Data;

namespace Sidekick.Windows.Models
{
  public class MenuItem : ModelBase, IComparable<MenuItem>
  {
    #region Constructors

    public MenuItem(
      string localizableTitle, string iconResourceName,
      int position, Type viewModelType)
    {
      Title = localizableTitle;
      IconResourceName = iconResourceName;
      Position = position;
      ViewModelType = viewModelType;
    }

    #endregion

    #region Properties

    public string Title { get; set; }
    public string IconResourceName { get; set; }
    public int Position { get; set; }
    public Type ViewModelType { get; set; }

    #endregion

    #region Methods

    public int CompareTo(MenuItem other)
    {
      return Position.CompareTo(other.Position);
    }

    #endregion

    //private string _localizableTitle;
  }
}