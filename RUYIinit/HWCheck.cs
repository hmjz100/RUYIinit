namespace RUYIinit
{
    using System;
    using System.IO;
    using System.Management;
    using System.Runtime.InteropServices;

    public class HWCheck
    {
        public string HDD1 = null;
        public string HDD2 = null;
        private ulong HDD1Size;
        private ulong HDD2Size;

        public bool CopyFile(string fileName, string sourcePath, string targetPath)
        {
            string path = Path.Combine(sourcePath, fileName);
            string destFileName = Path.Combine(targetPath, fileName);
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }
            if (File.Exists(path))
            {
                File.Copy(path, destFileName, true);
            }
            return File.Exists(destFileName);
        }

        public void GetDiskInfo()
        {
            string str2 = "MSFT_Disk";
            foreach (ManagementObject obj2 in new ManagementClass(@"\\.\ROOT\Microsoft\Windows\Storage" + ":" + str2).GetInstances())
            {
                string str3 = obj2["BusType"].ToString();
                string str4 = obj2["Model"].ToString();
                ulong num = ulong.Parse(obj2["Size"].ToString());
                string str5 = obj2["Number"].ToString();
                string str6 = obj2["Location"].ToString();
                if ((str3 == "11") && (str6 == "Integrated : Adapter 0 : Port 0"))
                {
                    this.HDD1 = str4;
                    this.HDD1Size = num;
                }
                if ((str3 == "11") && (str6 == "Integrated : Adapter 0 : Port 1"))
                {
                    this.HDD2 = str4;
                    this.HDD2Size = num;
                }
            }
        }

        public string GetDriver()
        {
            string name = null;
            foreach (DriveInfo info in DriveInfo.GetDrives())
            {
                try
                {
                    info.DriveFormat.Equals("FAT32");
                    Console.WriteLine(info.Name + ":" + info.IsReady.ToString());
                    if ((info.IsReady && info.DriveFormat.Equals("FAT32")) && info.VolumeLabel.Equals("RUYIOS"))
                    {
                        name = info.Name;
                    }
                }
                catch (IOException exception)
                {
                    Console.WriteLine("Exception caught: {0}", exception);
                }
            }
            return name;
        }

        [DllImport("wininet.dll")]
        private static extern bool InternetGetConnectedState(int Description, int ReservedValue);
        public bool IsConnectInternet() => 
            InternetGetConnectedState(0, 0);
    }
}

