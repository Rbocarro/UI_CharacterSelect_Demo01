using UnityEngine;
using UnityEngine.UI;
using PrimeTween;

public class SettingsManager : MonoBehaviour
{
    [Header("Settings UI Elements")]
    public GameObject settingsPanelParent;
    public GameObject settingsPanel;

    [Header("Volume Sliders")]
    public Slider volumeGlobalSlider;
    public Slider volumeMusicSlider;
    public Slider volumeSFXSlider;

    [Header("Buttons")]
    public Button openSettingsButton;
    public Button closeSettingsButton;

    private RectTransform settingsPanelRectTransform;

    void Start()
    {
        InitialiseUI();
    }
    void InitialiseUI()
    {
        settingsPanelRectTransform = settingsPanel.GetComponent<RectTransform>();
        SetupVolumeSliderListener();
        closeSettingsButton.onClick.AddListener(DisableSettingsPanel);
        openSettingsButton.onClick.AddListener(EnableSettingsPanel);
    }
    void SetupVolumeSliderListener()
    {
        if (AudioManager.instance != null)
        {   
            volumeGlobalSlider.value=AudioManager.instance.globalVolume;
            volumeMusicSlider.value = AudioManager.instance.globalMusicVolume;
            volumeSFXSlider.value = AudioManager.instance.globalSFXVolume;
        }
        volumeGlobalSlider.onValueChanged.AddListener(OnGlobalVolumeChanged);
        volumeMusicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        volumeSFXSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }
    void DisableSettingsPanel()
    {
        AudioManager.instance.Play("UI_CardClickEnter");
        Tween.Scale(settingsPanelRectTransform, Vector3.zero, 0.3f, Ease.InOutQuart).OnComplete(() => 
        settingsPanelParent.gameObject.SetActive(false));
    }
    void EnableSettingsPanel()
    {
        AudioManager.instance.Play("UI_CardClickEnter"); 
        settingsPanelParent.gameObject.SetActive(true);
        settingsPanelRectTransform.localScale = Vector3.zero;
        Tween.Scale(settingsPanel.GetComponent<RectTransform>(), Vector3.one, 0.5f, Ease.InOutQuart);
    }
    void OnGlobalVolumeChanged(float value)
    {
        AudioManager.instance.UpdateGlobalVolume(value);
    }
    void OnMusicVolumeChanged(float value)
    {
        AudioManager.instance.UpdateVolumeByType(AudioType.Music, value);
    }
    void OnSFXVolumeChanged(float value)
    {
        AudioManager.instance.UpdateVolumeByType(AudioType.SFX, value);
    }
    void OnDestroy()
    {
        if (volumeGlobalSlider != null) volumeGlobalSlider.onValueChanged.RemoveListener(OnGlobalVolumeChanged);

        if (volumeMusicSlider != null) volumeMusicSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);

        if (volumeSFXSlider != null) volumeSFXSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);

        if (closeSettingsButton != null) closeSettingsButton.onClick.RemoveListener(DisableSettingsPanel);

        if (openSettingsButton != null) openSettingsButton.onClick.RemoveListener(EnableSettingsPanel);
    }
}
