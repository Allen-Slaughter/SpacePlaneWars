using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileSystem : MonoBehaviour
{
    [SerializeField] int defaultAmount = 5;
    [SerializeField] float cooldownTime = 1f;
    [SerializeField] GameObject missilePrefab = null;
    [SerializeField] AudioData launchSFX = null;

    bool isReady = true;

    int amount;

    void Awake()
    {
        amount = defaultAmount;
    }

    void Start()
    {
        MissileDisplay.UpdateAmountText(amount);
    }

    public void Launch(Transform muzzleTransform)
    {
        if (amount == 0 || !isReady) return;        //TODO: Add SFX && UI VFX here

        isReady = false;

        //Release a missile clone from object pool
        PoolManager.Release(missilePrefab, muzzleTransform.position);
        //Play missile launch SFX
        AudioManager.Instance.PlayRandomSFX(launchSFX);
        amount--;
        MissileDisplay.UpdateAmountText(amount);

        if (amount == 0)
        {
            MissileDisplay.UpdateCooldownImage(1f);
        }
        else
        {
            //cooldown missile launching
            StartCoroutine(CooldownCoroutine());
        }
    }

    IEnumerator CooldownCoroutine()
    {
        //yield return new WaitForSeconds(cooldownTime);
        var cooldownValue = cooldownTime;

        while (cooldownValue > 0f)
        {
            MissileDisplay.UpdateCooldownImage(cooldownValue / cooldownTime);
            cooldownValue = Mathf.Max(cooldownValue - Time.deltaTime, 0f);

            yield return null;
        }

        isReady = true;
    }
}
