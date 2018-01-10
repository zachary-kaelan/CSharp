using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Bluetooth.AttributeIds;
using InTheHand.Net.Bluetooth.Widcomm;
using InTheHand.Net.Sockets;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Remoting.Messaging;

namespace BlueSharp
{
    class Program
    {

        static void Main(string[] args)
        {
            BluetoothListener l = new BluetoothListener(BluetoothService.CreateBluetoothUuid(0xFEE0));
            l.Start();
        }

        /*
        static void Main(string[] args)
        {
            //Guid SERVICE_HEARTRATE = new Guid("0000180d-0000-1000-8000-00805f9b34fb");
            //Guid NOTIF_HEARTRATE = new Guid("00002a37-0000-1000-8000-00805f9b34fb");
            //Guid CHAR_HEARTRATE = new Guid("00002a39-0000-1000-8000-00805f9b34fb");
            //Guid DESC_UPD_NOTIF = new Guid("00002902-0000-1000-8000-00805f9b34fb");

            //REClient.GetDevices();

            Device band = new Device("F0:E0:84:7D:8D:43", new List<Tuple<string, string, int>>() {
                new Tuple<string, string, int>(
                    "Battery",
                    "00000006-0000-3512-2118-0009af100700",
                    20
                ),
                new Tuple<string, string, int>(
                    "Steps",
                    "00000007-0000-3512-2118-0009af100700",
                    12
                ),
                new Tuple<string, string, int>(
                    "CustomVibrate",
                    "00001802-0000-1000-8000-00805f9b34b",
                    6
                ),
                new Tuple<string, string, int>(
                    "HeartRateService",
                    "0000180d-0000-1000-8000-00805f9b34fb",
                    1
                ),
                new Tuple<string, string, int>(
                    "NotificationDescriptor",
                    "00002902-0000-1000-8000-00805f9b34fb",
                    4
                ),
                new Tuple<string, string, int>(
                    "HeartRateUUID",
                    "00002a37-0000-1000-8000-00805f9b34fb",
                    1
                ),
                new Tuple<string, string, int>(
                    "HeartRate",
                    "00002a39-0000-1000-8000-00805f9b34fb",
                    2
                )
            });

            while (true)
            {
                Thread.Sleep(5000);
                while (Device.data.TryDequeue(out KeyValuePair<string, byte[]> data))
                {
                    Console.WriteLine(
                        data.Key + " : " + String.Join(
                            "\t", data.Value.Cast<int>().Select(
                                i => i.ToString()
                            )
                        )
                    );
                    Console.WriteLine("");
                }
            }


            /*
            BluetoothClient remoteDevice = null;
            List<BluetoothClient> clients = new List<BluetoothClient>();
            BluetoothListener l = new BluetoothListener(BluetoothAddress.Parse("AC:2B:6E:AC:A7:E1"), BluetoothService.SerialPort);
            l.Start();
            l.BeginAcceptBluetoothClient(new AsyncCallback(AcceptConnection), l);

            void AcceptConnection(IAsyncResult result)
            {
                if (result.IsCompleted)
                {
                    remoteDevice = ((BluetoothListener)result.AsyncState).EndAcceptBluetoothClient(result);
                    clients.Add(remoteDevice);
                    Console.WriteLine(remoteDevice.RemoteEndPoint.Address + " Connected.");
                    Console.WriteLine("    Link Policy: " + remoteDevice.LinkPolicy.ToString());
                    if (remoteDevice.RemoteEndPoint.Address != BluetoothAddress.Parse("F0:E0:84:7D:8D:43"))
                        l.BeginAcceptBluetoothClient(new AsyncCallback(AcceptConnection), l);
                }
                Console.WriteLine("Connected.");
            }

            SpinWait.SpinUntil(() => remoteDevice != null);

            var stream = remoteDevice.GetStream();

            const int BUFFER_SIZE = 4096;
            byte[] buffer;
            while (true)
            {
                Thread.Sleep(5000);
                buffer = new byte[BUFFER_SIZE];
                int read = stream.Read(buffer, 0, BUFFER_SIZE);
                Console.WriteLine("{0} bytes read...", read);
                Console.Write(buffer);
                //Console.OpenStandardOutput().Write(buffer, 0, read);
                Console.WriteLine();
            }
            */

            /*
            var local = BluetoothAddress.Parse("F0:E0:84:7D:8D:43");
            BluetoothDeviceInfo info = new BluetoothDeviceInfo(local);
            info.Update();

            REClient client = new REClient();
            Console.WriteLine();
            client.Scan();
            Console.WriteLine();
            client.Pair();
            Console.WriteLine();
            client.Connect(info);
            Console.WriteLine();

            while (true)
            {
                Thread.Sleep(5000);
                buffer = new byte[BUFFER_SIZE];
                int read = REClient.ms.Read(buffer, 0, BUFFER_SIZE);
                Console.WriteLine("{0} bytes read...", read);
                Console.OpenStandardOutput().Write(buffer, 0, read);
                Console.WriteLine();
            }
            */

            /*
            AsyncCallback newDevice = new AsyncCallback((result) =>
            {
                result.AsyncWaitHandle.WaitOne();
                AsyncResult res = (AsyncResult)result;
                res.
            });

            
            */

            /*
            BluetoothListener lsnr = new BluetoothListener(SERVICE_HEARTRATE);
            lsnr.Start();
            BluetoothClient HR_SERV = lsnr.AcceptBluetoothClient();
            NetworkStream str = HR_SERV.GetStream();
            Thread.Sleep(12000);
            FileStream fs = new FileStream(@"C:\Bluetooth\HR_SERV_DATA.txt", FileMode.Append, FileAccess.Write);
            str.CopyTo(fs);
            fs.Close();
            */
            
            /*
            BluetoothClient client = new BluetoothClient();

            var devices = client.DiscoverDevices();
            foreach (var device in devices)
            {
                Console.WriteLine(device.DeviceName + " - \t" + device.DeviceAddress + " - \t" + device.Connected);
                foreach (var guid in device.InstalledServices)
                {
                    Console.WriteLine("    " + guid.ToString());
                    try
                    {
                        foreach (var service in device.GetServiceRecords(guid))
                        {
                            Console.WriteLine("        Service Info:");
                            foreach (ServiceAttribute attr in service)
                            {
                                Console.WriteLine("            " + attr.Id + " - " + attr.Value);
                            }
                        }
                    }
                    catch
                    {
                        Console.WriteLine("        Service not found.");
                        continue;
                    }
                }
            }
            */
            
            
            //Console.Write("Press any key to exit...");
            //Console.ReadKey();
        //}
        
    }
}
