using System.IO;
using System.Windows;
using System.Windows.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using KitchenEquipmentManagement.Data;
using KitchenEquipmentManagement.Services;
using KitchenEquipmentManagement.Views;
using Wpf.Ui;
using Wpf.Ui.DependencyInjection;

namespace KitchenEquipmentManagement
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // The .NET Generic Host provides dependency injection, configuration, logging, and other services.
        private static IHost? _host;
        private static bool _isInitialized = false;

        /// <summary>
        /// Gets services - Static property for easy access
        /// </summary>
        public static IServiceProvider? ServiceProvider => _host?.Services;

        /// <summary>
        /// Occurs when the application is loading.
        /// </summary>
        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                // Initialize host first, before calling base.OnStartup
                await InitializeHostAsync();
                
                // Initialize database
                await InitializeDatabaseAsync();
                
                // Mark as initialized
                _isInitialized = true;
                
                // Now show the login window manually instead of using StartupUri
                var loginWindow = new LoginView();
                loginWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Application startup failed: {ex.Message}\n\nInner Exception: {ex.InnerException?.Message}", 
                              "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
                return;
            }

            // Call base startup (but don't let it create the window since we did it manually)
            // base.OnStartup(e);
        }

        private async Task InitializeHostAsync()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(c => 
                { 
                    c.SetBasePath(Path.GetDirectoryName(AppContext.BaseDirectory) ?? Directory.GetCurrentDirectory());
                    c.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    // Get configuration
                    var configuration = context.Configuration;

                    // Add Entity Framework DbContext
                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

                    // Add Kitchen Equipment Management services
                    services.AddScoped<IAuthenticationService, AuthenticationService>();

                    // Add configuration
                    services.AddSingleton(configuration);

                    // WPF UI Services (if you want to keep them)
                    services.AddSingleton<IThemeService, ThemeService>();
                    services.AddSingleton<ITaskBarService, TaskBarService>();
                    services.AddSingleton<INavigationService, NavigationService>();
                })
                .Build();

            await _host.StartAsync();
        }

        private async Task InitializeDatabaseAsync()
        {
            if (_host?.Services == null) return;

            using var scope = _host.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            try
            {
                await context.Database.EnsureCreatedAsync();
                await SeedDataAsync(context);
            }
            catch (Exception ex)
            {
                throw new Exception($"Database initialization failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Occurs when the application is closing.
        /// </summary>
        protected override async void OnExit(ExitEventArgs e)
        {
            if (_host != null)
            {
                await _host.StopAsync();
                _host.Dispose();
            }
            base.OnExit(e);
        }

        /// <summary>
        /// Occurs when an exception is thrown by an application but not handled.
        /// </summary>
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"An unexpected error occurred: {e.Exception.Message}\n\nInner Exception: {e.Exception.InnerException?.Message}", 
                          "Unexpected Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        /// <summary>
        /// Seeds initial data into the database
        /// </summary>
        private async Task SeedDataAsync(ApplicationDbContext context)
        {
            // Check if we need to seed data
            if (!await context.Users.AnyAsync())
            {
                var authService = new AuthenticationService(context);
                
                // Create a default SuperAdmin user
                var superAdmin = new Models.User
                {
                    FirstName = "Super",
                    LastName = "Admin",
                    EmailAddress = "admin@kitchenequipment.com",
                    UserName = "admin",
                    UserType = "SuperAdmin"
                };

                await authService.RegisterAsync(superAdmin, "admin123");

                // Create a default Admin user
                var admin = new Models.User
                {
                    FirstName = "John",
                    LastName = "Doe",
                    EmailAddress = "john.doe@kitchenequipment.com",
                    UserName = "john.doe",
                    UserType = "Admin"
                };

                await authService.RegisterAsync(admin, "password123");

                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Helper method to check if services are available
        /// </summary>
        public static bool IsInitialized => _isInitialized && ServiceProvider != null;
    }
}