using SG.Airlock;
using SG.Airlock.XR;
using UnityEngine;

namespace VentLib.Utilities.Extensions;

public class XRRigExtensions
{
    public static XRRig LocalPlayer()
    {
        GameStateManager stateManager = Object.FindObjectOfType<GameStateManager>();
        if (stateManager != null) return stateManager._xrRig;
        return null!;
    }
}