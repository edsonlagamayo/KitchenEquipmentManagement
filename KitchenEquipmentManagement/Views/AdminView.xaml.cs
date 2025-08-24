using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using KitchenEquipmentManagement.Services;
using KitchenEquipmentManagement.ViewModels;

namespace KitchenEquipmentManagement.Views
{
    public partial class AdminView : Window
    {
        public AdminView()
        {
            InitializeComponent();

            // Get services from the application's service provider 
            var authService = App.ServiceProvider.GetRequiredService<IAuthenticationService>();

            DataContext = new AdminViewModel(authService);
        }
    }
}