using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// 音频管理器，存储所有音频并且可以播放和停止
/// </summary>
public class AudioManagement : MonoBehaviour
{
    [System.Serializable]
    // 存储单个音频的信息
    public class Sound
    {
        [Header("音频剪辑")]
        public AudioClip clip;

        [Header("音频分组")]
        public AudioMixerGroup outputGroup;

        [Header("音频音量")]
        [Range(0,1)]
        public float volume;

        [Header("音频是否开局播放")]
        public bool playOnAwake;

        [Header("音频是否循环播放")]
        public bool loop;
    }
    /// <summary>
    /// 存储所有的音频
    /// </summary>
    public List<Sound> sounds;
    /// <summary>
    /// 每一个音频剪辑的名称对应一个音频组件
    /// </summary>
    private Dictionary<string, AudioSource> audiosDic;

    /// <summary>
    /// 单例模式
    /// </summary>
    private static AudioManagement instance;

    private void Awake()
    {
        instance = this;
        audiosDic = new Dictionary<string, AudioSource>();
    }

    private void Start()
    {
        foreach (var sound in sounds)
        {
            GameObject obj = new GameObject(sound.clip.name);
            obj.transform.SetParent(this.transform);

            AudioSource source = obj.AddComponent<AudioSource>();
            source.clip = sound.clip;
            sound.playOnAwake = sound.playOnAwake;
            sound.loop = sound.loop;
            sound.volume = sound.volume;
            source.outputAudioMixerGroup = sound.outputGroup;

            if(sound.playOnAwake)
                source.Play();

            audiosDic.Add(sound.clip.name,source);
        }

    }
    /// <summary>
    /// 播放某一个音频
    /// </summary>
    /// <param name="name">音频名称</param>
    /// <param name="isWait">是否等待音频播放完</param>
    public static void PlayAudio(string name, bool isWait = false )
    {
        if (!instance.audiosDic.ContainsKey(name))
        {
            print("名为\" + name + \"的音频不存在");
            return;
        }

        if (isWait)
        {
            if (!instance.audiosDic[name].isPlaying)
            {
                
                instance.audiosDic[name].Play();
            }
        }
        else
        {
            if (!instance.audiosDic[name].isPlaying)
                instance.audiosDic[name].Play();
        }
    }

    /// <summary>
    /// ͣ停止某一音频的播放
    /// </summary>
    /// <param name="name">音频名称</param>
    public static void StopAudio(string name)
    {
        if (!instance.audiosDic.ContainsKey(name))
        {
            print("名为\" + name + \"的音频不存在");
            return;
        }
        instance.audiosDic[name].Stop();
    }

    public static void StopAllAudio()
    {
        foreach(var music in instance.sounds)
        {          
            StopAudio(music.clip.name);
        }
    }
}
