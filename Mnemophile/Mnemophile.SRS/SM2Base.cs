using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mnemophile.SRS.Models;
using Mnemophile.SRS.Impl;

namespace Mnemophile.SRS
{
  public abstract class SM2Base
  {
    private const string NAME = "SM2";
    private const string VERSION = "0.1";

    public int Id => Name.GetHashCode();
    public string Name => NAME;
    public string Version => VERSION;

    internal void Test(Card c)
    {
      c.Answer(0);
    }
  }
}
