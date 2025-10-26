using UnityEngine;
using UnityEngine.UI;
#if TMP_PRESENT
using TMPro;
#endif

public class SettingsMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider volumeSlider;
#if TMP_PRESENT
    [SerializeField] private TMP_Dropdown qualityDropdown;
#else
    [SerializeField] private Dropdown qualityDropdown;
#endif
    [SerializeField] private Toggle fullscreenToggle;

    private const string VOL_KEY = "master_volume";
    private const string QUAL_KEY = "quality_index";
    private const string FULL_KEY = "fullscreen";

    private void OnEnable()
    {
        LoadSettingsToUI();
        ApplyAll();
    }

    public void OnVolumeChanged(float v)
    {
        AudioListener.volume = Mathf.Clamp01(v);
        PlayerPrefs.SetFloat(VOL_KEY, AudioListener.volume);
    }

    public void OnQualityChanged(int index)
    {
        QualitySettings.SetQualityLevel(index, true);
        PlayerPrefs.SetInt(QUAL_KEY, index);
    }

    public void OnFullscreenChanged(bool isFull)
    {
        Screen.fullScreen = isFull;
        PlayerPrefs.SetInt(FULL_KEY, isFull ? 1 : 0);
    }

    // ---- Helpers ----

    private void LoadSettingsToUI()
    {
        float vol = PlayerPrefs.GetFloat(VOL_KEY, 1f);
        int qual = PlayerPrefs.GetInt(QUAL_KEY, QualitySettings.GetQualityLevel());
        bool full = PlayerPrefs.GetInt(FULL_KEY, Screen.fullScreen ? 1 : 0) == 1;

        if (volumeSlider) volumeSlider.value = vol;
        if (qualityDropdown) qualityDropdown.value = qual;
        if (fullscreenToggle) fullscreenToggle.isOn = full;
    }

    private void ApplyAll()
    {
        if (volumeSlider) OnVolumeChanged(volumeSlider.value);
        if (qualityDropdown) OnQualityChanged(qualityDropdown.value);
        if (fullscreenToggle) OnFullscreenChanged(fullscreenToggle.isOn);
        PlayerPrefs.Save();
    }
}
