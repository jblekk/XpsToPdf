using System;
using System.Drawing;
using System.Windows.Forms;

namespace XpsToPdfLauncher
{
    public class MainForm : Form
    {
        private Button btnPlay;

        public MainForm()
        {
            Text = "XpsToPdf Launcher";
            Width = 300;
            Height = 140;
            StartPosition = FormStartPosition.CenterScreen;

            btnPlay = new Button
            {
                Text = "Single File",
                Width = 100,
                Height = 30,
                Location = new Point((ClientSize.Width - 100) / 2, 20),
                Anchor = AnchorStyles.Top
            };
            btnPlay.Click += BtnPlay_Click;
            Controls.Add(btnPlay);

            var lbl = new Label
            {
                Text = "Opens the XPS → PDF converter window",
                AutoSize = true,
                Location = new Point(12, 70)
            };
            Controls.Add(lbl);
        }

        private void BtnPlay_Click(object sender, EventArgs e)
        {
            try
            {
                // Ensure WPF assemblies are referenced by the WinForms project:
                // Add references to PresentationCore, PresentationFramework, WindowsBase, and System.Xaml (if needed).
                // Show the WPF ConvertWindow from your XpsToPdf namespace.
                var wpfWindow = new XpsToPdf.ConvertWindow();

                // If you want the WinForms window to be owner of the WPF window:
                var interopHelper = new System.Windows.Interop.WindowInteropHelper(wpfWindow)
                {
                    Owner = this.Handle
                };

                // Ensure a WPF Application exists (only create if null)
                if (System.Windows.Application.Current == null)
                {
                    new System.Windows.Application();
                }

                // Show as dialog so user must close it before returning
                wpfWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Failed to open converter: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(498, 398);
            this.Name = "MainForm";
            this.ResumeLayout(false);

        }
    }
}