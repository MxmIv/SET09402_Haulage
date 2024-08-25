using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using HaulageApp.Data;
using HaulageApp.Models;

namespace HaulageApp.ViewModels
{
    public class TripItemViewModel : INotifyPropertyChanged
    {
        private readonly HaulageDbContext _context;
        private Trip _trip;
        private bool _isEditing;

        private string _tempStartTimeString;
        private string _tempEndTimeString;
        
        public ICommand GoToExpensesCommand { get; set; }

        public TripItemViewModel(Trip trip, HaulageDbContext context)
        {
            _trip = trip;
            _context = context;

            _tempStartTimeString = _trip.StartTime.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            _tempEndTimeString = _trip.EndTime?.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture) ?? string.Empty;

            ToggleEditCommand = new Command(async () =>
            {
                if (IsEditing)
                {
                    await SaveTrip();
                }

                IsEditing = !IsEditing;

                OnPropertyChanged(nameof(EditSaveButtonText));
                OnPropertyChanged(nameof(IsEditing));
            });
            
            GoToExpensesCommand = new AsyncRelayCommand(GoToExpensesAsync);
        }

        public int TripId => _trip.Id;
        
        private async Task GoToExpensesAsync()
        {
            await Shell.Current.GoToAsync("expenses",
                new Dictionary<string, object> { { "trip", _trip } });
        }

        public string Status
        {
            get => _trip.Status;
            set
            {
                if (_trip.Status != value)
                {
                    _trip.Status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        public string StartTimeString
        {
            get => _tempStartTimeString;
            set
            {
                if (_tempStartTimeString != value)
                {
                    _tempStartTimeString = value;
                    OnPropertyChanged(nameof(StartTimeString));
                }
            }
        }

        public string EndTimeString
        {
            get => _tempEndTimeString;
            set
            {
                if (_tempEndTimeString != value)
                {
                    _tempEndTimeString = value;
                    OnPropertyChanged(nameof(EndTimeString));
                }
            }
        }

        public string EditSaveButtonText => IsEditing ? "Save" : "Edit";

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                if (_isEditing != value)
                {
                    _isEditing = value;
                    OnPropertyChanged(nameof(IsEditing));
                    OnPropertyChanged(nameof(EditSaveButtonText)); // Notify change here to update button text
                }
            }
        }

        public ObservableCollection<Waypoint> Waypoints => new ObservableCollection<Waypoint>(_trip.Waypoints);

        public ICommand ToggleEditCommand { get; }

        public async Task SaveTrip()
        {
            try
            {
                if (DateTime.TryParseExact(_tempStartTimeString, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedStartTime))
                {
                    _trip.StartTime = parsedStartTime;
                }

                if (DateTime.TryParseExact(_tempEndTimeString, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedEndTime))
                {
                    _trip.EndTime = parsedEndTime;
                }
                else
                {
                    _trip.EndTime = null;
                }

                if (_trip.EndTime.HasValue)
                {
                    if (_trip.Status != "completed")
                    {
                        Status = "completed";
                    }
                }
                else
                {
                    Status = "ongoing";
                }

                _trip.UpdatedAt = DateTime.Now;

                if (_context.Entry(_trip).State == Microsoft.EntityFrameworkCore.EntityState.Detached)
                {
                    _context.Attach(_trip);
                }

                _context.Update(_trip);
                await _context.SaveChangesAsync();

                OnPropertyChanged(nameof(Status));
                OnPropertyChanged(nameof(StartTimeString));
                OnPropertyChanged(nameof(EndTimeString));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving trip: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
