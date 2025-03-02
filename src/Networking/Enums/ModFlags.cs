using System;

namespace VentLib.Networking.Enums;

/// <summary>
/// Represents flags of the mod.
/// </summary>
[Flags]
public enum ModFlags
{
    /// <summary>
    /// No flags.
    /// </summary>
    None = 0,
    
    /// <summary>
    /// Requires all clients in a lobby to have the mod.
    /// </summary>
    RequireOnAllClients = 1 << 0,
    
    /// <summary>
    /// Requires the host of the lobby to have the mod.
    /// </summary>
    RequireOnHost = 1 << 2,
}