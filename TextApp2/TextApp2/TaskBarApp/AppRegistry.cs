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
using System.Security;
using System.Security.Cryptography;

namespace TaskBarApp
{
	internal class AppRegistry
	{
		private enum ApiAction
		{
			getValue,
			setValue
		}

		public class ConfigObject
		{
			public string phoneNumber;

			public string regKey;

			public string key;

			public object propertyValue;
		}

		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			public static readonly AppRegistry.<>c <>9 = new AppRegistry.<>c();

			internal bool cctor>b__34_0(NetworkInterface nic)
			{
				return nic.OperationalStatus == OperationalStatus.Up;
			}

			internal string cctor>b__34_1(NetworkInterface nic)
			{
				return nic.GetPhysicalAddress().ToString();
			}
		}

		private static string strHomeServer = "http://www.t2ll.com";

		private static byte[] KEY_64 = new byte[]
		{
			11,
			52,
			73,
			94,
			235,
			96,
			237,
			88
		};

		private static byte[] IV_64 = new byte[]
		{
			45,
			47,
			68,
			55,
			50,
			235,
			29,
			31
		};

		private static DateTime lastServerFetch = DateTime.Now;

		private static List<AppRegistry.ConfigObject> configList = new List<AppRegistry.ConfigObject>();

		private static string macAddr = NetworkInterface.GetAllNetworkInterfaces().Where(new Func<NetworkInterface, bool>(AppRegistry.<>c.<>9.<.cctor>b__34_0)).Select(new Func<NetworkInterface, string>(AppRegistry.<>c.<>9.<.cctor>b__34_1)).FirstOrDefault<string>();

		public ApplicationManager appManager
		{
			get;
			set;
		}

		public static string GetMachineID()
		{
			string text = string.Empty;
			string empty = string.Empty;
			RegistryKey rootKey = AppRegistry.GetRootKey(ref empty);
			AppRegistry.GetValue(rootKey, "local_MachineID", ref text, ref empty);
			if (string.IsNullOrEmpty(text))
			{
				text = AppRegistry.macAddr;
				if (string.IsNullOrEmpty(text))
				{
					text = Guid.NewGuid().ToString();
				}
				AppRegistry.SaveValue(rootKey, "local_MachineID", text, ref empty, false, RegistryValueKind.Unknown);
			}
			return text;
		}

		public static bool SyncServer(ref string strItemsUpdated, string strPhoneNumber)
		{
			bool result;
			try
			{
				string a = "";
				bool flag = false;
				AppRegistry.GetValue(AppRegistry.GetRootKey(ref a), "local_DisableRemoteFeatureUpdates", ref flag, ref a);
				if (!flag)
				{
					string value = string.Empty;
					HttpWebRequest expr_73 = (HttpWebRequest)WebRequest.Create(string.Concat(new string[]
					{
						AppRegistry.strHomeServer,
						"/api/MachineProfile/getmachinesettings?machineId=",
						AppRegistry.GetMachineID(),
						"&winUser=",
						Environment.UserName,
						"&phoneNumber=",
						strPhoneNumber
					}));
					expr_73.AutomaticDecompression = DecompressionMethods.GZip;
					expr_73.Timeout = 10000;
					using (HttpWebResponse httpWebResponse = (HttpWebResponse)expr_73.GetResponse())
					{
						using (Stream responseStream = httpWebResponse.GetResponseStream())
						{
							using (StreamReader streamReader = new StreamReader(responseStream))
							{
								value = streamReader.ReadToEnd();
							}
						}
					}
					AppRegistry.configList = JsonConvert.DeserializeObject<List<AppRegistry.ConfigObject>>(value);
					foreach (AppRegistry.ConfigObject current in AppRegistry.configList)
					{
						if (current.key.Substring(0, 6) != "local_" && current.key != "UserName" && current.key != "Password" && current.phoneNumber == strPhoneNumber)
						{
							RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(current.regKey.Replace("HKEY_CURRENT_USER\\", ""), true);
							object value2 = registryKey.GetValue(current.key);
							RegistryValueKind valueKind = registryKey.GetValueKind(current.key);
							if (value2 != null)
							{
								string arg_1C2_0 = value2.ToString();
								string text = (current.propertyValue == null) ? "" : current.propertyValue.ToString();
								if (arg_1C2_0 != text)
								{
									a = "";
									AppRegistry.SaveValue(registryKey, current.key, text, ref a, false, valueKind);
									if (!(a != ""))
									{
										strItemsUpdated = string.Concat(new string[]
										{
											strItemsUpdated,
											current.key,
											" has been changed to: ",
											text,
											"\r\n"
										});
									}
								}
							}
							registryKey.Close();
						}
					}
				}
				result = true;
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		public static bool AuthorizeUser(string PhoneNumber, ref string ErrorMessage)
		{
			bool result = true;
			try
			{
				string a = string.Empty;
				HttpWebRequest expr_22 = (HttpWebRequest)WebRequest.Create(AppRegistry.strHomeServer + "/api/MachineProfile/ValidateIP?phoneNumber=" + PhoneNumber);
				expr_22.AutomaticDecompression = DecompressionMethods.GZip;
				expr_22.Timeout = 10000;
				using (HttpWebResponse httpWebResponse = (HttpWebResponse)expr_22.GetResponse())
				{
					using (Stream responseStream = httpWebResponse.GetResponseStream())
					{
						using (StreamReader streamReader = new StreamReader(responseStream))
						{
							a = streamReader.ReadToEnd();
						}
					}
				}
				if (a == "false")
				{
					result = false;
				}
			}
			catch (Exception)
			{
			}
			return result;
		}

		private static object ContactConfigAPI(AppRegistry.ApiAction action, RegistryKey RegKey, string Key, object Value)
		{
			string empty = string.Empty;
			if (action == AppRegistry.ApiAction.setValue)
			{
				string text;
				if (RegKey.ToString().EndsWith("Text App"))
				{
					text = AppRegistry.GetUserName(AppRegistry.GetRootKey(ref empty), ref empty);
				}
				else
				{
					text = null;
				}
				string arg_52_0 = string.Empty;
				HttpWebRequest expr_D4 = (HttpWebRequest)WebRequest.Create(string.Concat(new object[]
				{
					AppRegistry.strHomeServer,
					"/api/MachineProfile/updatemachinesetting?machineId=",
					AppRegistry.GetMachineID(),
					"&regKey=",
					RegKey,
					"&key=",
					Key,
					"&propertyvalue=",
					Value.ToString(),
					"&phonenumber=",
					text,
					"&winUser=",
					Environment.UserName
				}));
				expr_D4.AutomaticDecompression = DecompressionMethods.GZip;
				expr_D4.Timeout = 10000;
				using (HttpWebResponse httpWebResponse = (HttpWebResponse)expr_D4.GetResponse())
				{
					using (Stream responseStream = httpWebResponse.GetResponseStream())
					{
						using (StreamReader streamReader = new StreamReader(responseStream))
						{
							streamReader.ReadToEnd();
						}
					}
				}
				if ((from i in AppRegistry.configList
				where i.regKey == RegKey.Name && i.key == Key
				select i).FirstOrDefault<AppRegistry.ConfigObject>() == null)
				{
					AppRegistry.configList.Add(new AppRegistry.ConfigObject
					{
						key = Key,
						regKey = RegKey.Name,
						propertyValue = Value
					});
				}
				else
				{
					(from i in AppRegistry.configList
					where i.regKey == RegKey.Name && i.key == Key
					select i).First<AppRegistry.ConfigObject>().propertyValue = Value;
				}
			}
			else if ((from i in AppRegistry.configList
			where i.regKey == RegKey.Name && i.key == Key
			select i).FirstOrDefault<AppRegistry.ConfigObject>() == null)
			{
				if ((from i in AppRegistry.configList
				where i.regKey == RegKey.Name && i.key == Key
				select i).FirstOrDefault<AppRegistry.ConfigObject>() == null)
				{
					AppRegistry.configList.Add(new AppRegistry.ConfigObject
					{
						key = Key,
						regKey = RegKey.Name,
						propertyValue = Value
					});
				}
				else
				{
					(from i in AppRegistry.configList
					where i.regKey == RegKey.Name && i.key == Key
					select i).First<AppRegistry.ConfigObject>().propertyValue = Value;
				}
			}
			return Value;
		}

		public static RegistryKey GetRootKey(ref string ErrorMessage)
		{
			RegistryKey registryKey = Registry.CurrentUser;
			try
			{
				Assembly entryAssembly = Assembly.GetEntryAssembly();
				AssemblyCompanyAttribute assemblyCompanyAttribute = (AssemblyCompanyAttribute)AppRegistry.GetAttribute(entryAssembly, typeof(AssemblyCompanyAttribute));
				AssemblyTitleAttribute assemblyTitleAttribute = (AssemblyTitleAttribute)AppRegistry.GetAttribute(entryAssembly, typeof(AssemblyTitleAttribute));
				string arg = (assemblyTitleAttribute != null) ? assemblyTitleAttribute.Title : entryAssembly.GetName().Name;
				string arg2 = (assemblyCompanyAttribute != null) ? assemblyCompanyAttribute.Company : "";
				string text = string.Format("Software\\\\{0}\\\\{1}", arg2, arg);
				RegistryKey registryKey2 = registryKey.OpenSubKey(text, true);
				if (registryKey2 == null)
				{
					registryKey = registryKey.CreateSubKey(text);
				}
				else
				{
					registryKey = registryKey2;
				}
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}
			return registryKey;
		}

		public static RegistryKey GetSubKey(RegistryKey RegKey, string SubKey, bool Create, ref string ErrorMessage)
		{
			RegistryKey registryKey = null;
			try
			{
				if (RegKey == null)
				{
					RegKey = AppRegistry.GetRootKey(ref ErrorMessage);
				}
				if (RegKey != null)
				{
					registryKey = RegKey.OpenSubKey(SubKey, true);
					if (registryKey == null & Create)
					{
						registryKey = RegKey.CreateSubKey(SubKey);
					}
					else if (registryKey == null)
					{
						ErrorMessage = AppRegistry.Append(ErrorMessage, SubKey + " was not found.", "\r\n");
					}
				}
			}
			catch (Exception ex)
			{
				ErrorMessage = AppRegistry.Append(ErrorMessage, "Error accessing key " + SubKey + ": " + ex.Message, "\r\n");
			}
			return registryKey;
		}

		public static string[] GetSubKeyNames(RegistryKey RegKey, string SubKey, ref string ErrorMessage)
		{
			string[] result = null;
			string text = string.Empty;
			try
			{
				if (SubKey != null)
				{
					RegistryKey subKey = AppRegistry.GetSubKey(RegKey, SubKey, false, ref text);
					if (subKey == null)
					{
						text = AppRegistry.Append(text, "Failed to open subkey.", "\r\n");
					}
					else
					{
						result = subKey.GetSubKeyNames();
					}
				}
				else
				{
					result = RegKey.GetSubKeyNames();
				}
			}
			catch (SecurityException ex)
			{
				text = AppRegistry.Append(text, "Security exception: " + ex.Message + ".", "\r\n");
				string[] result2 = null;
				return result2;
			}
			catch (IOException ex2)
			{
				text = AppRegistry.Append(text, "IO exception: " + ex2.Message + ".", "\r\n");
				string[] result2 = null;
				return result2;
			}
			catch (Exception ex3)
			{
				text = AppRegistry.Append(text, "Exception: " + ex3.Message + ".", "\r\n");
			}
			if (text.Length > 0)
			{
				ErrorMessage = AppRegistry.Append(ErrorMessage, string.Concat(new string[]
				{
					"Error getting the subkeys for ",
					RegKey.ToString(),
					(SubKey.Length > 0) ? ("//" + SubKey) : "",
					". ",
					text,
					"."
				}), "\r\n");
			}
			return result;
		}

		public static string[] GetValueNames(RegistryKey RegKey, string SubKey, ref string ErrorMessage)
		{
			string text = string.Empty;
			string[] result = null;
			try
			{
				if (SubKey.Length > 0)
				{
					RegistryKey subKey = AppRegistry.GetSubKey(RegKey, SubKey, false, ref text);
					if (subKey == null)
					{
						text = AppRegistry.Append(text, "Failed to open subkey", "\r\n");
					}
					else
					{
						result = subKey.GetValueNames();
					}
				}
				else
				{
					result = RegKey.GetValueNames();
				}
			}
			catch (SecurityException ex)
			{
				text = AppRegistry.Append(text, "Security exception: " + ex.Message, "\r\n");
				string[] result2 = null;
				return result2;
			}
			catch (IOException ex2)
			{
				text = AppRegistry.Append(text, "IO exception: " + ex2.Message, "\r\n");
				string[] result2 = null;
				return result2;
			}
			catch (Exception ex3)
			{
				text = AppRegistry.Append(text, "Exception: " + ex3.Message, "\r\n");
			}
			if (text.Length > 0)
			{
				ErrorMessage = AppRegistry.Append(ErrorMessage, string.Concat(new string[]
				{
					"Error getting the subkeys for ",
					RegKey.ToString(),
					(SubKey.Length > 0) ? ("//" + SubKey) : "",
					". ",
					text,
					"."
				}), "\r\n");
			}
			return result;
		}

		public static bool GetValue(RegistryKey RegKey, string Key, ref bool Value, ref string ErrorMessage)
		{
			string text = string.Empty;
			object obj = null;
			try
			{
				if (AppRegistry.GetValue(RegKey, Key, ref obj, ref text) && obj != null)
				{
					Value = (Convert.ToInt16(obj) != 0);
				}
			}
			catch (Exception ex)
			{
				text = AppRegistry.Append(text, "Error getting registry Key: " + Key + " Error: " + ex.Message, "\r\n");
			}
			if (text.Length > 0)
			{
				ErrorMessage = AppRegistry.Append(ErrorMessage, text, "\r\n");
				return false;
			}
			return true;
		}

		public static bool GetValue(RegistryKey RegKey, string Key, ref string Value, ref string ErrorMessage)
		{
			string text = string.Empty;
			object obj = null;
			Value = string.Empty;
			try
			{
				if (AppRegistry.GetValue(RegKey, Key, ref obj, ref text) && obj != null)
				{
					Value = (string)obj;
					if (RegKey != null && RegKey.Name.EndsWith("EncryptedData"))
					{
						Value = AppRegistry.Decrypt(Value, ref text);
					}
				}
			}
			catch (Exception ex)
			{
				text = AppRegistry.Append(text, "Error getting registry Key: " + Key + " Error: " + ex.Message, "\r\n");
			}
			if (text.Length > 0)
			{
				ErrorMessage = AppRegistry.Append(ErrorMessage, text, "\r\n");
				return false;
			}
			return true;
		}

		public static bool GetValue(RegistryKey RegKey, string Key, ref int Value, ref string ErrorMessage)
		{
			string text = string.Empty;
			object obj = null;
			try
			{
				if (AppRegistry.GetValue(RegKey, Key, ref obj, ref text) && obj != null)
				{
					Value = (int)obj;
				}
			}
			catch (Exception ex)
			{
				text = AppRegistry.Append(text, "Error getting registry Key: " + Key + " Error: " + ex.Message, "\r\n");
			}
			if (text.Length > 0)
			{
				ErrorMessage = AppRegistry.Append(ErrorMessage, text, "\r\n");
				return false;
			}
			return true;
		}

		public static bool GetValue(RegistryKey RegKey, string Key, ref object Value, ref string ErrorMessage)
		{
			bool result = false;
			try
			{
				Value = null;
				if (RegKey == null)
				{
					RegKey = AppRegistry.GetRootKey(ref ErrorMessage);
				}
				if (RegKey != null)
				{
					Value = RegKey.GetValue(Key);
					result = true;
				}
				else
				{
					ErrorMessage = AppRegistry.Append(ErrorMessage, "Invalid root registry key", "\r\n");
				}
			}
			catch (Exception ex)
			{
				ErrorMessage = AppRegistry.Append(ErrorMessage, ex.Message, "\r\n");
				Value = null;
			}
			return result;
		}

		public static bool SaveValue(RegistryKey RegKey, string Key, object Value, ref string ErrorMessage, bool callApi = false, RegistryValueKind rkKind = RegistryValueKind.Unknown)
		{
			string text = string.Empty;
			try
			{
				if (RegKey == null)
				{
					RegKey = AppRegistry.GetRootKey(ref text);
				}
				if (RegKey != null)
				{
					int num = Key.LastIndexOf('\\');
					if (num > -1)
					{
						string subKey = Key.Substring(0, num);
						RegKey = AppRegistry.GetSubKey(RegKey, subKey, true, ref text);
						if (RegKey == null)
						{
							ErrorMessage = AppRegistry.Append(ErrorMessage, "Failed to create registry sub-key", "\r\n");
							bool result = false;
							return result;
						}
						Key = Key.Substring(num + 1);
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
							Value = (((bool)Value) ? 1 : 0);
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
							AppRegistry.ContactConfigAPI(AppRegistry.ApiAction.setValue, RegKey, Key, Value);
						}
						catch (Exception)
						{
						}
					}
					RegKey.SetValue(Key, Value, rkKind);
				}
				else
				{
					text = AppRegistry.Append(text, "Invalid root registry key", "\r\n");
				}
			}
			catch (Exception ex)
			{
				text = AppRegistry.Append(text, ex.Message, ex.StackTrace);
				Value = null;
			}
			if (text.Length > 0)
			{
				ErrorMessage = AppRegistry.Append(ErrorMessage, text, "\r\n");
				return false;
			}
			return true;
		}

		public static string GetPassword(RegistryKey Key, ref string ErrorMessage)
		{
			string text = string.Empty;
			AppRegistry.GetValue(Key, "Password", ref text, ref ErrorMessage);
			if (text.Length > 0)
			{
				text = AppRegistry.Decrypt(text, ref ErrorMessage);
			}
			return text;
		}

		public static bool SavePassword(RegistryKey Key, string Password, ref string ErrorMessage)
		{
			string empty = string.Empty;
			string value = AppRegistry.Encrypt(Password, ref empty);
			if (empty.Length == 0)
			{
				return AppRegistry.SaveValue(Key, "Password", value, ref ErrorMessage, false, RegistryValueKind.Unknown);
			}
			ErrorMessage = AppRegistry.Append(ErrorMessage, empty, "\r\n");
			return false;
		}

		public static string GetUserName(RegistryKey Key, ref string ErrorMessage)
		{
			string empty = string.Empty;
			AppRegistry.GetValue(Key, "UserName", ref empty, ref ErrorMessage);
			return empty;
		}

		public static bool SaveUserName(RegistryKey Key, string UserName, ref string ErrorMessage)
		{
			return AppRegistry.SaveValue(Key, "UserName", UserName, ref ErrorMessage, false, RegistryValueKind.Unknown);
		}

		public static string Encrypt(string Data, ref string ErrorMessage)
		{
			string result = string.Empty;
			try
			{
				DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
				MemoryStream memoryStream = new MemoryStream();
				CryptoStream expr_29 = new CryptoStream(memoryStream, dESCryptoServiceProvider.CreateEncryptor(AppRegistry.KEY_64, AppRegistry.IV_64), CryptoStreamMode.Write);
				StreamWriter expr_2F = new StreamWriter(expr_29);
				expr_2F.Write(Data);
				expr_2F.Flush();
				expr_29.FlushFinalBlock();
				memoryStream.Flush();
				result = Convert.ToBase64String(memoryStream.GetBuffer(), 0, Convert.ToInt32(memoryStream.Length));
			}
			catch (Exception ex)
			{
				ErrorMessage = AppRegistry.Append(ErrorMessage, ex.Message, "\r\n");
			}
			return result;
		}

		public static string Decrypt(string Data, ref string ErrorMessage)
		{
			if (Data.Length == 0)
			{
				return Data;
			}
			string result = string.Empty;
			try
			{
				DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
				result = new StreamReader(new CryptoStream(new MemoryStream(Convert.FromBase64String(Data)), dESCryptoServiceProvider.CreateDecryptor(AppRegistry.KEY_64, AppRegistry.IV_64), CryptoStreamMode.Read)).ReadToEnd();
			}
			catch (Exception ex)
			{
				ErrorMessage = AppRegistry.Append(ErrorMessage, ex.Message, "\r\n");
			}
			return result;
		}

		public static string Append(string Source, string Append, string Separator = "\r\n")
		{
			if (Source.Length > 0)
			{
				Source += Separator;
			}
			return Source + Append;
		}

		private static Attribute GetAttribute(Assembly assembly, Type attributeType)
		{
			object[] customAttributes = assembly.GetCustomAttributes(attributeType, false);
			if (customAttributes.Length == 0)
			{
				return null;
			}
			return (Attribute)customAttributes[0];
		}
	}
}
