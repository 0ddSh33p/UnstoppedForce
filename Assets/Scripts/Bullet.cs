using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject player;
    public PlayerHealth playerHealth;
    [SerializeField] private int damage;
    [SerializeField] private LayerMask solidObjects;
    public Vector3 velocity;
    public float speed;

    public void setVelocity(Vector3 direction)
    {
        velocity = speed*direction/direction.magnitude;
    }


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        Destroy(gameObject, 15);
    }

    void Update()
    {
        transform.position += velocity * Time.deltaTime;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //bitwise shenanigans to determine if the collision in within the collision mask
        if ((solidObjects.value & (1 << collision.transform.gameObject.layer)) > 0)
            Destroy(gameObject);
    }
}
