using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoombaTest
{
    class Program
    {

        static void Main(string[] args)
        {
            Roomba roomba = new Roomba();
            if (roomba.TryToConnect())
                roomba.LEDsBlinking();
            Console.ReadLine();
            roomba.Dispose();
        }
    }
}
