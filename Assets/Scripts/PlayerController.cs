using UnityEngine;

using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //editable vars
    [SerializeField] private float speed, acceleration, maxSwitch, jumpPower, bouncePower, maxJumpTime, divePower, dirSwitchCoolDown, dashSpeed, dashCooldown;
    [SerializeField] [Range(0f,1f)] private float friction, airResistance, groundedSensitivity, wallSensitivity;
    [SerializeField] private LayerMask ground;
    [SerializeField] private AnimationCurve jumpFalloff;

    //internal vars
    private bool grounded = false, avalableJump = false;
    private float heldTime, switchTime, lastDir, dashTime, wallDir;


    //base data
    private Rigidbody2D rb;
    private RaycastHit2D hit;
    [SerializeField] private GameObject swordT;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer mSprite;
    private SwordTracker tracker;


    // input manager junk
    InputAction moveInput, jumpInput, dirSwitchInput, dashInput;
    Vector2 move;

    private void Awake()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        moveInput = InputSystem.actions.FindAction("Move");
        jumpInput = InputSystem.actions.FindAction("Jump");
        dashInput = InputSystem.actions.FindAction("Sprint");
        dirSwitchInput = InputSystem.actions.FindAction("Switch");
        tracker = swordT.GetComponent<SwordTracker>();

        lastDir = 1f;
        tracker.RegenerateCone();
    }

    void Update()
    {
        bool jump = jumpInput.IsPressed();
        bool swap = dirSwitchInput.WasPressedThisFrame();
        bool dash = dashInput.WasPressedThisFrame();

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector2 mouseDir = (mousePos - transform.position).normalized;
        tracker.look = mouseDir;
        tracker.RegenerateCone();



        if (jump)
        {   //If you are startiung a valid jump or are continuing an existing jump, add to the hold timer
            if (avalableJump && (grounded || heldTime > 0)) heldTime += Time.deltaTime;
        }
        //if jump is not being held, reset the timer
        else if (heldTime > 0)
        {
            avalableJump = false;
            heldTime = 0f;
        }

        //if the hold timer is on, we assume the player is jumping
        if (heldTime > 0f)
        {
            //as long as the maximum tinme has not been exceeded, add a vertical force based on the power, time, and falloff curve
            if (heldTime <= maxJumpTime)
            {
                rb.AddForceY(jumpPower * Time.deltaTime * (1 / maxJumpTime) * jumpFalloff.Evaluate(heldTime / maxJumpTime));
            }
        }

        if (swap || switchTime > 0f)
        {
            if (switchTime == 0f && tracker.hitGround)
            {
                tracker.StartCoroutine(tracker.Flash(0.2f, new Color(1, 0.2f, 0.12f, 0.8f)));
                //We need to check if the direction has a platfolrm to stick the sword into, but there is no level yet so for testing reasons this is not implemented
                if (Mathf.Abs(mouseDir.x) + Mathf.Abs(mouseDir.y) > 0f && Mathf.Abs(mouseDir.x) > Mathf.Abs(mouseDir.y))
                {
                    if (Mathf.Abs(rb.linearVelocityX) < maxSwitch)
                    {
                        rb.linearVelocityX += 0.667f * Mathf.Abs(rb.linearVelocityY) * (Mathf.Abs(mouseDir.x) / mouseDir.x);
                        rb.linearVelocityX = Mathf.Clamp(rb.linearVelocityX, -maxSwitch, maxSwitch);
                        rb.linearVelocityY = 0f;
                    }

                }
                else
                {
                    if (Mathf.Abs(rb.linearVelocityY) < maxSwitch)
                    {
                        rb.linearVelocityY += 0.667f * Mathf.Abs(rb.linearVelocityX) * (Mathf.Abs(mouseDir.y) / mouseDir.y);
                        rb.linearVelocityY = Mathf.Clamp(rb.linearVelocityY, -maxSwitch, maxSwitch);
                        rb.linearVelocityX = 0f;
                    }

                }
            }

            //update the sitch cooldown
            switchTime += Time.deltaTime;
            if (switchTime > dirSwitchCoolDown)
            {
                switchTime = 0;
            }
        }

        if (dash || dashTime > 0f)
        {
            // if the player dashes and isnt on cooldown, add dash force
            if (dashTime == 0f)
            {
                rb.AddForceX(dashSpeed * lastDir);
            }

            //increment the cooldown timer or reset it if it is out of time
            dashTime += Time.deltaTime;

            if (dashTime > dashCooldown)
            {
                dashTime = 0;
            }
        }

        jump = jumpInput.WasPressedThisFrame();
        if (jump && heldTime == 0 && wallDir != 0)
        {
            rb.AddForce(new Vector2(bouncePower * -wallDir, bouncePower * .44f));
        }

        //update animation
        anim.SetInteger("MoveDir", (int)rb.linearVelocityX);
        

    }

    void FixedUpdate()
    {
        //Check if the player is on the ground
        hit = Physics2D.Raycast(transform.position, Vector2.down, groundedSensitivity, ground);
        if (hit)
        {
            grounded = true;
            if (!jumpInput.IsPressed()) avalableJump = true;
        }
        else
        {
            grounded = false;
        }

        //Check for wall jumping collisions
        if (Physics2D.Raycast(transform.position, Vector2.right, wallSensitivity, ground))
        {
            wallDir = 1;
        }
        else if (Physics2D.Raycast(transform.position, Vector2.left, wallSensitivity, ground))
        {
            wallDir = -1;
        }
        else
        {
            wallDir = 0;
        }



        //read the players input and add it to the player speed if needed
        move = moveInput.ReadValue<Vector2>();
        if (move.x != 0)
        {
            if (Mathf.Abs(rb.linearVelocityX) < speed) rb.AddForceX(acceleration * 100f * Time.deltaTime * move.x);
            lastDir = move.x;
        }

        //apply friction manually because 2D unity friction is bad
        if (grounded)
        {
            rb.linearVelocityX *= 1f - friction;
        }
        else
        {
            rb.linearVelocityX *= 1f - airResistance;
            //allow the player to dive
            if (move.y < 0)
            {
                rb.AddForceY(-divePower);
            }
        }
    }
}
