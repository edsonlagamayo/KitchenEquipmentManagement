using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using KitchenEquipmentManagement.Data;
using KitchenEquipmentManagement.Services;
using KitchenEquipmentManagement.ViewModels;

namespace KitchenEquipmentManagement.Views
{
    public partial class UserMaintenanceView : UserControl
    {
        public UserMaintenanceView()
        {
            InitializeComponent();

            // Get services from the application's service provider
            var authService = App.ServiceProvider.GetRequiredService<IAuthenticationService>();
            var context = App.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            DataContext = new UserMaintenanceViewModel(context, authService);
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is UserMaintenanceViewModel viewModel)
            {
                viewModel.Password = ((PasswordBox)sender).Password;
            }
        }
    }
}