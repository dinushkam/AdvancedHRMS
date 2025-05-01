using AdvancedHRMS.Data;
using AdvancedHRMS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvancedHRMS.Services
{
    public interface IPayrollService
    {
        void GeneratePayroll(int employeeId, PayrollInputModel input);
        List<Payrolls> GetEmployeePayrollHistory(int employeeId);
        Payrolls GetPayrollById(int payrollId);
    }

    public class PayrollService : IPayrollService
    {
        private readonly ApplicationDbContext _context;

        public PayrollService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void GeneratePayroll(int employeeId, PayrollInputModel input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

        
            decimal overtimePay = input.OvertimeHours * input.OvertimeRate;
            decimal tax = (input.BasicSalary + overtimePay + input.Allowances) * input.TaxRate;

            var payroll = new Payrolls
            {
                EmployeeId = employeeId,
                BasicSalary = input.BasicSalary,
                OvertimeHours = input.OvertimeHours,
                OvertimePay = overtimePay,
                Allowances = input.Allowances,
                Deductions = input.Deductions,
                Bonuses = input.Bonuses,
                Tax = tax,
                PaymentDate = DateTime.Now
            };


            payroll.CalculateNetSalary();  

            _context.Payrolls.Add(payroll);
            _context.SaveChanges();
        }
        public List<Payrolls> GetEmployeePayrollHistory(int employeeId)
        {
            return _context.Payrolls
                .Include(p => p.Employee)
                .Where(p => p.EmployeeId == employeeId)
                .OrderByDescending(p => p.PaymentDate)
                .ToList();
        }

        public Payrolls GetPayrollById(int payrollId)
        {
            return _context.Payrolls
                .Include(p => p.Employee)
                .FirstOrDefault(p => p.PayrollId == payrollId);
        }
    }

    public class PayrollInputModel
    {
        public decimal BasicSalary { get; set; }
        public int OvertimeHours { get; set; }
        public decimal OvertimeRate { get; set; } = 500m; 
        public decimal Allowances { get; set; }
        public decimal Deductions { get; set; }
        public decimal Bonuses { get; set; }
        public decimal TaxRate { get; set; } = 0.15m; 
    }
}