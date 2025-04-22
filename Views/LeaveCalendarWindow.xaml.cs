using AdvancedHRMS.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AdvancedHRMS.Views
{
    /// <summary>
    /// Interaction logic for LeaveCalendarWindow.xaml
    /// </summary>
    public partial class LeaveCalendarWindow : Window
    {
        public LeaveCalendarWindow()
        {
            InitializeComponent();
            LoadLeaveData();
        }

        private void LoadLeaveData()
        {
            using var context = new ApplicationDbContext();
            var leaves = context.LeaveRequests
                .Include(lr => lr.Employee)
                .Where(lr => lr.Status == "Approved")
                .ToList();

            foreach (var leave in leaves)
            {
                for (var date = leave.StartDate; date <= leave.EndDate; date = date.AddDays(1))
                {
                    // Highlight calendar days
                }
            }
        }
    }
}
