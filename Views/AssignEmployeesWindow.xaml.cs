using AdvancedHRMS.Data;
using AdvancedHRMS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace AdvancedHRMS.Views
{
    public partial class AssignEmployeesWindow : Window
    {
        private readonly ApplicationDbContext _context;
        private readonly Department _department;
        private List<EmployeeSelectable> _employees;

        public AssignEmployeesWindow(Department department)
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            _department = department;
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            var allEmployees = _context.Employees.ToList(); // Fetch all
            var departmentEmployeeIds = _department.Employees?.Select(emp => emp.EmployeeId).ToList() ?? new List<int>();

            _employees = allEmployees
                .Select(e => new EmployeeSelectable
                {
                    EmployeeId = e.EmployeeId,
                    FullName = e.FullName,
                    Position = e.Position,
                    IsSelected = departmentEmployeeIds.Contains(e.EmployeeId)
                })
                .ToList();

            EmployeeGrid.ItemsSource = _employees;
        }


        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var selectedIds = _employees.Where(e => e.IsSelected).Select(e => e.EmployeeId).ToList();

            var selectedEmployees = _context.Employees.Where(e => selectedIds.Contains(e.EmployeeId)).ToList();

            _department.Employees.Clear(); // Clear previous
            foreach (var emp in selectedEmployees)
            {
                _department.Employees.Add(emp);
            }

            _context.Departments.Update(_department);
            _context.SaveChanges();

            MessageBox.Show("Employees assigned successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            this.DialogResult = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        protected override void OnClosed(System.EventArgs e)
        {
            _context.Dispose();
            base.OnClosed(e);
        }
    }

    // Helper class for checkbox binding
    public class EmployeeSelectable
    {
        public bool IsSelected { get; set; }
        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
    }
}
