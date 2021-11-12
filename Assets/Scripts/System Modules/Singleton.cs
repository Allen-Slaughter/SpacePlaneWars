using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//泛型单例
public class Singleton<T> : MonoBehaviour where T : Component
{
    //声明一个公开的静态泛型属性Instance实例
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        Instance = this as T;
    }
}
