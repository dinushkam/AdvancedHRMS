using AdvancedHRMS.Data;
using AdvancedHRMS.Models;
using Microsoft.VisualBasic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AdvancedHRMS.Views
{
    public partial class DepartmentManagementWindow : Window
    {
        private readonly ApplicationDbContext _context;

        public DepartmentManagementWindow()
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            LoadDepartments();
        }

        private void LoadDepartments()
        {
            DepartmentDataGrid.ItemsSource = _context.Departments.ToList();
        }

        private void AddDepartment_Click(object sender, RoutedEventArgs e)
        {
            string deptName = Interaction.InputBox("Enter Department Name:", "Add Department", "");
            if (!string.IsNullOrWhiteSpace(deptName))
            {
                var department = new Department { Name = deptName };
                _context.Departments.Add(department);
                _context.SaveChanges();
                LoadDepartments();
            }
        }

        private void EditDepartment_Click(object sender, RoutedEventArgs e)
        {
            if (DepartmentDataGrid.SelectedItem is Department selectedDept)
            {
                string newName = Interaction.InputBox("Edit Department Name:", "Edit Department", selectedDept.Name);
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    selectedDept.Name = newName;
                    _context.SaveChanges();
                    LoadDepartments();
                }
            }
        }

        private void DeleteDepartment_Click(object sender, RoutedEventArgs e)
        {
            if (DepartmentDataGrid.SelectedItem is Department selectedDept)
            {
                _context.Departments.Remove(selectedDept);
                _context.SaveChanges();
                LoadDepartments();
            }
        }
    }
}
