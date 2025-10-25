using NUnit.Framework;
using UnityEditor.Callbacks;
using UnityEngine;


public class Enemy : MonoBehaviour
{
    public float patrolSpeed = 2f;
    private float patrolDistance = 3f;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float edgeDetectionWidth = 0.5f;
    private float edgeDetectionDepth = 1.5f;
    private bool movingRight = true;
    public GameObject player;
    [SerializeField] public float currentDirection; //Keeps track of the enemy's current direction, +1 is right, -1 is left
    [SerializeField] public float alertRadius, viewRange, viewAngle, runSpeed, runAcceleration;
    [SerializeField] public string state;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void boot()
    {
        startPosition = transform.position;
        targetPosition = startPosition + Vector3.right * patrolDistance;
        state = "neutral";
        player = GameObject.Find("Player");
    }
    void Start()
    {
        boot();
    }

    // Update is called once per frame
    public void baseUpdate()
    {
        DetectPlayer();
    }

    public void faceDirection(float direction) //Change the enemy's current direction, +1 is right, -1 is left
    {
        currentDirection = direction;
        transform.localScale = new Vector3(direction, transform.localScale.y, transform.localScale.z);
    }

    public void run(float targetVelocity, float acceleration) //Target velocity is eventual velocity, positive is right, negative is left
    {
        Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
        string runState = "stopping";
        if (targetVelocity > 0) //going right
        {
            RaycastHit2D rightHit = Physics2D.Raycast(transform.position + new Vector3(edgeDetectionWidth, 0, 0), Vector3.down, edgeDetectionDepth);
            if (rightHit)
            {
                if (rightHit.collider.gameObject.tag == "SolidObject")
                {
                    runState = "running";
                }
            }
        }

        if (targetVelocity <= 0) //going left
        {
            RaycastHit2D leftHit = Physics2D.Raycast(transform.position + new Vector3(-edgeDetectionWidth, 0, 0), Vector3.down, edgeDetectionDepth);
            if (leftHit)
            {
                if (leftHit.collider.gameObject.tag == "SolidObject")
                {
                    runState = "running";
                }
            }
        }

        if (runState == "running")
        {
            float force_magnitude = (targetVelocity - rb.linearVelocityX) * acceleration;
            rb.AddForce(new Vector3(force_magnitude, 0, 0));
        }
    }

    public void DetectPlayer()
    {

        Vector3 playerDirection = player.transform.position - transform.position;
        if (playerDirection.magnitude < alertRadius)
        {
            state = "alerted";
        }

        float angleToPlayer = Vector3.Angle(transform.right, playerDirection);
        if (angleToPlayer < viewAngle / 2 && playerDirection.magnitude < viewRange)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, playerDirection, viewRange);
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                state = "alerted";
            }

        }

    }

    public void Patrol()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, patrolSpeed * Time.deltaTime);
        if (transform.position == targetPosition)
        {
            movingRight = !movingRight;
            if (movingRight)
            {
                targetPosition = startPosition + Vector3.right * patrolDistance;
                faceDirection(1);
            }
            else
            {
                targetPosition = startPosition - Vector3.right * patrolDistance;
                faceDirection(-1);
            }
        }
    }
}
