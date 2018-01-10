using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
    class Device
    {
        private Dictionary<string, KeyValuePair<BluetoothClient, int>> clients { get; set; }
        public static ConcurrentQueue<KeyValuePair<string, byte[]>> data { get; set; }
        private Thread[] threads { get; set; }

        public Device(string address, List<Tuple<string, string, int>> services)
        {
            this.clients = new Dictionary<string, KeyValuePair<BluetoothClient, int>>();
            data = new ConcurrentQueue<KeyValuePair<string, byte[]>>();
            foreach(var service in services)
            {
                BluetoothListener l = new BluetoothListener(new Guid(service.Item2));
                l.Start();
                this.clients.Add(
                    service.Item1,
                    new KeyValuePair<BluetoothClient, int>(
                        l.AcceptBluetoothClient(),
                        service.Item3
                    )
                );
                
            }

            /*
            this.clients = services.Select(s =>
                new KeyValuePair<string, KeyValuePair<BluetoothClient, int>>(
                    s.Item1,
                    new KeyValuePair<BluetoothClient, int>(
                        new BluetoothClient(
                            new BluetoothEndPoint(
                                BluetoothAddress.Parse(address),
                                new Guid(s.Item2)
                            )
                        ), s.Item3
                    )
            )).ToDictionary(kv => kv.Key, kv => kv.Value);*/

            this.threads = clients.Select(c => 
                new Thread(() => Listen(c.Key, c.Value.Key, c.Value.Value))
            ).ToArray();

        }

        public static void Listen(string name, BluetoothClient client, int expected)
        {
            var stream = client.GetStream();
            byte[] buffer;
            while (true)
            {
                int total = 0;
                buffer = new byte[expected];
                while (total < expected)
                {
                    total += stream.Read(buffer, total, expected - total);
                    Thread.Sleep(250);
                }
                data.Enqueue(new KeyValuePair<string, byte[]>(name, buffer));
            }
        }
    }
}
