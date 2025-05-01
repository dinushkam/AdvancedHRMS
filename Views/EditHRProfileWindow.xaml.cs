using AdvancedHRMS.Models;
using AdvancedHRMS.Data;
using System.Windows;
using Microsoft.EntityFrameworkCore;

namespace AdvancedHRMS.Views
{
    public partial class EditHRProfileWindow : Window
    {
        private readonly ApplicationDbContext _context;
        public User CurrentUser { get; set; }

        public EditHRProfileWindow(string userEmail)
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            LoadUserData(userEmail);
            DataContext = this;
        }

        private void LoadUserData(string email)
        {
            CurrentUser = _context.Users
                .AsNoTracking()
                .FirstOrDefault(u => u.Email == email);

            if (CurrentUser == null)
            {
                MessageBox.Show("User not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var userToUpdate = _context.Users.FirstOrDefault(u => u.Id == CurrentUser.Id);
                if (userToUpdate != null)
                {
                    
                    userToUpdate.FullName = CurrentUser.FullName;
                    userToUpdate.Email = CurrentUser.Email;
                    userToUpdate.Department = CurrentUser.Department;
                    userToUpdate.Phone = CurrentUser.Phone;
                    userToUpdate.Address = CurrentUser.Address;

                    _context.SaveChanges();
                    DialogResult = true;
                    Close();
                }
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Error saving changes: {ex.InnerException?.Message}", "Database Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _context.Dispose();
            base.OnClosed(e);
        }
    }
}