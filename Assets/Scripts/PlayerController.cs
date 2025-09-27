using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //editable vars
    [SerializeField] private float speed, acceleration, maxJumpVel, jumpPower, maxJumpTime, divePower, dirSwitchCoolDown, dashSpeed, dashCooldown;
    [SerializeField] [Range(0f,1f)] private float friction, airResistance, groundedDist;
    [SerializeField] private LayerMask ground;
    [SerializeField] private AnimationCurve jumpFalloff;

    //internal vars
    private bool grounded = false, avalableJump = false;
    private float heldTime, switchTime, lastDir, dashTime;


    //base data
    private Rigidbody2D rb;
    private RaycastHit2D groundCast;


    // input manager junk
    InputAction moveInput, jumpInput, dirSwitchInput, dashInput;
    Vector2 move;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        moveInput = InputSystem.actions.FindAction("Move");
        jumpInput = InputSystem.actions.FindAction("Jump");
        dashInput = InputSystem.actions.FindAction("Sprint");
        dirSwitchInput = InputSystem.actions.FindAction("Switch");

        lastDir = 1f;
    }

    void Update()
    {
        bool jump = jumpInput.IsPressed();
        bool swap = dirSwitchInput.WasPressedThisFrame();
        bool dash = dashInput.WasPressedThisFrame();

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector2 mouseDir = (mousePos - transform.position).normalized;

        //If you are startiung a valid jump or are continuing an existing jump, add to the hold timer
        if (jump)
        {
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
            if (switchTime == 0f)
            {
                //We need to check if the direction has a platfolrm to stick the sword into, but there is no level yet so for testing reasons this is not implemented
                if (Mathf.Abs(mouseDir.x) + Mathf.Abs(mouseDir.y) > 0f && Mathf.Abs(mouseDir.x) > Mathf.Abs(mouseDir.y))
                {
                    rb.linearVelocityX += Mathf.Abs(rb.linearVelocityY) * (Mathf.Abs(mouseDir.x) / mouseDir.x);
                    rb.linearVelocityY = 0f;
                }
                else
                {
                    rb.linearVelocityY += Mathf.Abs(rb.linearVelocityX) * (Mathf.Abs(mouseDir.y) / mouseDir.y);
                    rb.linearVelocityX = 0f;
                }
            }
            switchTime += Time.deltaTime;

            if (switchTime > dirSwitchCoolDown)
            {
                switchTime = 0;
            }
        }
        
        if (dash || dashTime > 0f)
        {
            if (dashTime == 0f)
            {
                rb.AddForceX(dashSpeed * lastDir);
            }

            dashTime += Time.deltaTime;
            
            if (dashTime > dashCooldown)
            {
                dashTime = 0;
            }
        }
    }

    void FixedUpdate()
    {
        //Check if the player is on the ground
        groundCast = Physics2D.Raycast(transform.position, Vector2.down, groundedDist, ground);
        if (groundCast)
        {
            grounded = true;
            if (!jumpInput.IsPressed()) avalableJump = true;
        }
        else
        {
            grounded = false;
        }

        //read the players input and add it to the player speed if needed
        move = moveInput.ReadValue<Vector2>();
        if (move.x != 0)
        {
            if (Mathf.Abs(rb.linearVelocityX) < speed) rb.AddForceX(acceleration * 100f * Time.deltaTime * move.x);
            lastDir = move.x;
        }

        //apply friction manually because unity friction is bad
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
