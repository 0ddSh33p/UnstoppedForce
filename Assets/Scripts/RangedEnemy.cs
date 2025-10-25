using System;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Random=UnityEngine.Random;

public class RangedEnemy : Enemy
{
    public GameObject projectile;
    [SerializeField] private float fireInterval, shootingAngleError;
    [SerializeField] private Vector3 projectileSpawnOffset;
    private float shootingCountdown = 0;

    void Action()
    {
        if (state == "alerted")
        {
            Vector3 playerDirection = player.transform.position - transform.position;
            faceDirection(Mathf.Sign(playerDirection.x));
            float shootingAngle = Random.Range(-shootingAngleError, shootingAngleError);
            Vector3 shootDirection = Quaternion.AngleAxis(shootingAngle, new Vector3(0, 0, 1))*playerDirection;


            run(-Mathf.Sign(playerDirection.x) * runSpeed, runAcceleration);
            if (shootingCountdown <= 0)
            {
                Vector3 directedprojectileSpawnOffset = new Vector3(currentDirection * projectileSpawnOffset.x, projectileSpawnOffset.y, projectileSpawnOffset.z);
                GameObject projectileObject = Instantiate(projectile, transform.position + directedprojectileSpawnOffset, Quaternion.identity);
                projectileObject.GetComponent<Bullet>().setVelocity(shootDirection);
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
