using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMissile : PlayerProjectileOverdirve
{
    [SerializeField] AudioData targetAcquiredVoice = null;

    [Header("==== SPEED CHANGE ====")]
    [SerializeField] float lowSpeed = 8f;
    [SerializeField] float hightSpeed = 25f;
    [SerializeField] float variableSpeedDelay = 0.5f;

    [Header("==== EXPLOSION ====")]
    [SerializeField] GameObject explosionVFX = null;
    [SerializeField] AudioData explosionSFX = null;
    [SerializeField] LayerMask enemyLayerMask = default;
    [SerializeField] float explosionRadius = 3f;
    [SerializeField] float explosionDamage = 100f;

    WaitForSeconds waitVariableSpeedDelay;

    protected override void Awake()
    {
        base.Awake();
        waitVariableSpeedDelay = new WaitForSeconds(variableSpeedDelay);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(nameof(VariableSpeedCoroutine));
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        //Spawn a explosion VFX
        PoolManager.Release(explosionVFX, transform.position);
        //Play explosion SFX
        AudioManager.Instance.PlayRandomSFX(explosionSFX);
        //Enemies in explosion take AOE damage
        var colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, enemyLayerMask);

        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.TakeDamage(explosionDamage);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    IEnumerator VariableSpeedCoroutine()
    {
        moveSpeed = lowSpeed;

        yield return waitVariableSpeedDelay;

        moveSpeed = hightSpeed;

        if (target != null)
        {
            AudioManager.Instance.PlayRandomSFX(targetAcquiredVoice);
        }
    }
}
