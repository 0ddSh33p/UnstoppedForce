using UnityEngine;

using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //editable vars
    [SerializeField] private float momentum, maxMomentum, momentumDecayRate, speed, acceleration, maxSwitch, jumpPower, bouncePower, maxJumpTime, divePower, dirSwitchCoolDown, dashSpeed, dashCooldown;
    [SerializeField] [Range(0f,1f)] private float friction, airResistance, groundedSensitivity, wallSensitivity;
    [SerializeField] private LayerMask ground;
    [SerializeField] private AnimationCurve jumpFalloff;

    // AUDIO 
    [Header("Audio")]
    [SerializeField] private AudioSource footstepSource;   
    [SerializeField] private AudioSource sfxSource;        
    [SerializeField] private AudioClip footstepClip;
    [SerializeField] private AudioClip swordSwingClip;
    [SerializeField] private AudioClip dashClip;
    [SerializeField] private AudioClip momentum50Clip;
    [SerializeField] private AudioClip momentum100Clip;
    private bool hasPlayed50 = false;
    private bool hasPlayed100 = false;

    private bool isWalking;    

    //internal vars
    private bool grounded = false, avalableJump = false, runJumpAnim = false;
    private float heldTime, switchTime, lastDir, dashTime, wallDir;


    //base data
    private Rigidbody2D rb;
    private RaycastHit2D hit;
    [SerializeField] private GameObject swordT;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer mSprite;
    private SwordTracker tracker;
    public MomentumBarUI momentumBarUI;

    // input manager junk
    InputAction moveInput, jumpInput, dirSwitchInput, dashInput;
    Vector2 move;

    void Start()
    {
        momentum = 0;
        rb = GetComponent<Rigidbody2D>();
        moveInput = InputSystem.actions.FindAction("Move");
        jumpInput = InputSystem.actions.FindAction("Jump");
        dashInput = InputSystem.actions.FindAction("Sprint");
        dirSwitchInput = InputSystem.actions.FindAction("Switch");
        tracker = swordT.GetComponent<SwordTracker>();
        momentumBarUI = GameObject.Find("Momentum").GetComponent<MomentumBarUI>();
        momentumBarUI.setMomentum(momentum, maxMomentum);

        lastDir = 1f;
        tracker.RegenerateCone();

        //Audio Setup
        if (footstepSource != null)
        {
            footstepSource.loop = true;
            footstepSource.playOnAwake = false;
            if (footstepClip != null)
                footstepSource.clip = footstepClip;
        }
        if (sfxSource != null)
        {
            sfxSource.playOnAwake = false;
        }
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

        //If pressing mouse left, play the sword swing sound
        if (Input.GetMouseButtonDown(0))
        {
            PlaySFX(swordSwingClip);
        }

        if (jump)
        {   //If you are startiung a valid jump or are continuing an existing jump, add to the hold timer
            if (avalableJump && (grounded || heldTime > 0)) heldTime += Time.deltaTime;
        }
        //if jump is not being held, reset the timer
        else if (heldTime > 0)
        {
            runJumpAnim = false;
            avalableJump = false;
            heldTime = 0f;
        }

        //if the hold timer is on, we assume the player is jumping
        if (heldTime > 0f)
        {
            if (!runJumpAnim)
            {
                anim.SetTrigger("Jump");
                runJumpAnim = true;
            }
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
                //We need to check if the direction has a platfolrm to stick the sword into, but im lazy, so oh well...
                anim.SetTrigger("Pivot");
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

        if ((dash || dashTime > 0f) && (10 < momentum))
        {
            // if the player dashes and isnt on cooldown, add dash force
            if (dashTime == 0f)
            {
                rb.AddForceX(dashSpeed * lastDir);
                PlaySFX(dashClip, 9f);
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
        if (lastDir > 0)
        {
            anim.SetInteger("MoveDir", 1);
        }
        else if (lastDir < 0)
        {
            anim.SetInteger("MoveDir", -1);
        }

        anim.SetInteger("UpVel", (int)rb.linearVelocityY);

        if(grounded && Mathf.Abs(rb.linearVelocityX) > 0.33f)
        {
            anim.SetBool("Walking", true);
        } else {
            anim.SetBool("Walking", false);
            
        }

        bool walkingNow = grounded && Mathf.Abs(rb.linearVelocityX) > 0.33f;

        // play footstep sound if the player is walking
        if (walkingNow && !isWalking)
        {
            if (footstepSource != null && footstepClip != null)
            {
                footstepSource.Play();
            }
        }
        else if (!walkingNow && isWalking)
        {
            // 停止走路 / 跳起
            if (footstepSource != null)
            {
                footstepSource.Stop();
            }
        }
        isWalking = walkingNow;


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

        momentum -= momentum * momentumDecayRate * Time.deltaTime;
        if (momentum < 50) hasPlayed50 = false;
        if (momentum < 100) hasPlayed100 = false;

        momentumBarUI.setMomentum(momentum, maxMomentum);
        if (BGMManager.Instance != null)
            BGMManager.Instance.UpdateMomentum(momentum);

    }

    public void increaseMomentum(float dm)
    {
        momentum += dm;
        momentum = Mathf.Clamp(momentum, 0, 100);

        //plays sound effects at momentum 50 and 100
        if (momentum >= 50 && !hasPlayed50)
        {
            PlaySFX(momentum50Clip);
            hasPlayed50 = true;
        }

        if (momentum >= 100 && !hasPlayed100)
        {
            PlaySFX(momentum100Clip, 1f);
            hasPlayed100 = true;
        }

        momentumBarUI.setMomentum(momentum, maxMomentum);
        if (BGMManager.Instance != null)
            BGMManager.Instance.UpdateMomentum(momentum);
    }

    private void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, volume);
    }
}
