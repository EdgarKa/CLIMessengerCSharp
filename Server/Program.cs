using System.Net.Sockets;
using ChatFramework;

namespace Server;

public class Program
{
    static readonly Messenger inStream = new();
    static readonly Messenger outStream = new();

    static readonly List<TcpClient> _clients = new();
    static readonly TcpListener listener = new(Constants.Address, Constants.PORT);

    static void Main(string[] args)
    {
        listener.Start();

        Console.WriteLine("Server Started");

        while (true)
        {
            AcceptClients();

            ReceiveMessage();
        }
    }

    static void AcceptClients()
    {
        for (var i = 0; i < 5; i++)
        {
            if (!listener.Pending()) continue;

            var client = listener.AcceptTcpClient();
            _clients.Add(client);
            Console.WriteLine("Client accepted");
        }
    }

    static void ReceiveMessage()
    {
        foreach (var client in _clients)
        {
            NetworkStream stream = client.GetStream();

            if (stream.DataAvailable)
            {
                var buffer = new byte[client.ReceiveBufferSize];
                var bytesRead = stream.Read(buffer, 0, buffer.Length);
                (var code, var message) = inStream.ParseMessagePacket(buffer.Take(bytesRead).ToArray());

                Console.WriteLine($"Received: [{code}] - {message} ({DateTime.Now:HH:mm})");

                Broadcast(client, message);
            }
        }
    }

    static void Broadcast(TcpClient sender, string message)
    {
        foreach (var client in _clients.Where(x => x != sender))
        {
            var packet = outStream.CreateMessagePacket(10, message);
            client.GetStream().Write(packet);
        }
    }
}
