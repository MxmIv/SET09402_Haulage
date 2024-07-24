using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using HaulageApp.Models;
using HaulageApp.Data;

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

        public NoteViewModel(HaulageDbContext notesDbContext)
        {
            _context = notesDbContext;
            _note = new Note();
        }

        public NoteViewModel(HaulageDbContext notesDbContext, Note note)
        {
            _note = note;
            _context = notesDbContext;
        }

        [RelayCommand]
        private async Task Save()
        {
            _note.CreatedAt = DateTime.Now;
            if (_note.Id == 0)
            {
                _context.Notes.Add(_note);
            }
            _context.SaveChanges();
            await Shell.Current.GoToAsync($"..?saved={_note.Id}");
        }

        [RelayCommand]
        private async Task Delete()
        {
            _context.Remove(_note);
            _context.SaveChanges();
            await Shell.Current.GoToAsync($"..?deleted={_note.Id}");
        }

        void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("load"))
            {
                _note = _context.Notes.Single(n => n.Id == int.Parse(query["load"].ToString()));
                RefreshProperties();
            }
        }

        public void Reload()
        {
            _context.Entry(_note).Reload();
            RefreshProperties();
        }

        private void RefreshProperties()
        {
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(Content));
            OnPropertyChanged(nameof(CreatedAt));
        }
    }
}
