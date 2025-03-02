using System;
using System.Collections.Generic;
using VentLib.Utilities.Collections;
using VentLib.Utilities.Extensions;

namespace VentLib.Networking.RPC;

public class RpcMeta
{
    public static RemoteList<Action<RpcMeta>> Subscribers = new();
    public static Remote<Action<RpcMeta>> AddSubscriber(Action<RpcMeta> metaConsumer) => Subscribers.Add(metaConsumer);
    
    public int Recipient { get; internal set; }
    public uint CallId { get; internal set; }
    public bool Immediate { get; internal set; }
    public bool RequiresHost { get; internal set; }
    public int PacketSize { get; internal set; }
    public List<object> Arguments { get; internal set; } = null!;

    public void Notify()
    {
        foreach (Action<RpcMeta> subscriber in Subscribers)
        {
            subscriber.Invoke(this);
        }
    }

    public override string ToString()
    {
        return $"({CallId} => PacketSize={PacketSize}, Arguments={Arguments.Fuse()})";
    }
}