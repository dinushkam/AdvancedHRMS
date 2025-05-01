using AdvancedHRMS.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace AdvancedHRMS.Views
{
    public partial class EmployeeDocumentsView : UserControl
    {
        public List<EmployeeDocument> Documents { get; set; }
        private string _statusMessage;

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
              
            }
        }

        public EmployeeDocumentsView()
        {
            InitializeComponent();
            LoadSampleDocuments(); 
            DocumentsGrid.ItemsSource = Documents;
        }

        private void LoadSampleDocuments()
        {
            Documents = new List<EmployeeDocument>
            {
                new EmployeeDocument { Name = "Employment Contract", Type = "Contract", UploadDate = DateTime.Now.AddDays(-30), Size = "2.5 MB" },
                new EmployeeDocument { Name = "NDA Agreement", Type = "Legal", UploadDate = DateTime.Now.AddDays(-15), Size = "1.1 MB" }
            };
        }

        private void Upload_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "All files (*.*)|*.*|PDF files (*.pdf)|*.pdf|Word documents (*.docx)|*.docx",
                Title = "Select a document to upload"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {

                    var fileInfo = new FileInfo(openFileDialog.FileName);
                    Documents.Add(new EmployeeDocument
                    {
                        Name = openFileDialog.SafeFileName,
                        Type = Path.GetExtension(openFileDialog.FileName).TrimStart('.'),
                        UploadDate = DateTime.Now,
                        Size = (fileInfo.Length / 1024) + " KB"
                    });

                    DocumentsGrid.Items.Refresh();
                    StatusMessage = $"{openFileDialog.SafeFileName} uploaded successfully!";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error uploading file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Download_Click(object sender, RoutedEventArgs e)
        {
            if (DocumentsGrid.SelectedItem is EmployeeDocument selectedDoc)
            {
                var saveFileDialog = new SaveFileDialog
                {
                    FileName = selectedDoc.Name,
                    Filter = "All files (*.*)|*.*"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                   
                    StatusMessage = $"Downloading {selectedDoc.Name}...";
                    MessageBox.Show($"In a real app, this would download {selectedDoc.Name}", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Please select a document first", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void View_Click(object sender, RoutedEventArgs e)
        {
            if (DocumentsGrid.SelectedItem is EmployeeDocument selectedDoc)
            {
                MessageBox.Show($"Would open {selectedDoc.Name} in viewer", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

    public class EmployeeDocument
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public DateTime UploadDate { get; set; }
        public string Size { get; set; }
    }
}