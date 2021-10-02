using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for any object that wants to inherit MonoBehavior and be maintained as the sole instance if itself
/// </summary>
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance;

    protected bool _markedForDestruction;

    protected void Awake()
    {
        if (Instance != null)
        {
            _markedForDestruction = true;
            Destroy(gameObject);
            return;
        }

        Instance = this as T;
        if(transform.parent == null) DontDestroyOnLoad(gameObject);
    }
}
