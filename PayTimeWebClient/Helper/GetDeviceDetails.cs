using System;
using System.IO;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace PayTimeWebClient.Helper
{
    public class GetDeviceDetails
    {
        #region ID

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern uint GetWindowsDirectory(StringBuilder lpBuffer, uint uSize);

        public static string GetSystemID()
        {
            try
            {
                string cpuID = GetProcessorID();
                string volumeSerial = GetHardDriveID();

                if (string.IsNullOrEmpty(volumeSerial))
                {
                    volumeSerial = "axp534cna20vna08".ToUpper();
                }
                if (string.IsNullOrEmpty(cpuID))
                {
                    cpuID = "";
                    while (cpuID.Length < 16)
                    {
                        cpuID += Reverse(volumeSerial);
                    }
                }
                return (cpuID.Substring(13) + cpuID.Substring(1, 4) + volumeSerial + cpuID.Substring(4, 5)).ToUpper();
            }
            catch (Exception ex)
            {
                //mantra.loger.LogWriter.Write_MANTRA_RDService(string.Format("{0}.{1}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name) + ".ex: " + ex.ToString());
                return "";
            }
        }
        public static string Reverse(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        private static string GetProcessorID()
        {
            try
            {
                var mbs = new ManagementObjectSearcher("Select ProcessorId From Win32_processor");
                ManagementObjectCollection mbsList = mbs.Get();
                string id = "";
                foreach (ManagementObject mo in mbsList)
                {
                    id = mo["ProcessorId"].ToString();
                    return id;
                }
            }
            catch (Exception ex)
            {
                //mantra.loger.LogWriter.Write_MANTRA_RDService(string.Format("{0}.{1}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name) + ".ex: " + ex.ToString());

            }
            return null;
        }

        private static string GetHardDriveID()
        {
            string drive = "";
            try
            {
                drive = Path.GetPathRoot(Environment.SystemDirectory).Replace(":", "").Replace("\\", "").Substring(0, 1);
            }
            catch
            {
                drive = WindowsDirectory().Substring(0, 1);
            }

            try
            {
                if (drive == null)
                {
                    drive = "C";
                }
                ManagementObject dsk = new ManagementObject(@"win32_logicaldisk.deviceid=""" + drive + @":""");
                dsk.Get();
                string volumeSerial = dsk["VolumeSerialNumber"].ToString();
                return volumeSerial;
            }
            catch (Exception ex)
            {
                //mantra.loger.LogWriter.Write_MANTRA_RDService(string.Format("{0}.{1}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name) + ".ex: " + ex.ToString());
            }
            return null;
        }

        private static string WindowsDirectory()
        {
            try
            {
                uint size = 0;
                size = GetWindowsDirectory(null, size);
                StringBuilder sb = new StringBuilder((int)size);
                GetWindowsDirectory(sb, size);
                return sb.ToString();
            }
            catch (Exception ex)
            {
                //mantra.loger.LogWriter.Write_MANTRA_RDService(string.Format("{0}.{1}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name) + ".ex: " + ex.ToString());
            }
            return null;
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
        #endregion

        #region MAC
        public static string GetIPAddress()
        {
            string ipaddress = "";

            IPHostEntry ipEntry = Dns.GetHostByName(Dns.GetHostName());
            IPAddress[] addr = ipEntry.AddressList;
            if (addr != null && addr.Length > 0)
            {
                ipaddress = addr[0].ToString();
            }
            if (ipaddress == null || ipaddress.Length == 0)
            {
                ipaddress = "127.0.0.1";
            }
            return ipaddress;
        }
        //public static string GetMacAddress()
        //{
        //    try
        //    {
        //        string macAddresses = "";
        //        foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
        //        {
        //            if (nic.NetworkInterfaceType != NetworkInterfaceType.Ethernet) continue;
        //            if (nic.OperationalStatus == OperationalStatus.Up)
        //            {
        //                macAddresses = nic.GetPhysicalAddress().ToString().Trim().Replace(":", "");
        //                break;
        //            }
        //        }
        //        if (macAddresses == "")
        //        {
        //            macAddresses = GetMacAddress2();
        //        }
        //        if (macAddresses == "")
        //        {
        //            macAddresses = GetMacAddress1();
        //        }

        //        return macAddresses;
        //    }
        //    catch (Exception ex)
        //    {
        //        //mantra.loger.LogWriter.Write_MANTRA_RDService(string.Format("{0}.{1}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name) + ".ex: " + ex.ToString());
        //        return "";
        //    }

        //}

        //private static string GetMacAddress1()
        //{
        //    try
        //    {
        //        ManagementObjectSearcher objMOS = new ManagementObjectSearcher("Select * FROM Win32_NetworkAdapterConfiguration");
        //        ManagementObjectCollection objMOC = objMOS.Get();
        //        string macAddress = String.Empty;
        //        foreach (ManagementObject objMO in objMOC)
        //        {
        //            object tempMacAddrObj = objMO["MacAddress"];

        //            if (tempMacAddrObj == null) //Skip objects without a MACAddress
        //            {
        //                continue;
        //            }
        //            if (macAddress == String.Empty) // only return MAC Address from first card that has a MAC Address
        //            {
        //                macAddress = tempMacAddrObj.ToString();
        //            }
        //            objMO.Dispose();
        //        }
        //        macAddress = macAddress.Replace(":", "");
        //        return macAddress;

        //    }
        //    catch (Exception ex)
        //    {
        //       // mantra.loger.LogWriter.Write_MANTRA_RDService(string.Format("{0}.{1}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name) + ".ex: " + ex.ToString());
        //        return "";
        //    }

        //}

        //private static string GetMacAddress2()
        //{
        //    try
        //    {
        //        NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
        //        string sMacAddress = string.Empty;
        //        foreach (NetworkInterface adapter in nics)
        //        {
        //            if (sMacAddress == String.Empty)// only return MAC Address from first card
        //            {
        //                //IPInterfaceProperties properties = adapter.GetIPProperties(); Line is not required
        //                sMacAddress = adapter.GetPhysicalAddress().ToString().Trim().Replace(":", "");
        //            }
        //        }
        //        return sMacAddress;
        //    }
        //    catch (Exception ex)
        //    {
        //       // mantra.loger.LogWriter.Write_MANTRA_RDService(string.Format("{0}.{1}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name) + ".ex: " + ex.ToString());
        //        return "";
        //    }

        //}
        #endregion

        public static void LogFileWrite(string message)
        {
            FileStream fileStream = null;
            StreamWriter streamWriter = null;
            try
            {
                string logFilePath = HttpContext.Current.Server.MapPath("/ErrorLog/");

                logFilePath = logFilePath + "ProgramLog" + "-" + DateTime.Today.ToString("yyyyMMdd") + "." + "txt";

                if (logFilePath.Equals("")) return;
                #region Create the Log file directory if it does not exists
                DirectoryInfo logDirInfo = null;
                FileInfo logFileInfo = new FileInfo(logFilePath);
                logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
                if (!logDirInfo.Exists) logDirInfo.Create();
                #endregion Create the Log file directory if it does not exists

                if (!logFileInfo.Exists)
                {
                    fileStream = logFileInfo.Create();
                }
                else
                {
                    fileStream = new FileStream(logFilePath, FileMode.Append);
                }
                streamWriter = new StreamWriter(fileStream);
                streamWriter.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "-" + message);
            }
            finally
            {
                if (streamWriter != null) streamWriter.Close();
                if (fileStream != null) fileStream.Close();
            }

        }

        public static void ProcessLogLogFileWrite(string Filename, string message)
        {
            FileStream fileStream = null;
            StreamWriter streamWriter = null;
            try
            {
                string logFilePath = HttpContext.Current.Server.MapPath("/ErrorLog/");

                logFilePath = logFilePath + Filename + "-" + DateTime.Today.ToString("yyyyMMdd") + "." + "txt";

                if (logFilePath.Equals("")) return;
                #region Create the Log file directory if it does not exists
                DirectoryInfo logDirInfo = null;
                FileInfo logFileInfo = new FileInfo(logFilePath);
                logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
                if (!logDirInfo.Exists) logDirInfo.Create();
                #endregion Create the Log file directory if it does not exists

                if (!logFileInfo.Exists)
                {
                    fileStream = logFileInfo.Create();
                }
                else
                {
                    fileStream = new FileStream(logFilePath, FileMode.Append);
                }
                streamWriter = new StreamWriter(fileStream);
                streamWriter.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "-" + message);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (streamWriter != null) streamWriter.Close();
                if (fileStream != null) fileStream.Close();
            }

        }

        public static void ifnotexistdirectory(string DirectoryPath)
        {
            DirectoryInfo logDirInfo = null;
            logDirInfo = new DirectoryInfo(DirectoryPath);
            if (!logDirInfo.Exists) logDirInfo.Create();
        }
    }


    public class Paymentgateway
    {



    }

}

