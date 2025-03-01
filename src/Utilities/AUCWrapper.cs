using System.Collections;
using System.Collections.Generic;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using HarmonyLib;
using UnityEngine;

namespace VentLib.Utilities;

internal class AUCWrapper
{
    internal static AUCWrapper? Instance;
    private List<IEnumerator> coroutines = new();
    private Coroutines _runner;
    
    public AUCWrapper()
    {
        Instance = this;
        _runner = Vents.Instance.AddComponent<Coroutines>();
    }

    public bool StartCoroutine(IEnumerator coroutine, out Coroutine? coroutineHandle)
    {
        coroutineHandle = null;
        if (_runner != null) 
            coroutineHandle = _runner.StartCoroutine(coroutine.WrapToIl2Cpp());
        else
            coroutines.Add(coroutine);
        return _runner;
    }

    public bool StartCoroutine(IEnumerator coroutine)
    {
        if (_runner != null) _runner.StartCoroutine(coroutine.WrapToIl2Cpp());
        else coroutines.Add(coroutine);
        return _runner;
    }

    internal void RunCached()
    {
        coroutines.Do(coroutine => _runner.StartCoroutine(coroutine.WrapToIl2Cpp()));
    }
    
    private class Coroutines: MonoBehaviour {}
}