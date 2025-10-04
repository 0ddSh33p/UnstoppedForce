using UnityEngine;


public class EnemyAI : MonoBehaviour
{
    public float patrolSpeed = 2f;
    private float patrolDistance = 3f;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool movingRight = true;
    [SerializeField] private Transform player;
    [SerializeField] private float viewRange = 3f;
    [SerializeField] private float viewAngle = 90f;
    private bool playerSeen = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition + Vector3.right * patrolDistance;
    }

    // Update is called once per frame
    void Update()
    {
        Patrol();
        DetectPlayer();
        if (playerSeen)
        {
            Debug.Log("Seen!");
        }
    }

    void DetectPlayer()
    {
        Vector3 playerDirection = player.position - transform.position;
        float angleToPlayer = Vector3.Angle(transform.right, playerDirection);
        if (angleToPlayer < viewAngle / 2 && playerDirection.magnitude < viewRange)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, playerDirection, viewRange);
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                playerSeen = true;
            }
            else
            {
                playerSeen = false;
            }
        }
        else
        {
            playerSeen = false;
        }

    }

    void Patrol()
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
