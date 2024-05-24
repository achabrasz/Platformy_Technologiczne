using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    using System;
    using System.IO;
    using System.Net.Sockets;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading;

    [Serializable]
    public class MyObject
    {
        public int Value { get; set; }
    }

    public class Client
    {
        private TcpClient client;
        private NetworkStream stream;
        private IFormatter formatter;

        public Client(string address, int port)
        {
            client = new TcpClient(address, port);
            stream = client.GetStream();
            formatter = new BinaryFormatter();
        }

        public void Start()
        {
            Thread sendThread = new Thread(SendData);
            sendThread.Start();

            Thread receiveThread = new Thread(ReceiveData);
            receiveThread.Start();
        }

        private void SendData()
        {
            while (true)
            {
                MyObject obj = new MyObject { Value = new Random().Next(100) };
                formatter.Serialize(stream, obj);
                Console.WriteLine("Sent object with Value: " + obj.Value);
                Thread.Sleep(2000); // Send data every 2 seconds
            }
        }

        private void ReceiveData()
        {
            while (true)
            {
                try
                {
                    MyObject receivedObject = (MyObject)formatter.Deserialize(stream);
                    Console.WriteLine("Received updated object with Value: " + receivedObject.Value);
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
            Client client = new Client("127.0.0.1", 8000);
            client.Start();
        }
    }

}
