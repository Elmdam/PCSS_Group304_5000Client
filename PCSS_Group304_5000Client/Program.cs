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
		public string MessageDecodeSender()
		{
			string returnMessage = "";
			string message = Console.ReadLine().ToLower();
			if (!GameBegin)
			{
				switch (message)
				{
					default:
						Console.WriteLine("Game has not started yet");
						break;
					case "start":
						GameBegin = true;
						returnMessage = "start game";
						break;
				}
			}
			else if (myTurn)
			{
				switch (message)
				{
					default:
						Console.WriteLine(
							"THESE ARE THE COMMANDS\n \"six dice\" - rolls 6 dices \"roll dice\" - rolls your current dices \"choose dice\" - Let's you choose which dice you want to take off the board \"end turn\" - ends your turn.");
						break;
					case "message":
						Console.WriteLine("Type in your message to everyone: ");
						returnMessage = Console.ReadLine();
						Console.WriteLine("Message sent");
						break;
					case "six dice":
						CreateDice();
						Console.WriteLine(" ");
						Console.WriteLine("Type 'choose dice' to choose a dice value");
						break;
					case "roll dice":
						RollDice();
						break;
					case "choose dice":
						ChooseDice();
						break;
					case "end turn":
						Console.WriteLine("Your turn has ended");
						SendString(scores[0].ToString());
						myTurn = false;
						return returnMessage = "end turn";
				}
			}
			else if (!myTurn)
			{
				switch (message)
				{
					default:
						Console.WriteLine("It is not your turn yet");
						break;
				}
			}
			if (!GameBegin)
			{
				switch (message)
				{
					default:
						Console.WriteLine("Game has not started yet");
						break;
					case "start":
						returnMessage = "start game";
						break;
				}
			}
			return returnMessage;
		}
	}
	public class Game
	{
		protected bool Lobby;
		protected bool GameBegin;
		protected bool GameEnd;
		protected bool myTurn = false;
		protected bool YouWon = false;
		int currentPoints;
		protected int[] scores = new int[3] { 0, 0, 0 };
		int min = 1;
		int max = 7;
		static List<int> dice = new List<int>();
		public void CreateDice()
		{
			if (dice.Count > 0)
			{
				dice.Clear();
				dice.Add(1);
				dice.Add(2);
				dice.Add(3);
				dice.Add(4);
				dice.Add(5);
				dice.Add(6);
			}
			else
			{
				dice.Add(1);
				dice.Add(2);
				dice.Add(3);
				dice.Add(4);
				dice.Add(5);
				dice.Add(6);
			}
			RollDice();
		}
		public void DisplayUI()
		{
			Console.Clear();
			Console.WriteLine("                         GET TO 5000 POINTS FIRST                               ");
			Console.WriteLine("________________________________________________________________________________");
			Console.WriteLine("\rPLAYER SCORES: Player 1: " + scores[0] + ", Player 2: " + scores[1] + ", Player 3: " +
							  scores[2]);
			Console.WriteLine("________________________________________________________________________________");
			Console.WriteLine("\rCurrent points: " + currentPoints);
			Console.WriteLine();
			if (YouWon)
			{
				Console.WriteLine("You won!");
			}
		}
		public void RollDice()
		{
			Random randNum = new Random();
			for (int i = 0; i < dice.Count; i++)
			{
				dice[i] = randNum.Next(min, max);
			}
			DisplayUI();
			DisplayDices();
		}
		void DisplayDices()
		{
			for (int i = 0; i < dice.Count; i++)
			{
				Console.WriteLine("Number of dice:  " + (i + 1));
				Console.WriteLine(" You got:  " + dice[i]);
			}
		}