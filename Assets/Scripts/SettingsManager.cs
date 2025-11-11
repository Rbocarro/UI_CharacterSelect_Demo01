using UnityEngine;
using TMPro;
using UnityEngine.UI;
using PrimeTween;

public class SettingsManager : MonoBehaviour
{   
    public GameObject settingsPanelParent;
    public Slider volumeGlobalSider;
    public Slider volumeMusicSlider;
    public Slider volumeSFXSlider;

    public Button openSettingsButton;
    public Button closeSettingsButton;

    public GameObject settingsPanel;

    void Start()
    {
        SetupVolumeSliderListner();
        closeSettingsButton.onClick.AddListener(DisableSettingsPanel);
        openSettingsButton.onClick.AddListener(EnableSettingsPanel);
    }

    void SetupVolumeSliderListner()
    {
        if (AudioManager.instance != null)
        {   
            volumeGlobalSider.value=AudioManager.instance.globalVolume;
            volumeMusicSlider.value = AudioManager.instance.globalMusicVolume;
            volumeSFXSlider.value = AudioManager.instance.globalSFXVolume;
        }

        volumeGlobalSider.onValueChanged.AddListener(value => AudioManager.instance.UpdateGlobalVolume(value));

        volumeMusicSlider.onValueChanged.AddListener(value =>
            AudioManager.instance.UpdateVolumeByType(AudioType.Music, value));

        volumeSFXSlider.onValueChanged.AddListener(value =>
            AudioManager.instance.UpdateVolumeByType(AudioType.SFX, value));
    }

    void DisableSettingsPanel()
    {
        Tween.Scale(settingsPanel.GetComponent<RectTransform>(), Vector3.zero, 0.3f, Ease.InOutQuart).OnComplete(() => 
        settingsPanelParent.gameObject.SetActive(false));
    }
    void EnableSettingsPanel()
    {
        AudioManager.instance.Play("UI_CardClickEnter"); 
        settingsPanelParent.gameObject.SetActive(true);
        settingsPanel.GetComponent<RectTransform>().localScale = Vector3.zero;
        Tween.Scale(settingsPanel.GetComponent<RectTransform>(), Vector3.one, 0.5f, Ease.InOutQuart);
    }
}
