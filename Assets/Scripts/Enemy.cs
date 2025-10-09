using UnityEngine;


public class Enemy : MonoBehaviour
{
    public float patrolSpeed = 2f;
    private float patrolDistance = 3f;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool movingRight = true;
    public GameObject player;
    [SerializeField] public float alertRadius, viewRange, viewAngle, runSpeed;
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
            }
            else
            {
                targetPosition = startPosition - Vector3.right * patrolDistance;
            }
        }
    }
}
