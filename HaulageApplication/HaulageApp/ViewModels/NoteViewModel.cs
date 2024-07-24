using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using HaulageApp.Models;
using HaulageApp.Data;
using Microsoft.Extensions.Logging;

namespace HaulageApp.ViewModels
{
    public partial class NoteViewModel : ObservableObject, IQueryAttributable
    {
        public string Title
        {
            get => _note.Title;
            set
            {
                if (_note.Title != value)
                {
                    _note.Title = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Content
        {
            get => _note.Content;
            set
            {
                if (_note.Content != value)
                {
                    _note.Content = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime CreatedAt => _note.CreatedAt;
        public int Id => _note.Id;

        private Note _note;
        private HaulageDbContext _context;
        private readonly ILogger<NoteViewModel> _logger;

        public NoteViewModel(HaulageDbContext notesDbContext, ILogger<NoteViewModel> logger)
        {
            _context = notesDbContext;
            _note = new Note();
            _logger = logger;
        }

        public NoteViewModel(HaulageDbContext notesDbContext, Note note, ILogger<NoteViewModel> logger)
        {
            _note = note;
            _context = notesDbContext;
            _logger = logger;
        }

        [RelayCommand]
        private async Task Save()
        {
            try
            {
                _note.CreatedAt = DateTime.Now;
                if (_note.Id == 0)
                {
                    _context.Notes.Add(_note);
                }
                _context.SaveChanges();
                _logger.LogInformation($"Note saved: {_note.Id}");
                await Shell.Current.GoToAsync($"..?saved={_note.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving note");
                await Shell.Current.DisplayAlert("Error", $"An error occurred while saving the note. {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task Delete()
        {
            try
            {
                _context.Remove(_note);
                _context.SaveChanges();
                _logger.LogInformation($"Note deleted: {_note.Id}");
                await Shell.Current.GoToAsync($"..?deleted={_note.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting note");
                await Shell.Current.DisplayAlert("Error", $"An error occurred while deleting the note. {ex.Message}", "OK");
            }
        }

        void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("load"))
            {
                try
                {
                    _note = _context.Notes.Single(n => n.Id == int.Parse(query["load"].ToString()));
                    RefreshProperties();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error loading note");
                }
            }
        }

        public void Reload()
        {
            try
            {
                _context.Entry(_note).Reload();
                RefreshProperties();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reloading note");
            }
        }

        private void RefreshProperties()
        {
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(Content));
            OnPropertyChanged(nameof(CreatedAt));
        }
    }
}
