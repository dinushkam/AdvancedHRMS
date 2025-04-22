using System.Windows;

namespace AdvancedHRMS.Views
{
    public partial class RejectReasonDialog : Window
    {
        public string Reason { get; private set; }

        public RejectReasonDialog()
        {
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            Reason = txtReason.Text;
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}