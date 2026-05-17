using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    [Header("音频混合器")]
    public AudioMixer audioMixer;

    [Header("音量Slider")]
    public Slider bgmSlider;
    public Slider sfxSlider;

    [Header("参数名称")]
    [SerializeField] private string bgmParam = "BGM";
    [SerializeField] private string sfxParam = "SFX";

    private void Start()
    {
        InitializeSlider(bgmSlider, bgmParam);
        InitializeSlider(sfxSlider, sfxParam);
    }

    private void InitializeSlider(Slider slider, string paramName)
    {
        if (slider != null && audioMixer != null)
        {
            // 获取当前音量值
            float volume = PlayerPrefs.GetFloat(paramName, 0.75f);
            slider.value = volume;

            // 设置AudioMixer
            SetMixerVolume(paramName, volume);

            // 添加监听
            slider.onValueChanged.AddListener((value) =>
            {
                SetMixerVolume(paramName, value);
                PlayerPrefs.SetFloat(paramName, value);
            });
        }
    }

    private void SetMixerVolume(string paramName, float value)
    {
        if (audioMixer != null)
        {
            // 转换为对数音量（-80dB到0dB）
            float volumeInDB = value > 0 ? Mathf.Log10(value) * 20 : -80;
            audioMixer.SetFloat(paramName, volumeInDB);
        }
    }
}
