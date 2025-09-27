using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //editable vars
    [SerializeField] private float speed, acceleration, maxJumpVel, jumpPower, maxJumpTime, divePower;
    [SerializeField] [Range(0f,1f)] private float friction, airResistance, groundedDist;
    [SerializeField] private LayerMask ground;
    [SerializeField] private AnimationCurve jumpFalloff;

    //internal vars
    private bool grounded = false, avalableJump = false;
    private RaycastHit2D groundCast;
    private float heldTime;

    //base data
    private Rigidbody2D rb;

    // input manager junk
    InputAction moveInput, jumpInput;
    Vector2 move;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        moveInput = InputSystem.actions.FindAction("Move");
        jumpInput = InputSystem.actions.FindAction("Jump");
    }

    void Update()
    {
        bool jump = jumpInput.IsPressed();
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
        if ((move.x != 0) && (Mathf.Abs(rb.linearVelocityX) < speed))
        {
            rb.AddForceX(acceleration * 100f * Time.deltaTime * move.x);
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
