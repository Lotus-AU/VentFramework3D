using Fusion;
using SG.Airlock;
using SG.Airlock.Network;
using UnityEngine;
using VentLib.Utilities.Extensions;

namespace VentLib.Utilities;

public class NetUtils
{
    public static float DeriveDelay(float flatDelay, float multiplier = 0.0003f)
    {
        PingManager pingManager = Object.FindObjectOfType<PingManager>();
        if (pingManager == null) return flatDelay;
        return pingManager.GetPing(XRRigExtensions.LocalPlayer().PState.PlayerId) * multiplier + flatDelay;
    }
}