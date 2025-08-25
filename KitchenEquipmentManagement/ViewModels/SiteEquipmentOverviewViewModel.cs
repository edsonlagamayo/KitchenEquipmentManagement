using System.Collections.ObjectModel;
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
    public class SiteEquipmentOverviewViewModel : BaseViewModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthenticationService _authService;
        private ObservableCollection<Site> _sites = new();
        private ObservableCollection<Equipment> _unassignedEquipment = new();
        private Site? _selectedSite;
        private Equipment? _selectedUnassignedEquipment;

        public SiteEquipmentOverviewViewModel(ApplicationDbContext context, IAuthenticationService authService)
        {
            _context = context;
            _authService = authService;

            // Initialize collections
            _sites = new ObservableCollection<Site>();
            _unassignedEquipment = new ObservableCollection<Equipment>();

            RefreshCommand = new RelayCommand(async () => await LoadDataAsync());
            ManageEquipmentCommand = new RelayCommand(ManageEquipmentForSite, CanManageEquipment);

            _ = LoadDataAsync();
        }

        #region Properties

        public ObservableCollection<Site> Sites
        {
            get => _sites;
            set => SetProperty(ref _sites, value);
        }

        public ObservableCollection<Equipment> UnassignedEquipment
        {
            get => _unassignedEquipment;
            set => SetProperty(ref _unassignedEquipment, value);
        }

        public Site? SelectedSite
        {
            get => _selectedSite;
            set
            {
                SetProperty(ref _selectedSite, value);
                ((RelayCommand)ManageEquipmentCommand).RaiseCanExecuteChanged();
            }
        }

        public Equipment? SelectedUnassignedEquipment
        {
            get => _selectedUnassignedEquipment;
            set => SetProperty(ref _selectedUnassignedEquipment, value);
        }

        // Summary Statistics
        public int TotalSites => Sites?.Count ?? 0;
        public int TotalEquipment => (Sites?.Sum(s => s.RegisteredEquipments?.Count ?? 0) ?? 0) + (UnassignedEquipment?.Count ?? 0);
        public int AssignedEquipmentCount => Sites?.Sum(s => s.RegisteredEquipments?.Count ?? 0) ?? 0;

        #endregion

        #region Commands

        public ICommand RefreshCommand { get; }
        public ICommand ManageEquipmentCommand { get; }

        #endregion

        #region Command Methods

        private bool CanManageEquipment()
        {
            return SelectedSite != null && !IsLoading;
        }

        private void ManageEquipmentForSite()
        {
            if (SelectedSite != null)
            {
                var siteEquipmentWindow = new SiteEquipmentMaintenanceView(SelectedSite);
                siteEquipmentWindow.ShowDialog();

                // Refresh data after equipment maintenance
                _ = LoadDataAsync();
            }
        }

        #endregion

        #region Private Methods

        private async Task LoadDataAsync()
        {
            await ExecuteAsync(async () =>
            {
                var currentUserId = _authService.CurrentUser?.UserId;
                if (currentUserId == null) return;

                // Load sites with equipment
                var sites = await _context.Sites
                    .Include(s => s.User)
                    .Include(s => s.RegisteredEquipments)
                        .ThenInclude(re => re.Equipment)
                    .Where(s => s.UserId == currentUserId)
                    .OrderBy(s => s.Description)
                    .ToListAsync();

                // Load unassigned equipment
                var unassignedEquipment = await _context.Equipment
                    .Include(e => e.User)
                    .Where(e => e.UserId == currentUserId &&
                               !_context.RegisteredEquipment.Any(re => re.EquipmentId == e.EquipmentId))
                    .OrderBy(e => e.Description)
                    .ToListAsync();

                // Update collections on UI thread
                Sites.Clear();
                foreach (var site in sites)
                {
                    Sites.Add(site);
                }

                UnassignedEquipment.Clear();
                foreach (var equipment in unassignedEquipment)
                {
                    UnassignedEquipment.Add(equipment);
                }

                // Notify summary properties changed
                OnPropertyChanged(nameof(TotalSites));
                OnPropertyChanged(nameof(TotalEquipment));
                OnPropertyChanged(nameof(AssignedEquipmentCount));
            });
        }

        #endregion
    }
}