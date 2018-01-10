using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RoombaTest
{
    public class Roomba : IDisposable
    {
        private SerialPort IO;

        private bool IsValid()
        {
            return IO.IsOpen;
        }

        public bool TryToConnect()
        {
            // Get all active ports in your PC as a string array
            var ports = SerialPort.GetPortNames();
            foreach (var port in ports)
            {
                var isPortSet = SetPort(port);
                if (isPortSet)
                {
                    return true;
                }
            }
            return false;
        }

        private bool SetPort(string portNum)
        {
            try
            {
                if (IO != null)
                {
                    IO.Close(); //Just in case port is already taken
                }
                IO = new SerialPort(portNum, 57600, Parity.None, 8, StopBits.One);
                IO.DtrEnable = false;
                IO.Handshake = Handshake.None;
                IO.RtsEnable = false;

                IO.Open();

                // Every stream of commands must start with byte 128
                IO.DataReceived += IO_DataReceived;
                return SendCommand(new byte[] { 128 });
            }
            catch
            {
                portNum = String.Empty;
                IO.Close();
                return false;
            }
        }

        public void LEDsBlinking()
        {
            int sleepTime = 500;
            int numBlinks = 1;
            byte[] off = new byte[] { 128, 139, 0, 0, 0 };

            for (int numIterations = 0; numIterations < 8; ++numIterations)
            {
                int shortSleepTime = (int)((Math.Sqrt(sleepTime) * 2) / Math.Sqrt(numBlinks));
                for (byte bits = 1; bits <= 14; ++bits)
                {
                    byte[] on = new byte[] { 128, 139, bits, (byte)(byte.MaxValue / bits), (byte)(byte.MaxValue - (byte.MaxValue / bits)) };
                    for (int i = 0; i < numBlinks; ++i)
                    {
                        SendCommand(on);
                        Thread.Sleep(shortSleepTime);
                        SendCommand(off);
                        Thread.Sleep(shortSleepTime);
                    }
                    Thread.Sleep(shortSleepTime);
                }

                Thread.Sleep(sleepTime);
                sleepTime += 50;
                ++numBlinks;
            }
        }

        private void IO_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var numOfBytes = IO.BytesToRead;
            byte[] sensorsData = new byte[numOfBytes];
            IO.Read(sensorsData, 0, numOfBytes);
            Console.WriteLine("[" + String.Join("][", sensorsData.Select(b => b.ToString())) + "]");
            //set sensors…
        }

        private bool SendCommand(IEnumerable<byte> commandCollection)
        {
            try
            {
                var commandArr = commandCollection.ToArray();
                IO.Write(commandArr, 0, commandArr.Length);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Dispose()
        {
            IO.Dispose();
            IO.Close();
            IO = null;
        }

        private static IEnumerable<byte> DecimalToHighLowBytes(int decimalNum)
        {
            byte highByte = (byte)(decimalNum >> 8);
            byte lowByte = (byte)(decimalNum & 255);
            var commands = new List<byte>() { highByte, lowByte };
            return commands;
        }

        private static int UnsignedHighLowBytesToDecimal(byte highByte, byte lowByte)
        {
            return 256 * highByte + lowByte;
        }

        private static int SignedHighLowBytesToDecimal(byte highByte, byte lowByte)
        {
            uint u = (uint)highByte << 8 | lowByte;
            int num = (int)(u >= (1u << 15) ? u - (1u << 16) : u);
            return num;
        }
    }
}
