using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 音樂撥放器的系統
/// </summary>
public class MusicSystem : MonoBehaviour
{
    public static MusicSystem instance;

    //音樂撥放器
    public AudioSource BGMPlayer;
    public AudioSource effectPlayer;

    /// <summary>
    /// 單例模式，避免重複生成的檢查
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        DontDestroyOnLoad(gameObject);

        //創建並設置撥放器
        SetPlayer();
    }

    /// <summary>
    /// 創建並設置撥放器
    /// </summary>
    private void SetPlayer()
    {
        BGMPlayer = gameObject.AddComponent<AudioSource>();
        BGMPlayer.loop = true;
        effectPlayer = gameObject.AddComponent<AudioSource>();
    }

    /// <summary>
    /// 撥放特效
    /// </summary>
    /// <param name="effectRoute">效果路徑</param>
    public void PlayEffect(string effectRoute)
    {
        AudioClip music = Resources.Load<AudioClip>(effectRoute);
        effectPlayer.clip = music;   //設置曲目
        effectPlayer.Play(); //撥放
    }

    /// <summary>
    /// 撥放指定音樂
    /// </summary>
    /// <param name="musicRoute">音樂路徑</param>
    public void PlayMusic(string musicRoute)
    {
        AudioClip music = Resources.Load<AudioClip>(musicRoute);
        BGMPlayer.clip = music;   //設置曲目
        BGMPlayer.Play(); //撥放
    }

    /// <summary>
    /// 改變音樂頻率
    /// </summary>
    /// <param name="newPitch">新的音樂頻率</param>
    public void ChangePitch(float newPitch)
    {
        BGMPlayer.pitch = newPitch;
    }

    /// <summary>
    /// 暫停音樂
    /// </summary>
    public void StopMusic()
    {
        BGMPlayer.Stop();
    }

    /// <summary>
    /// 撥放暫停的音樂
    /// </summary>
    public void StartMusic()
    {
        BGMPlayer.Play();
    }
}
