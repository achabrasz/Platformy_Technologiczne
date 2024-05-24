using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Server
{
    [Serializable]
    public class MyObject
    {
        public int Value { get; set; }
    }

    public class Server
    {
        private TcpListener listener;

        public void Start()
        {
            listener = new TcpListener(IPAddress.Any, 8000);
            listener.Start();
            Console.WriteLine("Server started...");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("Client connected...");
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
                clientThread.Start(client);
            }
        }

        private void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            NetworkStream stream = client.GetStream();
            IFormatter formatter = new BinaryFormatter();

            while (client.Connected)
            {
                try
                {
                    MyObject receivedObject = (MyObject)formatter.Deserialize(stream);
                    Console.WriteLine("Received object with Value: " + receivedObject.Value);

                    // Processing: increment the Value field
                    receivedObject.Value += 1;

                    formatter.Serialize(stream, receivedObject);
                    Console.WriteLine("Sent updated object with Value: " + receivedObject.Value);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex.Message);
                    client.Close();
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            server.Start();
        }
    }
}
