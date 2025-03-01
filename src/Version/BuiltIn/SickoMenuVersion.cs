using Hazel;

namespace VentLib.Version.BuiltIn;

/// <summary>
/// Version Representing SickoMenu (the continuation of AmongUsMenu) 
/// <br></br>
/// Do note that it seems like players can disable the RPC, so if they do, this detection will not work unfortunately.
/// </summary>
public class SickoMenuVersion: Version
{
    public override Version Read(MessageReader reader)
    {
        return new SickoMenuVersion();
    }

    protected override void WriteInfo(MessageWriter writer)
    {
    }

    public override string ToSimpleName()
    {
        return "SickoMenu (Hacking Menu)";
    }
}