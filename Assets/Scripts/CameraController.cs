using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float smoothingTime;
    [SerializeField] private Vector3 offset = new Vector3(0, 3, 0);
    private Vector3 velocity = Vector3.zero;


    void FixedUpdate()
    {
        Vector3 targetPos = new Vector3(player.transform.position.x, 
            player.transform.position.y, 
            transform.position.z) + offset;

        // Smoothly move camera towards target
        transform.position = Vector3.SmoothDamp(transform.position, 
            targetPos, 
            ref velocity, 
            smoothingTime);

    }
}
