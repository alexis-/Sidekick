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

namespace Sidekick.Windows.Services
{
  using System;
  using System.Collections.Generic;

  using Sidekick.Windows.Models;

  /// <summary>Navigation menu. Obsolete.</summary>
  public class NavigationMenuService
  {
    #region Fields

    private readonly SortedSet<MenuItem> _menuItems;

    #endregion



    #region Constructors

    /// <summary>Initializes a new instance of the <see cref="NavigationMenuService" /> class.</summary>
    public NavigationMenuService()
    {
      _menuItems = new SortedSet<MenuItem>();
    }

    #endregion



    #region Properties

    /// <summary>Gets the menu items.</summary>
    public IEnumerable<MenuItem> MenuItems => _menuItems;

    #endregion



    #region Methods

    /// <summary>Adds the specified localized text.</summary>
    /// <param name="localizedText">The localized text.</param>
    /// <param name="iconResource">The icon resource.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="viewModelType">Type of the view model.</param>
    public void Add(string localizedText, string iconResource, int priority, Type viewModelType)
    {
      _menuItems.Add(new MenuItem(localizedText, iconResource, priority, viewModelType));
    }

    #endregion
  }
}