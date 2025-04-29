using AdvancedHRMS.Data;
using AdvancedHRMS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AdvancedHRMS.Views
{
    public partial class EmployeeManagementWindow : Window
    {
        private readonly ApplicationDbContext _context;
        private List<Employee> _allEmployees;
        private List<string> _departments = new List<string>
        {
            "IT", "Finance"
        };
        private List<string> _positions = new List<string>
        {
            "Employee", "Manager", "Supervisor", "Director", "HR Specialist"
        };

        public EmployeeManagementWindow()
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            LoadData();
            InitializeFilters();
        }

        private void LoadData()
        {
            try
            {
                _allEmployees = _context.Employees.ToList();
                EmployeeDataGrid.ItemsSource = _allEmployees;

                // Get unique departments from actual employees
                _departments = _allEmployees
                    .Select(e => e.Department)
                    .Where(d => !string.IsNullOrEmpty(d))
                    .Distinct()
                    .OrderBy(d => d)
                    .ToList();

                // Get unique positions from actual employees
                _positions = _allEmployees
                    .Select(e => e.Position)
                    .Where(p => !string.IsNullOrEmpty(p))
                    .Distinct()
                    .OrderBy(p => p)
                    .ToList();

                // Reinitialize filters with updated data
                InitializeFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading employees: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitializeFilters()
        {
            // Initialize with "All" option first
            var allDepartments = new List<string> { "All Departments" };
            allDepartments.AddRange(_departments);

            var allPositions = new List<string> { "All Positions" };
            allPositions.AddRange(_positions);

            DepartmentFilter.ItemsSource = allDepartments;
            PositionFilter.ItemsSource = allPositions;

            // Select "All" by default
            DepartmentFilter.SelectedIndex = 0;
            PositionFilter.SelectedIndex = 0;
        }

        private void ApplyFilters()
        {
            var filtered = _allEmployees.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                filtered = filtered.Where(e =>
                    e.FullName.Contains(SearchTextBox.Text, StringComparison.OrdinalIgnoreCase) ||
                    e.Email.Contains(SearchTextBox.Text, StringComparison.OrdinalIgnoreCase) ||
                    e.EmployeeId.ToString().Contains(SearchTextBox.Text));
            }

            // Apply department filter (skip if "All Departments" is selected)
            if (DepartmentFilter.SelectedIndex > 0)
            {
                filtered = filtered.Where(e => e.Department == DepartmentFilter.SelectedItem.ToString());
            }

            // Apply position filter (skip if "All Positions" is selected)
            if (PositionFilter.SelectedIndex > 0)
            {
                filtered = filtered.Where(e => e.Position == PositionFilter.SelectedItem.ToString());
            }

            EmployeeDataGrid.ItemsSource = filtered.ToList();
        }

        private void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddEditEmployeeWindow();
            if (addWindow.ShowDialog() == true)
            {
                try
                {
                    _context.Employees.Add(addWindow.Employee);
                    _context.SaveChanges();
                    LoadData();
                    MessageBox.Show("Employee added successfully!", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding employee: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void EditEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeeDataGrid.SelectedItem is Employee selectedEmployee)
            {
                var editWindow = new AddEditEmployeeWindow(selectedEmployee);
                if (editWindow.ShowDialog() == true)
                {
                    try
                    {
                        _context.Entry(selectedEmployee).CurrentValues.SetValues(editWindow.Employee);
                        _context.SaveChanges();
                        LoadData();
                        MessageBox.Show("Employee updated successfully!", "Success",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating employee: {ex.Message}", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select an employee to edit.", "Warning",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeeDataGrid.SelectedItem is Employee selectedEmployee)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete {selectedEmployee.FullName}?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _context.Employees.Remove(selectedEmployee);
                        _context.SaveChanges();
                        LoadData();
                        MessageBox.Show("Employee deleted successfully.", "Success",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting employee: {ex.Message}", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select an employee to delete.", "Warning",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ClearFilters_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = "";
            DepartmentFilter.SelectedIndex = -1;
            PositionFilter.SelectedIndex = -1;
            ApplyFilters();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        protected override void OnClosed(EventArgs e)
        {
            _context.Dispose();
            base.OnClosed(e);
        }
    }
}