using System;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;

namespace XpsToPdf
{
    public partial class ConvertWindow : Window
    {
        public ConvertWindow()
        {
            InitializeComponent();
        }

        private void BrowseXps_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "XPS documents (*.xps)|*.xps|All files (*.*)|*.*",
                Title = "Select XPS file"
            };
            if (dlg.ShowDialog(this) == true)
            {
                TxtSource.Text = dlg.FileName;
                if (string.IsNullOrWhiteSpace(TxtOutput.Text))
                {
                    TxtOutput.Text = System.IO.Path.ChangeExtension(dlg.FileName, ".pdf");
                }
            }
        }

        private void BrowsePdf_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*",
                DefaultExt = "pdf",
                Title = "Save PDF as"
            };
            if (dlg.ShowDialog(this) == true)
            {
                TxtOutput.Text = dlg.FileName;
            }
        }

        private async void Convert_Click(object sender, RoutedEventArgs e)
        {
            string source = TxtSource.Text;
            string dest = TxtOutput.Text;

            if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(dest))
            {
                MessageBox.Show(this, "Please select both source XPS and output PDF paths.", "Missing paths", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            BtnConvert.IsEnabled = false;
            BtnBrowseXps.IsEnabled = false;
            BtnBrowsePdf.IsEnabled = false;
            StatusText.Text = "Converting...";

            try
            {
                await Task.Run(() =>
                {
                    PdfSharp.Xps.XpsConverter.Convert(source, dest, 0);
                });

                StatusText.Text = $"Conversion completed: {dest}";
                MessageBox.Show(this, "Conversion finished successfully.", "Done", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Error: {ex.Message}";
                MessageBox.Show(this, $"Conversion failed:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                BtnConvert.IsEnabled = true;
                BtnBrowseXps.IsEnabled = true;
                BtnBrowsePdf.IsEnabled = true;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnConvertFolder_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new ConvertFolderWindow { Owner = this };
            dlg.ShowDialog();
        }
    }
}