namespace VentLib.Networking;

public class MessageReader
{
    public MessageReader()
    {
        
    }

    public MessageReader(MessageWriter writer)
    {
        
    }

    public MessageReader(string rpcStr)
    {
        
    }
    
    public static MessageReader Get(MessageReader reader)
    {
        return (MessageReader)reader.MemberwiseClone();
    }

    public T Read<T>()
    {
        return default(T);
    }
}