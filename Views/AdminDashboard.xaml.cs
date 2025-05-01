using AdvancedHRMS.Data;
using AdvancedHRMS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;

namespace AdvancedHRMS.Views
{
    public partial class AdminDashboard : Window
    {
        private readonly ApplicationDbContext _context;
        private Window _currentChildWindow;

        public AdminDashboard()
        {
            if (!AuthService.IsAdmin)
            {
                MessageBox.Show("Unauthorized access!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
                return;
            }

            InitializeComponent();
            _context = new ApplicationDbContext();
            LoadUsers();
        }

        private void LoadUsers()
        {
            try
            {
                dgUsers.ItemsSource = _context.Users.ToList();
                int employeeCount = _context.Employees.Count();
                EmployeeCountText.Text = $"Total Employees: {employeeCount}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnApproveUser_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsers.SelectedItem is User selectedUser)
            {
                try
                {
                    var user = _context.Users.FirstOrDefault(u => u.Id == selectedUser.Id);
                    if (user != null)
                    {
                        user.IsApproved = true;
                        _context.SaveChanges();
                        LoadUsers();
                        MessageBox.Show("User Approved!", "Success",
                            MessageBoxButton.OK, MessageBoxImage.Information);

                       
                        var employee = _context.Employees.FirstOrDefault(emp => emp.Email == user.Email);
                        if (employee == null)
                        {
                            employee = new Employee
                            {
                                FullName = user.Username,
                                Email = user.Email,
                                Phone = "N/A",
                                DepartmentId = _context.Departments
    .FirstOrDefault(d => d.Name == "Not Assigned")?.DepartmentId ?? 0,

                                DateOfJoining = DateTime.Now,
                                Salary = 50000
                            };
                            _context.Employees.Add(employee);
                            _context.SaveChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error approving user: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a user first!", "Warning",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsers.SelectedItem is User selectedUser)
            {
                var result = MessageBox.Show("Are you sure you want to delete this user?",
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var user = _context.Users.FirstOrDefault(u => u.Id == selectedUser.Id);
                        if (user != null)
                        {
                            _context.Users.Remove(user);
                            _context.SaveChanges();
                            LoadUsers();
                            MessageBox.Show("User deleted successfully!", "Success",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting user: {ex.Message}", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a user first!", "Warning",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void OpenChildWindow(Window window)
        {
            btnBack.Visibility = Visibility.Visible;
            _currentChildWindow = window;
            window.Owner = this;
            window.Closed += (s, args) => {
                btnBack.Visibility = Visibility.Collapsed;
                LoadUsers(); 
            };
            window.Show();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (_currentChildWindow != null)
            {
                _currentChildWindow.Close();
                _currentChildWindow = null;
            }
            btnBack.Visibility = Visibility.Collapsed;
        }

        private void ManageEmployees_Click(object sender, RoutedEventArgs e)
            => OpenChildWindow(new EmployeeManagementWindow());

        private void ManageDepartments_Click(object sender, RoutedEventArgs e)
            => OpenChildWindow(new DepartmentManagementWindow());

        private void ManageLeaveRequests_Click(object sender, RoutedEventArgs e)
            => OpenChildWindow(new LeaveApprovalWindow());

        private void BtnPayroll_Click(object sender, RoutedEventArgs e)
            => OpenChildWindow(new PayrollWindow());

        private void ViewReports_Click(object sender, RoutedEventArgs e)
           => OpenChildWindow(new ReportingWindow());

        private void BtnAdminProfile_Click(object sender, RoutedEventArgs e)
            => new AdminProfileWindow() { Owner = this }.ShowDialog();

        protected override void OnClosed(EventArgs e)
        {
            _context.Dispose();
            base.OnClosed(e);
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {

            new LoginWindow().Show();
            this.Close();
        }

       
    }
}