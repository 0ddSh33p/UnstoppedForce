using UnityEngine;

public class SpikeDamage : MonoBehaviour
{
    private PlayerHealth playerHealth;
    [SerializeField] private float damage = 1.0f;
    [SerializeField] private float damageInterval = 0.8f;
    
    private float damageCountdown = 0f;
    [SerializeField] private bool playerCollided = false;

    void Start()
    {
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
    }

    void Update()
    {
		if (playerCollided){
			if (damageCountdown <= 0 && playerCollided)
			{
				playerHealth.TakeDamage(damage);
                damageCountdown = damageInterval;
			}
			else
			{
				damageCountdown -= Time.deltaTime;
			}
		}
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
			playerCollided = true;
            Debug.Log("entered");
        }
    }
	private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
			playerCollided = false;
            Debug.Log("exited");

        }
    }
}
