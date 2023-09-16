using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spitfire : EnemyBase
{

    [SerializeField] private Fireball fireball;
    [SerializeField] private bool shouldAttack = true;
    [SerializeField] private float fireballLifetime; 
    [SerializeField] private float fireballDelay;
    [SerializeField] private bool shouldShootLeft;
    [SerializeField] private bool shouldTrackPlayer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerInRange() && shouldTrackPlayer)
        {
            if (transform.position.x > GameManager.GetInstance().GetPlayerPosition().x)
            {
                shouldShootLeft = true; 
            }

            else
            {
                shouldShootLeft = false; 
            }
        }

        if (shouldAttack == true)
        {
            Attack();
        }
    }

    public override void Attack()
    {
        StartCoroutine(ThrowFireball());
    }

    private IEnumerator ThrowFireball()
    {
        shouldAttack = false;
        for (int i = 0; i < 3; i++)
        {


            GameObject newFireball = Instantiate(fireball.gameObject, transform.position, Quaternion.identity);
            Vector3 fireballDirection = Vector3.right;
            if (shouldShootLeft)
            {
                fireballDirection = Vector3.left;
            }
            newFireball.GetComponent<Fireball>().Init(fireballDirection, damage, fireballLifetime);
            yield return new WaitForSeconds(fireballDelay);
         }

        yield return new WaitForSeconds(attackDuration);
        shouldAttack = true;
    }

    public override void TakeDamage(float incomingDamage)
    {
        health -= incomingDamage;
        if (health <= 0)
        {
            OnDeath();
        }
    }

    protected override void Move()
    {
        throw new System.NotImplementedException();
    }
}
