using AdvancedHRMS.Data;
using AdvancedHRMS.Models;
using AdvancedHRMS.Services;
using AdvancedHRMS.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace AdvancedHRMS.Views
{
    public partial class PayrollWindow : Window
    {
        private readonly ApplicationDbContext _context;
        private readonly IPayrollService _payrollService;

        public PayrollWindow()
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            _payrollService = new PayrollService(_context);
            LoadPayrollData();
        }

        private void LoadPayrollData()
        {
            try
            {
               
                var payrolls = _context.Payrolls
                    .Include(p => p.Employee)
                    .AsEnumerable() 
                    .Select(p => {
                        p.CalculateNetSalary(); 
                        return p;
                    })
                    .ToList();

                PayrollDataGrid.ItemsSource = payrolls;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading payroll data: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GeneratePayslip_Click(object sender, RoutedEventArgs e)
        {
            if (PayrollDataGrid.SelectedItem is Payrolls selectedPayroll)
            {
                // Recalculate net salary to ensure accuracy
                selectedPayroll.CalculateNetSalary();

                var paySlipWindow = new PaySlipWindow(selectedPayroll);
                paySlipWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select a payroll record first.", "Warning",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadPayrollData();
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void PayrollDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            GeneratePayslip_Click(sender, e);
        }

        protected override void OnClosed(EventArgs e)
        {
            _context.Dispose();
            base.OnClosed(e);
        }


    }
}