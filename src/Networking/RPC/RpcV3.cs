using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fusion;
using SG.Airlock;
using VentLib.Logging;
using VentLib.Logging.Default;
using VentLib.Networking.Interfaces;
using VentLib.Networking.RPC.Inserters;
using VentLib.Networking.RPC.Interfaces;
using VentLib.Utilities;
using VentLib.Utilities.Debug.Profiling;
using VentLib.Utilities.Extensions;
using VentLib.Vanilla.Behaviour;
using PlayerControlExtensions = VentLib.Utilities.Extensions.PlayerControlExtensions;
using XRRigExtensions = VentLib.Utilities.Extensions.XRRigExtensions;

// ReSharper disable ParameterHidesMember

namespace VentLib.Networking.RPC;

public class RpcV3: MonoRpc, MassRpc, IChainRpc
{
    private static readonly StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(RpcV3));
    private uint callId;
    private bool immediate;
    private RpcBody? rpcBody;
    private bool isProtected;
    private bool isThreadSafe;

    private List<RpcV3> chainedRpcs = null!;
    private MassRpc parentRpc = null!;

    private RpcMeta? lastMeta;
    private Assembly assembly;
    
    private RpcV3()
    {
        assembly ??= Assembly.GetCallingAssembly();
    }
    
    public static MonoRpc Immediate(uint callId, RpcBody? rpcBody = null, bool isProtected = false)
    {
        return new RpcV3
        {
            callId = callId,
            immediate = true,
            rpcBody = rpcBody ?? new RpcBody(),
            isProtected = isProtected,
            assembly = Assembly.GetCallingAssembly()
        };
    }
    
    public static MonoRpc Standard(uint callId, RpcBody? rpcBody = null, bool isProtected = false)
    {
        return new RpcV3
        {
            callId = callId,
            immediate = false,
            rpcBody = rpcBody ?? new RpcBody(),
            isProtected = isProtected,
            assembly = Assembly.GetCallingAssembly()
        };
    }

    public static MassRpc Mass(bool isProtected = false, params IStrongRpc[]? messages)
    {
        List<RpcV3> strongRpcs = (messages ?? Array.Empty<IStrongRpc>()).Cast<RpcV3>().SelectMany(r => r.chainedRpcs != null! ? r.FlattenChained() : new List<RpcV3> { r }).ToList();
        return new RpcV3
        {
            callId = 100,
            isProtected = isProtected,
            chainedRpcs = strongRpcs
        };
    }
    
    public MassRpc Add(IStrongRpc rpc)
    {
        RpcV3 v3 = (RpcV3)rpc;
        if (v3.chainedRpcs == null!) chainedRpcs.Add(v3);
        else chainedRpcs.AddRange(v3.FlattenChained());
        return this;
    }
    
    public IChainRpc Start(uint callId)
    {
        RpcV3 childRpc = new()
        {
            callId = callId,
            rpcBody = new RpcBody(),
            parentRpc = this,
            assembly = Assembly.GetCallingAssembly()
        };
        chainedRpcs.Add(childRpc);
        return childRpc;
    }
    
    public MassRpc End() => parentRpc;

    public MonoRpc SetBody(RpcBody body)
    {
        rpcBody = body;
        return this;
    }
    
    public RpcBody ExtractBody()
    {
        return rpcBody!;
    }

    public MonoRpc Write(object obj)
    {
        rpcBody?.Write(obj);
        return this;
    }
    
    public MonoRpc Write(IRpcWritable obj)
    {
        rpcBody?.Write(obj);
        return this;
    }

    public MonoRpc Write(bool b)
    {
        rpcBody?.Write(b);
        return this;
    }

    public MonoRpc Write(byte b)
    {
        rpcBody?.Write(b);
        return this;
    }

    public MonoRpc Write(float b)
    {
        rpcBody?.Write(b);
        return this;
    }

    public MonoRpc Write(int b)
    {
        rpcBody?.Write(b);
        return this;
    }

    public MonoRpc Write(sbyte b)
    {
        rpcBody?.Write(b);
        return this;
    }

    public MonoRpc Write(string b)
    {
        rpcBody?.Write(b);
        return this;
    }

    public MonoRpc Write(uint b)
    {
        rpcBody?.Write(b);
        return this;
    }

    public MonoRpc Write(ulong b)
    {
        rpcBody?.Write(b);
        return this;
    }

    public MonoRpc Write(ushort b)
    {
        rpcBody?.Write(b);
        return this;
    }

    public MonoRpc Write(NetworkBehaviour networkBehaviour)
    {
        rpcBody?.Write(networkBehaviour);
        return this;
    }
    
    public MonoRpc Write(NetworkObject networkObject)
    {
        rpcBody?.Write(networkObject);
        return this;
    }

    IChainRpc IChainRpc.Write(object obj)
    {
        rpcBody?.Write(obj);
        return this;
    }

    public MonoRpc WritePacked(object obj)
    {
        rpcBody?.WritePacked(obj);
        return this;
    }

    public MonoRpc WritePacked(int i)
    {
        rpcBody?.WritePacked(i);
        return this;
    }

    IChainRpc IChainRpc.WritePacked(object obj)
    {
        rpcBody?.WritePacked(obj);
        return this;
    }
    
    public MonoRpc WritePacked(uint ui)
    {
        rpcBody?.WritePacked(ui);
        return this;
    }
    
    public MonoRpc WriteCustom<T>(T obj, Action<T, MessageWriter> writerFunction)
    {
        rpcBody?.WriteCustom(obj, new CustomInserter<T>(writerFunction));
        return this;
    }

    public MonoRpc Protected(bool isProtected)
    {
        this.isProtected = isProtected;
        return this;
    }

    public MonoRpc ThreadSafe(bool threadSafe)
    {
        isThreadSafe = threadSafe;
        return this;
    }
    
    public MonoRpc Clone()
    {
        return new RpcV3
        {
            callId = callId,
            assembly = assembly,
            immediate = immediate,
            rpcBody = rpcBody?.Clone(),
            isProtected = isProtected,
            isThreadSafe = isThreadSafe
        };
    }

    public RpcMeta? LastMeta() => lastMeta;

    public void SendInclusive(params int[] playerIds)
    {
        foreach (PlayerState player in GameStateBehaviour.GetConnectedPlayers())
        {
            try
            {
                int playerId = player.PlayerId;
                if (playerIds.Contains(playerId)) Send(playerId);
            }
            catch (Exception exception)
            {
                log.Exception("Failed to send RPC.", exception);
            }
        }
    }

    public void SendExcluding(params int[] playerIds)
    {
        foreach (PlayerState player in GameStateBehaviour.GetConnectedPlayers())
        {
            try
            {
                int playerId = player.PlayerId;
                if (!playerIds.Contains(playerId)) Send(playerId);
            }
            catch (Exception exception)
            {
                log.Exception("Failed to send RPC.", exception);
            }
        }
    }

    public void Send(int playerId = -1, bool notify = true)
    {
        if (isProtected && !PlayerControlExtensions.IsHost(XRRigExtensions.LocalPlayer())) return;
        if (isThreadSafe && !MainThreadAnchor.IsMainThread())
        {
            MainThreadAnchor.ExecuteOnMainThread(() => Send(playerId));
            return;
        }
        if (chainedRpcs != null!)
        {
            SendMessages(playerId);
            return;
        }

        Profiler.Sample sample = Profilers.Global.Sampler.Sampled();

        MessageWriter writer = new(callId, playerId);
        rpcBody?.WriteAll(writer);
        
        lastMeta = GenerateMeta(playerId, writer.Size);
        if (notify) lastMeta.Notify();

        if (!immediate) 
            writer.Export(); // why would you do this???
        else if (XRRigExtensions.LocalPlayer().IsHost())
        {
            if (playerId != -1)
                NetworkRunnerBehaviour.SendDataToClientAsHost(GameStateBehaviour.GetPlayerState(playerId), writer.ExportToByte());
            else
                GameStateBehaviour.GetConnectedPlayers().ForEach(ps => NetworkRunnerBehaviour.SendDataToClientAsHost(ps, writer.ExportToByte()));
        }
        else
            NetworkRunnerBehaviour.SendDataToHost(writer.ExportToByte());
        
        sample.Stop();
    }

    private RpcMeta GenerateMeta(int playerId, int packetSize)
    {
        return new RpcMeta
        {
            CallId = callId,
            Immediate = immediate,
            Recipient = playerId,
            RequiresHost = isProtected,
            Arguments = rpcBody!.Arguments,
            PacketSize = packetSize
        };
    }

    private void SendMessages(int playerId)
    {
        List<RpcMeta> childrenMeta = new();
        int writerLength = 0;
        chainedRpcs.ForEach(chained =>
        {
            chained.Send(playerId, false);
            childrenMeta.Add(chained.LastMeta()!);
            writerLength += chained.LastMeta()!.PacketSize;
        });
        lastMeta = new RpcMassMeta
        {
            ChildMeta = childrenMeta,
            CallId = callId,
            Immediate = immediate,
            Recipient = playerId,
            RequiresHost = isProtected,
            Arguments = rpcBody?.Arguments ?? new List<object>(),
            PacketSize = writerLength
        };
        lastMeta.Notify();
    }

    private List<RpcV3> FlattenChained()
    {
        if (chainedRpcs == null!) return new List<RpcV3>();
        List<RpcV3> flatList = new(chainedRpcs);
        flatList.AddRange(chainedRpcs.SelectMany(cr => cr.FlattenChained()));
        return flatList;
    }
}