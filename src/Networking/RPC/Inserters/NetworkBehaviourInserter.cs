using Fusion;
using VentLib.Networking.RPC.Interfaces;

namespace VentLib.Networking.RPC.Inserters;

public class NetworkBehaviourInserter: IRpcInserter<NetworkBehaviour>
{
    public static NetworkBehaviourInserter Instance = null!;

    public NetworkBehaviourInserter()
    {
        Instance = this;
    }
    
    public void Insert(NetworkBehaviour value, MessageWriter writer)
    {
        writer.Write(value);
    }
}