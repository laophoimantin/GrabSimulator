using System.Collections.Generic;
using UnityEngine;

public enum InputActionType 
{
    Move,
    Jump,
    Attack,
    Interact,
    
    BikeMove,
    BikeBrake,
    
    UI
}

public static class InputLocker
{
    private static readonly Dictionary<InputActionType, HashSet<object>> _locks = new();

    static InputLocker()
    {
        foreach (InputActionType type in System.Enum.GetValues(typeof(InputActionType)))
        {
            _locks[type] = new HashSet<object>();
        }
    }

    public static void Lock(InputActionType type, object source)
    {
        _locks[type].Add(source);
    }

    public static void Unlock(InputActionType type, object source)
    {
        _locks[type].Remove(source);
    }

    public static bool IsLocked(InputActionType type)
    {
        return _locks[type].Count > 0;
    }
    public static void ForceClearAll()
    {
        foreach (var hashSet in _locks.Values)
        {
            hashSet.Clear();
        }
    }
}