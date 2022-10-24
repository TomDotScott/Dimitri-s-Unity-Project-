using System;
using System.Collections;
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
    private bool canClimb = true;

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

    enum FacingDirection
    {
        None = 0,
        Left = -1,
        Right = 1,
    }

    // Establishing the facingRight variable
    private FacingDirection facingDirection = FacingDirection.Right;

    private Vector2 previousPosition;

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
    private FacingDirection right;
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

        // TODO: Move all of these into a structure or class so Inputs are manageable in one place

        // These return true if they were pressed down on the current frame
        bool jumpButtonPressed = Input.GetButtonDown(GameConstants.JUMP) || Input.GetKeyDown(KeyCode.Space);
        if (jumpButtonPressed)
        {
            Debug.Log("Jumped");
        }
        else
        {
            Debug.Log("Literally Nothing");
        }

        bool dashButtonPressed = Input.GetButtonDown(GameConstants.DASH);
        bool intangibleDashButtonPressed = Input.GetButtonDown(GameConstants.INTANGIBLE_DASH);
        bool grappleButtonPressed = Input.GetButtonDown(GameConstants.GRAPPLE);
        bool attackButtonPressed = Input.GetButtonDown(GameConstants.ATTACK);
        bool leftButtonPressed = Input.GetButtonDown(GameConstants.LEFT_BUTTON);
        bool rightButtonPressed = Input.GetButtonDown(GameConstants.RIGHT_BUTTON);
        bool upButtonPressed = Input.GetButtonDown(GameConstants.UP_BUTTON);
        bool downButtonPressed = Input.GetButtonDown(GameConstants.DOWN_BUTTON);

        // These return false if they were released on the current frame
        bool leftButtonReleased = Input.GetButtonUp(GameConstants.LEFT_BUTTON);
        bool rightButtonReleased = Input.GetButtonUp(GameConstants.RIGHT_BUTTON);

        // These return true as long as the button is held down
        bool glideButtonHeld = Input.GetButton(GameConstants.GLIDE);
        bool jumpButtonHeld = Input.GetButton(GameConstants.JUMP) || Input.GetKey(KeyCode.Space);
        bool wallClimbButtonHeld = Input.GetButton(GameConstants.WALL_CLIMB);
        bool leftButtonHeld = Input.GetButton(GameConstants.LEFT_BUTTON);
        bool rightButtonHeld = Input.GetButton(GameConstants.RIGHT_BUTTON);
        bool upButtonHeld = Input.GetButton(GameConstants.UP_BUTTON);
        bool downButtonHeld = Input.GetButton(GameConstants.DOWN_BUTTON);


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
            if (intangibleDashButtonPressed)
            {
                // Directional Dashing
                if (rightButtonHeld)
                {
                    IntangibleDash(new Vector2(1, 0));
                }
                if (leftButtonHeld)
                {
                    IntangibleDash(new Vector2(-1, 0));
                }
                if (upButtonHeld)
                {
                    IntangibleDash(new Vector2(0, 1));
                }
                if (downButtonHeld)
                {
                    IntangibleDash(new Vector2(0, -1));
                }

                // Handle the normal dash in the facing direction
                if (playerMovementState != eMovementState.Dashing)
                {
                    // TERNARY EXPRESSIONS:
                    // CONDITION ? IF TRUE : ELSE
                    // Changed the if-else into a ternary ?: operation... 
                    IntangibleDash(facingDirection == FacingDirection.Right ? new Vector2(1, 0) : new Vector2(-1, 0));
                }
            }

            else if (dashButtonPressed)
            {
                // Directional Dashing
                if (rightButtonHeld)
                {
                    Dash(new Vector2(1, 0));
                }
                if (leftButtonHeld)
                {
                    Dash(new Vector2(-1, 0));
                }
                if (upButtonHeld)
                {
                    Dash(new Vector2(0, 1));
                }
                if (downButtonHeld)
                {
                    Dash(new Vector2(0, -1));
                }

                // Handle the normal dash in the facing direction
                if (playerMovementState != eMovementState.Dashing)
                {
                    // TERNARY EXPRESSIONS:
                    // CONDITION ? IF TRUE : ELSE
                    // Changed the if-else into a ternary ?: operation... 
                    Dash(facingDirection == FacingDirection.Right ? new Vector2(1, 0) : new Vector2(-1, 0));
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
                Debug.Log("A");
            }

            if (playerAerialState == eAerialState.Grounded)
            {
                // Normal jump if we're on the ground
                Jump();
            }
            else if (playerAerialState == eAerialState.Falling && hangCountdown >= 0)
            {
                Jump();
                Debug.Log("Hang Time Activity");
            }
            else if (extraJumps > 0 && playerAerialState == eAerialState.Falling)
            {
                // Use up our double jump!
                Jump();
                extraJumps--;
                Debug.Log("B");
            }
        }



        // Hang Time (Coyote Time)
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
                hangCountdown -= Time.deltaTime;
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
            if (playerMovementState != eMovementState.WallClinging)
            {
                rb.gravityScale = fallSpeed;
                Debug.Log("435");
            }
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
                float verticalMovementValue = 0f;
                if (upButtonHeld && canClimb)
                {
                    verticalMovementValue += 1f;
                }

                if (downButtonHeld)
                {
                    verticalMovementValue -= 1f;
                }
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
                    if (playerMovementState != eMovementState.WallClinging)
                    {
                        rb.gravityScale = fallSpeed;
                        Debug.Log("473");
                    }
                }
            }
        }
        else
        {
            if (rb.gravityScale == 0 && canClimb)
            {
                rb.gravityScale = fallSpeed;
                Debug.Log("485");
            }
        }

        // Making player walk at the moveSpeed variable
        if (playerMovementState != eMovementState.Dashing)
        {
                Move(
                    new InputButton(leftButtonPressed, leftButtonHeld, leftButtonReleased),
                    new InputButton(rightButtonPressed, rightButtonHeld, rightButtonReleased)
                );
        }
        else if (playerMovementState == eMovementState.Dashing)
        {
            if (dashDirection.x != 0 && dashDirection.y != 0)
            {
                float diagonalDash = CalculateDiagonalDashSpeed();
                rb.velocity = dashDirection * diagonalDash;
            }
            else
            {
                rb.velocity = dashDirection * dashSpeed;
            }

            playerMovementState = eMovementState.Dashing;
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


    private float CalculateDiagonalDashSpeed()
    {
        return Mathf.Sqrt((dashSpeed * dashSpeed) / 2); // We use Pythagoras to work out the diagonal distance...
    }

    // Flip functions
    private void Flip(FacingDirection direction)
    {
        if (playerMovementState != eMovementState.Dashing && playerMovementState != eMovementState.WallClinging)
        {
            Vector3 scale = transform.localScale;
            if (direction == FacingDirection.Left)
            {
                // To look left we want it to always be negative. Abs returns the positive version of any number
                // We can't just do -scale.x because if it's already negative (i.e. we're looking left...) then it will be 
                // positive (we will look right) this is because - x - = +
                scale.x = -Mathf.Abs(scale.x);
            }
            else
            {
                scale.x = Mathf.Abs(scale.x);
            }

            transform.localScale = scale;
        }

    }

    // A struct is a way that we can create our own data type (like int, float, string, etc) in C#
    // Here, I'm using it so I can store 3 variables (pressed, held and released) under one name
    // (InputButton)
    private struct InputButton
    {
        // This is called a CONSTRUCTOR, it's what we use to create our new struct:
        //     - A CONSTRUCTOR is a function with the same name as the struct or class and is the first thing called
        //       when we create a new INSTANCE of that data type
        //     - InputButton's CONSTRUCTOR takes in 3 PARAMETERS and I set the struct's FIELDS based on what we 
        //       pass into it
        public InputButton(bool pressed, bool held, bool released)
        {
            buttonPressed = pressed;
            buttonHeld = held;
            buttonReleased = released;
        }

        // These booleans get set in the constructor, I decided to make them public so they can be read
        // outside of this struct, but READONLY so that they can't be edited
        // Just like the FIELDS we have at the top of this CLASS, these variables exist inside our struct can can
        // be accessed with the . operator
        public readonly bool buttonPressed;
        public readonly bool buttonHeld;
        public readonly bool buttonReleased;
    }

    // Walk/Run function
    private void Move(InputButton left, InputButton right)
    {
        // Check if we're walking, if we are, work out the direction we need to walk
        if (left.buttonHeld || right.buttonHeld)
        {
            // Step 1 - Set the initial facing directions depending on the button press
            if (left.buttonHeld)
            {
                facingDirection = FacingDirection.Left;
            }

            if (right.buttonHeld)
            {
                facingDirection = FacingDirection.Right;
            }
            rb.velocity = new Vector2((int)facingDirection * moveSpeed, rb.velocity.y);
            

            // Step 5 - Flip the sprite depending on the direction we decided to go
            if (rb.velocity.x < 0)
            {
                Flip(FacingDirection.Left);
            }
            else
            {
                Flip(FacingDirection.Right);
            }

            // Step 6 - Change state and move layers so that we can walk on lava since we're moving!
            playerMovementState = eMovementState.Moving;
            gameObject.layer = LayerMask.NameToLayer("Player Lava");
        }
        else
        {
            // If we're not pressing the walk buttons, stop and set to idle 
            rb.velocity = new Vector2(0, rb.velocity.y);
            playerMovementState = eMovementState.Idle;
            gameObject.layer = LayerMask.NameToLayer("Player");
        }
    }

    private Vector2 CalculateDashDirection(Vector2 direction)
    {
        if (facingDirection == FacingDirection.Right)
        {
            return new Vector2(1, direction.y);
        }

        if (facingDirection == FacingDirection.Left)
        {
            return new Vector2(-1, direction.y);
        }

        return direction;
        
    }

    // Dash function
    private void Dash(Vector2 direction) // Direction is a Vector2 to allow movement in all 4 quadrants
    {
        if (dashCount > 0)
        {
            playerMovementState = eMovementState.Dashing;
            dashDirection += direction;
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

        //dashDirection = dashDirection.normalized;
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

        else
        {
            Dash(direction);

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
                       if (playerAerialState != eAerialState.Jumping)
                       {
                           playerAerialState = eAerialState.Falling;
                       }
                    }
                    onWall = false;
                    onTopOfWall = false;
                    hangCountdown = hangTime;
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
        switch (collision.gameObject.tag)
        {
            case "Lava":
                TouchingLava = true;
                break;

            case "Roof of Wall":
                canClimb = false;
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Roof of Wall":
                canClimb = true;
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