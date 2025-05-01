using AdvancedHRMS.Data;
using AdvancedHRMS.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AdvancedHRMS.Views
{
    public partial class LeaveApprovalWindow : Window
    {
        private readonly ApplicationDbContext _context;

        public LeaveApprovalWindow()
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            LoadLeaveRequests();
        }

        private void LoadLeaveRequests()
        {
            using var context = new ApplicationDbContext();
            var leaveRequests = context.LeaveRequests
                .Include(lr => lr.Employee)  // ✅ This line is key
                .ToList();

            dgLeaveRequests.ItemsSource = leaveRequests;

            txtTotalRequests.Text = $"Total Requests: {leaveRequests.Count}";

        }
        private void Approve_Click(object sender, RoutedEventArgs e)
        {
            if (dgLeaveRequests.SelectedItem is LeaveRequest request)
            {
                request.Status = "Approved";
                request.ProcessedById = AuthService.CurrentUser.Id;
                request.ProcessedDate = DateTime.Now;

                _context.LeaveRequests.Update(request);
                _context.SaveChanges();
                MessageBox.Show("Leave request approved.");
                LoadLeaveRequests();
            }
        }

        private void Reject_Click(object sender, RoutedEventArgs e)
        {
            if (dgLeaveRequests.SelectedItem is LeaveRequest request)
            {
                request.Status = "Rejected";
                request.ProcessedById = AuthService.CurrentUser.Id;
                request.ProcessedDate = DateTime.Now;

                _context.LeaveRequests.Update(request);
                _context.SaveChanges();
                MessageBox.Show("Leave request rejected.");
                LoadLeaveRequests();
            }
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Closes the current Leave Approval Window
        }
        protected override void OnClosed(EventArgs e)
        {
            _context.Dispose();
            base.OnClosed(e);
        }
    }
}