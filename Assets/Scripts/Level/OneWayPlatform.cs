using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    private bool playerOnTop = false;

    private PlatformEffector2D effector;
    public float platformWaitTime;

    // Start is called before the first frame update
    void Start()
    {
        effector = GetComponent<PlatformEffector2D>();
    }

    // Update is called once per frame
    void Update()
    {
        platformWaitTime -= Time.deltaTime;

        if (playerOnTop == true)
        {

            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                platformWaitTime = 0.5f;
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                effector.rotationalOffset = 180f;
                platformWaitTime = 0.5f;
            }
        }

        if (platformWaitTime <= 0)
        {
            effector.rotationalOffset = 0f;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(GameConstants.PLAYER_TAG))
        {
            playerOnTop = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(GameConstants.PLAYER_TAG))
        {
            playerOnTop = false;
        }
    }
}
