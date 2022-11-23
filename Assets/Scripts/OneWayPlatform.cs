using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    bool jumpButtonHeld = Input.GetButton(GameConstants.JUMP) || Input.GetKey(KeyCode.Space);

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

        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            platformWaitTime = 0.5f;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            effector.rotationalOffset = 180f;
            platformWaitTime = 0.5f;
        }

        if (platformWaitTime <= 0)
        {
            effector.rotationalOffset = 0f;
        }
    }
}
