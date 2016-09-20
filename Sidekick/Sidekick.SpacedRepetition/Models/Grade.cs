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

namespace Sidekick.SpacedRepetition.Models
{
  /// <summary>Describe the grading rated by user during a card review.</summary>
  public struct Grade
  {
    //
    // Grades

    /// <summary>Grade values</summary>
    public const int Dismiss = -1,
                     FailSevere = 0,
                     FailMedium = 1,
                     Fail = 2,
                     Hard = 3,
                     Good = 4,
                     Easy = 5;

    //
    // Attribute & Constructor
    private readonly int _value;


    /// <summary>Initializes a new instance of the <see cref="Grade" /> struct.</summary>
    /// <param name="grade">The grade value.</param>
    private Grade(int grade)
    {
      _value = grade;
    }

    /// <summary>
    ///   Performs an implicit conversion from <see cref="Grade" /> to
    ///   <see cref="System.Int32" />.
    /// </summary>
    /// <param name="grade">The grade.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator int(Grade grade)
    {
      return grade._value;
    }

    /// <summary>
    ///   Performs an implicit conversion from <see cref="System.Int32" /> to
    ///   <see cref="Grade" />.
    /// </summary>
    /// <param name="grade">The grade.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator Grade(int grade)
    {
      return new Grade(grade);
    }

    /// <summary>
    ///   Determines whether the specified <see cref="System.Object" />, is equal to this
    ///   instance.
    /// </summary>
    /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
    /// <returns>
    ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance;
    ///   otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object obj)
    {
      Grade otherObj = (Grade)obj;

      return otherObj._value == _value;
    }

    /// <summary>Equalses the specified other.</summary>
    /// <param name="other">The other.</param>
    /// <returns></returns>
    public bool Equals(Grade other)
    {
      return _value == other._value;
    }

    /// <summary>Returns a hash code for this instance.</summary>
    /// <returns>
    ///   A hash code for this instance, suitable for use in hashing algorithms and data
    ///   structures like a hash table.
    /// </returns>
    public override int GetHashCode()
    {
      return _value.GetHashCode();
    }
  }
}