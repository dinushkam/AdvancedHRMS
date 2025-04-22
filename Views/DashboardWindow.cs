// DashboardWindow.cs
using AdvancedHRMS.Data;
using AdvancedHRMS.Models;
using System.Windows;

namespace AdvancedHRMS.Views
{
    public partial class DashboardWindow : Window
    {
        protected readonly ApplicationDbContext _context;
        protected User _currentUser;

        public DashboardWindow()
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            _currentUser = AuthService.CurrentUser;

            // Common initialization for all dashboards
            Title = $"{_currentUser.Role} Dashboard - {_currentUser.FullName}";
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void InitializeComponent()
        {
           
        }

        protected override void OnClosed(EventArgs e)
        {
            _context.Dispose();
            base.OnClosed(e);
        }
    }
}