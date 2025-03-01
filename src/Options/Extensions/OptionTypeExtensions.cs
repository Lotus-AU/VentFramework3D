using VentLib.Options.Enum;

namespace VentLib.Options.Extensions;

public static class OptionTypeExtensions
{
    public static bool CanOverride(this OptionType optionType) => optionType is not OptionType.Undefined;
}