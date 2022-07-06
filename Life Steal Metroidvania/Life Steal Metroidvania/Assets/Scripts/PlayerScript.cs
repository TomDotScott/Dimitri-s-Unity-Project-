﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{

    // Variable Uses
    // Int (Integer) - Used when you need a whole number
    // Float - Used when you need a full number or a decimal. Integer is faster, so we only use float when we NEED access to a decimal
    // Bool (Boolean) - Used when you need true or false
    // String - Used when you need text
    // Variable (Var) - Can be used in any of the above circumstances

    #region VARIABLES

    // Establishing Variables
    #region MOVEMENT_VARIABLES
    [Header("Movement Variables")]
    // Establishing moveSpeed variable
    [Range(10f, 200f)] public float moveSpeed;



    public float fallSpeed;
    public float glideDivisor;
    [SerializeField] private float glideTime;
    private float glideTimeCountdown;

    // Dash Variables
    [Range(0f, 100f)] public float dashSpeed;

    [Range(0f, 1f)] public float startDashTime;
    private float dashTime = 0f;
    private float dashIntangibilityCountdown;
    [SerializeField] private float dashIntangibilityCountdownTime;
    public LayerMask playerLayer;
    public LayerMask intangiblePlayerLayer;

    private bool isIntangible;

    private Vector2 dashDirection = Vector2.zero;

    public int dashCount;
    public int dashCountValue;

    // Establishing wall detection for the wall climb ability
    private bool onWall;

    #endregion

    // Used to list abilities
    public enum eMovementState
    {
        Idle,
        Moving,
        Dashing,
        WallClinging,
    }

    public enum eAerialState
    {
        Grounded,
        Jumping,
        Falling,
        Gliding,
    }

    [Header("Player State")]
    public eMovementState playerMovementState;

    public eAerialState playerAerialState;

    // Establishing the Rigidbody variable
    private Rigidbody2D rb;


    // Establishing the facingRight variable
    private bool facingRight = true;

    #region JUMP_VARIABLES
    [Header("Jumping Variables")]
    // The jump curve will describe the y velocity for the duration of the jump...
    [SerializeField] private AnimationCurve jumpCurve;

    // The jump height is the maximum point that out jump can reach
    [Range(10f, 100f)] public float jumpHeight;

    // Jump Duration describes the total amount of time we want our character to be in the air for
    [SerializeField] private float jumpDuration;

    // Air time describes the amount of time that the player has been in the air for this jump
    private float airTime;

    // Establishing jumping and double jumping abilities
    [SerializeField] private int extraJumps;

    public int extraJumpsValue;
    #endregion

    private bool onTopOfWall;

    #region HEALTH_VARIABLES
    [Header("Player Health")]
    // Health
    [Range(0f, 100)] public float totalHealthValue = 20;
    [SerializeField] private float currentHealthValue;

    private bool takingDamage = false;

    private bool touchingLava;
    public bool TouchingLava
    {
        get => touchingLava;
        set
        {
            if (value == true)
            {
                TakeDamage(4);
            }
            touchingLava = value;
        }
    }

    // Establishing death
    [SerializeField] private bool isPlayerDead;
    #endregion

    private bool beastMode;

    [Header("Coyote Time and Jump Buffering")]
    // Hang Time
    [Range(0f, 2f)] public float hangTime = .2f;
    private float hangCountdown;

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
        dashIntangibilityCountdown = 0f;

        onTopOfWall = false;
        hangCountdown = hangTime;
        currentHealthValue = totalHealthValue;
        isPlayerDead = false;
        takingDamage = false;
        beastMode = false;
        touchingLava = false;
        isIntangible = false;

    }


    // Update is called once per frame
    void Update()
    {
        // Getting input for stuff later on...

        // These return floats between -1 and 1 depending on whether the +ve or -ve input buttons were pressed
        float horizontalMovementValue = Input.GetAxisRaw(GameConstants.HORIZONTAL_MOVEMENT);
        float verticalMovementValue = Input.GetAxisRaw(GameConstants.VERTICAL_MOVEMENT);

        // These return true if they were pressed down on the current frame
        bool jumpButtonPressed = Input.GetButtonDown(GameConstants.JUMP);
        bool dashButtonPressed = Input.GetButtonDown(GameConstants.DASH);
        bool intangibleDashButtonPressed = Input.GetButtonDown(GameConstants.INTANGIBLE_DASH);
        bool grappleButtonPressed = Input.GetButtonDown(GameConstants.GRAPPLE);
        bool attackButtonPressed = Input.GetButtonDown(GameConstants.ATTACK);

        // These return true as long as the button is held down
        bool glideButtonHeld = Input.GetButton(GameConstants.GLIDE);
        bool jumpButtonHeld = Input.GetButton(GameConstants.JUMP);
        bool wallClimbButtonHeld = Input.GetButton(GameConstants.WALL_CLIMB);


        if (playerAerialState == eAerialState.Grounded)
        {
            extraJumps = extraJumpsValue;
            dashCount = dashCountValue;
        }

        // Dash Code
        if (playerAerialState == eAerialState.Grounded)
        {
            dashCount = dashCountValue;
        }

        #region DASH_CODE
        // Input for Dash
        if (dashDirection == Vector2.zero)
        {
            bool rightButtonPressed = horizontalMovementValue > 0f;
            bool leftButtonPressed = horizontalMovementValue < 0f;
            bool upButtonPressed = verticalMovementValue > 0f;
            bool downButtonPressed = verticalMovementValue < 0f;

            if (intangibleDashButtonPressed)
            { 
                // Directional Dashing
                if (rightButtonPressed)
                {
                    IntangibleDash(new Vector2(1, 0));
                }
                if (leftButtonPressed)
                {
                    IntangibleDash(new Vector2(-1, 0));
                }
                if (upButtonPressed)
                {
                    IntangibleDash(new Vector2(0, 1));
                }
                if (downButtonPressed)
                {
                    IntangibleDash(new Vector2(0, -1));
                }

                // Handle the normal dash in the facing direction
                if (playerMovementState != eMovementState.Dashing)
                {
                    // TERNARY EXPRESSIONS:
                    // CONDITION ? IF TRUE : ELSE
                    // Changed the if-else into a ternary ?: operation... 
                    IntangibleDash(facingRight ? new Vector2(1, 0) : new Vector2(-1, 0));
                }
            }

            else if (dashButtonPressed)
            {
                // Directional Dashing
                if (rightButtonPressed)
                {
                    Dash(new Vector2(1, 0));
                }
                if (leftButtonPressed)
                {
                    Dash(new Vector2(-1, 0));
                }
                if (upButtonPressed)
                {
                    Dash(new Vector2(0, 1));
                }
                if (downButtonPressed)
                {
                    Dash(new Vector2(0, -1));
                }

                // Handle the normal dash in the facing direction
                if (playerMovementState != eMovementState.Dashing)
                {
                    // TERNARY EXPRESSIONS:
                    // CONDITION ? IF TRUE : ELSE
                    // Changed the if-else into a ternary ?: operation... 
                    Dash(facingRight ? new Vector2(1, 0) : new Vector2(-1, 0));
                }
            }

            // After taking in all the inputs, determine if we have dashed. If we have, subtract one from the 
            // dashCount 
            if (playerMovementState == eMovementState.Dashing)
            {
                dashCount--;
            }
        }
        else
        {
            // When we reset dash, resets intangible dash countdown
            if (dashTime <= 0)
            {
                dashTime = startDashTime;
                // Debug.Log("Resetting Intangible Dash");
                isIntangible = false;
                gameObject.layer = LayerMask.NameToLayer("Player"); // Set the layer back to the normal collision layer 
                ResetDash();
            }
            else
            {
                dashTime -= Time.deltaTime;
            }
        }

        dashIntangibilityCountdown -= Time.deltaTime;
        #endregion

        #region JUMP_CODE
        // Jump Code
        hangCountdown -= Time.deltaTime;

        // If we run out of jumps and still want to, sacrifice some health
        if (jumpButtonPressed &&
            extraJumps <= 0 &&
            (playerAerialState == eAerialState.Jumping || playerAerialState == eAerialState.Falling) &&
            currentHealthValue > 4) // TODO: Change these Fours to be in one variable, this will be very annoying if we want to change all of them
        {
            Sacrifice(4);
            Jump();
        }

        if (jumpButtonPressed)
        {
            // If we're gliding, we want to be able to use our double jump
            if (playerAerialState == eAerialState.Gliding && extraJumps > 0)
            {
                Jump();
                extraJumps--;
            }

            if (playerAerialState == eAerialState.Grounded)
            {
                // Normal jump if we're on the ground
                Jump();
            }
            else if (extraJumps > 0)
            {
                // Use up our double jump!
                Jump();
                extraJumps--;
            }
            else if (hangCountdown >= 0)
            {
                Jump();
                playerAerialState = eAerialState.Jumping;
            }
        }



        // Hang Time (Coyote Time)
        hangCountdown -= Time.deltaTime;
        if (playerAerialState == eAerialState.Grounded)
        {
            hangCountdown = hangTime;
        }

        // Jump Buffer
        jumpBufferCount -= Time.deltaTime;
        if (jumpButtonPressed)
        {
            jumpBufferCount = jumpBufferLength;
        }

        if (playerAerialState == eAerialState.Jumping)
        {
            if (jumpButtonHeld && airTime <= jumpDuration)
            {
                // Add to our air time
                airTime += Time.deltaTime;

                // Work out how much velocity we need to give the player based on our curve...
                // We want the minimum of our percentage and 1 (can't go above 100% here!)
                float jumpPercentage = Mathf.Min(airTime / jumpDuration, 1.0f);

                // Next, grab the value from the y-axis based on how much of the jump we have completed...
                float curveY = jumpCurve.Evaluate(jumpPercentage);

                // Finally, apply the upwards velocity equal to the y value from our curve...
                rb.velocity = Vector2.up * jumpHeight * curveY;
            }
            else
            {
                playerAerialState = eAerialState.Falling;
            }
        }
        else
        {
            airTime = 0.0f;
        }


        // Glide Code
        if (playerAerialState != eAerialState.Grounded)
        {
            glideTimeCountdown -= Time.deltaTime;
        }

        if (glideButtonHeld &&
            playerAerialState != eAerialState.Grounded &&
            playerMovementState != eMovementState.WallClinging &&
            glideTimeCountdown <= 0)
        {
            // If we're jumping and we glide, we cut off the Upwards velocity
            playerAerialState = eAerialState.Gliding;
            Vector2 v = rb.velocity;
            if (v.y > 0)
            {
                v.y = 0;
                rb.velocity = v;
            }

            rb.gravityScale = fallSpeed / glideDivisor;
        }
        else
        {
            if (Input.GetButtonUp(GameConstants.GLIDE))
            {
                playerAerialState = eAerialState.Falling;
            }

            rb.gravityScale = fallSpeed;
        }

        // Wall Jump + Wall Resource Restoration
        if (playerMovementState == eMovementState.WallClinging)
        {
            extraJumps = extraJumpsValue;
            dashCount = dashCountValue;
        }
        #endregion

        #region MOVEMENT_CODE
        if (onWall)
        {
            if (wallClimbButtonHeld)
            {
                rb.velocity = new Vector2(rb.velocity.x, verticalMovementValue * moveSpeed);
                rb.gravityScale = 0f;
                playerMovementState = eMovementState.WallClinging;
            }
            else
            {
                if (playerAerialState == eAerialState.Gliding)
                {
                    rb.gravityScale = fallSpeed / glideDivisor;
                }
                else
                {
                    rb.gravityScale = fallSpeed;
                }

                playerMovementState = eMovementState.Idle;
            }
        }
        else
        {
            if (rb.gravityScale == 0)
            {
                rb.gravityScale = fallSpeed;
            }
        }

        // Making player walk at the moveSpeed variable
        if (playerMovementState != eMovementState.Dashing)
        {
            if (playerMovementState != eMovementState.WallClinging)
            {
                rb.velocity = new Vector2(horizontalMovementValue * moveSpeed, rb.velocity.y);
                if (horizontalMovementValue == 0 && verticalMovementValue == 0)
                {
                    playerMovementState = eMovementState.Idle;
                }
                else
                {
                    playerMovementState = eMovementState.Moving;
                }
            }
        }
        else
        {
            if (dashDirection.x != 0 && dashDirection.y != 0)
            {
                rb.velocity = dashDirection * WorkOutDiagonalDashSpeed();
            }
            else
            {
                rb.velocity = dashDirection * dashSpeed;
            }

            playerMovementState = eMovementState.Dashing;
        }

        // Turns around if they change walking direction
        if ((!facingRight && horizontalMovementValue > 0) || (facingRight && horizontalMovementValue < 0))
        {
            Flip();
        }
        #endregion


        // Beast Mode Code
        // TODO: ADD 'BEAST MODE' BUTTON IN THE INPUT AND IMPLEMENT THE BEASTMODE STUFF 
        /*
        if (Input.GetKeyDown(KeyCode.S) && currentHealthValue > 1 && beastMode == false)
        {
            ActivateBeastMode();
        }

        if (beastMode == true)
        {
            if (currentHealthValue <= 0)
            {
                beastMode = false;
            }
            // damageDealt * 2;
            // currentHealthValue - 2 * Time.deltaTime;
        }
        */
    }


    private float WorkOutDiagonalDashSpeed()
    {
        return Mathf.Sqrt((dashSpeed * dashSpeed) / 2); // We use Pythagoras to work out the diagonal distance...
    }

    // Flip functions
    private void Flip()
    {
        if (playerMovementState != eMovementState.Dashing && playerMovementState != eMovementState.WallClinging)
        {
            facingRight = !facingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1; // Making the X Scale negative, flips the pixels horizontally
            transform.localScale = scale;
        }

    }

    // Dash function
    private void Dash(Vector2 direction) // Direction is a Vector2 to allow movement in all 4 quadrants
    {
        if (dashCount > 0)
        {
            dashDirection += direction;
            playerMovementState = eMovementState.Dashing;
        }
        else
        {
            if (currentHealthValue > 4)
            {
                dashDirection += direction;
                playerMovementState = eMovementState.Dashing;
                Sacrifice(4);
            }

        }
    }

    // Intangible Dash function
    private void IntangibleDash(Vector2 direction)
    {
        if (dashIntangibilityCountdown <= 0f)
        {
            Dash(direction);
            isIntangible = true;
            gameObject.layer = LayerMask.NameToLayer("IntangiblePlayer"); // Swap to the physics layer we created that doesn't let us collide with enemies - to view the layers go to... Edit->Project Settings->Physics2D

            dashIntangibilityCountdown = dashIntangibilityCountdownTime;

        }
    }

    // Resetting dash
    public void ResetDash()
    {
        dashDirection = Vector2.zero;
        rb.velocity = Vector2.zero;
        playerMovementState = eMovementState.Moving;
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
        if (playerMovementState != eMovementState.Dashing)
        {
            playerAerialState = eAerialState.Jumping;
            glideTimeCountdown = glideTime;
        }
    }

    private void Glide()
    {
        playerAerialState = eAerialState.Gliding;
    }

    public void SetAirState(eAerialState state)
    {
        playerAerialState = state;
    }

    public void Teleport(Vector2 teleportPos)
    {
        gameObject.transform.position = teleportPos;
    }


    private void ActivateBeastMode()
    {
        beastMode = true;
    }


    public void Burn()
    {
        // Damage over time code here
    }

    // Killing the player and triggering a respawn
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

    // Health Sacrifice Mechanic
    public void Sacrifice(float healthSacrificeAmount)
    {
        TakeDamage(healthSacrificeAmount);
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
        // Switch-Case is like if-else but for SCOPED elements (i.e. things that can be immediately jumped to, like enum values, strings or integers)
        switch (collision.gameObject.tag)
        {
            case "Ground":
                playerAerialState = eAerialState.Grounded;
                ResetDash();
                dashCount = dashCountValue;
                glideTimeCountdown = glideTime;

                break;
            case "Wall":
                {
                    // 0 = Top, 1 = Right Side, 2 = Bottom, 3 = Left Side (Travels clockwise, +1 per side) of polygon
                    List<ContactPoint2D> contactPoints = new List<ContactPoint2D>();
                    collision.GetContacts(contactPoints);
                    if (contactPoints[0].normal.y == 1)
                    {
                        playerAerialState = eAerialState.Grounded;
                        onTopOfWall = true;
                        onWall = false;
                        playerMovementState = eMovementState.Moving;

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
                if (isIntangible == false)
                {
                    TakeDamage(totalHealthValue / 10f);

                }

                break;
        }
    }

    // The opposite of OnCollisionEnter, OnCollisionExit gets called when the object that collided
    // with the gameobject with this script attached is no longer colliding
    private void OnCollisionExit2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Wall":
                {
                    if (onTopOfWall)
                    {
                        playerAerialState = eAerialState.Jumping;
                    }
                    onWall = false;
                    onTopOfWall = false;
                    break;
                }
            case "Ground":
                // If we haven't jumped, we wanna let gravity take us away
                if (playerAerialState != eAerialState.Jumping)
                {
                    playerAerialState = eAerialState.Falling;
                }

                hangCountdown = hangTime;
                break;
        }
    }

    // On Trigger gets called when a gameobject whose collider is marked 'isTrigger' touches the 
    // object with this script attached. It's good for setting up in game events, such as hitting death boxes
    // or checkpoints
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Hit the climbing area");
        switch (collision.gameObject.tag)
        {
            case "Lava":
                TouchingLava = true;
                break;
        }
    }

    private void OnGUI()
    {
        GUI.color = Color.red;
        var rect = new Rect(0, 0, 100, 200);
        GUI.Label(rect, "Movement State:" + playerMovementState.ToString() + "\nAerial State:" + playerAerialState.ToString());
    }

    #endregion
}