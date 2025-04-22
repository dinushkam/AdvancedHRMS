using AdvancedHRMS.Views;
using System.Windows;

namespace AdvancedHRMS
{
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Set shutdown mode (keep this)
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            // Create and show login window
            var loginWindow = new LoginWindow();
            loginWindow.Show();

            // Handle login window closure
            loginWindow.Closed += (s, args) =>
            {
                if (AuthService.CurrentUser == null)
                {
                    Application.Current.Shutdown();
                    return;
                }

                // Create the appropriate dashboard
                Window dashboard = null;
                try
                {
                    dashboard = AuthService.CurrentUser.Role switch
                    {
                        "Admin" => new AdminDashboard(),
                        "Manager" => new HRDashboard(), // Handle both variants
                        "Employee" => new EmployeeDashboard(),
                        _ => throw new Exception($"Unknown role: {AuthService.CurrentUser.Role}")
                    };
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to create dashboard: {ex.Message}");
                    Application.Current.Shutdown();
                    return;
                }

                // Set as main window and show
                this.MainWindow = dashboard;
                dashboard.Show();
            };
        }
    }
}