using KitchenEquipmentManagement.Data;
using KitchenEquipmentManagement.Services;
using KitchenEquipmentManagement.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace KitchenEquipmentManagement.Views
{
    public partial class SignupView : Window
    {
        public SignupView()
        {
            InitializeComponent();

            // Get services from the application's service provider
            var authService = App.ServiceProvider.GetRequiredService<IAuthenticationService>();  

            DataContext = new SignupViewModel(authService);
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is SignupViewModel viewModel)
            {
                viewModel.Password = ((PasswordBox)sender).Password;
            }
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is SignupViewModel viewModel)
            {
                viewModel.ConfirmPassword = ((PasswordBox)sender).Password;
            }
        }
    }
}