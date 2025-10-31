using UnityEngine;

public class MomentumBarUI : MonoBehaviour
{
    public float width, height;
    [SerializeField] private RectTransform momentumBar;

    public void setMomentum(float momentum, float maxMomentum)
    {
        momentumBar.sizeDelta = new Vector2(width * momentum / maxMomentum, height);
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
