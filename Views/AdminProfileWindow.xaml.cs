using AdvancedHRMS.Data;
using AdvancedHRMS.Models;
using System.Linq;
using System.Windows;

namespace AdvancedHRMS.Views
{
    public partial class AdminProfileWindow : Window
    {
        private User _adminUser;

        public AdminProfileWindow()
        {
            InitializeComponent();
            LoadAdminProfile();
        }

        private void LoadAdminProfile()
        {
            using (var context = new ApplicationDbContext())
            {
                _adminUser = context.Users.FirstOrDefault(u => u.Id == AuthService.CurrentUser.Id);

                if (_adminUser != null)
                {
                    txtFullName.Text = _adminUser.Username;
                    txtEmail.Text = _adminUser.Email;
                    txtPhone.Text = _adminUser.Phone ?? "";
                    txtDepartment.Text = _adminUser.Department ?? "Administration";
                    txtRole.Text = _adminUser.Role;
                }
                else
                {
                    MessageBox.Show("Admin details not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new ApplicationDbContext())
            {
                var admin = context.Users.FirstOrDefault(u => u.Id == _adminUser.Id);
                if (admin != null)
                {
                    admin.Username = txtFullName.Text;
                    admin.Phone = txtPhone.Text;
                    admin.Department = txtDepartment.Text;

                    // Update password if a new one is provided
                   
                    context.SaveChanges();
                    MessageBox.Show("Profile updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
            }
        }
    }
}
