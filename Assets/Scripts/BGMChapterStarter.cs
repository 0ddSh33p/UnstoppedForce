using UnityEngine;

public class BGMChapterStarter : MonoBehaviour
{
    [Header("3 BGMS for this chapter")]
    [SerializeField] private AudioClip normalBgm;
    [SerializeField] private AudioClip momentum50Bgm;
    [SerializeField] private AudioClip momentum100Bgm;

    private void Start()
    {
        if (BGMManager.Instance != null)
        {
            // 进入新章节：切换到这一套 BGM，并从头开始播放
            BGMManager.Instance.SetBgmSet(normalBgm, momentum50Bgm, momentum100Bgm, true);
        }
    }
}
