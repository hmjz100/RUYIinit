namespace RUYIinit
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Security.Cryptography;
    using System.Threading;
    using System.Windows.Forms;

    public class Form2 : Form
    {
        private IContainer components = null;
        private TextBox textBox1;
        private Button button1;
        private BackgroundWorker backgroundWorker1;
        private Label label1;

        public Form2()
        {
            this.InitializeComponent();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            string driver = new HWCheck().GetDriver();
            string str2 = @"D:\Work\OSBuild\images\";
            string userState = null;
            string text = null;
            int p = 0;
            List<int> list = new List<int>();
            List<int> list2 = new List<int>();
            RootObject obj2 = JsonConvert.DeserializeObject<RootObject>(File.ReadAllText(@".\AOD.json"));
            int item = 0;
            while (true)
            {
                if (item >= obj2.MODULES.MODULE.Count)
                {
                    int num3 = 0;
                    while (true)
                    {
                        if (num3 >= list.Count)
                        {
                            if (list2.Count != 0)
                            {
                                int num5 = 0;
                                while (true)
                                {
                                    if (num5 >= list2.Count)
                                    {
                                        break;
                                    }
                                    p = this.SetProgress(p, 5);
                                    int num6 = list2[num5];
                                    string str8 = obj2.MODULES.MODULE[num6].ID;
                                    string str9 = obj2.MODULES.MODULE[num6].MD5;
                                    string filename = Path.Combine(str2, str8);
                                    object[] objArray2 = new object[] { "Downloading Module MD5:", str8, "(", num5 + 1, "/", list2.Count, ")\r\n" };
                                    userState = string.Concat(objArray2);
                                    worker.ReportProgress(p, userState);
                                    string str11 = Path.Combine("http://ruyios.playruyi.com/RecoveryImage/sources/", str8);
                                    string[] args = new string[] { str11, filename, userState };
                                    Application.Run(new HttpDownForm(args));
                                    if (!this.CheckModuleMD5(filename, str9))
                                    {
                                        text = text + "Download Module Fail:" + str8 + "\r\n";
                                    }
                                    num5++;
                                }
                            }
                            if (text != null)
                            {
                                MessageBox.Show(text, "Error103", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                Environment.Exit(0);
                            }
                            return;
                        }
                        p = this.SetProgress(p, 2);
                        int num4 = list[num3];
                        string iD = obj2.MODULES.MODULE[num4].ID;
                        string str6 = obj2.MODULES.MODULE[num4].MD5;
                        string path = Path.Combine(str2, iD);
                        if (!File.Exists(path))
                        {
                            if (!File.Exists(path) && obj2.Token.HttpDown)
                            {
                                list2.Add(num4);
                            }
                            else
                            {
                                text = text + "Missing Module:" + iD + "\r\n";
                            }
                        }
                        else if (obj2.Token.MD5Check)
                        {
                            object[] objArray1 = new object[] { "Checking Module MD5:", iD, "(", num3 + 1, "/", list.Count, ")\r\n" };
                            worker.ReportProgress(p, string.Concat(objArray1));
                            bool flag5 = this.CheckModuleMD5(path, str6);
                            if (!flag5)
                            {
                                if (!flag5 && obj2.Token.HttpDown)
                                {
                                    list2.Add(num4);
                                }
                                else
                                {
                                    text = text + "Check Module MD5 Fail:" + iD + "\r\n";
                                }
                            }
                        }
                        num3++;
                    }
                }
                bool choosed = obj2.MODULES.MODULE[item].Choosed;
                if (choosed)
                {
                    list.Add(item);
                }
                item++;
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.textBox1.Text = e.UserState.ToString();
            this.label1.Text = e.ProgressPercentage.ToString() + "%";
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                this.textBox1.Text = this.textBox1.Text + "Canceled!";
            }
            else if (e.Error != null)
            {
                this.textBox1.Text = this.textBox1.Text + "Error: " + e.Error.Message;
            }
            else
            {
                this.textBox1.Text = this.textBox1.Text + "Done!";
                base.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.backgroundWorker1.WorkerSupportsCancellation)
            {
                this.backgroundWorker1.CancelAsync();
            }
        }

        public bool CheckModuleMD5(string filename, string MD5hash)
        {
            bool flag2;
            using (MD5 md = MD5.Create())
            {
                using (FileStream stream = File.OpenRead(filename))
                {
                    string str = BitConverter.ToString(md.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
                    Console.WriteLine("RecordMD5:" + MD5hash.ToLower());
                    Console.WriteLine("ComputMD5:" + str.ToLower());
                    flag2 = str.ToLower() == MD5hash.ToLower();
                }
            }
            return flag2;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            if (!this.backgroundWorker1.IsBusy)
            {
                this.backgroundWorker1.RunWorkerAsync();
            }
        }

        public bool HttpDownModule(string filename, string destpath, string MD5hash)
        {
            Thread.Sleep(0x1388);
            return false;
        }

        private void InitializeComponent()
        {
            this.textBox1 = new TextBox();
            this.button1 = new Button();
            this.backgroundWorker1 = new BackgroundWorker();
            this.label1 = new Label();
            base.SuspendLayout();
            this.textBox1.Anchor = AnchorStyles.None;
            this.textBox1.Location = new Point(0x1d8, 0x1f5);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = ScrollBars.Both;
            this.textBox1.Size = new Size(0x1fa, 0x40);
            this.textBox1.TabIndex = 0;
            this.button1.Anchor = AnchorStyles.None;
            this.button1.Location = new Point(0x4cf, 0x43);
            this.button1.Name = "button1";
            this.button1.Size = new Size(0x92, 0x3d);
            this.button1.TabIndex = 1;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new EventHandler(this.button1_Click);
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            this.label1.Anchor = AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Location = new Point(0x2c5, 0x1a3);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0x20, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "1%";
            base.AutoScaleDimensions = new SizeF(9f, 20f);
            base.AutoScaleMode = AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = SystemColors.MenuHighlight;
            base.ClientSize = new Size(0x5aa, 0x2b3);
            base.Controls.Add(this.label1);
            base.Controls.Add(this.button1);
            base.Controls.Add(this.textBox1);
            base.FormBorderStyle = FormBorderStyle.None;
            base.Name = "Form2";
            base.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Form2";
            base.WindowState = FormWindowState.Maximized;
            base.Load += new EventHandler(this.Form2_Load);
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        public int SetProgress(int p, int max)
        {
            int num;
            if (p >= max)
            {
                num = max;
            }
            else
            {
                p++;
                num = p;
            }
            return num;
        }

        public void UnzipModule(string ModuleName, string SourcePath, string TargetPath)
        {
            string str = "Ruyi@2018!@#";
            ProcessStartInfo startInfo = new ProcessStartInfo {
                FileName = @".\7z.exe"
            };
            string[] textArray1 = new string[] { "x -p", str, " -y ", SourcePath, @"\", ModuleName, " -o", TargetPath };
            startInfo.Arguments = string.Concat(textArray1);
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            Console.WriteLine(startInfo.Arguments);
            Process.Start(startInfo).WaitForExit();
            Console.WriteLine("Success");
        }
    }
}

