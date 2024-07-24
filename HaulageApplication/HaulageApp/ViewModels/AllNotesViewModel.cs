using System.Collections;
using CommunityToolkit.Mvvm.Input;
using HaulageApp.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using HaulageApp.Data;
using Microsoft.Extensions.Logging;

namespace HaulageApp.ViewModels
{
    public class AllNotesViewModel : IQueryAttributable
    {
        public ObservableCollection<NoteViewModel> AllNotes { get; }
        public ICommand NewCommand { get; }
        public ICommand SelectNoteCommand { get; }

        private HaulageDbContext _context;
        private readonly ILogger<AllNotesViewModel> _logger;
        private readonly ILogger<NoteViewModel> _noteLogger;

        public AllNotesViewModel(HaulageDbContext notesContext, ILogger<AllNotesViewModel> logger, ILogger<NoteViewModel> noteLogger)
        {
            _context = notesContext;
            _logger = logger;
            _noteLogger = noteLogger;
            AllNotes = new ObservableCollection<NoteViewModel>(_context.Notes.ToList().Select(n => new NoteViewModel(_context, n, _noteLogger)));
            NewCommand = new AsyncRelayCommand(NewNoteAsync);
            SelectNoteCommand = new AsyncRelayCommand<NoteViewModel>(SelectNoteAsync);
        }

        private async Task NewNoteAsync()
        {
            try
            {
                await Shell.Current.GoToAsync(nameof(Views.NotePage));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error navigating to new note page");
                await Shell.Current.DisplayAlert("Error", "An error occurred while navigating to the new note page.", "OK");
            }
        }

        private async Task SelectNoteAsync(NoteViewModel? note)
        {
            if (note != null)
            {
                try
                {
                    await Shell.Current.GoToAsync($"{nameof(Views.NotePage)}?load={note.Id}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error navigating to selected note page");
                    await Shell.Current.DisplayAlert("Error", "An error occurred while navigating to the selected note page.", "OK");
                }
            }
        }

        void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query)
        {
            try
            {
                if (query.ContainsKey("deleted"))
                {
                    string noteId = query["deleted"].ToString();
                    NoteViewModel matchedNote = AllNotes.FirstOrDefault(n => n.Id == int.Parse(noteId));

                    if (matchedNote != null)
                    {
                        AllNotes.Remove(matchedNote);
                        _logger.LogInformation($"Note deleted: {noteId}");
                    }
                }
                else if (query.ContainsKey("saved"))
                {
                    string noteId = query["saved"].ToString();
                    NoteViewModel matchedNote = AllNotes.FirstOrDefault(n => n.Id == int.Parse(noteId));

                    if (matchedNote != null)
                    {
                        matchedNote.Reload();
                        AllNotes.Move(AllNotes.IndexOf(matchedNote), 0);
                        _logger.LogInformation($"Note updated: {noteId}");
                    }
                    else
                    {
                        AllNotes.Insert(0, new NoteViewModel(_context, _context.Notes.Single(n => n.Id == int.Parse(noteId)), _noteLogger));
                        _logger.LogInformation($"New note added: {noteId}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying query attributes");
            }
        }
    }
}
