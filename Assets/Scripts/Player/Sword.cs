using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{       
    [SerializeField] private Animator animator;
    private bool isAttacking = false;
    [SerializeField] private float swordDamage;
    public void ResetAnimations()
    {
        isAttacking = false;

        animator.SetTrigger(GameConstants.IDLE_ANIMATION);
        animator.SetBool(GameConstants.ATTACK_FORWARDS_ANIMATION, false);
        animator.SetBool(GameConstants.ATTACK_UPWARDS_ANIMATION, false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown(GameConstants.ATTACK) && isAttacking == false)
        {
            if (Input.GetButton(GameConstants.UP_BUTTON))
            {
                animator.SetBool(GameConstants.ATTACK_UPWARDS_ANIMATION, true);
            }
            else
            {
                animator.SetBool(GameConstants.ATTACK_FORWARDS_ANIMATION, true);
            }
        }
    }

    public float GetSwordDamage()
    {
        return swordDamage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(GameConstants.ENEMY_TAG))
        {
            collision.gameObject.GetComponent<EnemyBase>().TakeDamage(swordDamage);
            GameManager.GetInstance().GetPlayer().Heal(2);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

    }
}
