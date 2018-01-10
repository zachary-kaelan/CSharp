using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Management.Instrumentation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Bluetooth.Factory;
using InTheHand.Net.Bluetooth.Widcomm;
using InTheHand.Net.Sockets;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Remoting.Messaging;

namespace BlueSharp
{
    class REClient
    {
        const string BluetoothPath = @"G:\Bluetooth\";
        public static List<BluetoothDeviceInfo> deviceList = new List<BluetoothDeviceInfo>();
        public static MemoryStream ms = new MemoryStream();
        public const string DEVICE_PIN = "0000";
        private Thread dataThread { get; set; }

        BluetoothAddress mac { get; set; }
        BluetoothEndPoint localEP { get; set; }
        BluetoothClient localClient { get; set; }
        BluetoothComponent localComp { get; set; }
        static NetworkStream stream { get; set; }

        public REClient()
        {
            mac = BluetoothAddress.Parse("AC:2B:6E:AC:A7:E1");
            localEP = new BluetoothEndPoint(mac, BluetoothService.SerialPort);
            localClient = new BluetoothClient(localEP);
            localComp = new BluetoothComponent(localClient);
        }

        public static string FindMACAddr()
        {
            ManagementClass mgmt = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objCol = mgmt.GetInstances();
            string address = string.Empty;
            var description = string.Empty;

            foreach(var obj in objCol)
            {
                if (String.IsNullOrEmpty(address))
                {
                    if ((bool)obj["IPEnabled"])
                    {
                        address = obj["MacAddress"].ToString();
                        description = obj["Description"].ToString();
                    }
                }
                obj.Dispose();
            }

            return address;
        }

        public void Scan()
        {
            //BluetoothAddress bandAddr = new BluetoothAddress(264847086161219);

            localComp.DiscoverDevicesAsync(255, true, true, true, true, null);
            localComp.DiscoverDevicesProgress += component_DiscoverDevicesProgress;
            localComp.DiscoverDevicesComplete += component_DiscoverDevicesComplete;
        }

        public void Pair()
        {
            var paired = localClient.DiscoverDevices(255, false, true, false, false);

            paired.ToList().ForEach(dev =>
                Console.WriteLine(
                    dev.DeviceName +
                    " (" + dev.DeviceAddress +
                    "): Device is " +
                    (dev.Remembered ? "known." : "unknown.")
                )
            );

            var pairedDevices = deviceList.Select(d =>
                paired.Any(
                    p => d.Equals(p)
                ) ? BluetoothSecurity.PairRequest(
                    d.DeviceAddress, DEVICE_PIN)
                    : false
            );
        }

        public void Connect(BluetoothDeviceInfo device)
        {
            if (device.Authenticated)
            {
                localClient.SetPin(DEVICE_PIN);
                localClient.BeginConnect(
                    device.DeviceAddress, 
                    BluetoothService.HealthDevice, 
                    new AsyncCallback(Device_Connected), 
                    device
                );
            }

            Console.WriteLine("Device Name: " + localClient.GetRemoteMachineName(device.DeviceAddress));
            //Console.WriteLine("Device Name: " + localClient.GetRemoteMachineName(device.DeviceAddress));
        }

        public void Device_Connected(IAsyncResult result)
        {
            localClient.EndConnect(result);
            if (result.IsCompleted)
            {
                Console.WriteLine("Device is connected!");
                stream = localClient.GetStream();
                dataThread = new Thread(LogData);
            }
            //localClient.EndConnect(result);
        }

        public static void GetDevices()
        {
            BluetoothClient client = new BluetoothClient();

            var devices = client.DiscoverDevices(25, true, true, true, false);
            int deviceCount = 0;
            foreach(var device in devices)
            {
                device.Update();
                string devicePath = BluetoothPath + device.DeviceName + @"\";
                {
                    try
                    {
                        if (!Directory.Exists(devicePath))
                            Directory.CreateDirectory(devicePath);
                    }
                    catch
                    {
                        devicePath = BluetoothPath + "Device" + (deviceCount > 0 ? " - " + deviceCount.ToString() : "") + @"\";
                        if (!Directory.Exists(devicePath))
                            Directory.CreateDirectory(devicePath);
                    }
                }
                

                List<string> basicInfo = new List<string>();
                basicInfo.Add("Authenticated \t=> " + device.Authenticated);
                basicInfo.Add("Connected \t=> " + device.Connected);
                basicInfo.Add("Address \t=> " + device.DeviceAddress);
                basicInfo.Add("Last Seen \t=> " + device.LastSeen.ToString());
                basicInfo.Add("Last Used \t=> " + device.LastUsed.ToString());
                basicInfo.Add("Remembered \t=> " + device.Remembered);
                basicInfo.Add("");
                basicInfo.Add("Class:");
                basicInfo.AddRange(
                    device.ClassOfDevice.GetType().GetProperties().Select(p =>
                        "    " + p.Name + " \t=> " + p.GetValue(device.ClassOfDevice)
                        )
                );

                File.WriteAllLines(devicePath + "BasicInfo.txt", basicInfo);

                int count = 0;
                foreach(var service in device.InstalledServices)
                {
                    
                    List<string> serviceInfo = new List<string>();
                    serviceInfo.Add("ID \t=> " + service.ToString());
                    try
                    {
                        var records = device.GetServiceRecords(service);
                        foreach (var record in records)
                        {
                            serviceInfo.Add("");
                            foreach (var attr in record)
                            {
                                serviceInfo.Add(attr.Id.ToString());
                                var val = attr.Value;
                                serviceInfo.Add("    Value \t=> " + val.Value.ToString());
                                serviceInfo.Add("    UUID \t=> " + val.GetValueAsUuid().ToString());
                                serviceInfo.Add("    Type \t=> " + val.ElementType.ToString());
                                serviceInfo.Add("    Descriptor \t=> " + val.ElementTypeDescriptor.ToString());
                            }
                        }
                        
                    }
                    catch (Exception e)
                    {
                        File.WriteAllLines(devicePath + "SERVICE_ERROR.txt",
                            e.GetType().GetProperties().Select(p =>
                                String.Format("{0} \t=> {1}", p.Name, p.GetValue(e))
                            )
                        );
                    }
                    finally
                    {
                        File.WriteAllLines(devicePath + "Service" + (count > 0 ? " - " + count.ToString() : ""), serviceInfo);
                    }
                }
            }
        }

        private static void component_DiscoverDevicesProgress(object sender, DiscoverDevicesEventArgs e)
        {
            e.Devices.ToList().ForEach(dev =>
                Console.WriteLine(
                    dev.DeviceName + 
                    " (" + dev.DeviceAddress + 
                    "): Device is " + 
                    (dev.Remembered ? "known." : "unknown.")
                )
            );

            deviceList.AddRange(e.Devices);
        }

        private static void component_DiscoverDevicesComplete(object sender, DiscoverDevicesEventArgs e)
        {
            
        }

        private static void LogData()
        {
            const int BUFFER_SIZE = 1024;
            byte[] buffer;
            while (true)
            {
                SpinWait.SpinUntil(() => stream.DataAvailable);
                Thread.Sleep(250);
                buffer = new byte[BUFFER_SIZE];
                stream.Read(buffer, 0, BUFFER_SIZE);
                ms.Write(buffer, 0, 1024);
            }
        }
    }
}
