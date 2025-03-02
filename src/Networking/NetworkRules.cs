using System;
using Fusion;
using UnityEngine;
using VentLib.Networking.Interfaces;
using VentLib.Options;
using VentLib.Options.IO;
using VentLib.Utilities.Attributes;

namespace VentLib.Networking;

[LoadStatic]
public class NetworkRules
{
    public static readonly Type[] AllowedTypesOverNetwork =
    {
        typeof(bool), typeof(byte), typeof(float), typeof(int), typeof(sbyte), typeof(string), typeof(uint), typeof(ulong), typeof(ushort), typeof(short),
        typeof(Vector2), typeof(Vector3), typeof(NetworkObject), typeof(NetworkBehaviour), typeof(IRpcSendable<>), typeof(IBatchSendable)
    };

    public const int AbsoluteMaxPacketSize = 1024;
    public const int AbsoluteMinPacketSize = 256;

    internal const string VentSignature = "VentFramework3D";
    
    public static int MaxPacketSize
    {
        get => _maxPacketSize;
        set => _packetSizeOption.SetHardValue(_maxPacketSize = Mathf.Clamp(value, AbsoluteMinPacketSize, AbsoluteMaxPacketSize));
    }
    private static int _maxPacketSize;
    
    private static Option _packetSizeOption;
    static NetworkRules()
    {
        OptionManager networkManager = OptionManager.GetManager(file: "network.config");
        _packetSizeOption = new OptionBuilder()
            .Key("Max Packet Size")
            .Value(AbsoluteMaxPacketSize)
            .IOSettings(io => io.UnknownValueAction = ADEAnswer.Allow)
            .BindInt(_ => _packetSizeOption?.Manager?.DelaySave())
            .BuildAndRegister(networkManager);

        _maxPacketSize = _packetSizeOption.GetValue<int>();
    }
}