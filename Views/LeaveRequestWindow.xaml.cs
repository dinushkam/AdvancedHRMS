using AdvancedHRMS.Data;
using AdvancedHRMS.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace AdvancedHRMS.Views
{
    public partial class LeaveRequestWindow : Window
    {
        private readonly ApplicationDbContext _context;
        private readonly Employee _employee;

        public LeaveRequestWindow(Employee employee)
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            _employee = employee;
            DataContext = _employee;
            dpStartDate.SelectedDate = DateTime.Today;
            dpEndDate.SelectedDate = DateTime.Today;
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            if (dpStartDate.SelectedDate == null || dpEndDate.SelectedDate == null)
            {
                MessageBox.Show("Please select both start and end dates");
                return;
            }

            var leaveRequest = new LeaveRequest
            {
                EmployeeId = _employee.EmployeeId,
                StartDate = dpStartDate.SelectedDate.Value,
                EndDate = dpEndDate.SelectedDate.Value,
                LeaveType = (cmbLeaveType.SelectedItem as ComboBoxItem)?.Content.ToString(),
                Reason = txtReason.Text,
                Status = "Pending"
            };

            try
            {
                _context.LeaveRequests.Add(leaveRequest);
                _context.SaveChanges();
                MessageBox.Show("Leave request submitted successfully!");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error submitting leave request: {ex.Message}");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}