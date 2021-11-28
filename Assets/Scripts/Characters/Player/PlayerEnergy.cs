using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnergy : Singleton<PlayerEnergy>
{
    [SerializeField] EnegryBar enegryBar;
    [SerializeField] float overdirveInterval = 0.1f;

    bool available = true;

    public const int MAX = 100;
    public const int PERCENT = 1;
    int enegry;

    WaitForSeconds waitForOverdirveInterval;

    protected override void Awake()
    {
        base.Awake();
        waitForOverdirveInterval = new WaitForSeconds(overdirveInterval);
    }

    void OnEnable()
    {
        PlayerOverdirve.on += PlayerOverdirveOn;
        PlayerOverdirve.off += PlayerOverdirveOff;
    }

    void OnDisable()
    {
        PlayerOverdirve.on -= PlayerOverdirveOn;
        PlayerOverdirve.off -= PlayerOverdirveOff;
    }

    void Start()
    {
        enegryBar.Initialize(enegry, MAX);
    }

    public void Obtain(int value)
    {
        if (enegry == MAX || !available || !gameObject.activeSelf) return;

        enegry = Mathf.Clamp(enegry + value, 0, MAX);
        enegryBar.UpdateStats(enegry, MAX);
    }

    public void Use(int value)
    {
        enegry -= value;
        enegryBar.UpdateStats(enegry, MAX);

        if (enegry == 0 && !available)
        {
            PlayerOverdirve.off.Invoke();
        }
    }

    public bool IsEnough(int value) => enegry >= value;

    void PlayerOverdirveOn()
    {
        available = false;
        StartCoroutine(nameof(KeepUsingCoroutine));
    }

    void PlayerOverdirveOff()
    {
        available = true;
        StopCoroutine(nameof(KeepUsingCoroutine));
    }

    IEnumerator KeepUsingCoroutine()
    {
        while (gameObject.activeSelf && enegry > 0)
        {
            //Every 0.1 seconds
            yield return waitForOverdirveInterval;

            //use 1% of max enegry,every 1 second use 10% of max enegry

            //Means that overdirve last for 10 seconds
            Use(PERCENT);
        }
    }
}
