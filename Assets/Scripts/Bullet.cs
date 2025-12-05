using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject player;
    public PlayerHealth playerHealth;
    // [SerializeField] private int damage;
    [SerializeField] private LayerMask solidObjects;
    //public Vector3 velocity;
    public float speed;

    private Rigidbody2D rb; 

    public void setVelocity(Vector3 direction)
    {
        //rb not set yet
        GetComponent<Rigidbody2D>().linearVelocity = speed*direction/direction.magnitude; // implicit conversion from vector2 to vector3
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        Destroy(gameObject, 15);
    }

    void Update()
    {
        float z_rotation = Mathf.Rad2Deg * Mathf.Atan2(rb.linearVelocityY, rb.linearVelocityX);
        transform.rotation = Quaternion.AngleAxis(z_rotation, new Vector3(0, 0, 1));
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log(collision.gameObject.name + " : " + gameObject.name + " : " + Time.time);
        // if(collision.gameObject.tag == "SolidObject" || collision.gameObject.tag == "Player")
        // {
            Destroy(gameObject);
        // }
        //bitwise shenanigans to determine if the collision in within the collision mask
        // if ((solidObjects.value & (1 << collision.transform.gameObject.layer)) > 0)
        //     Destroy(gameObject);
    }
}
