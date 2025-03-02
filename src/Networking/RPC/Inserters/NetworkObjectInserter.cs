using Fusion;
using VentLib.Networking.RPC.Interfaces;

namespace VentLib.Networking.RPC.Inserters;

public class NetworkObjectInserter: IRpcInserter<NetworkObject>
{
    public static NetworkObjectInserter Instance = null!;

    public NetworkObjectInserter()
    {
        Instance = this;
    }
    
    public void Insert(NetworkObject value, MessageWriter writer)
    {
        writer.Write(value);
    }
}