using AdvancedHRMS.Data;
using AdvancedHRMS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Media.Imaging;

namespace AdvancedHRMS.Views
{
    public partial class EmployeeProfileView : Window
    {
        private readonly Employee _employee;
        private readonly ApplicationDbContext _context;

        public EmployeeProfileView(Employee employee)
        {
            InitializeComponent();
            _employee = employee ?? throw new ArgumentNullException(nameof(employee));
            _context = new ApplicationDbContext();

   
            _employee = _context.Employees
                .FirstOrDefault(e => e.EmployeeId == _employee.EmployeeId) ?? _employee;

            DataContext = _employee;
        }

        private void ChangePhoto_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg)|*.png;*.jpg|All files (*.*)|*.*",
                Title = "Select profile picture"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var image = new BitmapImage(new Uri(openFileDialog.FileName));
        
                    MessageBox.Show("Profile picture updated!", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading image: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new ApplicationDbContext())
            {
              
                var dbEmployee = context.Employees
                    .FirstOrDefault(emp => emp.EmployeeId == _employee.EmployeeId);

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

                  
                        _employee.FullName = dbEmployee.FullName;
                        _employee.Email = dbEmployee.Email;
                        _employee.Phone = dbEmployee.Phone;
                        _employee.Position = dbEmployee.Position;
                        _employee.Department = dbEmployee.Department;
                        _employee.Address = dbEmployee.Address;
                        _employee.Salary = dbEmployee.Salary;

                        DataContext = null;
                        DataContext = _employee;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving changes: {ex.Message}", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _context?.Dispose();
            base.OnClosed(e);
        }
    }
}