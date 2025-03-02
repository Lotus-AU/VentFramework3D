namespace VentLib.Networking.RPC.Interfaces;

// ReSharper disable once InconsistentNaming
public interface MassRpc: IStrongRpc
{
    public IChainRpc Start(uint callId);

    public MassRpc Add(IStrongRpc rpc);

    public void Send(int playerId = -1, bool notify = false);

    public void SendInclusive(params int[] playerIds);

    public void SendExcluding(params int[] playerIds);
}