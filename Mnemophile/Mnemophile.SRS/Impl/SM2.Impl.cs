using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mnemophile.Interfaces.DB;
using Mnemophile.Interfaces.SRS;
using Mnemophile.SRS.Models;
using Mnemophile.SRS.Impl;

namespace Mnemophile.SRS.Impl
{
  public class SM2Impl : ISRS
  {
    public int Id => Name.GetHashCode();
    public string Name => "SM2";
    public string Version => "0.1";

    //public IDatabase Database { get; }
    
    public static SM2Impl Instance { get; private set; }

    public static SM2Impl GetInstance()
    {
      return Instance ?? (Instance = new SM2Impl());
    }

    public INote CreateNote()
    {
      return new Note();
    }
  }
}
