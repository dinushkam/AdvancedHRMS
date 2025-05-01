using AdvancedHRMS.Data;
using AdvancedHRMS.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    _adminUser = context.Users.FirstOrDefault(u => u.Id == AuthService.CurrentUser.Id);

                    if (_adminUser == null)
                    {
                        MessageBox.Show("Admin details not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        Close();
                        return;
                    }

                    // Set control values on the UI thread
                    Dispatcher.Invoke(() =>
                    {
                        txtFullName.Text = _adminUser.FullName ?? _adminUser.Username;
                        txtUsername.Text = _adminUser.Username;
                        txtEmail.Text = _adminUser.Email;
                        txtPhone.Text = _adminUser.Phone ?? string.Empty;
                        txtAddress.Text = _adminUser.Address ?? string.Empty;
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading profile: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs())
                return;

            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var admin = context.Users.FirstOrDefault(u => u.Id == _adminUser.Id);
                    if (admin == null)
                    {
                        MessageBox.Show("User record not found in database!", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Update fields
                    admin.FullName = txtFullName.Text.Trim();
                    admin.Username = txtUsername.Text.Trim();
                    admin.Email = txtEmail.Text.Trim();
                    admin.Phone = txtPhone.Text.Trim();
                    admin.Address = txtAddress.Text.Trim();

                    context.SaveChanges();

                    MessageBox.Show("Profile updated successfully!", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving profile: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Full name is required", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtFullName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Username is required", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtUsername.Focus();
                return false;
            }

            // Add any other validations you need (phone format, etc.)

            return true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}