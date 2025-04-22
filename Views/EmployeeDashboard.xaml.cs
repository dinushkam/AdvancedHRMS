using AdvancedHRMS.Data;
using AdvancedHRMS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AdvancedHRMS.Views
{
    public partial class EmployeeDashboard : Window, INotifyPropertyChanged
    {
        private Employee _employee;
        private DateTime _currentDate = DateTime.Today;
        private bool _isCheckedIn = false;

        public Employee Employee
        {
            get => _employee;
            set
            {
                _employee = value;
                OnPropertyChanged(nameof(Employee));
            }
        }

        public DateTime CurrentDate
        {
            get => _currentDate;
            set
            {
                _currentDate = value;
                OnPropertyChanged(nameof(CurrentDate));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public EmployeeDashboard()
        {
            try
            {
                // Verify user exists
                if (AuthService.CurrentUser == null)
                {
                    MessageBox.Show("Session expired. Please login again.");
                    Application.Current.Shutdown();
                    return;
                }

                // Verify role
                if (!AuthService.CurrentUser.Role.Equals("Employee", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Access denied. Employee dashboard requires employee role.");
                    Application.Current.Shutdown();
                    return;
                }

               
                InitializeComponent();
                DataContext = this;
                LoadEmployeeData();

                // Ensure window visibility
                this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                this.Show();

               
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to initialize dashboard: {ex.Message}");
                Application.Current.Shutdown();
            }
        }

        private void RefreshLeaveRequests()
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var employee = context.Employees
                        .Include(e => e.LeaveRequests)
                        .FirstOrDefault(e => e.EmployeeId == Employee.EmployeeId);

                    if (employee != null)
                    {
                        Employee.LeaveRequests = employee.LeaveRequests;
                        dgMyLeaveRequests.ItemsSource = null;
                        dgMyLeaveRequests.ItemsSource = Employee.LeaveRequests;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to refresh leave requests: {ex.Message}");
            }
        }

        private void LoadEmployeeData()
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    // Ensure EmployeeId is correctly set in the User table
                    if (AuthService.CurrentUser.EmployeeId.HasValue)
                    {
                        Employee = context.Employees
                            .Include(e => e.LeaveRequests)
                            .FirstOrDefault(e => e.EmployeeId == AuthService.CurrentUser.EmployeeId);
                    }
                    else
                    {
                        // Fallback: Match by email if EmployeeId is missing
                        Employee = context.Employees
                            .Include(e => e.LeaveRequests)
                            .FirstOrDefault(e => e.Email == AuthService.CurrentUser.Email);
                    }

                    if (Employee == null)
                    {
                        MessageBox.Show("Employee record not found. Contact HR.");
                        return;
                    }

                    // Update UI bindings
                    DataContext = this; // Ensure DataContext is set
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}");
            }
            Debug.WriteLine($"Employee loaded: {Employee?.FullName ?? "NULL"}");
        }
       
        // Navigation Methods
        private void NavigateToProfile(object sender, RoutedEventArgs e)
        {
            ProfileTab.IsSelected = true;
        }

        private void NavigateToAttendance(object sender, RoutedEventArgs e)
        {
            AttendanceTab.IsSelected = true;
        }

        private void NavigateToLeave(object sender, RoutedEventArgs e)
        {
            LeaveTab.IsSelected = true;
        }

        private void NavigateToDocuments(object sender, RoutedEventArgs e)
        {
            DocumentsTab.IsSelected = true;
        }

        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new ApplicationDbContext())
            {
                // Get fresh employee data from database
                var dbEmployee = context.Employees
                    .FirstOrDefault(e => e.EmployeeId == Employee.EmployeeId);

                if (dbEmployee == null) return;

                var editWindow = new EditEmployeeProfile(dbEmployee);

                if (editWindow.ShowDialog() == true)
                {
                    try
                    {
                        // Update the database entity with edited values
                        dbEmployee.FullName = editWindow.Employee.FullName;
                        dbEmployee.Email = editWindow.Employee.Email;
                        dbEmployee.Phone = editWindow.Employee.Phone;
                        dbEmployee.Position = editWindow.Employee.Position;
                        dbEmployee.Department = editWindow.Employee.Department;
                        dbEmployee.Address = editWindow.Employee.Address;
                        dbEmployee.Salary = editWindow.Employee.Salary;

                        context.SaveChanges();

                        // Update the local employee object
                        Employee.FullName = dbEmployee.FullName;
                        Employee.Email = dbEmployee.Email;
                        Employee.Phone = dbEmployee.Phone;
                        Employee.Position = dbEmployee.Position;
                        Employee.Department = dbEmployee.Department;
                        Employee.Address = dbEmployee.Address;
                        Employee.Salary = dbEmployee.Salary;

                        // Force UI refresh
                        DataContext = null;
                        DataContext = this;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving changes: {ex.Message}", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void CheckIn_Click(object sender, RoutedEventArgs e)
        {
            _isCheckedIn = true;
            AttendanceStatus.Text = $"Checked in at {DateTime.Now.ToString("hh:mm tt")}";
            CheckInButton.IsEnabled = false;
            CheckOutButton.IsEnabled = true;
        }

        private void CheckOut_Click(object sender, RoutedEventArgs e)
        {
            _isCheckedIn = false;
            AttendanceStatus.Text += $"\nChecked out at {DateTime.Now.ToString("hh:mm tt")}";
            CheckOutButton.IsEnabled = false;
            CheckInButton.IsEnabled = true;
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {

            new LoginWindow().Show();
            this.Close(); 
        }

        private void RequestLeave_Click(object sender, RoutedEventArgs e)
        {
            var leaveWindow = new LeaveRequestWindow(Employee);
            leaveWindow.Owner = this;

            if (leaveWindow.ShowDialog() == true)
            {
                RefreshLeaveRequests();
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}