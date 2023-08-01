using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    private List<EnemyBase> enemiesHit;
    [SerializeField] private Animator animator;
    private bool isAttacking = false;
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
        enemiesHit = new List<EnemyBase>();
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
            else if (Input.GetButton(GameConstants.LEFT_BUTTON) || Input.GetButton(GameConstants.RIGHT_BUTTON))
            {
                animator.SetBool(GameConstants.ATTACK_FORWARDS_ANIMATION, true);
            }
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
