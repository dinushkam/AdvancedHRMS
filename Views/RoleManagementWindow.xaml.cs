using AdvancedHRMS.Data;
using AdvancedHRMS.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;

namespace AdvancedHRMS.Views
{
    public partial class RoleManagementWindow : Window
    {
        private readonly ApplicationDbContext _context;

        public RoleManagementWindow()
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            LoadUsers();
        }

        private void LoadUsers()
        {
            UsersDataGrid.ItemsSource = _context.Users.ToList();
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _context.SaveChanges();
                MessageBox.Show("Changes saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Error saving changes: {ex.InnerException?.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}