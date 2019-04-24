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
    class Program
    {

        static void Main(string[] args)
        {
            Roomba roomba = new Roomba();
            Console.WriteLine("Connected: " + roomba.TryToConnect("COM3"));
            Console.WriteLine("Sensor Query: " + roomba.SendCommand(true, 128, 149, 7, 21, 22, 23, 24, 25, 26, 27, 34, 35, 58));
            Console.ReadLine();
            roomba.Dispose();
        }
    }
}
