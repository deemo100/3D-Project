using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{
    [Header("슬라이더")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("패널")]
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private GameObject pausePanel;

    private float savedBGMVolume;
    private float savedSFXVolume;

    private void Start()
    {
        if (SoundManager.Instance != null)
        {
            savedBGMVolume = SoundManager.Instance.GetBGMVolume();
            savedSFXVolume = SoundManager.Instance.GetSFXVolume();

            bgmSlider.value = savedBGMVolume;
            sfxSlider.value = savedSFXVolume;
        }

        bgmSlider.onValueChanged.AddListener(ChangeBGMVolume);
        sfxSlider.onValueChanged.AddListener(ChangeSFXVolume);
    }

    private void ChangeBGMVolume(float value)
    {
        SoundManager.Instance?.SetBGMVolume(value);
    }

    private void ChangeSFXVolume(float value)
    {
        SoundManager.Instance?.SetSFXVolume(value);
    }

    public void OpenOption()
    {
        if (SoundManager.Instance != null)
        {
            // 열 때 저장된 볼륨 복구
            savedBGMVolume = SoundManager.Instance.GetBGMVolume();
            savedSFXVolume = SoundManager.Instance.GetSFXVolume();

            bgmSlider.value = savedBGMVolume;
            sfxSlider.value = savedSFXVolume;
        }

        optionPanel.SetActive(true);
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    public void OnClickBack()
    {
        // 저장된 볼륨 복구
        SoundManager.Instance?.SetBGMVolume(savedBGMVolume);
        SoundManager.Instance?.SetSFXVolume(savedSFXVolume);

        // 슬라이더도 복구
        bgmSlider.value = savedBGMVolume;
        sfxSlider.value = savedSFXVolume;

        optionPanel.SetActive(false);
        if (pausePanel != null)
            pausePanel.SetActive(true);
    }

    public void OnClickSave()
    {
        // 현재 슬라이더 값 그대로 저장 (이미 적용된 상태)
        savedBGMVolume = bgmSlider.value;
        savedSFXVolume = sfxSlider.value;

        SoundManager.Instance?.SaveVolume(); // 볼륨 값 저장

        optionPanel.SetActive(false);
        if (pausePanel != null)
            pausePanel.SetActive(true);
    }

}