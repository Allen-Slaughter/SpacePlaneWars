using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnergy : Singleton<PlayerEnergy>
{
    [SerializeField] EnegryBar enegryBar;

    public const int MAX = 100;
    public const int PERCENT = 1;
    int enegry;

    void Start()
    {
        enegryBar.Initialize(enegry, MAX);
    }

    public void Obtain(int value)
    {
        if (enegry == MAX) return;

        enegry = Mathf.Clamp(enegry + value, 0, MAX);
        enegryBar.UpdateStats(enegry, MAX);
    }

    public void Use(int value)
    {
        enegry -= value;
        enegryBar.UpdateStats(enegry, MAX);
    }

    public bool IsEnough(int value) => enegry >= value;
}
