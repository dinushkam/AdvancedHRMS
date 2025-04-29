using AdvancedHRMS.Data;
using AdvancedHRMS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;

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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to initialize dashboard: {ex.Message}");
                Application.Current.Shutdown();
            }
        }

        private void LoadEmployeeData()
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    if (AuthService.CurrentUser.EmployeeId.HasValue)
                    {
                        Employee = context.Employees
                            .Include(e => e.LeaveRequests)
                            .FirstOrDefault(e => e.EmployeeId == AuthService.CurrentUser.EmployeeId);
                    }
                    else
                    {
                        Employee = context.Employees
                            .Include(e => e.LeaveRequests)
                            .FirstOrDefault(e => e.Email == AuthService.CurrentUser.Email);
                    }

                    if (Employee == null)
                    {
                        MessageBox.Show("Employee record not found. Contact HR.");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}");
            }
        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            // Already showing profile info in main view
        }

        private void Attendance_Click(object sender, RoutedEventArgs e)
        {
            var attendanceWindow = new AttendanceWindow(Employee);
            attendanceWindow.Owner = this;
            attendanceWindow.ShowDialog();
        }

        private void LeaveRequests_Click(object sender, RoutedEventArgs e)
        {
            var leaveWindow = new LeaveRequestWindow(Employee);
            leaveWindow.Owner = this;
            leaveWindow.ShowDialog();
        }

        private void Documents_Click(object sender, RoutedEventArgs e)
        {
            var documentsWindow = new EmployeeDocumentsView();
           
        }

        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new ApplicationDbContext())
            {
                var dbEmployee = context.Employees
                    .FirstOrDefault(e => e.EmployeeId == Employee.EmployeeId);

                if (dbEmployee == null) return;

                var editWindow = new EditEmployeeProfile(dbEmployee);

                if (editWindow.ShowDialog() == true)
                {
                    try
                    {
                        dbEmployee.FullName = editWindow.Employee.FullName;
                        dbEmployee.Email = editWindow.Employee.Email;
                        dbEmployee.Phone = editWindow.Employee.Phone;
                        dbEmployee.Position = editWindow.Employee.Position;
                        dbEmployee.Department = editWindow.Employee.Department;
                        dbEmployee.Address = editWindow.Employee.Address;
                        dbEmployee.Salary = editWindow.Employee.Salary;

                        context.SaveChanges();

                        Employee.FullName = dbEmployee.FullName;
                        Employee.Email = dbEmployee.Email;
                        Employee.Phone = dbEmployee.Phone;
                        Employee.Position = dbEmployee.Position;
                        Employee.Department = dbEmployee.Department;
                        Employee.Address = dbEmployee.Address;
                        Employee.Salary = dbEmployee.Salary;

                        OnPropertyChanged(nameof(Employee));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving changes: {ex.Message}", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            this.Close();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}