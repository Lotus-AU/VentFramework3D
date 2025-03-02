using VentLib.Networking;

namespace VentLib.Version.BuiltIn;

/// <summary>
/// Version Representing no Version.
/// Essentially just a vanilla client.
/// </summary>
public sealed class NoVersion: Version
{
    public override Version Read(MessageReader reader)
    {
        return new NoVersion();
    }

    protected override void WriteInfo(MessageWriter writer) { }

    public override string ToSimpleName() => "";

    public override string ToString() => "NoVersion";
}