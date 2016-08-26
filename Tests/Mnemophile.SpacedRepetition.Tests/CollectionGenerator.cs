using System;
using System.Collections.Generic;
using System.Linq;
using Mnemophile.Interfaces.DB;
using Mnemophile.SpacedRepetition.Impl;
using Mnemophile.SpacedRepetition.Models;
using Ploeh.AutoFixture;

namespace Mnemophile.SpacedRepetition.Tests
{
  public class CollectionGenerator
  {
    // Config
    private int NoteCount { get; set; }
    private int MaxCardPerNote { get; }

    // Misc
    private HashSet<int> NotesIds { get; }

    private Fixture Fixture { get; }
    private CardGenerator CardGenerator { get; }
    private TimeGenerator TimeGenerator { get; }
    private CollectionConfig Config { get; }

    private IDatabase Db { get; }

    private static readonly Random Random = new Random();


    //
    // Constructor

    public CollectionGenerator(
      CardGenerator cardGenerator,
      int noteCount, int maxCardPerNote)
      : this(cardGenerator, null, noteCount, maxCardPerNote)
    {
    }

    public CollectionGenerator(
      CardGenerator cardGenerator, IDatabase db,
      int noteCount, int maxCardPerNote)
    {
      NotesIds = new HashSet<int>();

      Db = db;

      CardGenerator = cardGenerator;
      Fixture = CardGenerator.Fixture;
      TimeGenerator = CardGenerator.TimeGenerator;
      Config = CardGenerator.Config;

      NoteCount = noteCount;
      MaxCardPerNote = maxCardPerNote;
    }

    public IEnumerable<Note> Generate()
    {
      IEnumerable<Note> notes;

      // Generate notes
      if (Db != null)
      {
        Db.InsertAll(MakeNotes());

        notes = Db.Table<Note>().ToList();

        Db.BeginTransaction();
      }
      else
        notes = MakeNotes();


      // Generate cards
      foreach (Note note in notes)
      {
        int cardsLeft = CardGenerator.Left();
        int minCards = Math.Min(cardsLeft,
          (int)((float)cardsLeft / (float)NoteCount) >= MaxCardPerNote
          ? MaxCardPerNote
          : (NoteCount == 1 ? cardsLeft : 1));
        int maxCards = 1 + Math.Min(
          (int)((float)cardsLeft / (float)NoteCount) <= 1
          ? 1 : MaxCardPerNote,
          cardsLeft);

        int cardCount = Random.Next(minCards, maxCards);

        for (int i = 0; i < cardCount; i++)
        {
          Card card = CardGenerator.Generate(note.Id);

          note.Cards.Add(card);
          Db?.Insert(card);
        }

        NoteCount--;
      }

      Db?.CommitTransaction();

      return notes;
    }

    private IEnumerable<Note> MakeNotes()
    {
      return Fixture
        .Build<Note>()
        .Do(n => n.Id = TimeGenerator.RandomId(NotesIds))
        .Without(n => n.Cards)
        .Without(n => n.Id)
        .CreateMany(NoteCount);
    }
  }
}
