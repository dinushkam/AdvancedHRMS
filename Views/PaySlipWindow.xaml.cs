using AdvancedHRMS.Models;
using System;
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
                // Recalculate to ensure displayed values are correct
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

                // Add verification
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
            // Implement printing functionality
            MessageBox.Show("Print functionality would be implemented here", "Info",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}