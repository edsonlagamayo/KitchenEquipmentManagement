using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using KitchenEquipmentManagement.Services;
using KitchenEquipmentManagement.ViewModels;

namespace KitchenEquipmentManagement.Views
{
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();

            try
            {
                System.Diagnostics.Debug.WriteLine("LoginView initializing...");

                // Check if App is initialized
                if (!App.IsInitialized || App.ServiceProvider == null)
                {
                    throw new InvalidOperationException("Application services are not initialized yet.");
                }

                // Get services from the static service provider
                var authService = App.ServiceProvider.GetRequiredService<IAuthenticationService>();
                System.Diagnostics.Debug.WriteLine("AuthenticationService retrieved successfully");

                if (authService == null)
                {
                    throw new InvalidOperationException("AuthenticationService is null");
                }

                var viewModel = new LoginViewModel(authService);
                DataContext = viewModel;
                System.Diagnostics.Debug.WriteLine("LoginViewModel created and set as DataContext");

                // Set focus to username textbox
                Loaded += (s, e) =>
                {
                    System.Diagnostics.Debug.WriteLine("LoginView loaded, setting focus to username");
                    UsernameTextBox.Focus();
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing LoginView: {ex.Message}");
                MessageBox.Show($"Error initializing login: {ex.Message}\n\nPlease restart the application.",
                              "Initialization Error", MessageBoxButton.OK, MessageBoxImage.Error);

                // Try to shutdown gracefully
                Application.Current?.Shutdown();
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel viewModel)
            {
                viewModel.Password = ((PasswordBox)sender).Password;
                System.Diagnostics.Debug.WriteLine($"Password changed, length: {viewModel.Password?.Length ?? 0}");
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("LoginButton_Click event fired!");

            if (DataContext is LoginViewModel viewModel)
            {
                System.Diagnostics.Debug.WriteLine($"ViewModel found, Username: '{viewModel.Username}', Password length: {viewModel.Password?.Length ?? 0}");

                // Check if command can execute
                if (viewModel.LoginCommand.CanExecute(null))
                {
                    System.Diagnostics.Debug.WriteLine("LoginCommand can execute, executing...");
                    viewModel.LoginCommand.Execute(null);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("LoginCommand cannot execute!");
                    MessageBox.Show("Please enter both username and password.", "Login Required",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("DataContext is not LoginViewModel!");
                MessageBox.Show("Login system not initialized properly. Please restart the application.",
                              "System Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Test method to directly open signup
        private void TestSignup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var signupWindow = new SignupView();
                signupWindow.Show();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening signup: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}