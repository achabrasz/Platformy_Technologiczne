using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

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
            Thread clientThread = new Thread(() => HandleClient(client));
            clientThread.Start();
        }
    }

    private void HandleClient(TcpClient client)
    {
        using (NetworkStream stream = client.GetStream())
        using (StreamReader reader = new StreamReader(stream))
        using (StreamWriter writer = new StreamWriter(stream) { AutoFlush = true })
        {
            while (client.Connected)
            {
                try
                {
                    string json = reader.ReadLine();
                    MyObject receivedObject = JsonSerializer.Deserialize<MyObject>(json);
                    Console.WriteLine("Received object with Value: " + receivedObject.Value);

                    // Processing: increment the Value field
                    receivedObject.Value += 1;

                    string responseJson = JsonSerializer.Serialize(receivedObject);
                    writer.WriteLine(responseJson);
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
}

class Program
{
    static void Main(string[] args)
    {
        Server server = new Server();
        server.Start();
    }
}
