using System;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    public GameObject player;
    public GameObject projectile;
    [SerializeField] private float patrolSpeed, runSpeed, alertRadius, fireInterval;
    [SerializeField] private string state;
    [SerializeField] private Vector3 projectileSpawnOffset;
    private float shootingCountdown = 0;


    private Rigidbody2D rb;

    void Action()
    {
        Vector3 vector_to_player = player.transform.position - transform.position;
        if (state == "neutral")
        {
            if (vector_to_player.magnitude < alertRadius)
            {
                state = "alerted";
            }
        }
        if (state == "alerted")
        {
            Vector3 run_velocity = new Vector3(runSpeed * (-vector_to_player.x) / Math.Abs(vector_to_player.x), 0, 0);
            transform.position += run_velocity * Time.deltaTime;
            if (shootingCountdown <= 0)
            {
                GameObject projectileObject = Instantiate(projectile, transform.position + projectileSpawnOffset, Quaternion.identity);
                projectileObject.GetComponent<Bullet>().velocity = projectileObject.GetComponent<Bullet>().speed*vector_to_player/vector_to_player.magnitude;
                shootingCountdown = fireInterval;
            }
            else
            {
                shootingCountdown -= Time.deltaTime;
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        state = "neutral";
        print(player.transform.position.x);
    }

    // Update is called once per frame
    void Update()
    {
        Action();
    }
}
