using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private float health;
    public float maxHealth = 10;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            //Replace with respawn system in a future update
            Destroy(gameObject);
        }
    }
}
