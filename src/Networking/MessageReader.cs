using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Fusion;
using HarmonyLib;
using UnityEngine;
using VentLib.Logging.Default;
using VentLib.Networking.Batches;
using VentLib.Networking.Interfaces;
using ArgumentException = System.ArgumentException;
using Object = UnityEngine.Object;

namespace VentLib.Networking;

public class MessageReader
{
    private const string Pattern = @"(?<=^|[^\\])""([^""]*)""(?=$|[^\\])|[^|""\\]+";
    private Queue<string> messageQueue;
    
    public MessageReader(MessageWriter writer) : this(writer.Export()) {}

    public MessageReader(string rpcStr)
    {
        messageQueue = new(Regex.Matches(rpcStr, Pattern)
            .Cast<Match>()
            .Select(m => m.Value)
            .Select(s => s.Trim('"'))  // Remove quotes
            .ToArray());
    }
    
    /// <summary>
    /// Creates a new MessageReader from another reader.
    /// </summary>
    /// <param name="reader">The reader to copy.</param>
    /// <returns>The new MessageReader instance.</returns>
    public static MessageReader Get(MessageReader reader)
    {
        return (MessageReader)reader.MemberwiseClone();
    }

    /// <summary>
    /// Clears the rest of the message queue.
    /// </summary>
    public void Recycle()
    {
        messageQueue.Clear();
    }

    /// <summary>
    /// Reads a value from the Message.
    /// </summary>
    /// <typeparam name="T">The type of value to read.</typeparam>
    /// <returns>A value converted to T.</returns>
    /// <exception cref="NotSupportedException">This happens if you try to read a value that we do not currently support.</exception>
    public T Read<T>()
    {
        Type t = typeof(T);
        string message = messageQueue.Dequeue().Normalize(NormalizationForm.FormKC).Trim();
        
        #if (DEBUG)
            NoDepLogger.Debug($"Trying to convert '{message}' to {t.FullName}");
        #endif
        
        return t switch
        {
            _ when t == typeof(bool) => (T)(object)(message == "T"),
            _ when t == typeof(byte) => (T)(object)byte.Parse(message),
            _ when t == typeof(float) => (T)(object)float.Parse(message),
            _ when t == typeof(short) => (T)(object)short.Parse(message),
            _ when t == typeof(int) => (T)(object)int.Parse(message),
            _ when t == typeof(sbyte) => (T)(object)sbyte.Parse(message),
            _ when t == typeof(string) => (T)(object)message,
            _ when t == typeof(uint) => (T)(object)uint.Parse(message),
            _ when t == typeof(long) => (T)(object)long.Parse(message),
            _ when t == typeof(ushort) => (T)(object)ushort.Parse(message),
            _ when t == typeof(Vector2) => (T)(object)HandleVector2(message),
            _ when t == typeof(Vector3) => (T)(object)HandleVector3(message),
            _ when typeof(NetworkBehaviour).IsAssignableFrom(t) => (T)(object)Object.FindObjectsOfType<NetworkBehaviour>().FirstOrDefault(nb => nb.Id.Object.Raw == uint.Parse(message))!,
            _ when typeof(NetworkObject).IsAssignableFrom(t) => (T)(object)Object.FindObjectsOfType<NetworkObject>().FirstOrDefault(nb => nb.Id.Raw == uint.Parse(message))!,
            _ when typeof(IRpcReadable<>).IsAssignableFrom(t) => HandleReadable<T>(t, message),
            _ when typeof(IBatchSendable).IsAssignableFrom(t) => (T)(object)new BatchReader(this),
            _ => throw new InvalidOperationException($"Type {t} is not allowed to be sent over RPC."),
        };
    }

    /// <summary>
    /// Reads a value from the Message that was packed to save space. (Just reads from regular list for right now.)
    /// </summary>
    /// <typeparam name="T">The type of value to read.</typeparam>
    /// <returns>A value converted to T.</returns>
    /// <exception cref="NotSupportedException">This happens if you try to read a value that we do not currently support.</exception>
    public T ReadPacked<T>() => Read<T>();
    
    private Vector2 HandleVector2(string message)
    {
        float[] args = message.Split(',').Select(float.Parse).ToArray();
        return new Vector2(args[0], args[1]);
    }

    private Vector3 HandleVector3(string message)
    {
        
        float[] args = message.Split(',').Select(float.Parse).ToArray();
        return new Vector3(args[0], args[1], args[2]);
    }

    private T HandleReadable<T>(Type t, string message)
    {
        BindingFlags completeFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
        Type rpcableType = t.GetGenericArguments().Any() ? t.GetGenericArguments()[0] : t;
    
        object rpcable = rpcableType.GetConstructor(completeFlags, Array.Empty<Type>())?.Invoke(null)
                         ?? throw new InvalidOperationException($"No parameterless constructor found for {rpcableType}");
    
        return (T)rpcableType.GetMethod("Read", completeFlags, null, [typeof(MessageReader)], null)
                   ?.Invoke(rpcable, [this])! ?? throw new InvalidOperationException($"Failed to invoke Read method on {rpcableType}");
    }
    
    public dynamic ReadDynamic(Type parameter)
    {
        if (parameter.IsAbstract) return AbstractConstructors.Transform(this, parameter);
        if (parameter == typeof(bool)) return Read<bool>();
        if (parameter == typeof(byte)) return Read<byte>();
        if (parameter == typeof(float)) return Read<float>();
        if (parameter == typeof(int)) return Read<int>();
        if (parameter == typeof(short)) return Read<short>();
        if (parameter == typeof(sbyte)) return Read<sbyte>();
        if (parameter == typeof(string)) return Read<string>();
        if (parameter == typeof(uint)) return Read<uint>();
        if (parameter == typeof(ulong)) return Read<ulong>();
        if (parameter == typeof(ushort)) return Read<ushort>();
        if (parameter == typeof(Vector2)) return Read<Vector2>();
        if (parameter == typeof(Vector3)) return Read<Vector3>();
        if (parameter.IsAssignableTo(typeof(NetworkBehaviour))) return Read<NetworkBehaviour>();
        if (parameter.IsAssignableTo(typeof(NetworkObject))) return Read<NetworkObject>();
        if (parameter.IsAssignableTo(typeof(IBatchSendable))) return new BatchReader(this);
        if (parameter.IsAssignableTo(typeof(IList)))
        {
            Type genericType = parameter.GetGenericArguments()[0];
            object objectList = Activator.CreateInstance(parameter)!;
            MethodInfo add = AccessTools.Method(parameter, "Add");

            ushort amount = Read<ushort>();
            for (uint i = 0; i < amount; i++)
                add.Invoke(objectList, [ReadDynamic(genericType)]);
            return (IEnumerable<dynamic>)objectList;
        }
        if (parameter.IsAssignableTo(typeof(IRpcWritable)))
        {
            BindingFlags completeFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
            Type rpcableType = parameter.GetGenericArguments().Any() ? parameter.GetGenericArguments()[0] : parameter;
            object rpcable = rpcableType.GetConstructor(completeFlags, Array.Empty<Type>())!.Invoke(null);
            return rpcableType.GetMethod("Read", completeFlags, [typeof(MessageReader)])!.Invoke(rpcable, [this])!;
        }

        throw new ArgumentException($"Invalid Parameter Type {parameter}");
    }
}