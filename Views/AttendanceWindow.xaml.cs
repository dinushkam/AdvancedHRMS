using AdvancedHRMS.Data;
using AdvancedHRMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AdvancedHRMS.Views
{
    public partial class AttendanceWindow : UserControl
    {
        private readonly ApplicationDbContext _context;
        private List<AttendanceRecord> _attendanceRecords;

        public AttendanceWindow()
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            LoadAttendanceData();
        }

        private void LoadAttendanceData()
        {
            try
            {
                // Get current employee's attendance records
                var employeeEmail = AuthService.CurrentUser?.Email;
                if (employeeEmail == null) return;

                var employee = _context.Employees.FirstOrDefault(e => e.Email == employeeEmail);
                if (employee == null) return;

                // Sample data - replace with actual database records
                _attendanceRecords = new List<AttendanceRecord>
                {
                    new AttendanceRecord
                    {
                        Date = DateTime.Today,
                        CheckInTime = DateTime.Today.AddHours(9),
                        CheckOutTime = DateTime.Today.AddHours(17),
                        HoursWorked = "8 hours",
                        Status = "Completed"
                    },
                    new AttendanceRecord
                    {
                        Date = DateTime.Today.AddDays(-1),
                        CheckInTime = DateTime.Today.AddDays(-1).AddHours(9),
                        CheckOutTime = DateTime.Today.AddDays(-1).AddHours(17.5),
                        HoursWorked = "8.5 hours",
                        Status = "Completed"
                    }
                };

                AttendanceGrid.ItemsSource = _attendanceRecords;
                UpdateSummary();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading attendance data: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateSummary()
        {
            // Calculate totals
            var todayHours = _attendanceRecords
                .Where(r => r.Date == DateTime.Today)
                .Sum(r => r.HoursWorked.Contains("hours") ?
                    double.Parse(r.HoursWorked.Replace(" hours", "")) : 0);

            var monthHours = _attendanceRecords
                .Where(r => r.Date.Month == DateTime.Today.Month && r.Date.Year == DateTime.Today.Year)
                .Sum(r => r.HoursWorked.Contains("hours") ?
                    double.Parse(r.HoursWorked.Replace(" hours", "")) : 0);

            TxtTodayHours.Text = $"{todayHours} hours";
            TxtMonthHours.Text = $"{monthHours} hours";
        }

        private void CheckIn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var checkInTime = DateTime.Now;
                _attendanceRecords.Add(new AttendanceRecord
                {
                    Date = checkInTime.Date,
                    CheckInTime = checkInTime,
                    Status = "In Progress"
                });

                AttendanceGrid.Items.Refresh();
                BtnCheckIn.IsEnabled = false;
                BtnCheckOut.IsEnabled = true;
                UpdateSummary();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking in: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CheckOut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var checkOutTime = DateTime.Now;
                var todayRecord = _attendanceRecords.FirstOrDefault(r => r.Date == DateTime.Today && r.CheckOutTime == null);

                if (todayRecord != null)
                {
                    todayRecord.CheckOutTime = checkOutTime;
                    var hoursWorked = (checkOutTime - todayRecord.CheckInTime);
                    todayRecord.HoursWorked = $"{hoursWorked:0.0} hours";
                    todayRecord.Status = "Completed";
                }

                AttendanceGrid.Items.Refresh();
                BtnCheckIn.IsEnabled = true;
                BtnCheckOut.IsEnabled = false;
                UpdateSummary();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking out: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected virtual void OnClosed(EventArgs e)
        {
            _context?.Dispose();
        }
    }

    public class AttendanceRecord
    {
        public DateTime Date { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public string HoursWorked { get; set; }
        public string Status { get; set; }
    }
}