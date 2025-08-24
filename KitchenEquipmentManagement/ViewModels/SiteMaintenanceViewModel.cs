using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using KitchenEquipmentManagement.Data;
using KitchenEquipmentManagement.Models;
using KitchenEquipmentManagement.Services;
using KitchenEquipmentManagement.Utilities;
using KitchenEquipmentManagement.Views;
using RelayCommand = KitchenEquipmentManagement.Utilities.RelayCommand;

namespace KitchenEquipmentManagement.ViewModels
{
    public class SiteMaintenanceViewModel : BaseViewModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthenticationService _authService;
        private ObservableCollection<Site> _sites = new();
        private Site? _selectedSite;
        private bool _isEditMode;
        private bool _isAddMode;
        private string _description = string.Empty;
        private bool _active = true;

        public SiteMaintenanceViewModel(ApplicationDbContext context, IAuthenticationService authService)
        {
            _context = context;
            _authService = authService;

            AddCommand = new RelayCommand(AddSite);
            EditCommand = new RelayCommand(EditSite, CanEditSite);
            DeleteCommand = new RelayCommand(async () => await DeleteSiteAsync(), CanDeleteSite);
            EditSiteEquipmentCommand = new RelayCommand(EditSiteEquipment, CanEditSiteEquipment);
            SaveCommand = new RelayCommand(async () => await SaveSiteAsync(), CanSaveSite);
            CancelCommand = new RelayCommand(CancelEdit);

            _ = LoadSitesAsync();
        }

        public ObservableCollection<Site> Sites
        {
            get => _sites;
            set => SetProperty(ref _sites, value);
        }

        public Site? SelectedSite
        {
            get => _selectedSite;
            set
            {
                SetProperty(ref _selectedSite, value);
                ((RelayCommand)EditCommand).RaiseCanExecuteChanged();
                ((RelayCommand)DeleteCommand).RaiseCanExecuteChanged();
                ((RelayCommand)EditSiteEquipmentCommand).RaiseCanExecuteChanged();
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

        public string Description
        {
            get => _description;
            set
            {
                SetProperty(ref _description, value);
                ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }

        public bool Active
        {
            get => _active;
            set => SetProperty(ref _active, value);
        }

        public bool IsFormVisible => IsEditMode || IsAddMode;

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand EditSiteEquipmentCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        private async Task LoadSitesAsync()
        {
            await ExecuteAsync(async () =>
            {
                var currentUserId = _authService.CurrentUser?.UserId;
                if (currentUserId == null) return;

                var sites = await _context.Sites
                    .Include(s => s.User)
                    .Include(s => s.RegisteredEquipments)
                        .ThenInclude(re => re.Equipment)
                    .Where(s => s.UserId == currentUserId)
                    .OrderBy(s => s.Description)
                    .ToListAsync();

                Sites = new ObservableCollection<Site>(sites);
            });
        }

        private bool CanEditSite()
        {
            return SelectedSite != null && !IsLoading && !IsEditMode && !IsAddMode;
        }

        private bool CanDeleteSite()
        {
            return SelectedSite != null && !IsLoading && !IsEditMode && !IsAddMode;
        }

        private bool CanEditSiteEquipment()
        {
            return SelectedSite != null && !IsLoading && !IsEditMode && !IsAddMode;
        }

        private bool CanSaveSite()
        {
            return (IsEditMode || IsAddMode) && !IsLoading && !string.IsNullOrWhiteSpace(Description);
        }

        private void AddSite()
        {
            Description = string.Empty;
            Active = true;
            IsAddMode = true;
            OnPropertyChanged(nameof(IsFormVisible));
        }

        private void EditSite()
        {
            if (SelectedSite != null)
            {
                Description = SelectedSite.Description;
                Active = SelectedSite.Active;
                IsEditMode = true;
                OnPropertyChanged(nameof(IsFormVisible));
            }
        }

        private async Task DeleteSiteAsync()
        {
            if (SelectedSite == null) return;

            var equipmentCount = await _context.RegisteredEquipment
                .CountAsync(re => re.SiteId == SelectedSite.SiteId);

            string message = equipmentCount > 0
                ? $"Are you sure you want to delete site '{SelectedSite.Description}'?\n\nThis will unregister {equipmentCount} equipment(s) from this site."
                : $"Are you sure you want to delete site '{SelectedSite.Description}'?";

            var result = MessageBox.Show(message, "Confirm Delete",
                                       MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                await ExecuteAsync(async () =>
                {
                    _context.Sites.Remove(SelectedSite);
                    await _context.SaveChangesAsync();
                    await LoadSitesAsync();
                    SelectedSite = null;
                });
            }
        }

        private void EditSiteEquipment()
        {
            if (SelectedSite != null)
            {
                var siteEquipmentWindow = new SiteEquipmentMaintenanceView(SelectedSite);
                siteEquipmentWindow.ShowDialog();

                // Refresh sites after equipment maintenance
                _ = LoadSitesAsync();
            }
        }

        private async Task SaveSiteAsync()
        {
            await ExecuteAsync(async () =>
            {
                var currentUserId = _authService.CurrentUser?.UserId;
                if (currentUserId == null)
                {
                    ShowError("User not authenticated.");
                    return;
                }

                if (IsAddMode)
                {
                    var newSite = new Site
                    {
                        UserId = currentUserId.Value,
                        Description = Description.Trim(),
                        Active = Active,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now
                    };

                    _context.Sites.Add(newSite);
                }
                else if (IsEditMode && SelectedSite != null)
                {
                    SelectedSite.Description = Description.Trim();
                    SelectedSite.Active = Active;
                    SelectedSite.ModifiedDate = DateTime.Now;
                }

                await _context.SaveChangesAsync();
                await LoadSitesAsync();

                CancelEdit();

                string successMessage = IsAddMode ? "Site added successfully!" : "Site updated successfully!";
                MessageBox.Show(successMessage, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }

        private void CancelEdit()
        {
            IsEditMode = false;
            IsAddMode = false;
            Description = string.Empty;
            Active = true;
            OnPropertyChanged(nameof(IsFormVisible));
        }
    }
}