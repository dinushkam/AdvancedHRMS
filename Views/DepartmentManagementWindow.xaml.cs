using AdvancedHRMS.Data;
using AdvancedHRMS.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;

namespace AdvancedHRMS.Views
{
    public partial class DepartmentManagementWindow : Window
    {
        private readonly ApplicationDbContext _context;
        private Department _selectedDepartment;

        public DepartmentManagementWindow()
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            LoadDepartments();
        }

        private void LoadDepartments()
        {
            var departments = _context.Departments
                .Include(d => d.Employees)
                .ToList();

            DepartmentGrid.ItemsSource = departments;
        }


        private void DepartmentGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _selectedDepartment = DepartmentGrid.SelectedItem as Department;

            if (_selectedDepartment != null)
            {
                txtName.Text = _selectedDepartment.Name;
                txtBudget.Text = _selectedDepartment.Budget.ToString();
                txtDescription.Text = _selectedDepartment.Description;
                EmployeeGrid.ItemsSource = _selectedDepartment.Employees.ToList();
            }
        }

     

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedDepartment == null) return;

            _selectedDepartment.Name = txtName.Text;
            if (decimal.TryParse(txtBudget.Text, out var budget))
            {
                _selectedDepartment.Budget = budget;
            }
            _selectedDepartment.Description = txtDescription.Text;

            _context.Departments.Update(_selectedDepartment);
            _context.SaveChanges();
            LoadDepartments();

            MessageBox.Show("Department updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AssignEmployees_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedDepartment == null)
            {
                MessageBox.Show("Please select a department first.", "Warning");
                return;
            }

            var assignWindow = new AssignEmployeesWindow(_selectedDepartment);
            if (assignWindow.ShowDialog() == true)
            {
                _selectedDepartment = _context.Departments
     .Include(d => d.Employees)
     .FirstOrDefault(d => d.DepartmentId == _selectedDepartment.DepartmentId);

                LoadDepartments();

                if (_selectedDepartment != null)
                {
                    txtName.Text = _selectedDepartment.Name;
                    txtBudget.Text = _selectedDepartment.Budget.ToString();
                    txtDescription.Text = _selectedDepartment.Description;
                    EmployeeGrid.ItemsSource = _selectedDepartment.Employees.ToList();
                }

            }
        }




        protected override void OnClosed(System.EventArgs e)
        {
            _context.Dispose();
            base.OnClosed(e);
        }
    }
}
