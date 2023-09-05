using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [SerializeField] protected float health;
    [SerializeField] protected float damage;
    [SerializeField] protected float maxPlayerDistance;
    public abstract void Attack();
    [SerializeField] protected float attackDuration;
    public abstract void TakeDamage(float incomingDamage);
    protected abstract void Move();
    protected bool isPlayerInRange()
    {
        if (Vector3.Distance(transform.position, GameManager.GetInstance().getPlayerPosition()) <= maxPlayerDistance)
        {
            return true; 
        }

        else
        {
            return false;
        }
    }

    protected void OnDeath()
    {
        Destroy(gameObject);
    }
}
