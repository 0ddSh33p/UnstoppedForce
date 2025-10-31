using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public float width, height;
    [SerializeField] private RectTransform healthBar;

    public void setHealth(float health, float maxHealth)
    {
        healthBar.sizeDelta = new Vector2(width * health / maxHealth, height);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
