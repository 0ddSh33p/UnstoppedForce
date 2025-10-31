using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.InputSystem;


public class SwordTracker : MonoBehaviour
{
    [Range(20f, 360f)] public float angle = 90f;
    public float radius = 3f;
    [SerializeField] private float degreesPerPoint = 10f, damage = 4f; // ~1 point per 10°
    [SerializeField] private Vector3 origin;
    [SerializeField] private Animator anim;


    private SpriteShapeController ssc;
    private SpriteShapeRenderer ssr;
    private Spline spline;
    private Color baseColor;
    [HideInInspector] public bool hitGround;
    [SerializeField] private LayerMask ground, enemies;
    [HideInInspector] public Vector2 look = Vector2.right;

    InputAction dirSwitchInput;
    private bool attack, attacking;
 
    void Awake()
    {
        ssc = GetComponent<SpriteShapeController>();
        ssr = GetComponent<SpriteShapeRenderer>();
        dirSwitchInput = InputSystem.actions.FindAction("Attack");


        baseColor = ssr.color;
        spline = ssc.spline;
        attacking = false;
    }

    void Update()
    {
        attack = dirSwitchInput.WasPressedThisFrame();
        if (attack && !attacking)
        {
            attacking = true;
            anim.SetTrigger("Swing");
            StartCoroutine(Flash(0.2f, new Color(1, 0, 0, 0.8f)));
        }
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
        hitGround = false;
        GameObject[] prevHits = new GameObject[resolution];


        // Arc points
        for (int i = 0; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            float currentAngle = Mathf.Lerp(-halfAngleRad, halfAngleRad, t) + baseAngle;
            Vector3 dir = new Vector3(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle), 0f);


            if (Physics2D.Raycast(transform.position, dir, radius, ground)) hitGround = true;

            if (attack)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, radius, enemies);
                if (hit)
                {
                    if (hit.transform.gameObject.CompareTag("Enemy") && !prevHits.Contains(hit.transform.gameObject))
                    {
                        Enemy script = hit.transform.gameObject.GetComponent<Enemy>();
                        script.TakeDamage(damage);
                        prevHits[i] = hit.transform.gameObject;
                    }
                }
            }
             

            Vector3 pos = origin + dir * radius;
            spline.InsertPointAt(spline.GetPointCount(), pos);
        }
        // Refresh mesh
        ssc.RefreshSpriteShape();
    }

    public IEnumerator Flash(float time, Color color)
    {
        yield return new WaitForEndOfFrame();
        ssr.color = color;
        yield return new WaitForSeconds(time);
        ssr.color = baseColor;
        attacking = false;
    }
}
