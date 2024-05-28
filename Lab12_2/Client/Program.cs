using System;
using System.IO;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;

public class MyObject
{
    public int Value { get; set; }
}

public class Client
{
    private TcpClient client;
    private NetworkStream stream;
    private StreamReader reader;
    private StreamWriter writer;
    private MyObject receivedObject = new MyObject { Value = 0 };

    public Client(string address, int port)
    {
        client = new TcpClient(address, port);
        stream = client.GetStream();
        reader = new StreamReader(stream);
        writer = new StreamWriter(stream) { AutoFlush = true };
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
            MyObject obj = new MyObject { Value = receivedObject.Value + 1 };
            string json = JsonSerializer.Serialize(obj);
            writer.WriteLine(json);
            Console.WriteLine("Sent object with Value: " + obj.Value);
            Thread.Sleep(3000);
        }
    }

    private void ReceiveData()
    {
        while (true)
        {
            try
            {
                string json = reader.ReadLine();
                receivedObject = JsonSerializer.Deserialize<MyObject>(json);
                Console.WriteLine("Received updated object with Value: " + receivedObject.Value);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                client.Close();
                break;
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
