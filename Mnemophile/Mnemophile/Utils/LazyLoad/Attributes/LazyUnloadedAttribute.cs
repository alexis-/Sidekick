using System;

namespace Mnemophile.Utils.LazyLoad.Attributes
{
  [AttributeUsage(AttributeTargets.Property)]
  public class LazyUnloadedAttribute : Attribute { }
}