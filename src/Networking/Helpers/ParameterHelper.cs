using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fusion;
using HarmonyLib;
using UnityEngine;
using VentLib.Networking.Batches;
using VentLib.Networking.Interfaces;

namespace VentLib.Networking.Helpers;

internal static class ParameterHelper
{
    public static Type[] AllowedTypes =
    {
        typeof(bool), typeof(byte), typeof(float), typeof(int), typeof(sbyte), typeof(string), typeof(uint),
        typeof(ulong), typeof(ushort), typeof(Vector2), typeof(NetworkBehaviour), typeof(IRpcSendable<>), typeof(IBatchSendable)
    };

    public static bool IsTypeAllowed(Type type)
    {
        if (!type.IsAssignableTo(typeof(IEnumerable)) || type.GetGenericArguments().Length == 0)
            return type.IsAssignableTo(typeof(IRpcWritable)) || AllowedTypes.Any(type.IsAssignableTo);

        return IsTypeAllowed(type.GetGenericArguments()[0]);
    }


    public static Type[] Verify(ParameterInfo[] parameters)
    {
        return parameters.Select(p =>
        {
            if (!IsTypeAllowed(p.ParameterType))
                throw new ArgumentException($"\"Parameter \"{p.Name}\" cannot be type {p.ParameterType}\". Allowed Types: [{String.Join(", ", AllowedTypes.GetEnumerator())}");
            return p.ParameterType;
        }).ToArray();
    }

    public static object[] Cast(Type[] parameters, MessageReader reader) => parameters.Select(p => reader.ReadDynamic(p)).ToArray();
}