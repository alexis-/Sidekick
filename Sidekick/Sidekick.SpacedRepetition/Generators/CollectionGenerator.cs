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

namespace Sidekick.SpacedRepetition.Generators
{
  using System;
  using System.Collections.Generic;
  using System.Threading.Tasks;

  using Sidekick.Shared.Interfaces.Database;
  using Sidekick.SpacedRepetition.Models;

  /// <summary>
  ///   Generates a random collection of notes and cards.
  ///   Can be parametrized.
  /// </summary>
  public class CollectionGenerator
  {
    #region Fields

    private static readonly Random Random = new Random();

    #endregion



    #region Constructors

    // 
    // Constructor

    /// <summary>
    ///   Initializes a new instance of the <see cref="CollectionGenerator" /> class.
    /// </summary>
    /// <param name="cardGenerator">Parametrized card generator.</param>
    /// <param name="noteCount">Note count to generate.</param>
    /// <param name="maxCardPerNote">Maximum number of card per note.</param>
    public CollectionGenerator(CardGenerator cardGenerator, int noteCount, int maxCardPerNote)
    {
      CardGenerator = cardGenerator;
      TimeGenerator = CardGenerator.TimeGenerator;

      NotesIds = new HashSet<int>();
      NoteCount = noteCount;
      MaxCardPerNote = maxCardPerNote;
    }

    #endregion



    #region Properties

    // Config
    private int NoteCount { get; set; }
    private int MaxCardPerNote { get; }

    // Misc
    private HashSet<int> NotesIds { get; }

    private CardGenerator CardGenerator { get; }
    private TimeGenerator TimeGenerator { get; }

    #endregion



    #region Methods

    /// <summary>
    ///   Generate a new collection based on specified parameters.
    ///   If a <see cref="IDatabaseAsync" /> instance is specified, collection will also be saved
    ///   to Database.
    /// </summary>
    /// <param name="db">Database instance.</param>
    /// <returns>
    ///   Generated collection
    /// </returns>
    public async Task<IEnumerable<Note>> GenerateAsync(IDatabaseAsync db)
    {
      if (db == null)
        return Generate();

      IEnumerable<Note> notes = MakeNotes();

      // Generate notes
      await db.InsertAllAsync(notes).ConfigureAwait(false);

      // Generate cards
      await db.RunInTransactionAsync(dbSync => CreateCards(notes, dbSync));

      return notes;
    }

    /// <summary>
    ///   Generate a new collection based on specified parameters.
    /// </summary>
    /// <returns>Generated collection</returns>
    public IEnumerable<Note> Generate()
    {
      IEnumerable<Note> notes = MakeNotes();

      CreateCards(notes, null);

      return notes;
    }

    private void CreateCards(IEnumerable<Note> notes, IDatabase db)
    {
      // Generate cards
      foreach (Note note in notes)
      {
        int cardsLeft = CardGenerator.Left();

        double lowerCardPerNoteRatio = Math.Floor(cardsLeft / (double)NoteCount);
        double upperCardPerNoteRatio = Math.Ceiling(cardsLeft / (double)NoteCount);

        int upperMinCardRange = upperCardPerNoteRatio >= MaxCardPerNote
                                  ? MaxCardPerNote
                                  : (NoteCount == 1 ? cardsLeft : 1);

        int minCards = Math.Min(cardsLeft, upperMinCardRange);
        int maxCards = 1 + Math.Min(lowerCardPerNoteRatio <= 1 ? 1 : MaxCardPerNote, cardsLeft);

        int cardCount = Random.Next(minCards, maxCards);

        for (int i = 0; i < cardCount; i++)
        {
          Card card = CardGenerator.Generate(note.Id);

          note.Cards.Add(card);
          db?.Insert(card);
        }

        NoteCount--;
      }
    }

    private IEnumerable<Note> MakeNotes()
    {
      List<Note> notes = new List<Note>(NoteCount);

      for (int i = 0; i < NoteCount; i++)
      {
        int id = TimeGenerator.RandomId(NotesIds);

        notes.Add(new Note { Id = id, LastModified = Math.Max(TimeGenerator.RandomTime(), id) });
      }

      return notes;
    }

    #endregion
  }
}