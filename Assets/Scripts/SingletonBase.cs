using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonBase<T> : MonoBehaviour where T: MonoBehaviour
{
    // Check to see if we're about to be destroyed.
    public static bool shuttingDown = false;
    private static object lockObj = new object();
    private static T instance;

    /// <summary>
    /// Access singleton instance through this propriety.
    /// </summary>
    public static T Instance
    {
        get
        {
            if (shuttingDown)
            {
                Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                    "' already destroyed. Returning null.");
                return null;
            }

            lock (lockObj)
            {
                if (instance == null)
                {
                    // Search for existing instance.
                    instance = (T)FindObjectOfType(typeof(T));

                    // Create new instance if one doesn't already exist.
                    if (instance == null)
                    {
                        // Need to create a new GameObject to attach the singleton to.
                        var singletonObject = new GameObject();
                        instance = singletonObject.AddComponent<T>();
                        singletonObject.name = typeof(T).ToString() + " (Singleton)";

                        // Make instance persistent.
                        DontDestroyOnLoad(singletonObject);
                    }
                    else
                    {
                        DontDestroyOnLoad(instance.gameObject);
                    }
                }

                return instance;
            }
        }
    }

  

    public void OnApplicationQuit()
    {
        instance = null;
        shuttingDown = true;
    }


    public void OnDestroy()
    {
        instance = null;
        shuttingDown = true;
    }

    public void Reload()
    {
        shuttingDown = false;
    }

}
