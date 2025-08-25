using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using KitchenEquipmentManagement.Data;
using KitchenEquipmentManagement.Services;
using KitchenEquipmentManagement.ViewModels;

namespace KitchenEquipmentManagement.Views
{
    public partial class SiteEquipmentOverviewView : UserControl
    {
        public SiteEquipmentOverviewView()
        {
            InitializeComponent();

            // Get services from the static service provider
            var context = App.ServiceProvider?.GetRequiredService<ApplicationDbContext>();
            var authService = App.ServiceProvider?.GetRequiredService<IAuthenticationService>();

            if (context != null && authService != null)
            {
                DataContext = new SiteEquipmentOverviewViewModel(context, authService);
            }
        }
    }
}