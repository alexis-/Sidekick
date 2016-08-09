using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mnemophile.SRS.Models;
using Mnemophile.Tests;

namespace Mnemophile.Tests
{
  public class CardTestDb : TestDb
  {
    public CardTestDb()
    {
      CreateTable<Note>();
      CreateTable<Card>();
    }
  }
}
