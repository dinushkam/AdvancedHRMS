using AdvancedHRMS.Models;
using System;
using System.Windows;
using System.Windows.Input;

namespace AdvancedHRMS.Views
{
    public partial class AddEditEmployeeWindow : Window
    {
        public AddEditEmployeeWindow() : this(new Employee())
        {
        }

        public AddEditEmployeeWindow(Employee employee)
        {
            InitializeComponent();
            DataContext = new AddEditEmployeeViewModel(employee, this);
        }

        public Employee Employee => (DataContext as AddEditEmployeeViewModel)?.Employee;
    }

    public class AddEditEmployeeViewModel
    {
        public Employee Employee { get; }
        public string WindowTitle { get; }
        public ICommand SaveCommand { get; }

        public AddEditEmployeeViewModel(Employee employee, Window window)
        {
            Employee = employee ?? new Employee();
            WindowTitle = employee.EmployeeId == 0 ? "Add New Employee" : "Edit Employee";

            SaveCommand = new RelayCommand(_ =>
            {
                if (ValidateEmployee())
                {
                    window.DialogResult = true;
                    window.Close();
                }
            });
        }

        private bool ValidateEmployee()
        {
            if (string.IsNullOrWhiteSpace(Employee.FullName))
            {
                MessageBox.Show("Full name is required.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(Employee.Email) || !IsValidEmail(Employee.Email))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (Employee.Salary < 0)
            {
                MessageBox.Show("Salary cannot be negative.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
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
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;

        public void Execute(object parameter) => _execute(parameter);

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}