using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdvancedHRMS.Models
{
    public class Payrolls
    {
        [Key]
        public int PayrollId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual Employee? Employee { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Basic salary must be positive")]
        public decimal BasicSalary { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Overtime hours must be positive")]
        public int OvertimeHours { get; set; } = 0;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Overtime pay must be positive")]
        public decimal OvertimePay { get; set; } = 0;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Allowances must be positive")]
        public decimal Allowances { get; set; } = 0;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Deductions must be positive")]
        public decimal Deductions { get; set; } = 0;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Bonuses must be positive")]
        public decimal Bonuses { get; set; } = 0;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Tax must be positive")]
        public decimal Tax { get; set; } = 0;

        [Column(TypeName = "decimal(10,2)")]
        public decimal NetSalary { get; private set; }

        [Required]
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        // Status tracking
        public string Status { get; set; } = "Pending"; // Pending, Approved, Paid

        // Payment method
        public string PaymentMethod { get; set; } = "Bank Transfer"; // Bank Transfer, Cash, Check

        // Method to calculate and set the net salary with validation
        public void CalculateNetSalary()
        {
            // Validate inputs
            if (BasicSalary < 0) throw new ArgumentException("Basic salary cannot be negative");
            if (OvertimePay < 0) throw new ArgumentException("Overtime pay cannot be negative");
            if (Allowances < 0) throw new ArgumentException("Allowances cannot be negative");
            if (Bonuses < 0) throw new ArgumentException("Bonuses cannot be negative");
            if (Deductions < 0) throw new ArgumentException("Deductions cannot be negative");
            if (Tax < 0) throw new ArgumentException("Tax cannot be negative");

            // Calculate gross salary
            decimal grossSalary = BasicSalary + OvertimePay + Allowances + Bonuses;

            // Calculate total deductions
            decimal totalDeductions = Deductions + Tax;

            // Calculate net salary
            decimal calculatedNet = grossSalary - totalDeductions;

            // Ensure net salary isn't negative (though legally possible, we flag it)
            NetSalary = calculatedNet < 0 ? 0 : calculatedNet;
        }

        // Method to validate the payroll before saving
        public bool Validate(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (EmployeeId <= 0)
            {
                errorMessage = "Invalid employee ID";
                return false;
            }

            if (PaymentDate > DateTime.Now.AddDays(30))
            {
                errorMessage = "Payment date cannot be more than 30 days in the future";
                return false;
            }

            try
            {
                CalculateNetSalary();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }

            return true;
        }

        // Method to approve payroll
        public void ApprovePayroll(int approvedById)
        {
            if (Status != "Pending")
                throw new InvalidOperationException("Only pending payrolls can be approved");

            Status = "Approved";
            // In a real system, you might want to track who approved it
        }

        // Method to mark as paid
        public void MarkAsPaid(string paymentMethod)
        {
            if (Status != "Approved")
                throw new InvalidOperationException("Only approved payrolls can be marked as paid");

            Status = "Paid";
            PaymentMethod = paymentMethod;
        }
    }
}