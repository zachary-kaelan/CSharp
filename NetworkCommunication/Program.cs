using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkCommunication
{
    class Program
    {
        private static int port = 2222;
        private static Socket socket;
        static byte[] receiveBuffer = new byte[4096];

        static void Main(string[] args)
        {
            Console.Write("Client or Server? ");
            if (Console.ReadLine() == "Server")
            {
                Console.WriteLine("Starting Server...");
                Server();
            }
            else
            {
                Console.WriteLine("Starting Client...");
                Client();
            }
            Console.WriteLine("FINISHED");
            Console.ReadLine();
        }

        public static void Log(string message)
        {
            Console.WriteLine(
                "[{0}]\t{1}",
                DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss\.fff"),
                message
            );
        }

        public static void Server()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            SpinWait.SpinUntil(listener.Pending);
            try
            {
                socket = listener.AcceptSocket();
                if (socket.Connected)
                    Console.WriteLine("Client:\t" + socket.RemoteEndPoint.ToString());
                while(socket.Connected)
                {
                    int length = socket.Receive(receiveBuffer);
                    if (length > 0)
                        Log(Encoding.Unicode.GetString(receiveBuffer));
                }
            }
            catch (Exception e)
            {
                Log("ERROR ~ " + e.Message);
            }
        }

        public static void Client()
        {
            byte[] sendBuffer = new byte[4096];
            TcpClient client = new TcpClient();
            try
            {
                client.Connect("192.168.1.67", port);
                if (client.Connected)
                    Console.WriteLine("Connected...");
                ConsoleKeyInfo key = new ConsoleKeyInfo('s', ConsoleKey.S, false, false, false);
                bool established = false;
                NetworkStream nts = client.GetStream();
                do
                {
                    if (!established)
                    {
                        established = true;
                        sendBuffer = Encoding.Unicode.GetBytes("Connection Established");
                        if (nts.CanWrite)
                        {
                            nts.Write(sendBuffer, 0, sendBuffer.Length);
                            Log("Message Sent ~ Connection Established");
                        }
                    }
                    else
                    {
                        Console.Write("Send or Receive? (s/r): ");
                        key = Console.ReadKey();

                        if (key.KeyChar == 's' && nts.CanWrite)
                        {
                            Console.Write("Message: ");
                            string message = Console.ReadLine();
                            sendBuffer = Encoding.Unicode.GetBytes(message);
                            nts.Write(sendBuffer, 0, sendBuffer.Length);
                            Log("Message Sent ~ " + message);
                        }
                        else if (key.KeyChar == 'r')
                        {
                            Console.Write("Timeout: ");
                            int timeout = Convert.ToInt32(Console.ReadLine().Trim());
                            SpinWait.SpinUntil(() => client.Available > 0);
                            string message = "";
                            while (nts.Read(receiveBuffer, 0, receiveBuffer.Length) != 0)
                                message += Encoding.Unicode.GetString(receiveBuffer);
                            Log("Message Received ~ " + message);
                        }
                    }

                    Thread.Sleep(5000);
                }
                while (key.KeyChar != 'q');
            }
            catch (Exception e)
            {
                Log("ERROR ~ " + e.Message);
            }
        }
    }
}
