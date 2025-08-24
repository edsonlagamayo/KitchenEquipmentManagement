using System.Windows;
using System.Windows.Input;
using System.Text.RegularExpressions;
using KitchenEquipmentManagement.Models;
using KitchenEquipmentManagement.Services;
using KitchenEquipmentManagement.Utilities;
using KitchenEquipmentManagement.Views;
using RelayCommand = KitchenEquipmentManagement.Utilities.RelayCommand;

namespace KitchenEquipmentManagement.ViewModels
{
    public class SignupViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _authService;
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private string _emailAddress = string.Empty;
        private string _userName = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;
        private string _selectedUserType = "Admin";

        public SignupViewModel(IAuthenticationService authService)
        {
            _authService = authService;
            SignupCommand = new RelayCommand(async () => await SignupAsync(), CanSignup);
            BackToLoginCommand = new RelayCommand(BackToLogin);

            UserTypes = new List<string> { "Admin", "SuperAdmin" };
        }

        public string FirstName
        {
            get => _firstName;
            set
            {
                SetProperty(ref _firstName, value);
                ((RelayCommand)SignupCommand).RaiseCanExecuteChanged();
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                SetProperty(ref _lastName, value);
                ((RelayCommand)SignupCommand).RaiseCanExecuteChanged();
            }
        }

        public string EmailAddress
        {
            get => _emailAddress;
            set
            {
                SetProperty(ref _emailAddress, value);
                ((RelayCommand)SignupCommand).RaiseCanExecuteChanged();
            }
        }

        public string UserName
        {
            get => _userName;
            set
            {
                SetProperty(ref _userName, value);
                ((RelayCommand)SignupCommand).RaiseCanExecuteChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                SetProperty(ref _password, value);
                ((RelayCommand)SignupCommand).RaiseCanExecuteChanged();
            }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                SetProperty(ref _confirmPassword, value);
                ((RelayCommand)SignupCommand).RaiseCanExecuteChanged();
            }
        }

        public string SelectedUserType
        {
            get => _selectedUserType;
            set => SetProperty(ref _selectedUserType, value);
        }

        public List<string> UserTypes { get; }

        public ICommand SignupCommand { get; }
        public ICommand BackToLoginCommand { get; }

        private bool CanSignup()
        {
            return !IsLoading &&
                   !string.IsNullOrWhiteSpace(FirstName) &&
                   !string.IsNullOrWhiteSpace(LastName) &&
                   !string.IsNullOrWhiteSpace(EmailAddress) &&
                   !string.IsNullOrWhiteSpace(UserName) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   !string.IsNullOrWhiteSpace(ConfirmPassword);
        }

        private async Task SignupAsync()
        {
            await ExecuteAsync(async () =>
            {
                // Validate inputs
                if (!IsValidEmail(EmailAddress))
                {
                    ShowError("Please enter a valid email address.");
                    return;
                }

                if (Password != ConfirmPassword)
                {
                    ShowError("Passwords do not match.");
                    return;
                }

                if (Password.Length < 6)
                {
                    ShowError("Password must be at least 6 characters long.");
                    return;
                }

                // Check if username is available
                if (!await _authService.IsUsernameAvailableAsync(UserName))
                {
                    ShowError("Username is already taken.");
                    return;
                }

                // Check if email is available
                if (!await _authService.IsEmailAvailableAsync(EmailAddress))
                {
                    ShowError("Email address is already registered.");
                    return;
                }

                // Create new user
                var newUser = new User
                {
                    FirstName = FirstName.Trim(),
                    LastName = LastName.Trim(),
                    EmailAddress = EmailAddress.Trim().ToLower(),
                    UserName = UserName.Trim(),
                    UserType = SelectedUserType
                };

                bool success = await _authService.RegisterAsync(newUser, Password);

                if (success)
                {
                    MessageBox.Show("Registration successful! You can now login with your credentials.",
                                  "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    BackToLogin();
                }
            });
        }

        private void BackToLogin()
        {
            var loginWindow = new LoginView();
            loginWindow.Show();

            // Close signup window
            Application.Current.Windows.OfType<SignupView>().FirstOrDefault()?.Close();
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return emailRegex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }
    }
}