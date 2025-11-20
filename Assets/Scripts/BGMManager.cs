using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance { get; private set; }

    [Header("3 BGMS used in this chapter")]
    [SerializeField] private AudioClip normalBgm;       // momentum < 50
    [SerializeField] private AudioClip momentum50Bgm;   // 50 <= m < 100
    [SerializeField] private AudioClip momentum100Bgm;  // m >= 100

    private AudioSource audioSource;

    // 0 = normal, 1 = 50, 2 = 100, -1 = not set yet
    private int currentTier = -1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.loop = true;

    }

    /// <summary>
    /// enterring a new chapter, set the 3 bgm clips for this chapter
    /// </summary>
    public void SetBgmSet(AudioClip normal, AudioClip m50, AudioClip m100, bool restartFromBeginning = true)
    {
        normalBgm      = normal;
        momentum50Bgm  = m50;
        momentum100Bgm = m100;

        if (restartFromBeginning)
        {
            currentTier = -1;          // reset tier
            PlayClip(normalBgm);       // defautly plays normal
            currentTier = 0;
        }
    }

    /// <summary>
    /// check momentum and update BGM if needed
    /// </summary>
    public void UpdateMomentum(float momentum)
    {
        // if no bgm is set, do nothing
        if (normalBgm == null && momentum50Bgm == null && momentum100Bgm == null)
            return;

        int tier;
        if (momentum >= 100f)      tier = 2;
        else if (momentum >= 50f)  tier = 1;
        else                       tier = 0;

        if (tier == currentTier) return; 

        currentTier = tier;

        AudioClip target =
            tier == 0 ? normalBgm :
            tier == 1 ? momentum50Bgm :
                        momentum100Bgm;

        PlayClip(target);
    }

    private void PlayClip(AudioClip clip)
    {
        if (clip == null) return;

        // do not restart if the same clip is already playing
        if (audioSource.clip == clip && audioSource.isPlaying)
            return;

        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }
}
