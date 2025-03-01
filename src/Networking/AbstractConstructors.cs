using System;
using System.Collections.Generic;

namespace VentLib.Networking;

public static class AbstractConstructors
{
    private static Dictionary<Type, Func<MessageReader, object>> ReaderToObjectTransformers { get; } = new();

    public static void Register(Type type, Func<MessageReader, object> transformer)
    {
        ReaderToObjectTransformers[type] = transformer;
    }

    internal static object Transform(MessageReader reader, Type type)
    {
        if (!ReaderToObjectTransformers.TryGetValue(type, out Func<MessageReader, object>? transformer))
            throw new ArgumentException($"No Abstract Transformer Exists for type {type}");
        return transformer(reader);
    }
}