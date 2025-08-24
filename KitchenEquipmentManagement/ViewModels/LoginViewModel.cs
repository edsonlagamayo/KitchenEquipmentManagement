using System.Windows;
using System.Windows.Input;
using KitchenEquipmentManagement.Models;
using KitchenEquipmentManagement.Services;
using KitchenEquipmentManagement.Utilities;
using KitchenEquipmentManagement.Views;
using RelayCommand = KitchenEquipmentManagement.Utilities.RelayCommand;

namespace KitchenEquipmentManagement.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _authService;
        private string _username = string.Empty;
        private string _password = string.Empty;

        public LoginViewModel(IAuthenticationService authService)
        {
            _authService = authService;
            LoginCommand = new RelayCommand(async () => await LoginAsync(), CanLogin);
            SignupCommand = new RelayCommand(ShowSignup);
            ExitCommand = new RelayCommand(ExitApplication);
        }

        public string Username
        {
            get => _username;
            set
            {
                SetProperty(ref _username, value);
                ((RelayCommand)LoginCommand).RaiseCanExecuteChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                SetProperty(ref _password, value);
                ((RelayCommand)LoginCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand LoginCommand { get; }
        public ICommand SignupCommand { get; }
        public ICommand ExitCommand { get; }

        private bool CanLogin()
        {
            var canLogin = !IsLoading && !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
            System.Diagnostics.Debug.WriteLine($"CanLogin: {canLogin}, Username: '{Username}', Password: '{Password}', IsLoading: {IsLoading}");
            return canLogin;
        }

        private async Task LoginAsync()
        {
            System.Diagnostics.Debug.WriteLine($"LoginAsync called with Username: '{Username}', Password: '{Password}'");

            await ExecuteAsync(async () =>
            {
                System.Diagnostics.Debug.WriteLine("Starting login process...");

                try
                {
                    var user = await _authService.LoginAsync(Username, Password);
                    System.Diagnostics.Debug.WriteLine($"Login result: {(user != null ? "Success" : "Failed")}");

                    if (user != null)
                    {
                        System.Diagnostics.Debug.WriteLine("Login successful, opening Admin window...");

                        // Close current window and open Admin window
                        var adminWindow = new AdminView();
                        adminWindow.Show();

                        // Close login window
                        Application.Current.Windows.OfType<LoginView>().FirstOrDefault()?.Close();
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Login failed - invalid credentials");
                        ShowError("Invalid username or password.");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Login exception: {ex.Message}");
                    ShowError($"Login failed: {ex.Message}");
                }
            });
        }

        private void ShowSignup()
        {
            System.Diagnostics.Debug.WriteLine("ShowSignup called");
            var signupWindow = new SignupView();
            signupWindow.Show();

            // Close login window
            Application.Current.Windows.OfType<LoginView>().FirstOrDefault()?.Close();
        }

        private void ExitApplication()
        {
            System.Diagnostics.Debug.WriteLine("ExitApplication called");
            Application.Current.Shutdown();
        }
    }
}