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

namespace SQLite.Net.Platform.Test.Generic
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Linq.Expressions;
  using System.Reflection;

  using SQLite.Net.Interop;

  /// <summary>PCL reflection nightmare.</summary>
  /// <seealso cref="SQLite.Net.Interop.IReflectionService" />
  public class ReflectionServiceGeneric : IReflectionService
  {
    #region Methods

    /// <summary>Gets the public instance properties.</summary>
    /// <param name="mappedType">Type of the mapped.</param>
    /// <returns></returns>
    public IEnumerable<PropertyInfo> GetPublicInstanceProperties(Type mappedType)
    {
      return mappedType.GetTypeInfo().DeclaredProperties.Where(
                         pi =>
                         {
                           var setMethod = pi.SetMethod;

                           return pi.CanWrite && setMethod != null && !setMethod.IsStatic
                                  && setMethod.IsPublic;
                         });
    }

    /// <summary>Gets the member value.</summary>
    /// <param name="obj">The object.</param>
    /// <param name="expr">The expr.</param>
    /// <param name="member">The member.</param>
    /// <returns></returns>
    public object GetMemberValue(object obj, Expression expr, MemberInfo member)
    {
      if (member is PropertyInfo)
      {
        var m = (PropertyInfo)member;
        return m.GetValue(obj, null);
      }

      else if (member is FieldInfo)
      {
        var m = (FieldInfo)member;
        return m.GetValue(obj);
      }

      throw new NotSupportedException("MemberExpr: " + member.ToString());
    }

    #endregion
  }
}