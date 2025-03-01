using Hazel;

namespace VentLib.Version.BuiltIn;

/// <summary>
/// Version Representing AmongUsMenu
/// <br></br>
/// Do note that it seems like AmongUsMenu has been discontinued. So this will just be a relic of the past when the next few updates break it.
/// </summary>
public class AmongUsMenuVersion: Version
{
    public override Version Read(MessageReader reader)
    {
        return new AmongUsMenuVersion();
    }

    protected override void WriteInfo(MessageWriter writer)
    {
    }

    public override string ToSimpleName()
    {
        return "Among Us Menu (Hacking Menu)";
    }
}