using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using Windows.Devices;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Storage;
using Windows.Storage.Streams;

namespace BlueConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceOp = GattDeviceService.FromIdAsync("08590F7E-DB05-467E-8757-72F6FAEB13D5");
            SpinWait.SpinUntil(() => serviceOp.Status == AsyncStatus.Completed);
            GattDeviceService service = serviceOp.GetResults();
            var serviceOp2 = service.GetCharacteristicsForUuidAsync(new Guid("08590F7E-DB05-467E-8757-72F6FAEB13F5"));
            SpinWait.SpinUntil(() => serviceOp2.Status == AsyncStatus.Completed);
            GattCharacteristic chr = serviceOp2.GetResults().Characteristics.First();
            chr.ValueChanged += Chr_ValueChanged;

            /*BluetoothLEAdvertisementWatcher activeWatcher = new BluetoothLEAdvertisementWatcher();
            activeWatcher.ScanningMode = BluetoothLEScanningMode.Active;
            activeWatcher.Received += ActiveWatcher_Received;

            BluetoothLEAdvertisementWatcher passiveWatcher = new BluetoothLEAdvertisementWatcher();
            passiveWatcher.ScanningMode = BluetoothLEScanningMode.Passive;*/


        }

        private static void Chr_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            DataReader reader = DataReader.FromBuffer(args.CharacteristicValue);
            var loadOp = reader.LoadAsync(args.CharacteristicValue.Length).GetResults();
            StringBuilder sb = new StringBuilder(args.Timestamp.ToString("[HH:mm:ss]\n"));
            while (reader.UnconsumedBufferLength > 0)
            {
                uint bytesToRead = reader.ReadUInt32();
                sb.Append("\t");
                sb.AppendLine(reader.ReadString(bytesToRead));
            }
            sb.AppendLine("END TRANSMISSION\n");
            Console.WriteLine(sb.ToString());
            System.IO.File.AppendAllText(@"E:\Bluetooth\log.txt", sb.ToString());
        }

        private static void ActiveWatcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            throw new NotImplementedException();
        }


    }
}
