using AdvancedHRMS.Data;
using AdvancedHRMS.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace AdvancedHRMS.Views
{
    public partial class HRProfileWindow : Window, INotifyPropertyChanged
    {
        private readonly ApplicationDbContext _context;
        private User _hrUser;

        public User CurrentUser
        {
            get => _hrUser;
            set
            {
                _hrUser = value;
                OnPropertyChanged(nameof(CurrentUser));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public HRProfileWindow(string email)
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            DataContext = this;

            // Load HR user details
            CurrentUser = _context.Users.FirstOrDefault(u => u.Email == email);
            if (CurrentUser == null)
            {
                MessageBox.Show("User not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private void UpdateProfile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _context.Entry(CurrentUser).State = EntityState.Modified;
                _context.SaveChanges();
                MessageBox.Show("Profile Updated Successfully!", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating profile: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected override void OnClosed(EventArgs e)
        {
            _context.Dispose();
            base.OnClosed(e);
        }
    }
}
