namespace RUYIinit2
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Xml;

    internal class ParseXML
    {
        public string RecoveryDrv;
        public string SetEnv_ID;
        public DataTable tableTokens = new DataTable();
        public string RUYIOSVersion;
        public string AppPath = Application.StartupPath;
        public JArray jaLG;
        public JArray jaOM;
        public DataTable ModuleList = new DataTable();

        public void GetDriver()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            int index = 0;
            while (true)
            {
                if (index < drives.Length)
                {
                    DriveInfo info = drives[index];
                    bool isReady = info.IsReady;
                    if (!isReady || (info.VolumeLabel != "RUYIOS"))
                    {
                        index++;
                        continue;
                    }
                    this.RecoveryDrv = info.Name;
                }
                return;
            }
        }

        public void GetSetEnV()
        {
            string str = this.SetEnv_ID;
            string appPath = this.AppPath;
            string path = Path.Combine(this.RecoveryDrv + @"Sources\", str);
            string destFileName = Path.Combine(appPath, str);
            if (!Directory.Exists(appPath))
            {
                Directory.CreateDirectory(appPath);
            }
            if (File.Exists(path))
            {
                File.Copy(path, destFileName, true);
            }
        }

        public void GetTokenInfo()
        {
            using (StreamReader reader = File.OpenText(@".\Language.json"))
            {
                using (JsonTextReader reader2 = new JsonTextReader(reader))
                {
                    JObject obj2 = (JObject) JToken.ReadFrom(reader2);
                    this.jaLG = (JArray) obj2["LANGUAGE"];
                    this.jaOM = (JArray) obj2["OSMODE"];
                }
            }
        }

        public void GetXML()
        {
            string str = "RUYIOS.xml";
            string appPath = this.AppPath;
            string path = Path.Combine(this.RecoveryDrv + @"Sources\", str);
            string destFileName = Path.Combine(appPath, str);
            if (!Directory.Exists(appPath))
            {
                Directory.CreateDirectory(appPath);
            }
            if (File.Exists(path))
            {
                File.Copy(path, destFileName, true);
            }
        }

        public void GetXMLInfo()
        {
            XmlDocument document = new XmlDocument();
            document.Load(this.AppPath + @"\RUYIOS.xml");
            XmlElement documentElement = document.DocumentElement;
            if (documentElement.HasAttribute("RUYIOSVersion"))
            {
                this.RUYIOSVersion = documentElement.GetAttribute("RUYIOSVersion");
            }
            XmlNodeList elementsByTagName = documentElement.GetElementsByTagName("Module");
            Console.WriteLine("elemList.Count;" + elementsByTagName.Count);
            int num = 0;
            while (true)
            {
                if (num >= elementsByTagName.Count)
                {
                    this.ModuleList.Columns.Add("name", Type.GetType("System.String"));
                    this.ModuleList.Columns.Add("Version", Type.GetType("System.String"));
                    this.ModuleList.Columns.Add("ID", Type.GetType("System.String"));
                    this.ModuleList.Columns.Add("MD5", Type.GetType("System.String"));
                    this.ModuleList.Columns.Add("Type", Type.GetType("System.String"));
                    this.ModuleList.Columns.Add("Condition", Type.GetType("System.String"));
                    for (int i = 0; i < elementsByTagName.Count; i++)
                    {
                        object[] values = new object[] { elementsByTagName[i].Attributes["name"].Value, elementsByTagName[i].Attributes["Version"].Value, elementsByTagName[i].Attributes["ID"].Value, elementsByTagName[i].Attributes["MD5"].Value, elementsByTagName[i].Attributes["Type"].Value, elementsByTagName[i].Attributes["Condition"].Value };
                        this.ModuleList.Rows.Add(values);
                    }
                    return;
                }
                if (elementsByTagName[num].Attributes["name"].Value == "SetEnv")
                {
                    this.SetEnv_ID = elementsByTagName[num].Attributes["ID"].Value;
                    Console.WriteLine(this.SetEnv_ID);
                }
                num++;
            }
        }

        public void UnzipModule(string ModuleName, string SourcePath, string TargetPath)
        {
            string str = "Ruyi@2018!@#";
            ProcessStartInfo startInfo = new ProcessStartInfo {
                FileName = this.AppPath + @"\7z.exe"
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

