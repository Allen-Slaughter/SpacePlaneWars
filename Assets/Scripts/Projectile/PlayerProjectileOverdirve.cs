using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileOverdirve : PlayerProjectile
{
    [SerializeField] ProjectileGuidanceSystem guidanceSystem;
    protected override void OnEnable()
    {
        SetTarget(EnemyManager.Instance.RandomEnemy);
        transform.rotation = Quaternion.identity;

        if (target == null)
        {
            base.OnEnable();
        }
        else
        {
            //Track target
            StartCoroutine(guidanceSystem.HomingCoroutine(target));
        }
    }
}
