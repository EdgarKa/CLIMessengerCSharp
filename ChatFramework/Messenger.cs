using System.Text;

namespace ChatFramework;

public class Messenger
{
    public byte[] CreateMessagePacket(int code, string message)
    {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);

        writer.Write(code);
        writer.Write(message);

        return ms.ToArray();
    }

    public (int code, string message) ParseMessagePacket(byte[] data)
    {
        using var ms = new MemoryStream(data);
        using var reader = new BinaryReader(ms, Encoding.ASCII);

        var code = reader.ReadInt32();
        var message = reader.ReadString();

        return (code, message);
    }
}
