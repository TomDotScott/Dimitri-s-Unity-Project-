using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spitfire : EnemyBase
{

    [SerializeField] private Fireball fireball;
    [SerializeField] private bool shouldAttack;
    [SerializeField] private float fireballLifetime; 
    [SerializeField] private float fireballDelay;
    private Vector3 attackDirection;
    [SerializeField] private bool shouldTrackPlayer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldTrackPlayer == true) {
            RaycastHit2D leftHit = Physics2D.Raycast(transform.position, new Vector2(-1, 0), Mathf.Infinity, ~LayerMask.NameToLayer(GameConstants.ENEMY_LAYER));
            Debug.DrawRay(transform.position, new Vector2(-1, 0) * 10, Color.red, 0.016f);
            Debug.Log(leftHit.collider.gameObject.name);
            RaycastHit2D rightHit = Physics2D.Raycast(transform.position, new Vector2(1, 0), Mathf.Infinity, ~LayerMask.NameToLayer(GameConstants.ENEMY_LAYER));
            Debug.DrawRay(transform.position, new Vector2(1, 0) * 10, Color.red, 0.016f);
            Debug.Log(rightHit.collider.gameObject.name);
            if (/*leftHit.collider != null &&*/ leftHit.collider.gameObject.CompareTag(GameConstants.PLAYER_TAG))
            {
                shouldAttack = true;
            }

            else if (/*rightHit.collider != null &&*/ rightHit.collider.gameObject.CompareTag(GameConstants.PLAYER_TAG))
            {
                shouldAttack = true;
                Debug.Log(rightHit.collider.gameObject.name);
            }

            else
            {
                shouldAttack = false;
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
            newFireball.GetComponent<Fireball>().Init(Vector3.right, damage, fireballLifetime);
            yield return new WaitForSeconds(fireballDelay);
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
