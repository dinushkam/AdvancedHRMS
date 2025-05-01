using AdvancedHRMS.Data;
using AdvancedHRMS.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace AdvancedHRMS.Views
{
    public partial class PasswordResetWindow : Window
    {
        public PasswordResetWindow()
        {
            InitializeComponent();
            txtEmail.Focus();
        }

        private void BtnResetPassword_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string newPassword = txtNewPassword.Password;
            string confirmPassword = txtConfirmPassword.Password;

            // Input validation
            if (string.IsNullOrEmpty(email) || !email.Contains("@"))
            {
                ShowMessage("Please enter a valid email address.", false);
                return;
            }

            if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 8)
            {
                ShowMessage("Password must be at least 8 characters long.", false);
                return;
            }

            if (newPassword != confirmPassword)
            {
                ShowMessage("Passwords do not match.", false);
                return;
            }

            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var user = context.Users.FirstOrDefault(u => u.Email == email);

                    if (user == null)
                    {
                        ShowMessage("No account found with this email address.", false);
                        return;
                    }

                    // Hash the new password
                    string hashedPassword = HashPassword(newPassword);

                    // Update password
                    user.PasswordHash = hashedPassword;
                    context.SaveChanges();

                    ShowMessage("Password reset successfully! You can now login with your new password.", true);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error resetting password: {ex.Message}", false);
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }

        private void ShowMessage(string message, bool isSuccess)
        {
            txtMessage.Text = message;
            txtMessage.Foreground = isSuccess ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Red;
            txtMessage.Visibility = Visibility.Visible;
        }

        private void BtnBackToLogin_Click(object sender, RoutedEventArgs e)
        {
            if (this.Owner != null)
            {
                this.Owner.Show();  // ✅ Re-show original login window
            }
            this.Close();
        }
    }
}