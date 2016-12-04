using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;


namespace clientpcss
{
    class Program
    {
        static void Main()
        {
            Client thisClient = new Client();
        }
    }

    public class Client : Game
    {
        private static readonly Socket ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream,
            ProtocolType.Tcp);

        static Thread thread;
        private const int PORT = 1234;
        private string request;

        public Client()
        {
            Console.Title = "Client";
            ConnectToServer();
            RequestLoop();
            Exit();
        }

        private void ConnectToServer()
        {
            while (!ClientSocket.Connected)
            {
                try
                {
                    ClientSocket.Connect(IPAddress.Loopback, PORT);
                }

                catch (SocketException)
                {
                    Console.Clear();
                }
            }

            Console.Clear();
            Console.WriteLine("Connected");
        }

        private void RequestLoop()
        {
            DisplayUI();

            while (true)
            {
                thread = new Thread(ReceiveResponse);
                thread.Start();
                SendRequest();
            }
        }

        private void Exit()
        {
            SendString("exit"); // Tell the server we are exiting
            ClientSocket.Shutdown(SocketShutdown.Both);
            ClientSocket.Close();
            Environment.Exit(0);
        }

        private void SendRequest()
        {
            if (!YouWon)
            {


                Console.WriteLine("Send a request: ");
            }
            // What is it possible to send?
            request = MessageDecodeSender();
            // this is possible because  MessageDecodeSender is a string method

            SendString(request);

            if (request.ToLower() == "exit")
            {
                Exit();
            }

        }

        public static void SendString(string text)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(text);
            ClientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);
        }

        private void ReceiveResponse()
        {
            byte[] buffer = new byte[2048];
            int received = ClientSocket.Receive(buffer, SocketFlags.None);
            if (received == 0)
                return; // break the method

            byte[] data = new byte[received];
            Array.Copy(buffer, data, received);
            string text = Encoding.ASCII.GetString(data);
            MessageDecoderChooser(text);
        }


        private void MessageDecoderChooser(string message)
        {

            switch (message)
            {
                default:
                    Console.WriteLine(message);

                    break;
                case "start":
                    Console.WriteLine("You recieved the start");
                    break;
                case "end":
                    GameEnd = true;
                    break;
                case "updatePlayer":

                    break;

                case "message":
                    Console.WriteLine(message);
                    break;

                case "your turn":
                    myTurn = true;
                    GameBegin = true;
                    Console.WriteLine("ITS YOUR TURN");
                    Console.WriteLine("Type 'six dice' to roll");
                    break;

                case "game started":
                    GameBegin = true;
                    break;

                case "updatescore":
                    // update individual score to other players as well...
                    break;
            }
        }