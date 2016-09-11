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

using Catel.Services;
using Catel.Services.Models;
using Sidekick.Shared.Interfaces.Database;
using Sidekick.SpacedRepetition.Interfaces;
using Sidekick.SpacedRepetition.Models;
using Sidekick.SpacedRepetition.Review;

namespace Sidekick.SpacedRepetition
{
  public class SM2Impl : ISpacedRepetition
  {
    #region Constructors

    public SM2Impl()
    {
    }

    #endregion

    #region Properties

    public int Id => Name.GetHashCode();
    public string Name => "SM2";
    public string Version => "0.1";

    #endregion

    #region Methods

    //
    // Core methods

    public Note CreateNote()
    {
      return new Note();
    }

    public IReviewCollection GetReviewCollection(IDatabaseAsync db)
    {
      return new ReviewCollectionImpl(db, CollectionConfig.Default);
    }


    //
    // Misc

    public ILanguageSource GetLanguageSource()
    {
      return new LanguageResourceSource(
        "Sidekick.SpacedRepetition",
        "Sidekick.SpacedRepetition.Properties",
        "Resources");
    }

    #endregion
  }
}