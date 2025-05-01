using AdvancedHRMS.Data;
using AdvancedHRMS.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace AdvancedHRMS.Views
{
    public partial class LoginWindow : Window
    {
        private readonly ApplicationDbContext _context;
        public bool ShowAdminLogin { get; set; } = false; 

        public LoginWindow()
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            DataContext = this;
            txtUsername.Focus();

        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowError("Username and password are required!");
                return;
            }

            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Username == username);

                if (user == null || !VerifyPassword(password, user.PasswordHash))
                {
                    ShowError("Invalid username or password!");
                    return;
                }

                if (!user.IsApproved)
                {
                    ShowError("Your account is pending approval. Please contact administrator.");
                    return;
                }

              
                AuthService.CurrentUser = user;


                AuthService.CurrentUserEmail = user.Email;



              
                Window dashboard = null;

                if (user.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                {
                    dashboard = new AdminDashboard();
                }
                else if (user.Role.Equals("Manager", StringComparison.OrdinalIgnoreCase))
                {
                    dashboard = new HRDashboard();
                }
                else if (user.Role.Equals("Employee", StringComparison.OrdinalIgnoreCase))
                {
                    dashboard = new EmployeeDashboard();
                }
                else
                {
                    ShowError("Invalid user role");
                    return;
                }

                dashboard.Title = $"Dashboard: {user.Username}"; 
                dashboard.Show();

                
                this.Close();
            }
            catch (Exception ex)
            {
                ShowError($"Login failed: {ex.Message}");
            }
        }


        private void BtnSignup_Click(object sender, RoutedEventArgs e)
        {
            var signupWindow = new SignupWindow();
            signupWindow.Owner = this;
            this.Hide(); 
            signupWindow.ShowDialog();
            this.Show(); 
        }


        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            var resetWindow = new PasswordResetWindow();
            resetWindow.Owner = this;
            this.Hide();
            resetWindow.ShowDialog(); ;
            this.Show();
        }

       

        private bool VerifyPassword(string enteredPassword, string storedHash)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] enteredBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(enteredPassword));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in enteredBytes)
                    builder.Append(b.ToString("x2"));

                return builder.ToString() == storedHash;
            }
        }

        private void ShowError(string message)
        {
            txtError.Text = message;
            txtError.Visibility = Visibility.Visible;
        }

        protected override void OnClosed(EventArgs e)
        {
            _context.Dispose();
            base.OnClosed(e);
        }

        private void txtUsername_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
} 