namespace RUYIinit
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Net;
    using System.Windows.Forms;

    public class HttpDownForm : Form
    {
        private const long ProgressUpdateInterval = 0x3e8L;
        private long _UpdateTimestamp;
        private string[] args;
        private IContainer components = null;
        private TableLayoutPanel tableLayoutPanel1;
        private ProgressBar _ProgressBar;
        private Label _ProgressLabel;

        public HttpDownForm(string[] args)
        {
            this.args = args;
            this.Text = args[2];
            int x = (Screen.PrimaryScreen.WorkingArea.Size.Width / 2) - 300;
            base.SetDesktopLocation(x, Screen.PrimaryScreen.WorkingArea.Size.Height - 50);
            this.InitializeComponent();
            this.UpdateProgress(0.0);
            this._ProgressBar.Enabled = true;
            this._UpdateTimestamp = this.GetCurrentTimestamp();
            WebClient client = new WebClient();
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(this.OnDownloadProgressChanged);
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(this.OnDownloadFileCompleted);
            client.DownloadFileAsync(new Uri(args[0]), args[1]);
        }

        private double CalculatePercentage(long part, long total) => 
            (((double) part) / ((double) total)) * 100.0;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private long GetCurrentTimestamp() => 
            DateTime.Now.Ticks / 0x2710L;

        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new TableLayoutPanel();
            this._ProgressBar = new ProgressBar();
            this._ProgressLabel = new Label();
            this.tableLayoutPanel1.SuspendLayout();
            base.SuspendLayout();
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            this.tableLayoutPanel1.Controls.Add(this._ProgressBar, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this._ProgressLabel, 0, 0);
            this.tableLayoutPanel1.Dock = DockStyle.Fill;
            this.tableLayoutPanel1.Location = new Point(0, 0);
            this.tableLayoutPanel1.Margin = new Padding(2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new Padding(3);
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 33f));
            this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 33f));
            this.tableLayoutPanel1.Size = new Size(0x256, 0x26);
            this.tableLayoutPanel1.TabIndex = 0;
            this.tableLayoutPanel1.SetColumnSpan(this._ProgressBar, 3);
            this._ProgressBar.Dock = DockStyle.Fill;
            this._ProgressBar.Enabled = false;
            this._ProgressBar.Location = new Point(0x99, 8);
            this._ProgressBar.Margin = new Padding(2, 5, 2, 5);
            this._ProgressBar.Name = "_ProgressBar";
            this._ProgressBar.Size = new Size(440, 0x16);
            this._ProgressBar.Step = 1;
            this._ProgressBar.Style = ProgressBarStyle.Continuous;
            this._ProgressBar.TabIndex = 5;
            this._ProgressLabel.AutoSize = true;
            this._ProgressLabel.Dock = DockStyle.Fill;
            this._ProgressLabel.Location = new Point(6, 6);
            this._ProgressLabel.Margin = new Padding(3);
            this._ProgressLabel.Name = "_ProgressLabel";
            this._ProgressLabel.Size = new Size(0x8e, 0x1a);
            this._ProgressLabel.TabIndex = 7;
            this._ProgressLabel.Text = "Progress";
            this._ProgressLabel.TextAlign = ContentAlignment.MiddleLeft;
            base.AutoScaleDimensions = new SizeF(6f, 12f);
            base.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = SystemColors.MenuHighlight;
            base.ClientSize = new Size(0x256, 0x26);
            base.ControlBox = false;
            base.Controls.Add(this.tableLayoutPanel1);
            this.ForeColor = SystemColors.ControlLightLight;
            base.FormBorderStyle = FormBorderStyle.None;
            base.IsMdiContainer = true;
            base.Margin = new Padding(2);
            base.Name = "MainForm";
            base.StartPosition = FormStartPosition.Manual;
            base.TopMost = true;
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            base.ResumeLayout(false);
        }

        private void OnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.UpdateProgress(100.0);
            this._ProgressBar.Enabled = false;
        }

        private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            long currentTimestamp = this.GetCurrentTimestamp();
            if ((currentTimestamp - this._UpdateTimestamp) >= 0x3e8L)
            {
                double progress = this.CalculatePercentage(e.BytesReceived, e.TotalBytesToReceive);
                this.UpdateProgress(progress);
                this._UpdateTimestamp = currentTimestamp;
            }
        }

        private void UpdateProgress(double progress)
        {
            if (!(progress == 100.0))
            {
                this._ProgressLabel.Text = "Progress\n" + progress.ToString("0.0") + "%";
            }
            else
            {
                this._ProgressLabel.Text = "Progress\nCompleted";
                base.Close();
            }
            this._ProgressBar.Value = (int) progress;
            this._ProgressLabel.Update();
            this._ProgressBar.Update();
        }
    }
}

