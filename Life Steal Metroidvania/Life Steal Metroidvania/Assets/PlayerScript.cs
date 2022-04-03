using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{

    // Variable Uses
    // Int (Integer) - Used when you need a whole number
    // Float - Used when you need a full number or a decimal. Integer is faster, so we only use float when we NEED access to a decimal
    // Bool (Bullion) - Used when you need true or false
    // String - Used when you need text
    // Variable (Var) - Can be used in any of the above circumstances

    #region VARIABLES

    // Establishing Variables
    #region MOVEMENT_VARIABLES
    [Header("Movement Variables")]
    // Establishing moveSpeed variable
    [Range(10f, 200f)] public float moveSpeed;

    // Establishing jump height variable
    [Range(10f, 100f)] public float jumpHeight;

    // Dash Variables
    [Range(0f, 100f)] public float dashSpeed;

    [Range(0f, 1f)] public float startDashTime;
    private float dashTime = 0f;

    private Vector2 dashDirection = Vector2.zero;

    public int dashCount;
    public int dashCountValue;

    #endregion

    // Used to list abilities
    public enum ePlayerState // TODO: USE THIS INSTEAD OF THE HUNDREDS OF BOOLS!
    {
        idle,
        walking,
        jumping,
        dashing,
        wallClinging,

    }

    [Header("Player State")]
    public ePlayerState playerState;

    #region TO_BE_REPLACED_WITH_STATE
    // TODO: To be replaced with Player State stuff in future...
    private bool isDashing = false;

    // Establishing wall detection for the wall climb ability
    private bool onWall;
    private bool isWallClinging;

    #endregion

    // Establishing the Rigidbody variable
    private Rigidbody2D rb;


    // Establishing the facingRight variable
    private bool facingRight = true;

    #region JUMP_VARIABLES
    [Header("Jumping Variables")]
    // Establishing ground detection for jump and double jump etc
    private bool isGrounded;

    // Establishing jumping and double jumping abilities
    [SerializeField] private int extraJumps;

    public int extraJumpsValue;

    // Glide
    private bool isGliding;

    #endregion

    private bool onTopOfWall;

    private float gravityScale;
    private float mass;



    #region HEALTH_VARIABLES
    [Header("Player Health")]
    // Health
    [Range(0f, 100)] public float totalHealthValue = 20;
    [SerializeField] private float currentHealthValue;

    private bool takingDamage = false;

    private bool touchingLava;

    // Establishing death
    [SerializeField] private bool isPlayerDead;
    #endregion

    private bool beastMode;

    [Header("Coyote Time and Jump Buffering")]
    // Hang Time
    [Range(0f, 2f)] public float hangTime = .2f;
    private float hangCounter;

    [Range(0f, 2f)] public float jumpBufferLength = .1f;
    private float jumpBufferCount;
    #endregion



    // Start is called before the first frame update
    void Start()
    {
        // Giving the player Rigidbody (Making them a solid, physics based object)
        rb = GetComponent<Rigidbody2D>();
        dashDirection = Vector2.zero;
        extraJumps = extraJumpsValue;
        dashCount = dashCountValue;
        dashTime = startDashTime;
        gravityScale = 7.5f;
        mass = 10f;
        onTopOfWall = false;
        hangCounter = hangTime;
        currentHealthValue = totalHealthValue;
        isPlayerDead = false;
        takingDamage = false;
        beastMode = false;
        touchingLava = false;
    }


    // Update is called once per frame
    void Update()
    {
        if (isGrounded == true)
        {
            extraJumps = extraJumpsValue;
            dashCount = dashCountValue;
        }


        //// Hang Time (Coyote Time)
        //if (isGrounded == true)
        //{
        //    hangCounter = hangTime;
        //}
        //else
        //{
        //    hangCounter -= Time.deltaTime;
        //}

        //// Jump Buffer
        //if (Input.GetKeyDown(KeyCode.UpArrow))
        //{
        //    jumpBufferCount = jumpBufferLength;
        //}
        //else
        //{
        //    jumpBufferCount -= Time.deltaTime;
        //}



        // Dash Code
        if (isGrounded == true)
        {
            dashCount = dashCountValue;
        }

        #region DASH_CODE
        // Input for Dash
        if (dashDirection == Vector2.zero)
        {
            if (dashCount > 0)
            {
                // Cleaning this up so we can use arrows or WASD using Input.GetAxis
                bool right = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
                bool left = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
                bool up = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
                bool down = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);

                if (right && Input.GetKeyDown(KeyCode.Z))
                {
                    Debug.Log("RIGHT");
                    Dash(new Vector2(1, 0));
                }

                if (left && Input.GetKeyDown(KeyCode.Z))
                {
                    Debug.Log("LEFT");
                    Dash(new Vector2(-1, 0));
                }

                if (up && Input.GetKeyDown(KeyCode.Z))
                {
                    Debug.Log("UP");
                    Dash(new Vector2(0, 1));
                }

                if (down && Input.GetKeyDown(KeyCode.Z))
                {
                    Debug.Log("DOWN");
                    Dash(new Vector2(0, -1));
                }

                if (Input.GetKeyDown(KeyCode.Z))
                {
                    if (!isDashing)
                    {
                        if (facingRight == true)
                        {
                            Debug.Log("JUST DASHING RIGHT");

                            Dash(new Vector2(1, 0));
                        }
                        else
                        {
                            Debug.Log("JUST DASHING LEFT");
                            Dash(new Vector2(-1, 0));
                        }
                    }
                }

                // After taking in all the inputs, determine if we have dashed. If we have, subtract one from the 
                // dashCount 
                if (isDashing)
                {
                    dashCount--;
                }
            }
        }

        else
        {
            if (dashTime <= 0)
            {
                dashTime = startDashTime;
                ResetDash();
            }
            else
            {
                dashTime -= Time.deltaTime;
            }
        }
        #endregion

        #region JUMP_CODE
        // Jump Code
        hangCounter -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded == true)
            {
                Jump();


            }

            else if (extraJumps > 0)
            {
                Jump();
                extraJumps--;
            }

            else if (hangCounter >= 0)
            {
                Jump();
                isGrounded = false;
            }
        }

     




        // Hang Time (Coyote Time)
        if (isGrounded == true)
        {
            hangCounter = hangTime;
        }
        else
        {
            hangCounter -= Time.deltaTime;
        }

        // Jump Buffer
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferCount = jumpBufferLength;
        }
        else
        {
            jumpBufferCount -= Time.deltaTime;
        }
        

        // Glide Code
        if (Input.GetKey(KeyCode.UpArrow) && isGrounded == false)
        {
            isGliding = true;
        }
        else
        {
            isGliding = false;
        }

        if (isGliding == true)
        {
            mass = 5f;
        }

        if (isGliding == false)
        {
            mass = 10f;
        }


        // Wall Jump + Wall Resource Restoration
        if (onWall == true)
        {
            extraJumps = extraJumpsValue;
            dashCount = dashCountValue;
        }
        #endregion

        #region MOVEMENT_CODE
        // Moving input
        float moveInput = Input.GetAxisRaw("Horizontal");

        if (onWall == true)
        {
            if (Input.GetKey(KeyCode.X))
            {
                float upMovement = Input.GetAxisRaw("Vertical");
                rb.velocity = new Vector2(rb.velocity.x, upMovement * moveSpeed);
                rb.gravityScale = 0f;
            }
            else
            {
                rb.gravityScale = 7f;
            }
        }
        else
        {
            if (rb.gravityScale == 0f)
            {
                rb.gravityScale = 7;
            }
        }

        // Making player walk at the moveSpeed variable
        if (isDashing == false)
        {
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        }
        else
        {
            // Debug.Log("DASHING!");
            if (dashDirection.x != 0 && dashDirection.y != 0)
            {
                Debug.Log("DASHING DIAGONALLY!");
                rb.velocity = dashDirection * WorkOutDiagonalDashSpeed();
            }
            else
            {
                rb.velocity = dashDirection * dashSpeed;
            }
        }

        // Turns around if they change walking direction
        if ((facingRight == false && moveInput > 0) || (facingRight == true && moveInput < 0))
        {
            Flip();
        }
        #endregion


        // Beast Mode Code
        if (Input.GetKeyDown(KeyCode.S) && currentHealthValue > 1 && beastMode == false){
            ActivateBeastMode();
        }

        if (beastMode == true){
            if (currentHealthValue <= 0){
                beastMode = false;
            }
        }

        if (beastMode == true)
        {
            // damageDealt * 2;
            // currentHealthValue - 2 * Time.deltaTime;
        }
    }


    private float WorkOutDiagonalDashSpeed()
    {
        return Mathf.Sqrt((dashSpeed * dashSpeed) / 2); ;
    }

    // Flip functions
    private void Flip()
    {
        if (isDashing == false)
        {
             facingRight = !facingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }

    }

    // Dash function
    private void Dash(Vector2 direction)
    {
        if (dashCount > 0)
        {
            dashDirection += direction;
            isDashing = true;
        }
    }

    // Resetting dash
    public void ResetDash()
    {
        dashDirection = Vector2.zero;
        rb.velocity = Vector2.zero;
        isDashing = false;
        ResetDoubleJump();
    }

    // Resetting double jump
    private void ResetDoubleJump()
    {
        extraJumps = extraJumpsValue;
    }

    // Jump function
    private void Jump()
    {
        if (!isDashing)
        {
            rb.velocity = Vector2.up * jumpHeight;
            isGrounded = false;
        }

    }

    private void Glide(){
        isGliding = true;
    }


    private void ActivateBeastMode(){
        beastMode = true;
    }


    public void Burn(){
        // Damage over time code here
    }
     
    // Killing and respawing the player
    public void KillPlayer()
    {
        isPlayerDead = true;
        extraJumps = extraJumpsValue;
        dashCount = dashCountValue;
        currentHealthValue = totalHealthValue;
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealthValue -= damageAmount;
    }

    public float GetPlayerHealth()
    {
        return currentHealthValue;
    }

    public float GetMaxPlayerHealth()
    {
        return totalHealthValue;
    }

    #region UNITY_EVENTS

    // OnCollisionEnter is a Unity Event that gets called the first frame a collision happens between two objects.
    private void OnCollisionEnter2D(Collision2D collision) // Collision is an input for our function. It exists only in CollisionEnter2D, making it a local variable //
    {
        switch (collision.gameObject.tag)
        {
            case "Ground":
                Debug.Log("Ground Collision Detected");
                isGrounded = true;
                ResetDash();
                dashCount = dashCountValue;
                break;
            case "Wall":
                {
                    // 0 = Top, 1 = Right Side, 2 = Bottom, 3 = Left Side (Travels clockwise, +1 per side) of polygon
                    List<ContactPoint2D> contactPoints = new List<ContactPoint2D>();
                    collision.GetContacts(contactPoints);
                    if (contactPoints[0].normal.y == 1)
                    {
                        isGrounded = true;
                        onTopOfWall = true;
                        onWall = false;
                        isWallClinging = false;

                        ResetDash();
                        dashCount = dashCountValue;
                    }

                    if (contactPoints[1].normal.x == 1)
                    {
                        onWall = true;
                    }

                    if (transform.position.x <= collision.gameObject.transform.position.x)
                    {
                        onWall = true;
                    }

                    break;
                }
            case "Enemy":
                Debug.Log("Enemy Collision Detected");
                TakeDamage(totalHealthValue / 10f);
                break;
        }
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            if (onTopOfWall == true)
            {
                isGrounded = false;
            }
            onWall = false;
            onTopOfWall = false;
        }

        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = false;

            hangCounter = hangTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Hit the climbing area");
    }
#endregion
}