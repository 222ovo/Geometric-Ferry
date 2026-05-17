using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingManager : MonoBehaviour
{
    [Header("“Ù∆µªÏœÏ∆˜")]
    public static AudioMixer mixer;

    public static void SetBGMVolume(float value)
    {
        mixer.SetFloat("BGM", value);
    }

    public static void SetSFXVolume(float value)
    {
        mixer.SetFloat("SFX",value);
    }
}
