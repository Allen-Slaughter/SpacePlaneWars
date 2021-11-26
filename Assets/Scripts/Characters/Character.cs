using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("---- DEATH ----")]
    [SerializeField] GameObject deathVFX;
    [SerializeField] AudioData[] deathSFX;

    [Header("---- HEALTH ----")]
    [SerializeField] protected float maxHealth;

    [SerializeField] bool showOnHeadHealthBar = true;
    [SerializeField] StatsBar onHeadHealthBar;

    protected float health;

    protected virtual void OnEnable()
    {
        health = maxHealth;

        if (showOnHeadHealthBar)
        {
            ShowOnHeadHealthBar();
        }
        else
        {
            HideOnHeadHealthBar();
        }
    }

    public void ShowOnHeadHealthBar()
    {
        onHeadHealthBar.gameObject.SetActive(true);
        onHeadHealthBar.Initialize(health, maxHealth);
    }
    public void HideOnHeadHealthBar()
    {
        onHeadHealthBar.gameObject.SetActive(false);
    }

    public virtual void TakeDamage(float damage)
    {
        health -= damage;

        if (showOnHeadHealthBar && gameObject.activeSelf)
        {
            onHeadHealthBar.UpdateStats(health, maxHealth);
        }

        if (health <= 0f)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        health = 0f;
        AudioManager.Instance.PlayRandomSFX(deathSFX);
        PoolManager.Release(deathVFX, transform.position);
        gameObject.SetActive(false);
    }

    public virtual void RestoreHealth(float value)
    {
        if (health == maxHealth) return;

        health = Mathf.Clamp(health + value, 0f, maxHealth);

        if (showOnHeadHealthBar)
        {
            onHeadHealthBar.UpdateStats(health, maxHealth);
        }
    }

    //生命再生协程
    protected IEnumerator HealthRegenerateCoroutine(WaitForSeconds waitTime, float percent)
    {
        while (health < maxHealth)
        {
            yield return waitTime;

            RestoreHealth(maxHealth * percent);
        }
    }

    protected IEnumerator DamageOveTimeCoroutine(WaitForSeconds waitTime, float percent)
    {
        while (health > 0f)
        {
            yield return waitTime;

            RestoreHealth(maxHealth * percent);
        }
    }
}
