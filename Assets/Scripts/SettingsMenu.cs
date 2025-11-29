using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown fullscreenDropdown; 

    private const string VOL_KEY = "master_volume";
    private const string QUAL_KEY = "quality_index";
    private const string FULL_KEY = "fullscreen_mode";
    private const string RES_KEY = "resolution_index";

    // Index 0 = 1920x1080 (We will make this the default)
    private readonly Vector2Int[] supportedResolutions = new Vector2Int[]
    {
        new Vector2Int(1920, 1080), 
        new Vector2Int(1600, 900),
        new Vector2Int(1280, 720)
    };

    private void Start()
    {
        // 1. SETUP RESOLUTION DROPDOWN
        if (resolutionDropdown != null)
        {
            resolutionDropdown.ClearOptions();
            List<string> resOptions = new List<string>();
            
            // Default to 0 (1920x1080) if nothing else is found
            int defaultResIndex = 0; 

            for (int i = 0; i < supportedResolutions.Length; i++)
            {
                string option = supportedResolutions[i].x + " x " + supportedResolutions[i].y;
                resOptions.Add(option);

                // Optional: You can keep this if you want to respect the user's current window,
                // BUT if you want to force 1920x1080 on first run, ignore this check.
                // For now, I will leave it to auto-detect if it matches, but default to 0 otherwise.
                if (supportedResolutions[i].x == Screen.width &&
                    supportedResolutions[i].y == Screen.height)
                {
                    defaultResIndex = i;
                }
            }

            resolutionDropdown.AddOptions(resOptions);
            
            // CHANGED: We use defaultResIndex (which defaults to 0/1080p) if no key is found
            int savedResIndex = PlayerPrefs.GetInt(RES_KEY, defaultResIndex);
            
            resolutionDropdown.value = savedResIndex;
            resolutionDropdown.RefreshShownValue();
            
            resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);

            // 1. Force the visual slider to 2
            volumeSlider.value = 2; 

            // 2. Force the actual audio to match immediately
            OnVolumeChanged(2);
        }

        // 2. SETUP FULLSCREEN DROPDOWN
        if (fullscreenDropdown != null)
        {
            fullscreenDropdown.ClearOptions();
            List<string> modeOptions = new List<string> { "Windowed", "Fullscreen" };
            fullscreenDropdown.AddOptions(modeOptions);

            // CHANGED: Force default to 1 (Fullscreen)
            // The '1' here means: If no save data exists, assume Fullscreen.
            int savedModeIndex = PlayerPrefs.GetInt(FULL_KEY, 1);

            fullscreenDropdown.value = savedModeIndex;
            fullscreenDropdown.RefreshShownValue();

            fullscreenDropdown.onValueChanged.AddListener(OnFullscreenModeChanged);
        }

        // 3. APPLY SETTINGS TO GAME
        // We call this at the end of Start to ensure the game creates the window 
        // based on these defaults immediately.
        ApplyAll();
    }

    // Removing OnEnable to prevent conflict logic. 
    // Start() is sufficient for initialization.

    // ---- LOGIC FUNCTIONS ----

    public void OnResolutionChanged(int index)
    {
        Vector2Int res = supportedResolutions[index];
        Screen.SetResolution(res.x, res.y, Screen.fullScreen);
        PlayerPrefs.SetInt(RES_KEY, index);
    }

    public void OnFullscreenModeChanged(int index)
    {
        // 0 = Windowed, 1 = Fullscreen
        bool isFullscreen = (index == 1);
        
        Screen.SetResolution(Screen.width, Screen.height, isFullscreen);
        Screen.fullScreen = isFullscreen;
        
        PlayerPrefs.SetInt(FULL_KEY, index);
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

    private void ApplyAll()
    {
        // Apply Volume
        if (volumeSlider) 
        {
            float savedVol = PlayerPrefs.GetFloat(VOL_KEY, 1f);
            volumeSlider.value = savedVol;
            OnVolumeChanged(savedVol);
        }

        // Apply Quality
        if (qualityDropdown)
        {
            int savedQual = PlayerPrefs.GetInt(QUAL_KEY, QualitySettings.GetQualityLevel());
            qualityDropdown.value = savedQual;
            OnQualityChanged(savedQual);
        }

        // Apply Resolution & Fullscreen
        // We read from the dropdowns because we just set them correctly in Start()
        if (resolutionDropdown) OnResolutionChanged(resolutionDropdown.value);
        if (fullscreenDropdown) OnFullscreenModeChanged(fullscreenDropdown.value);
        
        PlayerPrefs.Save();
    }
}