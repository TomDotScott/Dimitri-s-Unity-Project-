using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerJump : MonoBehaviour
{
    [SerializeField] private float maxGroundDistance;
    [SerializeField] private float jumpHeight;
    private Rigidbody2D rb;

    [SerializeField] private float coyoteTime;
    private float coyoteTimeCountdown;
    [SerializeField] private float jumpBuffer;
    private float jumpBufferCountdown;
    private bool airborne;
    public int extraJumps;
    private int extraJumpsValue;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsGrounded())
        {
            Debug.Log("Grounded;)");
            coyoteTimeCountdown = coyoteTime;
            airborne = false;
            extraJumps = extraJumpsValue;
            
        }
        else
        {
            Debug.Log("Airborne");
            coyoteTimeCountdown -= Time.deltaTime;
        }

        if (Input.GetButtonDown(GameConstants.JUMP))
        {
            jumpBufferCountdown = jumpBuffer;
        }
        else
        {
            jumpBufferCountdown -= Time.deltaTime;
        }



        if (coyoteTimeCountdown > 0 && jumpBufferCountdown > 0 && !airborne)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
            airborne = true;
            jumpBufferCountdown = 0;
        }

    }

    private bool IsGrounded()
    {
        Debug.DrawRay(transform.position, Vector3.down * maxGroundDistance, Color.magenta, 0.1f);
        RaycastHit2D[] raycastHits = Physics2D.RaycastAll(transform.position, Vector3.down, maxGroundDistance);
        return raycastHits.Any(hit => hit.collider.gameObject.CompareTag("Ground"));
    }
}


