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
            var requests = _context.LeaveRequests
                .Include(lr => lr.Employee)
                .Where(lr => lr.Status == "Pending")
                .ToList();

           

            dgLeaveRequests.ItemsSource = requests;
        }

        private void Approve_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var request = button?.DataContext as LeaveRequest;

            if (request != null)
            {
                request.Status = "Approved";
                request.ProcessedById = AuthService.CurrentUser.Id;
                request.ProcessedDate = DateTime.Now;
                _context.SaveChanges();
                LoadLeaveRequests();
            }
        }

        private void Reject_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var request = button?.DataContext as LeaveRequest;

            if (request != null)
            {
                var dialog = new RejectReasonDialog();
                if (dialog.ShowDialog() == true)
                {
                    request.Status = "Rejected";
                    request.ProcessedById = AuthService.CurrentUser.Id;
                    request.ProcessedDate = DateTime.Now;
                    request.RejectionReason = dialog.Reason;
                    _context.SaveChanges();
                    LoadLeaveRequests();
                }
            }
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Closes the current Leave Approval Window
        }

    }
}