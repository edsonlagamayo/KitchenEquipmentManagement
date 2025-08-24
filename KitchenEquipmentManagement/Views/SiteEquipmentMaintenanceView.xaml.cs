using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using KitchenEquipmentManagement.Data;
using KitchenEquipmentManagement.Models;
using KitchenEquipmentManagement.Services;
using KitchenEquipmentManagement.ViewModels;

namespace KitchenEquipmentManagement.Views
{
    public partial class SiteEquipmentMaintenanceView : Window
    {
        public SiteEquipmentMaintenanceView(Site site)
        {
            InitializeComponent();

            // Get services from the application's service provider
            var authService = App.ServiceProvider.GetRequiredService<IAuthenticationService>();
            var context = App.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            DataContext = new SiteEquipmentMaintenanceViewModel(context, authService, site);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}