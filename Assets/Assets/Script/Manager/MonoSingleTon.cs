using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MonoSingleTon<T>: MonoBehaviourPunCallbacks where T: class, new()
{
    private static T inst = null;
    private static object _lock = new object();  
    public static T INSTANCE
    {
        get 
        {
            if (inst == null)
            {
                inst = GameObject.FindObjectOfType(typeof(T)) as T;//find
                if (inst == null)
                {
                    lock (_lock)//single thread
                    {
                        GameObject newInst = new GameObject(typeof(T).ToString(), typeof(T));
                        inst = newInst.GetComponent<T>();
                    }
                }
            }
            return inst;
        }
    }
}
