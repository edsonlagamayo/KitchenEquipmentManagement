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
    public class SiteEquipmentMaintenanceViewModel : BaseViewModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthenticationService _authService;
        private readonly Site _site;
        private ObservableCollection<Equipment> _assignedEquipment = new();
        private ObservableCollection<Equipment> _availableEquipment = new();
        private Equipment? _selectedAssignedEquipment;
        private Equipment? _selectedAvailableEquipment;

        public SiteEquipmentMaintenanceViewModel(ApplicationDbContext context, IAuthenticationService authService, Site site)
        {
            _context = context;
            _authService = authService;
            _site = site;

            AddEquipmentCommand = new RelayCommand(async () => await AddEquipmentToSiteAsync(), CanAddEquipment);
            RemoveEquipmentCommand = new RelayCommand(async () => await RemoveEquipmentFromSiteAsync(), CanRemoveEquipment);

            _ = LoadDataAsync();
        }

        public string SiteName => _site.Description;

        public ObservableCollection<Equipment> AssignedEquipment
        {
            get => _assignedEquipment;
            set => SetProperty(ref _assignedEquipment, value);
        }

        public ObservableCollection<Equipment> AvailableEquipment
        {
            get => _availableEquipment;
            set => SetProperty(ref _availableEquipment, value);
        }

        public Equipment? SelectedAssignedEquipment
        {
            get => _selectedAssignedEquipment;
            set
            {
                SetProperty(ref _selectedAssignedEquipment, value);
                ((RelayCommand)RemoveEquipmentCommand).RaiseCanExecuteChanged();
            }
        }

        public Equipment? SelectedAvailableEquipment
        {
            get => _selectedAvailableEquipment;
            set
            {
                SetProperty(ref _selectedAvailableEquipment, value);
                ((RelayCommand)AddEquipmentCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand AddEquipmentCommand { get; }
        public ICommand RemoveEquipmentCommand { get; }

        private async Task LoadDataAsync()
        {
            await ExecuteAsync(async () =>
            {
                var currentUserId = _authService.CurrentUser?.UserId;
                if (currentUserId == null) return;

                // Load assigned equipment for this site
                var assignedEquipment = await _context.RegisteredEquipment
                    .Include(re => re.Equipment)
                    .Where(re => re.SiteId == _site.SiteId)
                    .Select(re => re.Equipment)
                    .OrderBy(e => e.Description)
                    .ToListAsync();

                AssignedEquipment = new ObservableCollection<Equipment>(assignedEquipment);

                // Load available equipment (user's equipment not assigned to any site)
                var assignedEquipmentIds = assignedEquipment.Select(e => e.EquipmentId).ToList();

                var availableEquipment = await _context.Equipment
                    .Where(e => e.UserId == currentUserId &&
                               !_context.RegisteredEquipment.Any(re => re.EquipmentId == e.EquipmentId))
                    .OrderBy(e => e.Description)
                    .ToListAsync();

                AvailableEquipment = new ObservableCollection<Equipment>(availableEquipment);
            });
        }

        private bool CanAddEquipment()
        {
            return SelectedAvailableEquipment != null && !IsLoading;
        }

        private bool CanRemoveEquipment()
        {
            return SelectedAssignedEquipment != null && !IsLoading;
        }

        private async Task AddEquipmentToSiteAsync()
        {
            if (SelectedAvailableEquipment == null) return;

            await ExecuteAsync(async () =>
            {
                var registeredEquipment = new RegisteredEquipment
                {
                    EquipmentId = SelectedAvailableEquipment.EquipmentId,
                    SiteId = _site.SiteId,
                    RegisteredDate = DateTime.Now
                };

                _context.RegisteredEquipment.Add(registeredEquipment);
                await _context.SaveChangesAsync();

                await LoadDataAsync();
                SelectedAvailableEquipment = null;
            });
        }

        private async Task RemoveEquipmentFromSiteAsync()
        {
            if (SelectedAssignedEquipment == null) return;

            var result = MessageBox.Show(
                $"Are you sure you want to remove '{SelectedAssignedEquipment.Description}' from site '{SiteName}'?",
                "Confirm Remove",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                await ExecuteAsync(async () =>
                {
                    var registeredEquipment = await _context.RegisteredEquipment
                        .FirstOrDefaultAsync(re => re.EquipmentId == SelectedAssignedEquipment.EquipmentId &&
                                                 re.SiteId == _site.SiteId);

                    if (registeredEquipment != null)
                    {
                        _context.RegisteredEquipment.Remove(registeredEquipment);
                        await _context.SaveChangesAsync();

                        await LoadDataAsync();
                        SelectedAssignedEquipment = null;
                    }
                });
            }
        }
    }
}