using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScript1 : MonoBehaviour
{

    // Variable Uses
    // Int (Integer) - Used when you need a whole number
    // Float - Used when you need a full number or a decimal. Integer is faster, so we only use float when we NEED access to a decimal
    // Bool (Bullion) - Used when you need true or false
    // String - Used when you need text
    // Variable (Var) - Can be used in any of the above circumstances




    // Establishing Variables

    // Used to list abilities
    public enum ePlayerState {
        idle,
        walking,
        jumping,
        dashing,
        wallClinging,

    }

    public ePlayerState playerState;

    private enum eDashDirection {
        up,
        down,
        left,
        right,

        none,

    }

    // Establishing move speed variable
    public float speed;

    // Establishing jump height variable
    public float jumpForce;


    // Establishing the moveInput variable
    private float moveInput;


    // Establishing the Rigidbody variable
    private Rigidbody2D rb;


    // Establishing the facingRight variable
    private bool facingRight = true;


    // Establishing ground detection for jump and double jump etc
    private bool isGrounded;
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask whatIsGround;

    // Establishing wall detection for the wall climb ability
    private bool onWall;
    // Establishing a check for the presence of a wall
    public Transform wallCheck;
    // Establishing what is considered a wall
    public LayerMask whatIsWall;

    private bool isWallClinging;


    // Establishing jumping and double jumping abilities
    private int extraJumps;
    public int extraJumpsValue;


    private bool hasWallJumpOcc = false;


    // Dash Variables
    public float dashSpeed;
    private float dashTime;
    public float startDashTime;
    private eDashDirection dashDirection;

    public float extraDashValue;
    private float extraDashes;













    // Hang Time
    public float hangTime = .2f;
    private float hangCounter;

    public float jumpBufferLength = .1f;
    private float jumpBufferCount;



    // Establishing x and y variables
    float x = Input.GetAxisRaw("Horizontal");
    float y = Input.GetAxisRaw("Vertical");










    // Start is called before the first frame update
    void Start()
    {

        // Giving the player Rigidbody (Making them a solid, physics based object)
        rb = GetComponent<Rigidbody2D>();

        dashDirection = eDashDirection.none;

        // Extra jumps begins at the Extra Jumps Value (1)
        extraJumps = extraJumpsValue;

        dashTime = startDashTime;

        // Extra dashes begins at Extra Dashes Value (1)

        hangCounter = hangTime;

    }






    // Update is called once per frame
    void Update()
    {
        if (isGrounded == true)
        {
            extraJumps = extraJumpsValue;
            extraDashes = extraDashValue;
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
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            jumpBufferCount = jumpBufferLength;
        }
        else
        {
            jumpBufferCount -= Time.deltaTime;
        }



        // Dash Code

        // Input for Dash
        if (dashDirection == eDashDirection.none)
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    dashDirection = eDashDirection.right;

                }
            }

            else if (Input.GetKey(KeyCode.LeftArrow) && Input.GetKeyDown(KeyCode.Z))
            {
                dashDirection = eDashDirection.left;

            }

            else if (Input.GetKey(KeyCode.UpArrow) && Input.GetKeyDown(KeyCode.Z))
            {
                dashDirection = eDashDirection.up;

            }

            else if (Input.GetKey(KeyCode.DownArrow) && Input.GetKeyDown(KeyCode.Z))
            {
                dashDirection = eDashDirection.down;

            }

        }
        else
        {


            if (dashTime <= 0)
            {
                dashDirection = eDashDirection.none;
                dashTime = startDashTime;
                rb.velocity = Vector2.zero;



            }
            else
            {
                dashTime -= Time.deltaTime;

                // Movement for Dash
                if (dashDirection == eDashDirection.right)
                {
                    rb.velocity = Vector2.right * dashSpeed;
                }

                else if (dashDirection == eDashDirection.left)
                {
                    rb.velocity = Vector2.left * dashSpeed;
                }

                else if (dashDirection == eDashDirection.up)
                {
                    rb.velocity = Vector2.up * dashSpeed;
                }

                else if (dashDirection == eDashDirection.down)
                {
                    rb.velocity = Vector2.down * dashSpeed;
                }
            }
        }

        Debug.Log(rb.velocity);





        // If you jump and you have an extra jump, you can jump again
        if (Input.GetKeyDown(KeyCode.UpArrow) && extraJumps > 0)
        {

            rb.velocity = Vector2.up * jumpForce;
            extraJumps--;

        }




        // If you jump and you are grounded, it doesn't take away any extra jumps
        else if (Input.GetKeyDown(KeyCode.UpArrow) && hangCounter > 0f)
        {

            rb.velocity = Vector2.up * jumpForce;

        }

        if (Input.GetKeyUp(KeyCode.UpArrow) && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * .5f);
        }

        if (isGrounded == true)
        {
            extraJumps = extraJumpsValue;
        }

    }
































    // Manages all Physics related function
    void FixedUpdate()
    {


        // Establishing isGrounded variable to create an Overlap Circle which detects ground
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
        // isGrounded but for wall
        onWall = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsWall);
        // Moving input
        moveInput = Input.GetAxisRaw("Horizontal");
        Debug.Log(moveInput);


        // Making player walk at the speed variable
        // rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
        if(dashDirection == eDashDirection.none)
        {
            rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
        }





        // Turns around if they change walking directi
        if (facingRight == false && moveInput > 0)
        {

            Flip();

        }


        else if (facingRight == true && moveInput < 0)
        {

            Flip();

        }






        void Flip()
        {


            facingRight = !facingRight;
            Vector3 Scaler = transform.localScale;
            Scaler.x *= -1;
            transform.localScale = Scaler;

        }

    }

}


    













































