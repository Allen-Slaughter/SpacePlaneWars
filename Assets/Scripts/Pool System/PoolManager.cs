using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [SerializeField] Pool[] playerProjectilePools;

    static Dictionary<GameObject, Pool> dictionary;

    void Start()
    {
        dictionary = new Dictionary<GameObject, Pool>();

        Initialize(playerProjectilePools);
    }

#if UNITY_EDITOR
    void OnDestroy()
    {
        CheckPoolSize(playerProjectilePools);
    }
#endif

    void CheckPoolSize(Pool[] pools)
    {
        foreach (var pool in pools)
        {
            if (pool.RuntimeSize > pool.Size)
            {
                Debug.LogError(
                    string.Format("Pool: {0} has a runtime size {1} bigger than its initial size {2}!",
                    pool.Prefab, name,
                    pool.RuntimeSize,
                    pool.Size));
            }
        }
    }

    public void Initialize(Pool[] pools)
    {
        foreach (var pool in pools)
        {
#if UNITY_EDITOR
            if (dictionary.ContainsKey(pool.Prefab))
            {
                Debug.LogError("Same prefab in multiple pools! Prefab: " + pool.Prefab.name);

                continue;
            }
#endif
            dictionary.Add(pool.Prefab, pool);

            Transform poolParent = new GameObject("Pool " + pool.Prefab.name).transform;

            poolParent.parent = transform;
            pool.Initialize(poolParent);
        }
    }

    /// <summary>
    /// <para>Return a specified<paramref name="prefab"></paramref>gameObject in the pool.</para>
    /// <para>根据传入的<paramref name="prefab"></paramref>参数.返回对象池中预备好的游戏对象</para>
    /// </summary>
    /// <param name="prefab">
    /// <para>Specified gameObject prefab.</para>
    /// <para>指定的游戏对象预制体.</para>
    /// </param>
    /// <returns>
    /// <para>Prepared gameObject in the pool.</para>
    /// <para>对象池中预备好的游戏对象.</para>
    /// </returns>
    public static GameObject Release(GameObject prefab)
    {
#if UNITY_EDITOR
        if (!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("Pool Manager could NOT find prefab: " + prefab.name);

            return null;
        }
#endif
        return dictionary[prefab].PreparedObject();
    }

    public static GameObject Release(GameObject prefab, Vector3 position)
    {
#if UNITY_EDITOR
        if (!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("Pool Manager could NOT find prefab: " + prefab.name);

            return null;
        }
#endif
        return dictionary[prefab].PreparedObject(position);
    }

    public static GameObject Release(GameObject prefab, Vector3 position, Quaternion rotation)
    {
#if UNITY_EDITOR
        if (!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("Pool Manager could NOT find prefab: " + prefab.name);

            return null;
        }
#endif
        return dictionary[prefab].PreparedObject(position, rotation);
    }

    public static GameObject Release(GameObject prefab, Vector3 position, Quaternion rotation, Vector3 localScale)
    {
#if UNITY_EDITOR
        if (!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("Pool Manager could NOT find prefab: " + prefab.name);

            return null;
        }
#endif
        return dictionary[prefab].PreparedObject(position, rotation, localScale);
    }
}
