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
  using System.Collections.ObjectModel;

  using Catel.Collections;
  using Catel.Data;

  /// <summary>
  ///   CollectionQuery collection container.
  /// </summary>
  /// <seealso cref="Catel.Data.ModelBase" />
  public class CollectionQueries : ModelBase
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CollectionQueries"/> class.
    /// </summary>
    /// <remarks>
    /// Must have a public constructor in order to be serializable.
    /// </remarks>
    public CollectionQueries()
    {
      Queries = new ObservableCollection<CollectionQuery>();
    }

    /// <summary>
    ///   CollectionQuery observable collection. Used by CollectionQueryManagerService.
    /// </summary>
    public ObservableCollection<CollectionQuery> Queries { get; private set; }
  }
}