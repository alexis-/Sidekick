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

namespace Sidekick.SpacedRepetition.Const
{
  public struct CardPracticeState
  {
    //
    // Attribute & Constructor
    private readonly short _value;

    private CardPracticeState(short state)
    {
      _value = state;
    }


    //
    // States

    public const short Deleted = -1,
                       Due = 0,
                       New = 1,
                       Learning = 2;


    //
    // Core methods

    public override bool Equals(object obj)
    {
      CardPracticeState otherObj = (CardPracticeState)obj;

      return otherObj._value == this._value;
    }

    public bool Equals(CardPracticeState other)
    {
      return _value == other._value;
    }

    public override int GetHashCode()
    {
      return _value.GetHashCode();
    }

    public static implicit operator short(CardPracticeState state)
    {
      return state._value;
    }

    public static implicit operator CardPracticeState(short state)
    {
      return new CardPracticeState(state);
    }
  }
}