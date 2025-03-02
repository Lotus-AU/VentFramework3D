using System;
using System.Reflection;
using VentLib.Networking.Enums;

namespace VentLib.Networking.Attributes;

/// <summary>
/// Describes the <see cref="Networking.Enums.ModFlags"/> of the associated plugin.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class VentModFlagsAttribute: Attribute
{
    /// <summary>
    /// Gets flags of the mod.
    /// </summary>
    public ModFlags Flags { get; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="VentModFlagsAttribute"/> class.
    /// </summary>
    /// <param name="flags">Flags associated with the mod.</param>
    public VentModFlagsAttribute(ModFlags flags)
    {
        Flags = flags;
    }

    internal static ModFlags GetVentModFlags(Type type)
    {
        var attribute = type.GetCustomAttribute<VentModFlagsAttribute>();
        if (attribute != null) return attribute.Flags;
        return ModFlags.None;
    }
}