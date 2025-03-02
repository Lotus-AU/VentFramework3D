using SG.Airlock;
using SG.Airlock.XR;
using UnityEngine;
using VentLib.Vanilla.Behaviour;

namespace VentLib.Utilities.Extensions;

public static class XRRigExtensions
{
    public static XRRig LocalPlayer()
    {
        GameStateManager stateManager = GameStateBehaviour.GetManager();
        if (stateManager != null) return stateManager._xrRig;
        return null!;
    }

    public static void SnapTo(this XRRig localPlayer, Vector3 position)
    {
        GameObject temp = new GameObject("Temp");
        temp.transform.position = position;
        localPlayer.MoveRigToPosition(temp.transform, true, false);
        temp.Destroy();
    }
}