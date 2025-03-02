using UnityEngine;
using VentLib.Networking.RPC.Interfaces;

namespace VentLib.Networking.RPC.Inserters;

public class Vector3Inserter: IRpcInserter<Vector3>
{
    public void Insert(Vector3 value, MessageWriter writer)
    {
        writer.Write(value);
    }
}