using System.Windows;
using System.Windows.Input;
using KitchenEquipmentManagement.Services;
using KitchenEquipmentManagement.Utilities;
using KitchenEquipmentManagement.Views;
using RelayCommand = KitchenEquipmentManagement.Utilities.RelayCommand;

namespace KitchenEquipmentManagement.ViewModels
{
    public class AdminViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _authService;
        private object? _currentContent;
        private string _currentUserName = string.Empty;
        private string _currentUserType = string.Empty;

        public AdminViewModel(IAuthenticationService authService)
        {
            _authService = authService;

            UsersCommand = new RelayCommand(ShowUsers, CanAccessUsers);
            SitesCommand = new RelayCommand(ShowSites);
            EquipmentCommand = new RelayCommand(ShowEquipment);
            LogoutCommand = new RelayCommand(Logout);

            LoadUserInfo();
        }

        public object? CurrentContent
        {
            get => _currentContent;
            set => SetProperty(ref _currentContent, value);
        }

        public string CurrentUserName
        {
            get => _currentUserName;
            set => SetProperty(ref _currentUserName, value);
        }

        public string CurrentUserType
        {
            get => _currentUserType;
            set => SetProperty(ref _currentUserType, value);
        }

        public bool IsSuperAdmin => _authService.CurrentUser?.UserType == "SuperAdmin";

        public ICommand UsersCommand { get; }
        public ICommand SitesCommand { get; }
        public ICommand EquipmentCommand { get; }
        public ICommand LogoutCommand { get; }

        private void LoadUserInfo()
        {
            var user = _authService.CurrentUser;
            if (user != null)
            {
                CurrentUserName = user.FullName;
                CurrentUserType = user.UserType;
            }
        }

        private bool CanAccessUsers()
        {
            return IsSuperAdmin;
        }

        private void ShowUsers()
        {
            if (IsSuperAdmin)
            {
                CurrentContent = new UserMaintenanceView();
            }
            else
            {
                MessageBox.Show("Access denied. Only SuperAdmins can manage users.",
                              "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ShowSites()
        {
            CurrentContent = new SiteMaintenanceView();
        }

        private void ShowEquipment()
        {
            CurrentContent = new EquipmentMaintenanceView();
        }

        private void Logout()
        {
            _authService.Logout();

            var loginWindow = new LoginView();
            loginWindow.Show();

            // Close admin window
            Application.Current.Windows.OfType<AdminView>().FirstOrDefault()?.Close();
        }
    }
}