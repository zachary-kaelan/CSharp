namespace TaskBarApp
{
    using Microsoft.Win32;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Cryptography;

    internal class AppRegistry
    {
        private static List<ConfigObject> configList = new List<ConfigObject>();
        private static byte[] IV_64 = new byte[] { 0x2d, 0x2f, 0x44, 0x37, 50, 0xeb, 0x1d, 0x1f };
        private static byte[] KEY_64 = new byte[] { 11, 0x34, 0x49, 0x5e, 0xeb, 0x60, 0xed, 0x58 };
        private static DateTime lastServerFetch = DateTime.Now;
        private static string macAddr = NetworkInterface.GetAllNetworkInterfaces().Where<NetworkInterface>(i => i.OperationalStatus == OperationalStatus.Up).Select<NetworkInterface, string>(i => i.GetPhysicalAddress().ToString()).FirstOrDefault<string>();
        private static string strHomeServer = "http://www.t2ll.com";

        public static string Append(string Source, string Append, string Separator = "\r\n")
        {
            if (Source.Length > 0)
            {
                Source = Source + Separator;
            }
            return (Source + Append);
        }

        public static bool AuthorizeUser(string PhoneNumber, ref string ErrorMessage)
        {
            bool flag = true;
            try
            {
                string str = string.Empty;
                HttpWebRequest request1 = (HttpWebRequest) WebRequest.Create(strHomeServer + "/api/MachineProfile/ValidateIP?phoneNumber=" + PhoneNumber);
                request1.AutomaticDecompression = DecompressionMethods.GZip;
                request1.Timeout = 0x2710;
                using (HttpWebResponse response = (HttpWebResponse) request1.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            str = reader.ReadToEnd();
                        }
                    }
                }
                if (str == "false")
                {
                    flag = false;
                }
            }
            catch (Exception)
            {
            }
            return flag;
        }

        private static object ContactConfigAPI(ApiAction action, RegistryKey RegKey, string Key, object Value)
        {
            string errorMessage = string.Empty;
            if (action == ApiAction.setValue)
            {
                string userName = null;
                if (RegKey.ToString().EndsWith("Text App"))
                {
                    RegistryKey rootKey = GetRootKey(ref errorMessage);
                    userName = GetUserName(rootKey, ref errorMessage);
                }
                else
                {
                    userName = null;
                }
                object[] objArray1 = new object[] { strHomeServer, "/api/MachineProfile/updatemachinesetting?machineId=", GetMachineID(), "&regKey=", RegKey, "&key=", Key, "&propertyvalue=", Value.ToString(), "&phonenumber=", userName, "&winUser=", Environment.UserName };
                HttpWebRequest request1 = (HttpWebRequest) WebRequest.Create(string.Concat(objArray1));
                request1.AutomaticDecompression = DecompressionMethods.GZip;
                request1.Timeout = 0x2710;
                using (HttpWebResponse response = (HttpWebResponse) request1.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            reader.ReadToEnd();
                        }
                    }
                }
                if ((from i in configList
                    where (i.regKey == RegKey.Name) && (i.key == Key)
                    select i).FirstOrDefault<ConfigObject>() == null)
                {
                    ConfigObject item = new ConfigObject {
                        key = Key,
                        regKey = RegKey.Name,
                        propertyValue = Value
                    };
                    configList.Add(item);
                    return Value;
                }
                (from i in configList
                    where (i.regKey == RegKey.Name) && (i.key == Key)
                    select i).First<ConfigObject>().propertyValue = Value;
                return Value;
            }
            if ((from i in configList
                where (i.regKey == RegKey.Name) && (i.key == Key)
                select i).FirstOrDefault<ConfigObject>() == null)
            {
                if ((from i in configList
                    where (i.regKey == RegKey.Name) && (i.key == Key)
                    select i).FirstOrDefault<ConfigObject>() == null)
                {
                    ConfigObject obj2 = new ConfigObject {
                        key = Key,
                        regKey = RegKey.Name,
                        propertyValue = Value
                    };
                    configList.Add(obj2);
                    return Value;
                }
                (from i in configList
                    where (i.regKey == RegKey.Name) && (i.key == Key)
                    select i).First<ConfigObject>().propertyValue = Value;
            }
            return Value;
        }

        public static string Decrypt(string Data, ref string ErrorMessage)
        {
            if (Data.Length == 0)
            {
                return Data;
            }
            string str = string.Empty;
            try
            {
                DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
                str = new StreamReader(new CryptoStream(new MemoryStream(Convert.FromBase64String(Data)), provider.CreateDecryptor(KEY_64, IV_64), CryptoStreamMode.Read)).ReadToEnd();
            }
            catch (Exception exception)
            {
                ErrorMessage = Append(ErrorMessage, exception.Message, "\r\n");
            }
            return str;
        }

        public static string Encrypt(string Data, ref string ErrorMessage)
        {
            string str = string.Empty;
            try
            {
                DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
                MemoryStream stream = new MemoryStream();
                CryptoStream stream1 = new CryptoStream(stream, provider.CreateEncryptor(KEY_64, IV_64), CryptoStreamMode.Write);
                StreamWriter writer1 = new StreamWriter(stream1);
                writer1.Write(Data);
                writer1.Flush();
                stream1.FlushFinalBlock();
                stream.Flush();
                str = Convert.ToBase64String(stream.GetBuffer(), 0, Convert.ToInt32(stream.Length));
            }
            catch (Exception exception)
            {
                ErrorMessage = Append(ErrorMessage, exception.Message, "\r\n");
            }
            return str;
        }

        private static Attribute GetAttribute(Assembly assembly, Type attributeType)
        {
            object[] customAttributes = assembly.GetCustomAttributes(attributeType, false);
            if (customAttributes.Length == 0)
            {
                return null;
            }
            return (Attribute) customAttributes[0];
        }

        public static string GetMachineID()
        {
            string macAddr = string.Empty;
            string errorMessage = string.Empty;
            RegistryKey rootKey = GetRootKey(ref errorMessage);
            GetValue(rootKey, "local_MachineID", ref macAddr, ref errorMessage);
            if (string.IsNullOrEmpty(macAddr))
            {
                macAddr = AppRegistry.macAddr;
                if (string.IsNullOrEmpty(macAddr))
                {
                    macAddr = Guid.NewGuid().ToString();
                }
                SaveValue(rootKey, "local_MachineID", macAddr, ref errorMessage, false, RegistryValueKind.Unknown);
            }
            return macAddr;
        }

        public static string GetPassword(RegistryKey Key, ref string ErrorMessage)
        {
            string str = string.Empty;
            GetValue(Key, "Password", ref str, ref ErrorMessage);
            if (str.Length > 0)
            {
                str = Decrypt(str, ref ErrorMessage);
            }
            return str;
        }

        public static RegistryKey GetRootKey(ref string ErrorMessage)
        {
            RegistryKey currentUser = Registry.CurrentUser;
            try
            {
                Assembly entryAssembly = Assembly.GetEntryAssembly();
                AssemblyCompanyAttribute attribute = (AssemblyCompanyAttribute) GetAttribute(entryAssembly, typeof(AssemblyCompanyAttribute));
                AssemblyTitleAttribute attribute2 = (AssemblyTitleAttribute) GetAttribute(entryAssembly, typeof(AssemblyTitleAttribute));
                string str = (attribute2 != null) ? attribute2.Title : entryAssembly.GetName().Name;
                string str2 = (attribute != null) ? attribute.Company : "";
                string name = $"Software\\{str2}\\{str}";
                RegistryKey key2 = currentUser.OpenSubKey(name, true);
                if (key2 == null)
                {
                    return currentUser.CreateSubKey(name);
                }
                currentUser = key2;
            }
            catch (Exception exception)
            {
                ErrorMessage = exception.Message;
            }
            return currentUser;
        }

        public static RegistryKey GetSubKey(RegistryKey RegKey, string SubKey, bool Create, ref string ErrorMessage)
        {
            RegistryKey key = null;
            try
            {
                if (RegKey == null)
                {
                    RegKey = GetRootKey(ref ErrorMessage);
                }
                if (RegKey == null)
                {
                    return key;
                }
                key = RegKey.OpenSubKey(SubKey, true);
                if ((key == null) & Create)
                {
                    return RegKey.CreateSubKey(SubKey);
                }
                if (key == null)
                {
                    ErrorMessage = Append(ErrorMessage, SubKey + " was not found.", "\r\n");
                }
            }
            catch (Exception exception)
            {
                ErrorMessage = Append(ErrorMessage, "Error accessing key " + SubKey + ": " + exception.Message, "\r\n");
            }
            return key;
        }

        public static string[] GetSubKeyNames(RegistryKey RegKey, string SubKey, ref string ErrorMessage)
        {
            string[] subKeyNames = null;
            string errorMessage = string.Empty;
            try
            {
                if (SubKey != null)
                {
                    RegistryKey key = GetSubKey(RegKey, SubKey, false, ref errorMessage);
                    if (key == null)
                    {
                        errorMessage = Append(errorMessage, "Failed to open subkey.", "\r\n");
                    }
                    else
                    {
                        subKeyNames = key.GetSubKeyNames();
                    }
                }
                else
                {
                    subKeyNames = RegKey.GetSubKeyNames();
                }
            }
            catch (SecurityException exception)
            {
                errorMessage = Append(errorMessage, "Security exception: " + exception.Message + ".", "\r\n");
                return null;
            }
            catch (IOException exception2)
            {
                errorMessage = Append(errorMessage, "IO exception: " + exception2.Message + ".", "\r\n");
                return null;
            }
            catch (Exception exception3)
            {
                errorMessage = Append(errorMessage, "Exception: " + exception3.Message + ".", "\r\n");
            }
            if (errorMessage.Length > 0)
            {
                ErrorMessage = Append(ErrorMessage, "Error getting the subkeys for " + RegKey.ToString() + ((SubKey.Length > 0) ? ("//" + SubKey) : "") + ". " + errorMessage + ".", "\r\n");
            }
            return subKeyNames;
        }

        public static string GetUserName(RegistryKey Key, ref string ErrorMessage)
        {
            string str = string.Empty;
            GetValue(Key, "UserName", ref str, ref ErrorMessage);
            return str;
        }

        public static bool GetValue(RegistryKey RegKey, string Key, ref bool Value, ref string ErrorMessage)
        {
            string errorMessage = string.Empty;
            object obj2 = null;
            try
            {
                if (GetValue(RegKey, Key, ref obj2, ref errorMessage) && (obj2 != null))
                {
                    Value = Convert.ToInt16(obj2) > 0;
                }
            }
            catch (Exception exception)
            {
                errorMessage = Append(errorMessage, "Error getting registry Key: " + Key + " Error: " + exception.Message, "\r\n");
            }
            if (errorMessage.Length > 0)
            {
                ErrorMessage = Append(ErrorMessage, errorMessage, "\r\n");
                return false;
            }
            return true;
        }

        public static bool GetValue(RegistryKey RegKey, string Key, ref int Value, ref string ErrorMessage)
        {
            string errorMessage = string.Empty;
            object obj2 = null;
            try
            {
                if (GetValue(RegKey, Key, ref obj2, ref errorMessage) && (obj2 != null))
                {
                    Value = (int) obj2;
                }
            }
            catch (Exception exception)
            {
                errorMessage = Append(errorMessage, "Error getting registry Key: " + Key + " Error: " + exception.Message, "\r\n");
            }
            if (errorMessage.Length > 0)
            {
                ErrorMessage = Append(ErrorMessage, errorMessage, "\r\n");
                return false;
            }
            return true;
        }

        public static bool GetValue(RegistryKey RegKey, string Key, ref object Value, ref string ErrorMessage)
        {
            try
            {
                Value = null;
                if (RegKey == null)
                {
                    RegKey = GetRootKey(ref ErrorMessage);
                }
                if (RegKey != null)
                {
                    Value = RegKey.GetValue(Key);
                    return true;
                }
                ErrorMessage = Append(ErrorMessage, "Invalid root registry key", "\r\n");
            }
            catch (Exception exception)
            {
                ErrorMessage = Append(ErrorMessage, exception.Message, "\r\n");
                Value = null;
            }
            return false;
        }

        public static bool GetValue(RegistryKey RegKey, string Key, ref string Value, ref string ErrorMessage)
        {
            string errorMessage = string.Empty;
            object obj2 = null;
            Value = string.Empty;
            try
            {
                if (GetValue(RegKey, Key, ref obj2, ref errorMessage) && (obj2 != null))
                {
                    Value = (string) obj2;
                    if ((RegKey != null) && RegKey.Name.EndsWith("EncryptedData"))
                    {
                        Value = Decrypt(Value, ref errorMessage);
                    }
                }
            }
            catch (Exception exception)
            {
                errorMessage = Append(errorMessage, "Error getting registry Key: " + Key + " Error: " + exception.Message, "\r\n");
            }
            if (errorMessage.Length > 0)
            {
                ErrorMessage = Append(ErrorMessage, errorMessage, "\r\n");
                return false;
            }
            return true;
        }

        public static string[] GetValueNames(RegistryKey RegKey, string SubKey, ref string ErrorMessage)
        {
            string errorMessage = string.Empty;
            string[] valueNames = null;
            try
            {
                if (SubKey.Length > 0)
                {
                    RegistryKey key = GetSubKey(RegKey, SubKey, false, ref errorMessage);
                    if (key == null)
                    {
                        errorMessage = Append(errorMessage, "Failed to open subkey", "\r\n");
                    }
                    else
                    {
                        valueNames = key.GetValueNames();
                    }
                }
                else
                {
                    valueNames = RegKey.GetValueNames();
                }
            }
            catch (SecurityException exception)
            {
                errorMessage = Append(errorMessage, "Security exception: " + exception.Message, "\r\n");
                return null;
            }
            catch (IOException exception2)
            {
                errorMessage = Append(errorMessage, "IO exception: " + exception2.Message, "\r\n");
                return null;
            }
            catch (Exception exception3)
            {
                errorMessage = Append(errorMessage, "Exception: " + exception3.Message, "\r\n");
            }
            if (errorMessage.Length > 0)
            {
                ErrorMessage = Append(ErrorMessage, "Error getting the subkeys for " + RegKey.ToString() + ((SubKey.Length > 0) ? ("//" + SubKey) : "") + ". " + errorMessage + ".", "\r\n");
            }
            return valueNames;
        }

        public static bool SavePassword(RegistryKey Key, string Password, ref string ErrorMessage)
        {
            string errorMessage = string.Empty;
            string str2 = Encrypt(Password, ref errorMessage);
            if (errorMessage.Length == 0)
            {
                return SaveValue(Key, "Password", str2, ref ErrorMessage, false, RegistryValueKind.Unknown);
            }
            ErrorMessage = Append(ErrorMessage, errorMessage, "\r\n");
            return false;
        }

        public static bool SaveUserName(RegistryKey Key, string UserName, ref string ErrorMessage) => 
            SaveValue(Key, "UserName", UserName, ref ErrorMessage, false, RegistryValueKind.Unknown);

        public static bool SaveValue(RegistryKey RegKey, string Key, object Value, ref string ErrorMessage, bool callApi = false, RegistryValueKind rkKind = 0)
        {
            string errorMessage = string.Empty;
            try
            {
                if (RegKey == null)
                {
                    RegKey = GetRootKey(ref errorMessage);
                }
                if (RegKey != null)
                {
                    int length = Key.LastIndexOf('\\');
                    if (length > -1)
                    {
                        string subKey = Key.Substring(0, length);
                        RegKey = GetSubKey(RegKey, subKey, true, ref errorMessage);
                        if (RegKey == null)
                        {
                            ErrorMessage = Append(ErrorMessage, "Failed to create registry sub-key", "\r\n");
                            return false;
                        }
                        Key = Key.Substring(length + 1);
                    }
                    if (rkKind == RegistryValueKind.Unknown)
                    {
                        if (Value is string)
                        {
                            rkKind = RegistryValueKind.String;
                        }
                        else if (Value is int)
                        {
                            rkKind = RegistryValueKind.DWord;
                        }
                        else if (Value is bool)
                        {
                            rkKind = RegistryValueKind.DWord;
                            Value = ((bool) Value) ? 1 : 0;
                        }
                        else
                        {
                            rkKind = RegistryValueKind.Unknown;
                        }
                    }
                    if (callApi)
                    {
                        try
                        {
                            ContactConfigAPI(ApiAction.setValue, RegKey, Key, Value);
                        }
                        catch (Exception)
                        {
                        }
                    }
                    RegKey.SetValue(Key, Value, rkKind);
                }
                else
                {
                    errorMessage = Append(errorMessage, "Invalid root registry key", "\r\n");
                }
            }
            catch (Exception exception)
            {
                errorMessage = Append(errorMessage, exception.Message, exception.StackTrace);
                Value = null;
            }
            if (errorMessage.Length > 0)
            {
                ErrorMessage = Append(ErrorMessage, errorMessage, "\r\n");
                return false;
            }
            return true;
        }

        public static bool SyncServer(ref string strItemsUpdated, string strPhoneNumber)
        {
            try
            {
                string errorMessage = "";
                bool flag = false;
                GetValue(GetRootKey(ref errorMessage), "local_DisableRemoteFeatureUpdates", ref flag, ref errorMessage);
                if (!flag)
                {
                    string str2 = string.Empty;
                    string[] textArray1 = new string[] { strHomeServer, "/api/MachineProfile/getmachinesettings?machineId=", GetMachineID(), "&winUser=", Environment.UserName, "&phoneNumber=", strPhoneNumber };
                    HttpWebRequest request1 = (HttpWebRequest) WebRequest.Create(string.Concat(textArray1));
                    request1.AutomaticDecompression = DecompressionMethods.GZip;
                    request1.Timeout = 0x2710;
                    using (HttpWebResponse response = (HttpWebResponse) request1.GetResponse())
                    {
                        using (Stream stream = response.GetResponseStream())
                        {
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                str2 = reader.ReadToEnd();
                            }
                        }
                    }
                    configList = JsonConvert.DeserializeObject<List<ConfigObject>>(str2);
                    foreach (ConfigObject obj2 in configList)
                    {
                        if (((obj2.key.Substring(0, 6) != "local_") && (obj2.key != "UserName")) && ((obj2.key != "Password") && (obj2.phoneNumber == strPhoneNumber)))
                        {
                            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(obj2.regKey.Replace(@"HKEY_CURRENT_USER\", ""), true);
                            object obj3 = regKey.GetValue(obj2.key);
                            RegistryValueKind valueKind = regKey.GetValueKind(obj2.key);
                            if (obj3 != null)
                            {
                                string str3 = (obj2.propertyValue == null) ? "" : obj2.propertyValue.ToString();
                                if (obj3.ToString() != str3)
                                {
                                    errorMessage = "";
                                    SaveValue(regKey, obj2.key, str3, ref errorMessage, false, valueKind);
                                    if (errorMessage == "")
                                    {
                                        string[] textArray2 = new string[] { strItemsUpdated, obj2.key, " has been changed to: ", str3, "\r\n" };
                                        strItemsUpdated = string.Concat(textArray2);
                                    }
                                }
                            }
                            regKey.Close();
                        }
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public ApplicationManager appManager { get; set; }

        /*[Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly AppRegistry.<>c <>9 = new AppRegistry.<>c();

            internal bool <.cctor>b__34_0(NetworkInterface nic) => 
                (nic.OperationalStatus == OperationalStatus.Up);

            internal string <.cctor>b__34_1(NetworkInterface nic) => 
                nic.GetPhysicalAddress().ToString();
        }*/

        private enum ApiAction
        {
            getValue,
            setValue
        }

        public class ConfigObject
        {
            public string key;
            public string phoneNumber;
            public object propertyValue;
            public string regKey;
        }
    }
}

