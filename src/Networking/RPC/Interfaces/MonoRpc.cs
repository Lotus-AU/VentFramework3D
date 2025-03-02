using System;
using Fusion;
using VentLib.Networking.Interfaces;

namespace VentLib.Networking.RPC.Interfaces;

// ReSharper disable once InconsistentNaming
public interface MonoRpc: IStrongRpc
{
    public MonoRpc SetBody(RpcBody body);

    public MonoRpc Write(object obj);

    public MonoRpc Write(IRpcWritable obj);

    public MonoRpc Write(bool b);
    
    public MonoRpc Write(byte b);
    
    public MonoRpc Write(float b);
    
    public MonoRpc Write(int b);
    
    public MonoRpc Write(sbyte b);
    
    public MonoRpc Write(string b);
    
    public MonoRpc Write(uint b);
    
    public MonoRpc Write(ulong b);
    
    public MonoRpc Write(ushort b);

    public MonoRpc Write(NetworkBehaviour innerNetObject);
    
    public MonoRpc Write(NetworkObject innerNetObject);

    public MonoRpc WriteCustom<T>(T obj, Action<T, MessageWriter> writerFunction);

    public MonoRpc WritePacked(object obj);
    
    public MonoRpc WritePacked(int i);
    
    public MonoRpc WritePacked(uint ui);

    public MonoRpc Protected(bool isProtected);

    public MonoRpc ThreadSafe(bool threadSafe);

    public void Send(int playerId = -1, bool notify = false);

    public void SendInclusive(params int[] playerIds);

    public void SendExcluding(params int[] playerIds);

    public MonoRpc Clone();
}