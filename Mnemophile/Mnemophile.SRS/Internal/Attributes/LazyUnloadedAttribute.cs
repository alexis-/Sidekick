using System;

namespace Mnemophile.SRS.Internal.Attributes
{
  [AttributeUsage(AttributeTargets.Property)]
  public class LazyUnloadedAttribute : Attribute { }
}