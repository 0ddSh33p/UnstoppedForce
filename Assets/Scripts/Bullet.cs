using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject player;
    public PlayerHealth playerHealth;
    [SerializeField] private int damage;
    public Vector3 velocity;
    public float speed;

    public void setVelocity(Vector3 direction)
    {
        velocity = speed*direction/direction.magnitude;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Player");

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += velocity * Time.deltaTime;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        print("collided");
        if (collision.gameObject.tag == "Player")
        {
            //playerHealth.TakeDamage(damage);
            Destroy(gameObject);
        }

        if (collision.gameObject.tag == "SolidObject")
        {
            Destroy(gameObject);
        }
    }
}
