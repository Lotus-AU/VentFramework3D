using System;

namespace VentLib.Options.Enum;

public enum OptionType
{
    Undefined = -1,
    Bool = 1,
    Int,
    Float,
    String,
    Role, // Level 1 Options only can have this Type.
    Player, // should really not be used... I won't implement it just yet as I don't see why but maybe some time in the future.
    Title, // will be treated as Undefined
}