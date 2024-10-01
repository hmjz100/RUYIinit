namespace RUYIinit
{
    using Newtonsoft.Json;
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;

    public class Form1 : Form
    {
        public string SourcePath;
        private HWCheck HC = new HWCheck();
        private IContainer components = null;
        private Label label1;
        private Label label8;
        private Button buttonShutdown;
        private Label label7;
        private Button buttonStart;
        private Label label6;
        private Label label5;
        private Label labelHDD2;
        private Label labelHDD1;
        private CheckBox checkBoxHDD2;
        private CheckBox checkBoxHDD1;
        private Label label2;
        private Button buttonNetRetry;
        private Panel panel1;
        private ComboBox comboBox1;
        private Label label9;
        private Label label17;
        private Label label16;
        private ComboBox comboBox2;
        private Label label15;
        private Label labelNetStatus;
        private Label label1NetStatus;
        private Label labelOSVersion;
        private Label label1OSVersion;
        private Label label10;
        private Label label18;
        private Button buttonCheck;
        private Label label4;
        private Label label3;

        public Form1()
        {
            this.InitializeComponent();
        }

        public void BindSource(RootObject root)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Text", System.Type.GetType("System.String"));
            table.Columns.Add("Value", System.Type.GetType("System.String"));
            int num = 0;
            while (true)
            {
                if (num >= root.TOKENS.LANGUAGE.Count)
                {
                    this.comboBox1.DataSource = table;
                    this.comboBox1.DisplayMember = "Text";
                    this.comboBox1.ValueMember = "Value";
                    DataTable table2 = new DataTable();
                    table2.Columns.Add("Text", System.Type.GetType("System.String"));
                    table2.Columns.Add("Value", System.Type.GetType("System.String"));
                    int num2 = 0;
                    while (true)
                    {
                        if (num2 >= root.TOKENS.OSMODE.Count)
                        {
                            this.comboBox2.DataSource = table2;
                            this.comboBox2.DisplayMember = "Text";
                            this.comboBox2.ValueMember = "Value";
                            return;
                        }
                        object[] objArray2 = new object[] { root.TOKENS.OSMODE[num2].Description, root.TOKENS.OSMODE[num2].BP };
                        table2.Rows.Add(objArray2);
                        num2++;
                    }
                }
                object[] values = new object[] { root.TOKENS.LANGUAGE[num].Description, root.TOKENS.LANGUAGE[num].BP };
                table.Rows.Add(values);
                num++;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.labelNetStatus.Text = !this.HC.IsConnectInternet() ? "NO" : "OK";
        }

        private void buttonCheck_Click(object sender, EventArgs e)
        {
            RootObject obj2 = JsonConvert.DeserializeObject<RootObject>(File.ReadAllText(Application.StartupPath + @"\RUYIOS.json"));
            string tokenLP = this.comboBox1.SelectedValue.ToString();
            string tokenOSMODE = this.comboBox2.SelectedValue.ToString();
            bool flag = this.checkBoxHDD2.Checked;
            string text = "" + "RUYIOSVersion:" + obj2.VERSION.RUYIOSVersion + "\r\n";
            string[] textArray1 = new string[] { text, this.comboBox1.Text, ":", tokenLP, "\r\n" };
            text = string.Concat(textArray1);
            string[] textArray2 = new string[] { text, this.comboBox2.Text, ":", tokenOSMODE, "\r\n" };
            text = string.Concat(textArray2) + "CheckBox2.Checked:" + flag.ToString() + "\r\n";
            ParseCondition condition = new ParseCondition();
            int num = 0;
            while (true)
            {
                if (num >= obj2.MODULES.MODULE.Count)
                {
                    obj2.Token.LANGUAGE = tokenLP;
                    obj2.Token.OSMODE = tokenOSMODE;
                    obj2.Token.SecondHDDClean = flag;
                    MessageBox.Show(text);
                    return;
                }
                string str4 = obj2.MODULES.MODULE[num].Condition;
                if (!condition.Parse(str4, tokenLP, tokenOSMODE))
                {
                    obj2.MODULES.MODULE[num].Choosed = false;
                }
                else
                {
                    obj2.MODULES.MODULE[num].Choosed = true;
                    object[] objArray1 = new object[] { text, "Module", num, obj2.MODULES.MODULE[num].ID, "\r\n" };
                    text = string.Concat(objArray1);
                }
                num++;
            }
        }

        private void buttonShutdown_Click(object sender, EventArgs e)
        {
            Process process = new Process {
                StartInfo = { 
                    UseShellExecute = false,
                    FileName = "wpeutil.exe",
                    Arguments = "shutdown"
                }
            };
            process.Start();
            process.WaitForExit();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            this.buttonStart.Enabled = false;
            RootObject obj2 = JsonConvert.DeserializeObject<RootObject>(File.ReadAllText(Application.StartupPath + @"\RUYIOS.json"));
            string tokenLP = this.comboBox1.SelectedValue.ToString();
            string tokenOSMODE = this.comboBox2.SelectedValue.ToString();
            bool flag = this.checkBoxHDD2.Checked;
            string fileName = null;
            ParseCondition condition = new ParseCondition();
            int num = 0;
            while (true)
            {
                if (num >= obj2.MODULES.MODULE.Count)
                {
                    obj2.Token.LANGUAGE = tokenLP;
                    obj2.Token.OSMODE = tokenOSMODE;
                    obj2.Token.SecondHDDClean = flag;
                    obj2.Token.SourcePath = this.SourcePath;
                    File.WriteAllText(Application.StartupPath + @"\AOD.json", JsonConvert.SerializeObject(obj2, Formatting.Indented));
                    if (fileName != null)
                    {
                        bool flag7 = this.HC.CopyFile(fileName, this.SourcePath, Application.StartupPath);
                        this.UnzipModule(fileName, Application.StartupPath, Application.StartupPath);
                    }
                    else
                    {
                        MessageBox.Show("没有SetEnv文件\r\nThere is no SetEnv file", "Error104", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        base.Close();
                    }
                    base.TopMost = false;
                    base.WindowState = FormWindowState.Minimized;
                    string filename = "powershell.exe";
                    this.StartExe(filename, "-executionpolicy bypass -WindowStyle Minimized -file " + Application.StartupPath + @"\auditpe.ps1 ");
                    base.Close();
                    return;
                }
                string str6 = obj2.MODULES.MODULE[num].Condition;
                bool flag2 = condition.Parse(str6, tokenLP, tokenOSMODE);
                if (obj2.MODULES.MODULE[num].Name.Equals("SetEnv"))
                {
                    fileName = obj2.MODULES.MODULE[num].ID;
                }
                obj2.MODULES.MODULE[num].Choosed = flag2;
                num++;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string filename = "wpeinit.exe";
            this.StartExe(filename, "");
            this.buttonCheck.Hide();
            this.buttonNetRetry.Hide();
            this.labelNetStatus.Text = !this.HC.IsConnectInternet() ? "NO" : "OK";
            this.HC.GetDiskInfo();
            if (ReferenceEquals(this.HC.HDD1, null))
            {
                MessageBox.Show("没有检测到主硬盘\r\nThere is HDD1", "Error101", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                base.Close();
            }
            this.labelHDD1.Text = this.HC.HDD1;
            this.labelHDD2.Text = this.HC.HDD2;
            this.checkBoxHDD1.Checked = true;
            this.checkBoxHDD2.Checked = false;
            string driver = this.HC.GetDriver();
            if (Directory.Exists(@"i:\MFGimages\"))
            {
                this.SourcePath = @"i:\MFGimages\";
            }
            else if (!Directory.Exists(@"i:\MFGimages\") && (driver != null))
            {
                this.SourcePath = driver + @"sources\";
            }
            else
            {
                MessageBox.Show("没有系统安装盘\r\nThere is no device with RUYIOS Recovery System", "Error102", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                base.Close();
            }
            if (!this.HC.CopyFile("RUYIOS.json", this.SourcePath, Application.StartupPath))
            {
                MessageBox.Show("没有系统安装配置文件\r\nThere is no config file of RUYIOS", "Error103", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                base.Close();
            }
            RootObject root = JsonConvert.DeserializeObject<RootObject>(File.ReadAllText(Application.StartupPath + @"\RUYIOS.json"));
            this.labelOSVersion.Text = root.VERSION.RUYIOSVersion;
            if (!root.Token.Choosed)
            {
                this.BindSource(root);
            }
            else
            {
                Console.WriteLine("Token:" + root.Token.Choosed.ToString());
                string lANGUAGE = root.Token.LANGUAGE;
                string oSMODE = root.Token.OSMODE;
                ParseCondition condition = new ParseCondition();
                int num = 0;
                while (true)
                {
                    if (num >= root.MODULES.MODULE.Count)
                    {
                        File.WriteAllText(Application.StartupPath + @"\AOD.json", JsonConvert.SerializeObject(root, Formatting.Indented));
                        base.Close();
                        break;
                    }
                    string str6 = root.MODULES.MODULE[num].Condition;
                    bool flag10 = condition.Parse(str6, lANGUAGE, oSMODE);
                    root.MODULES.MODULE[num].Choosed = flag10;
                    num++;
                }
            }
        }

        private void InitializeComponent()
        {
            this.buttonNetRetry = new Button();
            this.label1 = new Label();
            this.label8 = new Label();
            this.buttonShutdown = new Button();
            this.label7 = new Label();
            this.buttonStart = new Button();
            this.label6 = new Label();
            this.label5 = new Label();
            this.labelHDD2 = new Label();
            this.labelHDD1 = new Label();
            this.checkBoxHDD2 = new CheckBox();
            this.checkBoxHDD1 = new CheckBox();
            this.label2 = new Label();
            this.panel1 = new Panel();
            this.buttonCheck = new Button();
            this.label18 = new Label();
            this.label17 = new Label();
            this.label16 = new Label();
            this.comboBox2 = new ComboBox();
            this.label15 = new Label();
            this.labelNetStatus = new Label();
            this.label1NetStatus = new Label();
            this.labelOSVersion = new Label();
            this.label1OSVersion = new Label();
            this.label10 = new Label();
            this.comboBox1 = new ComboBox();
            this.label9 = new Label();
            this.label3 = new Label();
            this.label4 = new Label();
            this.panel1.SuspendLayout();
            base.SuspendLayout();
            this.buttonNetRetry.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.buttonNetRetry.FlatStyle = FlatStyle.System;
            this.buttonNetRetry.Location = new Point(0x4ca, 0xa2);
            this.buttonNetRetry.Name = "buttonNetRetry";
            this.buttonNetRetry.Size = new Size(30, 30);
            this.buttonNetRetry.TabIndex = 15;
            this.buttonNetRetry.Text = "R";
            this.buttonNetRetry.UseVisualStyleBackColor = false;
            this.buttonNetRetry.Click += new EventHandler(this.button1_Click);
            this.label1.AutoSize = true;
            this.label1.Font = new Font("Arial Narrow", 15f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.label1.Location = new Point(0x1c7, 140);
            this.label1.Margin = new Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0x18f, 0x23);
            this.label1.TabIndex = 13;
            this.label1.Text = "Welcome to OS Recovery Program";
            this.label1.TextAlign = ContentAlignment.MiddleCenter;
            this.label1.Click += new EventHandler(this.label1_Click);
            this.label8.AutoSize = true;
            this.label8.Font = new Font("Arial Narrow", 10f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.label8.Location = new Point(0xc9, 620);
            this.label8.Margin = new Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new Size(60, 0x18);
            this.label8.TabIndex = 12;
            this.label8.Text = "HDD2:";
            this.buttonShutdown.Font = new Font("微软雅黑", 12f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.buttonShutdown.Location = new Point(0x2cf, 0x2f4);
            this.buttonShutdown.Margin = new Padding(4, 5, 4, 5);
            this.buttonShutdown.Name = "buttonShutdown";
            this.buttonShutdown.Size = new Size(0xe1, 80);
            this.buttonShutdown.TabIndex = 3;
            this.buttonShutdown.Text = "关机\r\nShutdown(&D)";
            this.buttonShutdown.UseVisualStyleBackColor = true;
            this.buttonShutdown.Click += new EventHandler(this.buttonShutdown_Click);
            this.label7.AutoSize = true;
            this.label7.Font = new Font("Arial Narrow", 10f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.label7.Location = new Point(0xc9, 540);
            this.label7.Margin = new Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new Size(60, 0x18);
            this.label7.TabIndex = 11;
            this.label7.Text = "HDD1:";
            this.buttonStart.Font = new Font("微软雅黑", 12f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.buttonStart.Location = new Point(0x400, 0x2f4);
            this.buttonStart.Margin = new Padding(4, 5, 4, 5);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new Size(0xe1, 80);
            this.buttonStart.TabIndex = 1;
            this.buttonStart.Text = "开始\r\nStart(&S)";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new EventHandler(this.buttonStart_Click);
            this.label6.AutoSize = true;
            this.label6.Font = new Font("宋体", 20.1f, FontStyle.Bold, GraphicsUnit.Point, 0x86);
            this.label6.Location = new Point(0x1b5, 0x53);
            this.label6.Margin = new Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new Size(0x1b6, 0x29);
            this.label6.TabIndex = 10;
            this.label6.Text = "欢迎使用系统还原程序";
            this.label6.TextAlign = ContentAlignment.MiddleCenter;
            this.label5.AutoSize = true;
            this.label5.Font = new Font("Arial Narrow", 12.9f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.label5.ForeColor = Color.Red;
            this.label5.Location = new Point(0x67, 0x2be);
            this.label5.Margin = new Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new Size(0x286, 0x3e);
            this.label5.TabIndex = 9;
            this.label5.Text = "Important: Recovery Program will clean the selected HDD drivers. \r\nPlease backup the personal data before starting!";
            this.labelHDD2.AutoSize = true;
            this.labelHDD2.Font = new Font("Arial Narrow", 12f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.labelHDD2.Location = new Point(0x142, 0x241);
            this.labelHDD2.Margin = new Padding(4, 0, 4, 0);
            this.labelHDD2.Name = "labelHDD2";
            this.labelHDD2.Size = new Size(0x5b, 0x1d);
            this.labelHDD2.TabIndex = 8;
            this.labelHDD2.Text = "loading...";
            this.labelHDD1.AutoSize = true;
            this.labelHDD1.Font = new Font("Arial Narrow", 12f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.labelHDD1.Location = new Point(0x142, 0x1f1);
            this.labelHDD1.Margin = new Padding(4, 0, 4, 0);
            this.labelHDD1.Name = "labelHDD1";
            this.labelHDD1.Size = new Size(0x5b, 0x1d);
            this.labelHDD1.TabIndex = 7;
            this.labelHDD1.Text = "loading...";
            this.checkBoxHDD2.AutoSize = true;
            this.checkBoxHDD2.Font = new Font("宋体", 12f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.checkBoxHDD2.Location = new Point(0xaf, 0x242);
            this.checkBoxHDD2.Margin = new Padding(4, 5, 4, 5);
            this.checkBoxHDD2.Name = "checkBoxHDD2";
            this.checkBoxHDD2.Size = new Size(120, 0x1c);
            this.checkBoxHDD2.TabIndex = 6;
            this.checkBoxHDD2.Text = "硬盘2：";
            this.checkBoxHDD2.UseVisualStyleBackColor = true;
            this.checkBoxHDD1.AutoSize = true;
            this.checkBoxHDD1.Enabled = false;
            this.checkBoxHDD1.Font = new Font("宋体", 12f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.checkBoxHDD1.Location = new Point(0xaf, 0x1f2);
            this.checkBoxHDD1.Margin = new Padding(4, 5, 4, 5);
            this.checkBoxHDD1.Name = "checkBoxHDD1";
            this.checkBoxHDD1.Size = new Size(120, 0x1c);
            this.checkBoxHDD1.TabIndex = 5;
            this.checkBoxHDD1.Text = "硬盘1：";
            this.checkBoxHDD1.UseVisualStyleBackColor = true;
            this.label2.AutoSize = true;
            this.label2.Font = new Font("宋体", 15f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.label2.ForeColor = Color.Red;
            this.label2.Location = new Point(0x67, 0x292);
            this.label2.Margin = new Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new Size(0x3af, 30);
            this.label2.TabIndex = 4;
            this.label2.Text = "重要：还原程序将会重置选择的磁盘驱动器，开始前先备份个人数据！";
            this.panel1.Anchor = AnchorStyles.None;
            this.panel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.panel1.BackColor = SystemColors.HighlightText;
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.buttonCheck);
            this.panel1.Controls.Add(this.label18);
            this.panel1.Controls.Add(this.label17);
            this.panel1.Controls.Add(this.buttonNetRetry);
            this.panel1.Controls.Add(this.label16);
            this.panel1.Controls.Add(this.comboBox2);
            this.panel1.Controls.Add(this.label15);
            this.panel1.Controls.Add(this.labelNetStatus);
            this.panel1.Controls.Add(this.label1NetStatus);
            this.panel1.Controls.Add(this.labelOSVersion);
            this.panel1.Controls.Add(this.label1OSVersion);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.comboBox1);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.buttonStart);
            this.panel1.Controls.Add(this.buttonShutdown);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.labelHDD2);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.checkBoxHDD1);
            this.panel1.Controls.Add(this.checkBoxHDD2);
            this.panel1.Controls.Add(this.labelHDD1);
            this.panel1.Location = new Point(0xee, 0xc4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new Size(0x54e, 0x377);
            this.panel1.TabIndex = 1;
            this.buttonCheck.Font = new Font("Microsoft Sans Serif", 12f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.buttonCheck.Location = new Point(0x400, 0x1c7);
            this.buttonCheck.Margin = new Padding(4, 5, 4, 5);
            this.buttonCheck.Name = "buttonCheck";
            this.buttonCheck.Size = new Size(0xe1, 80);
            this.buttonCheck.TabIndex = 0x1d;
            this.buttonCheck.Text = "Check";
            this.buttonCheck.UseVisualStyleBackColor = true;
            this.buttonCheck.Click += new EventHandler(this.buttonCheck_Click);
            this.label18.AutoSize = true;
            this.label18.Font = new Font("Arial Narrow", 12f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.label18.Location = new Point(0x68, 0x1c7);
            this.label18.Margin = new Padding(4, 0, 4, 0);
            this.label18.Name = "label18";
            this.label18.Size = new Size(0x106, 0x1d);
            this.label18.TabIndex = 0x1c;
            this.label18.Text = "Select the HDDs to format：";
            this.label17.AutoSize = true;
            this.label17.Font = new Font("Arial Narrow", 12f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.label17.Location = new Point(0x67, 0x170);
            this.label17.Margin = new Padding(4, 0, 4, 0);
            this.label17.Name = "label17";
            this.label17.Size = new Size(0xb3, 0x1d);
            this.label17.TabIndex = 0x1b;
            this.label17.Text = "Select OS Mode：";
            this.label16.AutoSize = true;
            this.label16.Font = new Font("Arial Narrow", 12f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.label16.Location = new Point(0x67, 0x119);
            this.label16.Margin = new Padding(4, 0, 4, 0);
            this.label16.Name = "label16";
            this.label16.Size = new Size(0xd9, 0x1d);
            this.label16.TabIndex = 0x1a;
            this.label16.Text = "Select OS Language：";
            this.comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBox2.FormattingEnabled = true;
            object[] items = new object[] { "RUYI & PC 双模式", "PC 单模式" };
            this.comboBox2.Items.AddRange(items);
            this.comboBox2.Location = new Point(0x164, 0x149);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new Size(0x15f, 0x1c);
            this.comboBox2.TabIndex = 0x19;
            this.label15.AutoSize = true;
            this.label15.Font = new Font("宋体", 15f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.label15.Location = new Point(0x67, 0x144);
            this.label15.Name = "label15";
            this.label15.Size = new Size(0xdf, 30);
            this.label15.TabIndex = 0x18;
            this.label15.Text = "选择系统模式：";
            this.labelNetStatus.AutoSize = true;
            this.labelNetStatus.Location = new Point(0x47c, 0xa4);
            this.labelNetStatus.Name = "labelNetStatus";
            this.labelNetStatus.Size = new Size(0x48, 20);
            this.labelNetStatus.TabIndex = 0x17;
            this.labelNetStatus.Text = "loading...";
            this.label1NetStatus.AutoSize = true;
            this.label1NetStatus.Location = new Point(0x41d, 0xa4);
            this.label1NetStatus.Name = "label1NetStatus";
            this.label1NetStatus.Size = new Size(0x59, 20);
            this.label1NetStatus.TabIndex = 0x16;
            this.label1NetStatus.Text = "网络连接：";
            this.labelOSVersion.AutoSize = true;
            this.labelOSVersion.Location = new Point(0x47c, 0x53);
            this.labelOSVersion.Name = "labelOSVersion";
            this.labelOSVersion.Size = new Size(0x48, 20);
            this.labelOSVersion.TabIndex = 0x15;
            this.labelOSVersion.Text = "loading...";
            this.label1OSVersion.AutoSize = true;
            this.label1OSVersion.Location = new Point(0x41d, 0x53);
            this.label1OSVersion.Name = "label1OSVersion";
            this.label1OSVersion.Size = new Size(0x59, 20);
            this.label1OSVersion.TabIndex = 20;
            this.label1OSVersion.Text = "系统版本：";
            this.label10.AutoSize = true;
            this.label10.Font = new Font("宋体", 15f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.label10.Location = new Point(0x67, 0x19b);
            this.label10.Name = "label10";
            this.label10.Size = new Size(0x139, 30);
            this.label10.TabIndex = 0x13;
            this.label10.Text = "选择需格式化的磁盘：";
            this.comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new Point(0x164, 0xf4);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new Size(0x15f, 0x1c);
            this.comboBox1.TabIndex = 0x12;
            this.label9.AutoSize = true;
            this.label9.Font = new Font("宋体", 15f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.label9.Location = new Point(0x67, 0xed);
            this.label9.Name = "label9";
            this.label9.Size = new Size(0xdf, 30);
            this.label9.TabIndex = 0x11;
            this.label9.Text = "选择系统语言：";
            this.label3.AutoSize = true;
            this.label3.Font = new Font("Microsoft Sans Serif", 7f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.label3.Location = new Point(0x41e, 0x67);
            this.label3.Name = "label3";
            this.label3.Size = new Size(0x54, 0x11);
            this.label3.TabIndex = 30;
            this.label3.Text = "OS Version:";
            this.label4.AutoSize = true;
            this.label4.Font = new Font("Microsoft Sans Serif", 7f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.label4.Location = new Point(0x41e, 0xb8);
            this.label4.Name = "label4";
            this.label4.Size = new Size(0x3f, 0x11);
            this.label4.TabIndex = 30;
            this.label4.Text = "Network:";
            base.AutoScaleDimensions = new SizeF(9f, 20f);
            base.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = SystemColors.MenuHighlight;
            base.ClientSize = new Size(0x72a, 0x54a);
            base.Controls.Add(this.panel1);
            base.FormBorderStyle = FormBorderStyle.None;
            base.Margin = new Padding(4, 5, 4, 5);
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "Form1";
            base.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "RUYIOS Recovery";
            base.TopMost = true;
            base.WindowState = FormWindowState.Maximized;
            base.Load += new EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            base.ResumeLayout(false);
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        public int StartExe(string filename, string args)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo {
                FileName = filename,
                Arguments = args
            };
            Console.WriteLine(startInfo.Arguments);
            Process process = Process.Start(startInfo);
            process.WaitForExit();
            return process.ExitCode;
        }

        public void UnzipModule(string ModuleName, string SourcePath, string TargetPath)
        {
            string str = "Ruyi@2018!@#";
            string filename = Application.StartupPath + @"\7z.exe";
            string[] textArray1 = new string[] { "x -p", str, " -y ", SourcePath, @"\", ModuleName, " -o", TargetPath };
            this.StartExe(filename, string.Concat(textArray1));
        }
    }
}

