using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    private float health;
    public float maxHealth = 10;
    public HealthBarUI healthBarUI;
    private Vector3 spawn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()

    {
        spawn = gameObject.transform.position;
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
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        Debug.Log(name + " " + health);
    }
}
