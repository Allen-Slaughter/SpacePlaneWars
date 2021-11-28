using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField] int scorePoint = 100;
    [SerializeField] int deathEnegryBonus = 3;

    public override void Die()
    {
        ScoreManager.Instance.AddScore(scorePoint);
        PlayerEnergy.Instance.Obtain(deathEnegryBonus);
        EnemyManager.Instance.RemoveFromList(gameObject);
        base.Die();
    }
}
