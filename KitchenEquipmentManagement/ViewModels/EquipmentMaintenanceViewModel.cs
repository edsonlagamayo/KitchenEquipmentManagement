using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using KitchenEquipmentManagement.Data;
using KitchenEquipmentManagement.Models;
using KitchenEquipmentManagement.Services;
using KitchenEquipmentManagement.Utilities;
using RelayCommand = KitchenEquipmentManagement.Utilities.RelayCommand;

namespace KitchenEquipmentManagement.ViewModels
{
    public class EquipmentMaintenanceViewModel : BaseViewModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthenticationService _authService;
        private ObservableCollection<Equipment> _equipment = new();
        private Equipment? _selectedEquipment;
        private bool _isEditMode;
        private bool _isAddMode;
        private string _serialNumber = string.Empty;
        private string _description = string.Empty;
        private string _selectedCondition = "Working";

        public EquipmentMaintenanceViewModel(ApplicationDbContext context, IAuthenticationService authService)
        {
            _context = context;
            _authService = authService;

            AddCommand = new RelayCommand(AddEquipment);
            EditCommand = new RelayCommand(EditEquipment, CanEditEquipment);
            DeleteCommand = new RelayCommand(async () => await DeleteEquipmentAsync(), CanDeleteEquipment);
            SaveCommand = new RelayCommand(async () => await SaveEquipmentAsync(), CanSaveEquipment);
            CancelCommand = new RelayCommand(CancelEdit);

            Conditions = new List<string> { "Working", "Not Working" };

            _ = LoadEquipmentAsync();
        }

        public ObservableCollection<Equipment> Equipment
        {
            get => _equipment;
            set => SetProperty(ref _equipment, value);
        }

        public Equipment? SelectedEquipment
        {
            get => _selectedEquipment;
            set
            {
                SetProperty(ref _selectedEquipment, value);
                ((RelayCommand)EditCommand).RaiseCanExecuteChanged();
                ((RelayCommand)DeleteCommand).RaiseCanExecuteChanged();
            }
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        public bool IsAddMode
        {
            get => _isAddMode;
            set => SetProperty(ref _isAddMode, value);
        }

        public string SerialNumber
        {
            get => _serialNumber;
            set
            {
                SetProperty(ref _serialNumber, value);
                ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                SetProperty(ref _description, value);
                ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }

        public string SelectedCondition
        {
            get => _selectedCondition;
            set => SetProperty(ref _selectedCondition, value);
        }

        public List<string> Conditions { get; }

        public bool IsFormVisible => IsEditMode || IsAddMode;

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        private async Task LoadEquipmentAsync()
        {
            await ExecuteAsync(async () =>
            {
                var currentUserId = _authService.CurrentUser?.UserId;
                if (currentUserId == null) return;

                var equipment = await _context.Equipment
                    .Include(e => e.User)
                    .Include(e => e.RegisteredEquipments)
                        .ThenInclude(re => re.Site)
                    .Where(e => e.UserId == currentUserId)
                    .OrderBy(e => e.Description)
                    .ToListAsync();

                Equipment = new ObservableCollection<Equipment>(equipment);
            });
        }

        private bool CanEditEquipment()
        {
            return SelectedEquipment != null && !IsLoading && !IsEditMode && !IsAddMode;
        }

        private bool CanDeleteEquipment()
        {
            return SelectedEquipment != null && !IsLoading && !IsEditMode && !IsAddMode;
        }

        private bool CanSaveEquipment()
        {
            return (IsEditMode || IsAddMode) && !IsLoading &&
                   !string.IsNullOrWhiteSpace(SerialNumber) &&
                   !string.IsNullOrWhiteSpace(Description);
        }

        private void AddEquipment()
        {
            SerialNumber = string.Empty;
            Description = string.Empty;
            SelectedCondition = "Working";
            IsAddMode = true;
            OnPropertyChanged(nameof(IsFormVisible));
        }

        private void EditEquipment()
        {
            if (SelectedEquipment != null)
            {
                SerialNumber = SelectedEquipment.SerialNumber;
                Description = SelectedEquipment.Description;
                SelectedCondition = SelectedEquipment.Condition;
                IsEditMode = true;
                OnPropertyChanged(nameof(IsFormVisible));
            }
        }

        private async Task DeleteEquipmentAsync()
        {
            if (SelectedEquipment == null) return;

            var currentSite = SelectedEquipment.CurrentSite;
            string message = currentSite != null
                ? $"Are you sure you want to delete equipment '{SelectedEquipment.Description}' (Serial: {SelectedEquipment.SerialNumber})?\n\nThis equipment is currently assigned to site '{currentSite.Description}'."
                : $"Are you sure you want to delete equipment '{SelectedEquipment.Description}' (Serial: {SelectedEquipment.SerialNumber})?";

            var result = MessageBox.Show(message, "Confirm Delete",
                                       MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                await ExecuteAsync(async () =>
                {
                    _context.Equipment.Remove(SelectedEquipment);
                    await _context.SaveChangesAsync();
                    await LoadEquipmentAsync();
                    SelectedEquipment = null;
                });
            }
        }

        private async Task SaveEquipmentAsync()
        {
            await ExecuteAsync(async () =>
            {
                var currentUserId = _authService.CurrentUser?.UserId;
                if (currentUserId == null)
                {
                    ShowError("User not authenticated.");
                    return;
                }

                // Check if serial number is unique (excluding current equipment if editing)
                var existingEquipment = await _context.Equipment
                    .FirstOrDefaultAsync(e => e.SerialNumber.ToLower() == SerialNumber.ToLower() &&
                                            (IsAddMode || e.EquipmentId != SelectedEquipment!.EquipmentId));

                if (existingEquipment != null)
                {
                    ShowError("Serial number already exists. Please use a unique serial number.");
                    return;
                }

                if (IsAddMode)
                {
                    var newEquipment = new Equipment
                    {
                        UserId = currentUserId.Value,
                        SerialNumber = SerialNumber.Trim(),
                        Description = Description.Trim(),
                        Condition = SelectedCondition,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now
                    };

                    _context.Equipment.Add(newEquipment);
                }
                else if (IsEditMode && SelectedEquipment != null)
                {
                    SelectedEquipment.SerialNumber = SerialNumber.Trim();
                    SelectedEquipment.Description = Description.Trim();
                    SelectedEquipment.Condition = SelectedCondition;
                    SelectedEquipment.ModifiedDate = DateTime.Now;
                }

                await _context.SaveChangesAsync();
                await LoadEquipmentAsync();

                CancelEdit();

                string successMessage = IsAddMode ? "Equipment added successfully!" : "Equipment updated successfully!";
                MessageBox.Show(successMessage, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }

        private void CancelEdit()
        {
            IsEditMode = false;
            IsAddMode = false;
            SerialNumber = string.Empty;
            Description = string.Empty;
            SelectedCondition = "Working";
            OnPropertyChanged(nameof(IsFormVisible));
        }
    }
}