using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace XpsToPdf
{
    public partial class ConvertFolderWindow : Window
    {
        public ConvertFolderWindow()
        {
            InitializeComponent();
        }

        private void BtnBrowseSource_Click(object sender, RoutedEventArgs e)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    TxtSourceFolder.Text = dlg.SelectedPath;
            }
        }

        private void BtnBrowseOutput_Click(object sender, RoutedEventArgs e)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    TxtOutputFolder.Text = dlg.SelectedPath;
            }
        }

        private async void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            string src = TxtSourceFolder.Text.Trim();
            string outDir = TxtOutputFolder.Text.Trim();

            if (string.IsNullOrEmpty(src) || string.IsNullOrEmpty(outDir) || !Directory.Exists(src))
            {
                MessageBox.Show(this, "Please select existing source and output folders.", "Missing paths", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            BtnStart.IsEnabled = false;
            BtnBrowseSource.IsEnabled = false;
            BtnBrowseOutput.IsEnabled = false;
            StatusText.Text = "Scanning folder...";

            var files = Directory.GetFiles(src, "*.xps", SearchOption.TopDirectoryOnly)
                                 .Concat(Directory.GetFiles(src, "*.oxps", SearchOption.TopDirectoryOnly))
                                 .ToArray();

            if (files.Length == 0)
            {
                StatusText.Text = "No XPS/OXPS files found.";
                BtnStart.IsEnabled = true;
                BtnBrowseSource.IsEnabled = true;
                BtnBrowseOutput.IsEnabled = true;
                return;
            }

            int processed = 0;
            int failed = 0;

            await Task.Run(() =>
            {
                foreach (var xpsFile in files)
                {
                    try
                    {
                        string targetName = Path.GetFileNameWithoutExtension(xpsFile) + ".pdf";
                        string pdfPath = Path.Combine(outDir, targetName);

                        // Convert XPS to PDF (overwrite existing PDFs)
                        PdfSharp.Xps.XpsConverter.Convert(xpsFile, pdfPath, 0, false);

                        processed++;
                        Dispatcher.Invoke(() => StatusText.Text = $"Processed {processed}/{files.Length}: {Path.GetFileName(xpsFile)}");
                    }
                    catch (Exception ex)
                    {
                        failed++;
                        Dispatcher.Invoke(() => StatusText.Text = $"Error on {Path.GetFileName(xpsFile)}: {ex.Message}");
                    }
                }
            });

            StatusText.Text = $"Done. Processed: {processed}, Failed: {failed}.";
            BtnStart.IsEnabled = true;
            BtnBrowseSource.IsEnabled = true;
            BtnBrowseOutput.IsEnabled = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
