using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private float health;
    public float maxHealth = 10;
    public HealthBarUI healthBarUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;
        healthBarUI = GameObject.Find("Health").GetComponent<HealthBarUI>();
        healthBarUI.setHealth(health, maxHealth);
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        health = Mathf.Clamp(health, 0, maxHealth);
        healthBarUI.setHealth(health, maxHealth);
        if (health <= 0)
        {
            //Replace with respawn system in a future update
            Destroy(gameObject);
        }
        Debug.Log(name + " " + health);
    }
}
