using System;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class RangedEnemy : Enemy
{
    public GameObject projectile;
    [SerializeField] private float fireInterval;
    [SerializeField] private Vector3 projectileSpawnOffset;
    private float shootingCountdown = 0;

    void Action()
    {

        if (state == "alerted")
        {
            Vector3 playerDirection = player.transform.position - transform.position;
            Vector3 run_velocity = new Vector3(runSpeed * (-playerDirection.x) / Math.Abs(playerDirection.x), 0, 0);
            transform.position += run_velocity * Time.deltaTime;
            if (shootingCountdown <= 0)
            {
                GameObject projectileObject = Instantiate(projectile, transform.position + projectileSpawnOffset, Quaternion.identity);
                projectileObject.GetComponent<Bullet>().setVelocity(playerDirection);
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
        //fix this using superclass stuff please
        boot();
    }

    // Update is called once per frame
    void Update()
    {
        baseUpdate();
        if (state == "alerted")
        {
            Action();
        }
        else
        {
            Patrol();
        }

        if (state == "alerted")
        {
            //Debug.Log("Seen!");
        }
    }
}
