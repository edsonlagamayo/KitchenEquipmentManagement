using KitchenEquipmentManagement.ViewModels.Pages;
using Wpf.Ui.Abstractions.Controls;

namespace KitchenEquipmentManagement.Views.Pages
{
    public partial class SettingsPage : INavigableView<SettingsViewModel>
    {
        public SettingsViewModel ViewModel { get; }

        public SettingsPage(SettingsViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }
    }
}
