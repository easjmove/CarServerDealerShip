using CarLibraryDealerShip;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;

namespace CarServerDealerShip
{
    class Program
    {
        //Remember to add the CarLibraryDealerShip DLL as a project reference
        static void Main(string[] args)
        {
            //Writes to the console what is running
            Console.WriteLine("Car server");

            //Creates a Listener that listens on all network adapters on port 10002
            TcpListener listener = new TcpListener(IPAddress.Any, 10002);
            //Starts the listener
            listener.Start();

            //Handles more clients
            while (true)
            {
                //Waits for a client to connect
                TcpClient socket = listener.AcceptTcpClient();
                //Makes the server concurrent
                //And sends the client to the method HandleClient
                Task.Run(() => HandleClient(socket));
            }
        }

        private static void HandleClient(TcpClient socket)
        {
            //Gets the stream object from the socket. The stream object is able to recieve and send data
            NetworkStream ns = socket.GetStream();
            //The StreamReader is an easier way to read data from a Stream, it uses the NetworkStream
            StreamReader reader = new StreamReader(ns);

            //no writer here, as it doesn't send data back to the client

            //Reads a line from the client, here it expects the entire JSON in one line
            string message = reader.ReadLine();

            //Here it converts the message the client send to a CarDealerShip object
            //if the client sends something wrong, properties won't be set
            CarDealerShip receivedDealerShip = JsonSerializer.Deserialize<CarDealerShip>(message);

            //Writes the dealership info first
            Console.WriteLine($"Name: {receivedDealerShip.Name} Address: {receivedDealerShip.Address}");
            //then iterates through all the cars recieved
            foreach (Car car in receivedDealerShip.Cars)
            {
                //writes the 3 properties to the console, one car per line
                Console.WriteLine($"Car Model: {car.Model} Car Color: {car.Color} Car RegistrationNumber: {car.RegistrationNumber}");
            }
            //closes the socket, as it doesn't expect anything more from the client
            socket.Close();

            //JSON example to test with
            //{"Name":"MoveAuto","Address":"Home 12","Cars":[{"Model":"BMW","Color":"Black","RegistrationNumber":"AB12345"},{"Model":"VW","Color":"Silver","RegistrationNumber":"BC23456"},{"Model":"Fiat","Color":"White","RegistrationNumber":"CD34567"}]}
        }
    }
}
