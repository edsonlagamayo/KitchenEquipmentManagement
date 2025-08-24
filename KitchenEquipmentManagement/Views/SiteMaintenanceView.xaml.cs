using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using KitchenEquipmentManagement.Data;
using KitchenEquipmentManagement.Services;
using KitchenEquipmentManagement.ViewModels;

namespace KitchenEquipmentManagement.Views
{
    public partial class SiteMaintenanceView : UserControl
    {
        public SiteMaintenanceView()
        {
            InitializeComponent();

            // Get services from the application's service provider
            var authService = App.ServiceProvider.GetRequiredService<IAuthenticationService>();
            var context = App.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            DataContext = new SiteMaintenanceViewModel(context, authService);
        }
    }
}