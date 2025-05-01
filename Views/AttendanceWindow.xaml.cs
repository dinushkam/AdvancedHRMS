using AdvancedHRMS.Data;
using AdvancedHRMS.Models;
using System;
using System.Linq;
using System.Windows;

namespace AdvancedHRMS.Views
{
    public partial class AttendanceWindow : Window
    {
        private readonly ApplicationDbContext _context;
        private readonly Employee _employee;

        public AttendanceWindow(Employee employee)
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            _employee = employee;
            LoadAttendanceData();
        }

        private void LoadAttendanceData()
        {
            try
            {
                AttendanceGrid.ItemsSource = _context.Attendances
                    .Where(a => a.EmployeeId == _employee.EmployeeId)
                    .OrderByDescending(a => a.Date)
                    .ToList();

                var today = _context.Attendances
                    .FirstOrDefault(a => a.EmployeeId == _employee.EmployeeId && a.Date == DateTime.Today);

                if (today != null)
                {
                    BtnCheckIn.IsEnabled = today.CheckInTime == null;
                    BtnCheckOut.IsEnabled = today.CheckInTime != null && today.CheckOutTime == null;
                }
                else
                {
                    BtnCheckIn.IsEnabled = true;
                    BtnCheckOut.IsEnabled = false;
                }

                UpdateSummary();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading attendance: {ex.Message}");
            }
        }

        private void UpdateSummary()
        {
            var attendances = _context.Attendances
                .Where(a => a.EmployeeId == _employee.EmployeeId)
                .ToList();

            double todayHours = attendances
                .Where(r => r.Date.Date == DateTime.Today)
                .Sum(r => r.HoursWorked);

            double monthHours = attendances
                .Where(r => r.Date.Month == DateTime.Today.Month && r.Date.Year == DateTime.Today.Year)
                .Sum(r => r.HoursWorked);

            TxtTodayHours.Text = $"{todayHours:0.0} hours";
            TxtMonthHours.Text = $"{monthHours:0.0} hours";
        }

        private void CheckIn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var today = DateTime.Today;

                var existing = _context.Attendances
                    .FirstOrDefault(a => a.EmployeeId == _employee.EmployeeId && a.Date == today);

                if (existing != null)
                {
                    MessageBox.Show("Already checked in today!");
                    return;
                }

                var newRecord = new Attendance
                {
                    EmployeeId = _employee.EmployeeId,
                    Date = today,
                    CheckInTime = DateTime.Now,
                    Status = "In Progress"
                };

                _context.Attendances.Add(newRecord);
                _context.SaveChanges();

                LoadAttendanceData();
                MessageBox.Show("Checked In Successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during check-in: {ex.Message}");
            }
        }

        private void CheckOut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var today = DateTime.Today;

                var todayRecord = _context.Attendances
                    .FirstOrDefault(a => a.EmployeeId == _employee.EmployeeId && a.Date == today);

                if (todayRecord == null || todayRecord.CheckInTime == null)
                {
                    MessageBox.Show("No check-in found for today!");
                    return;
                }

                todayRecord.CheckOutTime = DateTime.Now;
                todayRecord.Status = "Completed";

                _context.SaveChanges();

                LoadAttendanceData();
                MessageBox.Show("Checked Out Successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during check-out: {ex.Message}");
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _context?.Dispose();
            base.OnClosed(e);
        }
    }
}
