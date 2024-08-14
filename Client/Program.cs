using System.Net.Sockets;
using ChatFramework;

namespace Client;

public class Program
{
    static readonly TcpClient client = new();

    static readonly Messenger inStream = new();
    static readonly Messenger outStream = new();

    static readonly List<string> outgoingMessages = [];

    static void Main(string[] args)
    {
        client.Connect(Constants.Address, Constants.PORT);
        Console.WriteLine("Connected to the server!");

        new TaskFactory().StartNew(() =>
        {
            while (true)
            {
                Console.Write("> ");
                var msg = Console.ReadLine();
                outgoingMessages.Add(msg);
            }
        });

        // Main loop to read incoming packets and send outgoing packets 
        while (true)
        {
            ReadPackets();
            SendPackets();
        }

    }

    static void ReadPackets()
    {
        var stream = client.GetStream();
        for (var i = 0; i < 10; i++)
        {
            if (stream.DataAvailable)
            {
                var buffer = new byte[client.ReceiveBufferSize];
                var bytesRead = stream.Read(buffer, 0, buffer.Length);
                (var code, var message) = inStream.ParseMessagePacket(buffer.Take(bytesRead).ToArray());
                Console.WriteLine($"Received: [{code}] - {message} ({DateTime.Now:HH:mm})");
                Console.Write("> ");
            }
        }
    }

    static void SendPackets()
    {
        if (outgoingMessages.Count > 0)
        {
            var msg = outgoingMessages[0];
            var packet = outStream.CreateMessagePacket(10, msg);
            client.GetStream().Write(packet);
            outgoingMessages.RemoveAt(0);
            //Console.Write(">");
        }
    }
}
