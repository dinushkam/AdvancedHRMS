using AdvancedHRMS.Models;
using System.Windows;

namespace AdvancedHRMS.Views
{
    public partial class EditEmployeeProfile : Window
    {
        public Employee Employee { get; }

        public EditEmployeeProfile(Employee employee)
        {
            InitializeComponent();
            Employee = employee ?? throw new ArgumentNullException(nameof(employee));

 
            Employee = new Employee
            {
                EmployeeId = employee.EmployeeId,
                FullName = employee.FullName,
                Email = employee.Email,
                Phone = employee.Phone,
                Position = employee.Position,
                Department = employee.Department,
                Address = employee.Address,
                Salary = employee.Salary,
                DateOfJoining = employee.DateOfJoining
            };

            DataContext = Employee;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Employee.FullName))
            {
                MessageBox.Show("Full name is required!", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!IsValidEmail(Employee.Email))
            {
                MessageBox.Show("Please enter a valid email address!", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DialogResult = true;
            Close();
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

}