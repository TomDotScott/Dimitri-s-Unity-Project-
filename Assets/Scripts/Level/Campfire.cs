using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire : MonoBehaviour
{
    public bool withinCampfireRadius;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(GameConstants.PLAYER_TAG))
        {
            PlayerScript player = collision.gameObject.GetComponent<PlayerScript>();
            player.FullHeal();
            withinCampfireRadius = true;
            GameManager.GetInstance().SetCampfireRespawnPosition(transform.position);
        }
    }
}
