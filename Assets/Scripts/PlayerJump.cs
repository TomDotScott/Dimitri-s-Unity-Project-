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
    [SerializeField] private float sacrificeAmount;

    private PlayerScript playerScript; 

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerScript = GetComponent<PlayerScript>();
    }

    // Update is called once per frame
    void Update()
    {
            if (IsOnGround())
        {
            coyoteTimeCountdown = coyoteTime;
            airborne = false;
            extraJumpsValue = extraJumps;
        }
        else
        {
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

        if (extraJumpsValue > 0)
        {
            if (coyoteTimeCountdown > 0f && jumpBufferCountdown > 0f && !airborne)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpHeight);

                jumpBufferCountdown = 0f;
                airborne = true;
                extraJumpsValue--;
            }
            else if (Input.GetButtonDown(GameConstants.JUMP))
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
                extraJumpsValue--;
            }
        }
        else
        {
            if (Input.GetButtonDown(GameConstants.JUMP))
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
                extraJumpsValue--;


                playerScript.Sacrifice(sacrificeAmount);
            }
        }

        if (Input.GetButtonUp(GameConstants.JUMP) && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);

            coyoteTimeCountdown = 0f;
        }
    }
    private bool IsOnGround()
    {
        Debug.DrawRay(transform.position, Vector3.down * maxGroundDistance, Color.yellow, 0.5f);

        RaycastHit2D[] rayCastHits = Physics2D.RaycastAll(transform.position, Vector2.down, maxGroundDistance);

        return rayCastHits.Any(hit => hit.collider.gameObject.CompareTag(GameConstants.GROUND_TAG) || hit.collider.CompareTag(GameConstants.WALL_TAG));
    }

    private bool IsGrounded()
    {
        Debug.DrawRay(transform.position, Vector3.down * maxGroundDistance, Color.magenta, 0.1f);
        RaycastHit2D[] raycastHits = Physics2D.RaycastAll(transform.position, Vector3.down, maxGroundDistance);
        return raycastHits.Any(hit => hit.collider.gameObject.CompareTag(GameConstants.GROUND_TAG) || hit.collider.gameObject.CompareTag(GameConstants.WALL_TAG));
    }
}


