using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace AdvancedHRMS.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Department { get; set; }







        public string Position { get; set; } = "Employee";
        public string Address { get; set; }

        [NotMapped]
        public string FullAddress => $"{Address}";

        public virtual ICollection<Attendance> Attendances { get; set; }






        public DateTime DateOfJoining { get; set; }

        public decimal Salary { get; set; }
        public int TotalLeaveDays { get; set; } = 21;

        // public bool IsApproved { get; set; } = false;

        public virtual ICollection<LeaveRequest> LeaveRequests { get; set; }

        [NotMapped]
        public int RemainingLeaveDays => TotalLeaveDays - (LeaveRequests?
            .Where(lr => lr.Status == "Approved")
            .Sum(lr => (lr.EndDate - lr.StartDate).Days + 1) ?? 0);
        [NotMapped]
        public Dictionary<string, int> LeaveBalances => new()
    {
        {"Vacation", 15 - GetUsedLeaveDays("Vacation")},
        {"Sick", 10 - GetUsedLeaveDays("Sick")},
        {"Personal", 5 - GetUsedLeaveDays("Personal")}
    };

        private int GetUsedLeaveDays(string leaveType)
        {
            return LeaveRequests?
                .Where(lr => lr.LeaveType == leaveType && lr.Status == "Approved")
                .Sum(lr => lr.Days) ?? 0;
        }
    }
}