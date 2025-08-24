using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using KitchenEquipmentManagement.Data;
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

                // Get services from the static service provider
                var authService = App.ServiceProvider.GetRequiredService<IAuthenticationService>(); 
                System.Diagnostics.Debug.WriteLine("AuthenticationService retrieved successfully");

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
                MessageBox.Show($"Error initializing login: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
    }
}