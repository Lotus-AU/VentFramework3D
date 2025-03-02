using System.Text;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace VentLib.Utilities;

public static class Converter
{
    public static string ByteArrayToString(Il2CppStructArray<byte> value)
    {
        string str = Encoding.Unicode.GetString(value);
        return str;
    }
}