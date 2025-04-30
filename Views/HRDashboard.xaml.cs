using AdvancedHRMS.Data;
using AdvancedHRMS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

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
            LoadCurrentUser();
            DataContext = this;
        }
        private void LoadCurrentUser()
        {
            try
            {
                CurrentUser = _context.Users
                    .FirstOrDefault(u => u.Email == AuthService.CurrentUserEmail);

                if (CurrentUser == null)
                {
                    MessageBox.Show("User not found in database.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Debug output to check if ID exists
                Debug.WriteLine($"Loaded user ID: {CurrentUser.EmployeeId}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading user data: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ManageEmployees_Click(object sender, RoutedEventArgs e)
        {
            var empWindow = new EmployeeManagementWindow();
            empWindow.Show();
        }

        private void ManageDepartments_Click(object sender, RoutedEventArgs e)
        {
            var departmentWindow= new DepartmentManagementWindow();
            departmentWindow.ShowDialog();
        }
        
        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new EditHRProfileWindow(CurrentUser.Email);
            if (editWindow.ShowDialog() == true)
            {
                LoadCurrentUser(); // Refresh user data
            }
        }

        private void ViewReports_Click(object sender, RoutedEventArgs e)
        {
            var reportingWindow = new ReportingWindow();
            reportingWindow.ShowDialog();

        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {

            new LoginWindow().Show();
            this.Close();
        }

        private void ManageLeaveRequests_Click(object sender, RoutedEventArgs e)
        {
            var leaveWindow = new LeaveApprovalWindow();
            leaveWindow.Owner = this;
            leaveWindow.ShowDialog();
        }

        protected override void OnClosed(EventArgs e)
        {
            _context.Dispose();
            base.OnClosed(e);
        }
    }
}