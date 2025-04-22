using AdvancedHRMS.Data;
using AdvancedHRMS.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using Microsoft.VisualBasic; // For input dialogs

namespace AdvancedHRMS.Views
{
    public partial class EmployeeManagementWindow : Window
    {
        private readonly ApplicationDbContext _context;

        public EmployeeManagementWindow()
        {
            InitializeComponent();
            _context = new ApplicationDbContext(new DbContextOptions<ApplicationDbContext>());
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            EmployeeDataGrid.ItemsSource = _context.Employees.ToList();
        }

        private void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            string fullName = Interaction.InputBox("Enter Full Name:", "Add Employee", "");
            string email = Interaction.InputBox("Enter Email:", "Add Employee", "");
            string phone = Interaction.InputBox("Enter Phone:", "Add Employee", "");
            string department = Interaction.InputBox("Enter Department:", "Add Employee", "");
            string salaryStr = Interaction.InputBox("Enter Salary:", "Add Employee", "50000");

            if (!string.IsNullOrWhiteSpace(fullName) && !string.IsNullOrWhiteSpace(email))
            {
                var newEmployee = new Employee
                {
                    FullName = fullName,
                    Email = email,
                    Phone = phone,
                    Department = department,
                    DateOfJoining = System.DateTime.Now,
                    Salary = decimal.Parse(salaryStr)
                };

                _context.Employees.Add(newEmployee);
                _context.SaveChanges();
                LoadEmployees();
            }
            else
            {
                MessageBox.Show("Name and Email are required!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeeDataGrid.SelectedItem is Employee selectedEmployee)
            {
                string newName = Interaction.InputBox("Enter New Name:", "Edit Employee", selectedEmployee.FullName);
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    selectedEmployee.FullName = newName;
                    _context.Employees.Update(selectedEmployee);
                    _context.SaveChanges();
                    LoadEmployees();
                }
            }
        }

        private void DeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeeDataGrid.SelectedItem is Employee selectedEmployee)
            {
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete {selectedEmployee.FullName}?",
                                                          "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    _context.Employees.Remove(selectedEmployee);
                    _context.SaveChanges();
                    LoadEmployees();
                }
            }
        }
    }
}
