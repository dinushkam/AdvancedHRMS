
using AdvancedHRMS.Data;
using AdvancedHRMS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvancedHRMS.Services
{
    public class DepartmentService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public DepartmentService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Department>> GetAllDepartmentsAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Departments
                .Include(d => d.Manager)
                .Include(d => d.Employees)
                .ToListAsync();
        }

        public async Task<Department> GetDepartmentByIdAsync(int id)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Departments
                .Include(d => d.Manager)
                .Include(d => d.Employees)
                .FirstOrDefaultAsync(d => d.DepartmentId == id);
        }

        public async Task<bool> UpdateDepartmentAsync(Department department)
        {
            using var context = _contextFactory.CreateDbContext();
            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var existingDept = await context.Departments
                    .Include(d => d.Employees)
                    .FirstOrDefaultAsync(d => d.DepartmentId == department.DepartmentId);

                if (existingDept == null) return false;

                bool nameChanged = existingDept.Name != department.Name;
                context.Entry(existingDept).CurrentValues.SetValues(department);

                existingDept.Manager = department.ManagerId.HasValue
                    ? await context.Employees.FindAsync(department.ManagerId.Value)
                    : null;

             
                if (nameChanged)
                {
                    foreach (var employee in existingDept.Employees)
                    {
                        employee.DepartmentId = department.DepartmentId;


                    }
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<Employee>> GetAllManagersAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Employees
                .Where(e => e.Position == "Manager" || e.Position == "Director")
                .OrderBy(e => e.FullName)
                .ToListAsync();
        }
    }
}