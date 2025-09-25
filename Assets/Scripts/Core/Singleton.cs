using System;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour    
{   
    [SerializeField]
    protected bool _destroyOnLoad = false;

    private static T _instance;
    private static readonly object _lock = new object();
    private static bool _applicationIsQuitting = false;

    public static T Instance        
    {            
        get            
        {
            if (_applicationIsQuitting)
            {
                Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' already destroyed on application quit. Won't create again - returning null.");
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<T>();

                    if (_instance == null)
                    {
                        GameObject singletonObject = new GameObject(typeof(T).Name);
                        _instance = singletonObject.AddComponent<T>();
                        Debug.Log($"[Singleton] An instance of {typeof(T)} was created automatically.");
                    }
                }

                return _instance;
            }
        }
    }    

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;

            if(!_destroyOnLoad)
                DontDestroyOnLoad(gameObject);
        }


        else if (_instance != this)
        {
            Debug.LogWarning($"[Singleton] Destroying duplicate instance of {typeof(T)}");
            Destroy(gameObject);
        }
    }

    protected virtual void OnApplicationQuit()
    {
        _applicationIsQuitting = true;
    }

    protected virtual void OnDestroy()
    {
        //if (_instance == this)
            //_applicationIsQuitting = true;
    }
}
