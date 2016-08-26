using System;

namespace Sidekick.Shared.Utils.LazyLoad.Attributes
{
  [AttributeUsage(AttributeTargets.Property)]
  public class LazyUnloadedAttribute : Attribute { }
}