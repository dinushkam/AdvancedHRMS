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
                        NetSalary = p.BasicSalary + p.OvertimePay + p.Allowances + p.Bonuses - (p.Deductions + p.Tax),

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
            if (ReportDataGrid.ItemsSource == null || ReportDataGrid.Items.Count == 0)
            {
                MessageBox.Show("Generate a report first!", "Warning");
                return;
            }

            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "Report",
                DefaultExt = ".pdf",
                Filter = "PDF files (*.pdf)|*.pdf"
            };

            if (dlg.ShowDialog() != true) return;

            var doc = new PdfDocument();
            var page = doc.AddPage();
            page.Orientation = PdfSharp.PageOrientation.Landscape;
            var gfx = XGraphics.FromPdfPage(page);

            var titleFont = new XFont("Verdana", 14, XFontStyleEx.Bold);
            var headerFont = new XFont("Verdana", 10, XFontStyleEx.Bold);
            var bodyFont = new XFont("Verdana", 9);
            var pen = new XPen(XColors.LightGray, 0.75);

            double margin = 40;
            double y = margin;
            double spacing = 5;

            // Logo
            string logoPath = "Assets/logo.png";
            if (File.Exists(logoPath))
            {
                XImage logo = XImage.FromFile(logoPath);

                double logoWidth = 100;
                double logoHeight = 60;

                // Center horizontally
                double centerX = (page.Width - logoWidth) / 2;

                gfx.DrawImage(logo, centerX, 20, logoWidth, logoHeight);
                y += logoHeight + 30;
            }


            y += 50;

            // Report title
            string reportTitle = ((ComboBoxItem)ReportTypeComboBox.SelectedItem)?.Content?.ToString() ?? "HR Report";
            gfx.DrawString(reportTitle, titleFont, XBrushes.Black, new XRect(0, y, page.Width, 20), XStringFormats.TopCenter);
            y += 30;

            var columns = ReportDataGrid.Columns;
            int columnCount = columns.Count;

            double usableWidth = page.Width - (2 * margin);
            double colWidth = usableWidth / columnCount;

            double rowHeight = 22;

            // Draw header row background
            gfx.DrawRectangle(XBrushes.LightGray, margin, y, usableWidth, rowHeight);

            // Header values
            double colX = margin;
            foreach (var col in columns)
            {
                string header = col.Header?.ToString() ?? "";
                gfx.DrawString(header, headerFont, XBrushes.Black, new XRect(colX + 2, y + 4, colWidth, rowHeight), XStringFormats.TopLeft);
                colX += colWidth;
            }

            // Header bottom border
            gfx.DrawLine(pen, margin, y + rowHeight, page.Width - margin, y + rowHeight);
            y += rowHeight;

            // Data rows
            foreach (var item in ReportDataGrid.Items)
            {
                if (item == null) continue;
                var props = item.GetType().GetProperties();

                colX = margin;
                foreach (var col in columns)
                {
                    string binding = col.SortMemberPath;
                    string value = "";

                    if (!string.IsNullOrEmpty(binding))
                    {
                        var prop = props.FirstOrDefault(p => p.Name == binding);
                        if (prop != null)
                        {
                            var val = prop.GetValue(item);
                            value = val != null ? val.ToString() : "";
                        }
                    }

                    gfx.DrawRectangle(XBrushes.White, colX, y, colWidth, rowHeight);
                    gfx.DrawString(value, bodyFont, XBrushes.Black, new XRect(colX + 2, y + 4, colWidth - 4, rowHeight), XStringFormats.TopLeft);
                    gfx.DrawLine(pen, colX, y, colX, y + rowHeight); // vertical line
                    colX += colWidth;
                }

                gfx.DrawLine(pen, margin, y + rowHeight, page.Width - margin, y + rowHeight); // horizontal row line
                y += rowHeight;

                if (y > page.Height - 50)
                {
                    page = doc.AddPage();
                    page.Orientation = PdfSharp.PageOrientation.Landscape;
                    gfx = XGraphics.FromPdfPage(page);
                    y = margin;
                }
            }

            // Footer
            y = page.Height - 30;
            gfx.DrawLine(XPens.Black, margin, y - 5, page.Width - margin, y - 5);
            gfx.DrawString("Generated by Advanced HRMS • " + DateTime.Now.ToString("dd-MMM-yyyy"),
                bodyFont, XBrushes.Gray, new XRect(0, y, page.Width, 20), XStringFormats.TopCenter);

            doc.Save(dlg.FileName);
            MessageBox.Show("Exported to PDF successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            Process.Start(new ProcessStartInfo(dlg.FileName) { UseShellExecute = true });
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
                    FileName = "HR_Report",
                    DefaultExt = ".csv",
                    Filter = "CSV files (*.csv)|*.csv"
                };

                if (dlg.ShowDialog() == true)
                {
                    using (var sw = new StreamWriter(dlg.FileName))
                    {
                        string reportType = (ReportTypeComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Report";
                        sw.WriteLine($"Report: {reportType}");
                        if (StartDatePicker.SelectedDate.HasValue && EndDatePicker.SelectedDate.HasValue)
                        {
                            sw.WriteLine($"Date Range: {StartDatePicker.SelectedDate:yyyy-MM-dd} to {EndDatePicker.SelectedDate:yyyy-MM-dd}");
                        }
                        sw.WriteLine(); // Empty line

                        // Header
                        sw.WriteLine(string.Join(",", ReportDataGrid.Columns.Select(c => c.Header.ToString())));

                        // Rows
                        foreach (var item in ReportDataGrid.Items)
                        {
                            if (item == null) continue;
                            var row = item.GetType().GetProperties()
                                .Select(p => $"\"{p.GetValue(item)?.ToString()?.Replace("\"", "\"\"")}\"")
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
