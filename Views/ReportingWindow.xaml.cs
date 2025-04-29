using AdvancedHRMS.Data;
using AdvancedHRMS.Models;
using Microsoft.EntityFrameworkCore;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace AdvancedHRMS.Views
{
    public partial class ReportingWindow : Window
    {
        private readonly ApplicationDbContext _context;

        public ReportingWindow()
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            LoadDepartments();
        }

        private void LoadDepartments()
        {
            DepartmentFilterComboBox.ItemsSource = _context.Departments.Select(d => d.Name).ToList();
        }

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            string selectedReport = (ReportTypeComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
            string departmentFilter = DepartmentFilterComboBox.SelectedItem?.ToString();
            DateTime? startDate = StartDatePicker.SelectedDate;
            DateTime? endDate = EndDatePicker.SelectedDate;

            if (string.IsNullOrEmpty(selectedReport))
            {
                MessageBox.Show("Please select a report type.", "Warning");
                return;
            }

            if (selectedReport == "Attendance Report")
            {
                var data = _context.Attendances
                    .Include(a => a.Employee)
                    .Where(a => (!startDate.HasValue || a.Date >= startDate) &&
                                (!endDate.HasValue || a.Date <= endDate) &&
                                (string.IsNullOrEmpty(departmentFilter) || a.Employee.Department != null && a.Employee.Department.Name == departmentFilter
))
                    .Select(a => new
                    {
                        a.EmployeeId,
                        a.Employee.FullName,
                        a.Employee.Department,
                        a.Date,
                        a.CheckInTime,
                        a.CheckOutTime,
                        a.HoursWorked
                    }).ToList();

                ReportDataGrid.ItemsSource = data;
            }
            else if (selectedReport == "Payroll Report")
            {
                var data = _context.Payrolls
                    .Include(p => p.Employee)
                    .Where(p => (!startDate.HasValue || p.PaymentDate >= startDate) &&
                                (!endDate.HasValue || p.PaymentDate <= endDate) &&
                                (string.IsNullOrEmpty(departmentFilter) || p.Employee.Department != null && p.Employee.Department.Name == departmentFilter
))
                    .Select(p => new
                    {
                        p.EmployeeId,
                        p.Employee.FullName,
                        p.Employee.Department,
                        p.BasicSalary,
                        p.Bonuses,
                        p.Deductions,
                        p.Tax,
                        p.NetSalary,
                        p.PaymentDate
                    }).ToList();

                ReportDataGrid.ItemsSource = data;
            }
            else if (selectedReport == "Employee Performance Report")
            {
                var data = _context.Employees
                    .Include(e => e.LeaveRequests)
                    .Include(e => e.Attendances)
                    .Where(e => string.IsNullOrEmpty(departmentFilter) ||e.Department != null && e.Department.Name == departmentFilter
)
                    .Select(e => new
                    {
                        e.EmployeeId,
                        e.FullName,
                        e.Department,
                        TotalAttendanceDays = e.Attendances.Count,
                        TotalApprovedLeaves = e.LeaveRequests.Count(lr => lr.Status == "Approved"),
                        e.RemainingLeaveDays,
                        e.Salary
                    }).ToList();

                ReportDataGrid.ItemsSource = data;
            }
        }

        private void ExportToPdf_Click(object sender, RoutedEventArgs e)
        {
            if (ReportDataGrid.ItemsSource == null)
            {
                MessageBox.Show("Generate a report first!", "Warning");
                return;
            }

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "HR_Report",
                DefaultExt = ".pdf",
                Filter = "PDF files (*.pdf)|*.pdf"
            };

            if (dlg.ShowDialog() == true)
            {
                var doc = new PdfDocument();
                var page = doc.AddPage();
                var gfx = XGraphics.FromPdfPage(page);
                var font = new XFont("Arial", 10, XFontStyleEx.Regular);

                double y = 40;

                // Draw logo
                string logoPath = "Assets/logo.png"; // Ensure this exists
                if (File.Exists(logoPath))
                {
                    XImage logo = XImage.FromFile(logoPath);
                    gfx.DrawImage(logo, 40, 20, 100, 30);
                    y += 40;
                }

                // Draw headers
                double colX = 40;
                foreach (var col in ReportDataGrid.Columns)
                {
                    string header = col.Header.ToString();
                    gfx.DrawString(header, font, XBrushes.Black, new XRect(colX, y, 100, page.Height), XStringFormats.TopLeft);
                    colX += 100;
                }

                y += 25;

                // Draw rows
                foreach (var item in ReportDataGrid.Items)
                {
                    if (item == null) continue;
                    var props = item.GetType().GetProperties();

                    colX = 40;
                    foreach (var prop in props)
                    {
                        string value = prop.GetValue(item)?.ToString();
                        
                        colX += 100;
                    }
                    y += 20;

                    if (y > page.Height - 50)
                    {
                        page = doc.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        y = 40;
                    }
                }

                doc.Save(dlg.FileName);
                MessageBox.Show("Exported to PDF successfully!");
                Process.Start(new ProcessStartInfo(dlg.FileName) { UseShellExecute = true });
            }
        }

        private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ReportDataGrid.ItemsSource == null)
                {
                    MessageBox.Show("No report to export!", "Warning");
                    return;
                }

                var dlg = new Microsoft.Win32.SaveFileDialog
                {
                    FileName = "Report",
                    DefaultExt = ".csv",
                    Filter = "CSV files (*.csv)|*.csv"
                };

                if (dlg.ShowDialog() == true)
                {
                    using (var sw = new StreamWriter(dlg.FileName))
                    {
                        // Write header
                        sw.WriteLine(string.Join(",", ReportDataGrid.Columns.Select(c => c.Header.ToString())));

                        foreach (var item in ReportDataGrid.Items)
                        {
                            if (item == null) continue;
                            var row = item.GetType().GetProperties()
                                          .Select(p => p.GetValue(item)?.ToString())
                                          .ToArray();
                            sw.WriteLine(string.Join(",", row));
                        }
                    }

                    MessageBox.Show("Exported to Excel successfully!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _context.Dispose();
            base.OnClosed(e);
        }
    }
}
