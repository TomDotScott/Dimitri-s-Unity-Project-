using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [SerializeField] protected float health;
    [SerializeField] protected float damage;
    public abstract void Attack();
    [SerializeField] protected float attackDuration;
    public abstract void TakeDamage(float incomingDamage);
    protected abstract void Move();
}
