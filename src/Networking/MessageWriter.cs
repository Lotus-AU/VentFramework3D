using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Fusion;
using UnityEngine;
using VentLib.Networking.Interfaces;
using VentLib.Utilities.Extensions;
using NetworkBehaviour = Fusion.NetworkBehaviour;

namespace VentLib.Networking;

public class MessageWriter
{
    private List<string> messages;
    private uint rpcCall;

    /// <summary>
    /// The size of this message in bytes.
    /// </summary>
    public int Size => Encoding.Unicode.GetByteCount(Export());
    
    public MessageWriter(uint rpcCall, int targetPlayerId)
    {
        this.rpcCall = rpcCall;
        messages = new()
        {
            NetworkRules.VentSignature,
            rpcCall.ToString(),
            targetPlayerId.ToString()
        };
    }

    internal MessageWriter()
    {
        messages = new();
    }
    
    /// <summary>
    /// Creates a MessageWriter with no information. <br/>
    /// This removes the signal that tells the other clients this is a Vent Framework RPC. <br/>
    /// Only use if you know what you are doing.
    /// </summary>
    /// <returns>An empty MessageWriter.</returns>
    public static MessageWriter Get() => new();

    /// <summary>
    /// Deletes all values currently written to this reader.
    /// </summary>
    public void Recycle()
    {
        messages.Clear();
        messages.Add(NetworkRules.VentSignature);
        messages.Add(rpcCall.ToString());
    }

    /// <summary>
    /// Writes a value to the Message.
    /// </summary>
    /// <param name="item">The value to write.</param>
    /// <typeparam name="T">The type of value to write.</typeparam>
    /// <exception cref="NotSupportedException">This happens if you try to write a value that we do not currently support.</exception>
    public void Write<T>(T item)
    {
        Type t = typeof(T);
        
        string output = t switch
        {
            _ when item is bool boolean => boolean ? "T" : "F",
            _ when item is byte byteItem => byteItem.ToString(),
            _ when item is float floatItem => floatItem.ToString(),
            _ when item is short shortItem => shortItem.ToString(),
            _ when item is int intItem => intItem.ToString(),
            _ when item is sbyte sbyteItem => sbyteItem.ToString(),
            _ when item is string str => $"\"{str}\"",
            _ when item is uint uintItem => uintItem.ToString(),
            _ when item is long longItem => longItem.ToString(),
            _ when item is ushort ushortItem => ushortItem.ToString(),
            _ when item is Vector2 vector => $"{vector.x},{vector.y}",
            _ when item is Vector3 vector => $"{vector.x},{vector.y},{vector.z}",
            _ when item is NetworkBehaviour networkBehaviour => networkBehaviour.Id.Object.Raw.ToString(),
            _ when item is NetworkObject networkObject => networkObject.Id.Raw.ToString(),
            _ when item is IRpcWritable writable => HandleWritable(writable),
            _ when typeof(IBatchSendable).IsAssignableFrom(t) => "",
            _ => throw new InvalidOperationException($"Type {t} is not allowed to be sent over RPC."),
        };
        if (output == string.Empty) return;
        
        messages.Add(output);

        string HandleWritable(IRpcWritable writable)
        {
            writable.Write(this);
            return string.Empty;
        }
    }
    
    /// <summary>
    /// Writes a value to the Message, but packed to save space. (Doesn't do anything currently)
    /// </summary>
    /// <param name="item">The value to write.</param>
    /// <typeparam name="T">The type of value to write.</typeparam>
    /// <exception cref="NotSupportedException">This happens if you try to write a value that we do not currently support.</exception>
    public void WritePacked<T>(T item) => Write(item);

    /// <summary>
    /// Exports the Writer as a string to be sent.
    /// </summary>
    /// <returns>A string representing this Writer.</returns>
    public string Export() => string.Join("|", messages);
    
    /// <summary>
    /// Exports the current items as an array. Editing this array will not edit the original list.
    /// </summary>
    /// <returns>An array of the currently written values.</returns>
    public string[] ExportToArray() => messages.ToArray();
    
    /// <summary>
    /// Exports the current items as an array of bytes.
    /// </summary>
    /// <returns>Bytes representing the currently written values.</returns>
    public byte[] ExportToByte() => Encoding.Unicode.GetBytes(Export());
}