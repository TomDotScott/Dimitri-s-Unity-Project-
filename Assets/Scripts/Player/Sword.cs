using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    private List <EnemyBase> enemiesHit;
    [SerializeField] private Animator animator;
    private bool isAttacking = false;
    public void Reset()
    {
        isAttacking = false;
        animator.SetTrigger(GameConstants.IDLE);
    }

    // Start is called before the first frame update
    void Start()
    {
        enemiesHit = new List<EnemyBase>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown(GameConstants.ATTACK) && isAttacking == false)
        {
            animator.SetBool("AttackForwards", true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // if (collision.gameObject.CompareTag(GameConstants.))
        {

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

    }
}
