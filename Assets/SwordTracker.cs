using UnityEngine;
using UnityEngine.U2D;

public class SwordTracker : MonoBehaviour
{
    [Range(20f, 360f)] public float angle = 90f;
    public float radius = 3f;
    [SerializeField] private float degreesPerPoint = 10f; // ~1 point per 10°
    [SerializeField] private Vector3 origin;

    private SpriteShapeController ssc;
    private Spline spline;

    [HideInInspector] public Vector2 look = Vector2.right;

    void Awake()
    {
        ssc = GetComponent<SpriteShapeController>();
        spline = ssc.spline;
    }

    public void RegenerateCone()
    {
        if (spline == null) return;

        spline.Clear();
        
        // Center point
        spline.InsertPointAt(0, origin);

        int resolution = Mathf.Max(1, Mathf.CeilToInt(angle / degreesPerPoint));
        float halfAngleRad = angle * 0.5f * Mathf.Deg2Rad;
        float baseAngle = Mathf.Atan2(look.y, look.x);

        // Arc points
        for (int i = 0; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            float currentAngle = Mathf.Lerp(-halfAngleRad, halfAngleRad, t) + baseAngle;
            Vector3 dir = new Vector3(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle), 0f);
            Vector3 pos = origin + dir * radius;
            spline.InsertPointAt(spline.GetPointCount(), pos);
        }
        // Refresh mesh
        ssc.RefreshSpriteShape();
    }
}
