using Fusion;
using SG.Airlock;
using SG.Airlock.Network;
using UnityEngine;
using VentLib.Utilities.Extensions;
using VentLib.Vanilla.Behaviour;

namespace VentLib.Utilities;

public class NetUtils
{
    public static float DeriveDelay(float flatDelay, float multiplier = 0.0003f) => (float)NetworkRunnerBehaviour.GetPing() * multiplier + flatDelay;
}