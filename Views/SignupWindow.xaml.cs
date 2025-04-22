using AdvancedHRMS.Data;
using AdvancedHRMS.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace AdvancedHRMS.Views
{
    public partial class SignupWindow : Window
    {
        public SignupWindow()
        {
            InitializeComponent();
        }

        private void BtnSignup_Click(object sender, RoutedEventArgs e)
        {
            string fullName = txtFullName.Text;
            string username = txtUsername.Text;
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string address = txtAddress.Text;
            string password = txtPassword.Password;
            string confirmPassword = txtConfirmPassword.Password;
            string role = (cmbRole.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(address) || string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show("All fields are required!");
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match!");
                return;
            }
            if (!IsValidEmail(email))
            {
                MessageBox.Show("Please enter a valid email address (e.g., user@example.com)");
                return;
            }

            if (!IsValidPhone(phone))
            {
                MessageBox.Show("Please enter a valid phone number (10-15 digits, may include + - ( ) or spaces)");
                return;
            }

            using (var context = new ApplicationDbContext())
            {
                if (context.Users.Any(u => u.Username == username))
                {
                    MessageBox.Show("Username already exists!");
                    return;
                }

                if (context.Users.Any(u => u.Email == email))
                {
                    MessageBox.Show("Email already exists!");
                    return;
                }
                

        string hashedPassword = HashPassword(password);
                var newUser = new User
                {
                    FullName = fullName,
                    Username = username,
                    Email = email,
                    Phone = phone,
                    Address = address,
                    PasswordHash = hashedPassword,
                    Role = role,
                    IsApproved = false  // Requires admin approval
                };

                context.Users.Add(newUser);
                context.SaveChanges();

                MessageBox.Show("Signup successful! Please wait for admin approval.");
                this.Close();
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
        private bool IsValidEmail(string email)
        {
            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhone(string phone)
        {
            // Basic phone number validation
            return Regex.IsMatch(phone, @"^[0-9\-\+\(\)\s]{10,20}$");
        }

    }
}
