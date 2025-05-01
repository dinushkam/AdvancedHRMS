using AdvancedHRMS.Models;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace AdvancedHRMS.Views
{
    public partial class PaySlipWindow : Window
    {
        public PaySlipWindow(Payrolls payroll)
        {
            InitializeComponent();
            LoadPayslipData(payroll);
        }

        private void LoadPayslipData(Payrolls payroll)
        {
            if (payroll == null)
            {
                MessageBox.Show("Payroll data is missing!", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
                return;
            }

            try
            {
                
                payroll.CalculateNetSalary();

                TxtEmployeeId.Text = payroll.EmployeeId.ToString();
                TxtEmployeeName.Text = payroll.Employee?.FullName ?? "N/A";
                TxtBasicSalary.Text = payroll.BasicSalary.ToString("C");
                TxtOvertimePay.Text = payroll.OvertimePay.ToString("C");
                TxtAllowances.Text = payroll.Allowances.ToString("C");
                TxtBonuses.Text = payroll.Bonuses.ToString("C");
                TxtTax.Text = payroll.Tax.ToString("C");
                TxtDeductions.Text = payroll.Deductions.ToString("C");
                TxtNetSalary.Text = payroll.NetSalary.ToString("C");

            
                decimal calculatedNet = payroll.BasicSalary + payroll.OvertimePay +
                                      payroll.Allowances + payroll.Bonuses -
                                      payroll.Deductions - payroll.Tax;

                if (Math.Abs(calculatedNet - payroll.NetSalary) > 0.01m)
                {
                    MessageBox.Show("Note: Net salary calculation discrepancy detected. Values may have been manually adjusted.",
                        "Calculation Notice", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading payslip: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                   
                    PrintArea.Measure(new Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight));
                    PrintArea.Arrange(new Rect(new Point(0, 0), PrintArea.DesiredSize));

                   
                    printDialog.PrintVisual(PrintArea, "Employee Payslip");

                    MessageBox.Show("Payslip printed successfully!", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to print payslip: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GeneratePdf_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "PDF files (*.pdf)|*.pdf",
                    FileName = $"Payslip_{TxtEmployeeId.Text}_{DateTime.Now:yyyyMMdd}.pdf"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    
                    using (var document = new iTextSharp.text.Document())
                    {
                        PdfWriter.GetInstance(document, new FileStream(saveFileDialog.FileName, FileMode.Create));
                        document.Open();

                       
                        document.Add(new iTextSharp.text.Paragraph("EMPLOYEE PAYSLIP"));
                        document.Add(new iTextSharp.text.Paragraph($"Employee ID: {TxtEmployeeId.Text}"));
                        document.Add(new iTextSharp.text.Paragraph($"Employee Name: {TxtEmployeeName.Text}"));
                        

                        MessageBox.Show("PDF generated successfully!", "Success",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to generate PDF: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}