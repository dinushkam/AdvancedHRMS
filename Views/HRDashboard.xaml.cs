using AdvancedHRMS.Data;
using AdvancedHRMS.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Windows;

namespace AdvancedHRMS.Views
{
    public partial class HRDashboard : Window, INotifyPropertyChanged
    {
        private readonly ApplicationDbContext _context;
        private User _currentUser;
        public User CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged(nameof(CurrentUser));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public HRDashboard()
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            LoadCurrentUser(); // Load user data
            LoadHRData();
            this.DataContext = this; // Set DataContext to self
        }

        private void LoadCurrentUser()
        {
            // Get current user from your authentication service
            CurrentUser = _context.Users.FirstOrDefault(u => u.Email == AuthService.CurrentUserEmail);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // ... rest of your existing code ...


        private void LoadHRData()
        {
            // Load HR-specific data
            ReportsDataGrid.ItemsSource = _context.Employees.ToList();
        }

        private void ViewEmployees_Click(object sender, RoutedEventArgs e)
        {
            var empWindow = new EmployeeManagementWindow();
            empWindow.Show();
        }

      
        private void ManageBenefits_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Benefits management would go here");
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {

            new LoginWindow().Show();
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            _context.Dispose();
            base.OnClosed(e);
        }
        private void ManageLeaveRequests_Click(object sender, RoutedEventArgs e)
        {
            var leaveWindow = new LeaveApprovalWindow();
            leaveWindow.Owner = this;
            leaveWindow.ShowDialog();
        }

        private void ViewEditProfile_Click(object sender, RoutedEventArgs e)
        {
            var profileWindow = new HRProfileWindow(AuthService.CurrentUserEmail);
            if (profileWindow.ShowDialog() == true)
            {
                // Refresh the current user data
                LoadCurrentUser();
                MessageBox.Show("Profile updated successfully!", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}