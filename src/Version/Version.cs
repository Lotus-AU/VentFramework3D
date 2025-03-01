using System;
using HarmonyLib;
using Hazel;
using VentLib.Networking;
using VentLib.Networking.Interfaces;
using VentLib.Utilities;
using VentLib.Version.Assembly;
using VentLib.Version.BuiltIn;
using VentLib.Version.Git;

namespace VentLib.Version;

public abstract class Version: IRpcSendable<Version>
{
    static Version()
    {
        AbstractConstructors.Register(typeof(Version), ReadStatic);
    }

    private static Version ReadStatic(MessageReader reader)
    {
        VersionType type = (VersionType)reader.Read<byte>();
        switch (type)
        {
            case VersionType.None:
                return AccessTools.CreateInstance<NoVersion>();
            case VersionType.Git:
                return AccessTools.CreateInstance<GitVersion>().Read(reader);
            case VersionType.Assembly:
                return AccessTools.CreateInstance<AssemblyVersion>().Read(reader);
            case VersionType.Custom:
                string assemblyName = reader.Read<string>();
                var containingAssembly = AssemblyUtils.FindAssemblyFromFullName(assemblyName);
                if (containingAssembly == null) throw new NullReferenceException($"Could not find assembly \"{assemblyName}\" for Custom type");
                string customTypeName = reader.Read<string>();
                Type? versionType = containingAssembly.GetType(customTypeName);
                if (versionType == null)
                    throw new NullReferenceException($"Could not find Version class \"{customTypeName}\" in assembly \"{assemblyName}\"");
                object? constructed = AccessTools.CreateInstance(versionType);
                if (constructed is not Version customVersion)
                    throw new ArgumentException($"Constructed type \"{constructed?.GetType()}\" does not inherit VentLib.Version");
                return customVersion.Read(reader);
            default:
                throw new ArgumentOutOfRangeException($"Unexpected VersionType {type}");
        }
    }
    
    public abstract Version Read(MessageReader reader);
    
    protected abstract void WriteInfo(MessageWriter writer);
    
    public void Write(MessageWriter writer)
    {
        VersionType type = this switch
        {
            NoVersion => 0,
            GitVersion => VersionType.Git,
            AssemblyVersion => VersionType.Assembly,
            _ => VersionType.Custom
        };
        writer.Write((byte)type);
        WriteInfo(writer);
    }

    public static bool operator ==(Version? versionA, Version? versionB)
    {
        return versionA?.Equals(versionB) ?? ReferenceEquals(versionB, null);
    }
    
    public static bool operator !=(Version? versionA, Version? versionB)
    {
        return !versionA?.Equals(versionB) ?? !ReferenceEquals(versionB, null);
    }

    public override bool Equals(object? obj) => obj is Version;

    // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
    public override int GetHashCode() => base.GetHashCode();

    public abstract string ToSimpleName();
}

public enum VersionType: byte
{
    None,
    Git,
    Assembly,
    Custom
}