using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spitfire : EnemyBase
{

    [SerializeField] private Fireball fireball;
    [SerializeField] private bool shouldAttack;
    [SerializeField] private float fireballLifetime; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
            newFireball.GetComponent<Fireball>().Init(Vector3.right, damage, fireballLifetime);
            yield return new WaitForSeconds(0.1f);
         }

        yield return new WaitForSeconds(attackDuration);
        shouldAttack = true;
    }

    public override void TakeDamage(float incomingDamage)
    {
        throw new System.NotImplementedException();
    }

    protected override void Move()
    {
        throw new System.NotImplementedException();
    }
}
