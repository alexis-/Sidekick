// 
// The MIT License (MIT)
// Copyright (c) 2016 Incogito
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

using System.Windows.Input;
using Catel.MVVM;
using Catel.Services;
using Ploeh.AutoFixture;
using Sidekick.Shared.Interfaces.DB;
using Sidekick.SpacedRepetition.Impl;
using Sidekick.SpacedRepetition.Tests;

namespace Sidekick.Windows.ViewModels.Settings
{
  public class SettingsSpacedRepetitionViewModel : ViewModelBase
  {
    private readonly IDatabase _database;
    private readonly IMessageService _messageService;

    #region Constructors

    public SettingsSpacedRepetitionViewModel(
      IDatabase database,
      IMessageService messageService)
    {
      _database = database;
      _messageService = messageService;

      GenerateTestCollectionCommand = new Command(
        OnGenerateTestCollectionCommandExecute);
    }

    #endregion

    #region Properties

    public ICommand GenerateTestCollectionCommand { get; set; }

    #endregion

    #region Methods

    public void OnGenerateTestCollectionCommandExecute()
    {
      var collectionGenerator = new CollectionGenerator(
        new CardGenerator(
          new Fixture(),
          new TimeGenerator(90),
          CollectionConfig.Default,
          200),
        _database,
        100, 3);

      collectionGenerator.Generate();

      _messageService.ShowInformationAsync(
        "Collection successfully Generated", "Collection Debugging");
    }

    #endregion
  }
}