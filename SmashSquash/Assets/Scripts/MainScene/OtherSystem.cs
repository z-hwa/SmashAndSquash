using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 主介面的設定系統
/// </summary>
public class OtherSystem : MonoBehaviour
{
    public static OtherSystem instance;

    [Header("音量相關設定")]
    public GameObject musicPage;
    public Slider BGMSlider;
    public Slider effectSlider;

    private MusicSystem musicSystem;

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
    }

    void Start()
    {
        //加載音樂系統
        musicSystem = MusicSystem.instance;
    }

    /// <summary>
    /// 顯示或關閉音樂撥放器頁面
    /// </summary>
    /// <param name="isShowed">是否顯示</param>
    public void SetMusicPage(bool isShowed)
    {
        if(isShowed == true)
        {
            musicPage.SetActive(true);

            //設置音量顯示
            BGMSlider.value = musicSystem.BGMPlayer.volume;
            effectSlider.value = musicSystem.effectPlayer.volume;
        }
        else if(isShowed == false) musicPage.SetActive(false);
    }

    /// <summary>
    /// 改變音量
    /// </summary>
    /// <param name="pleyerName">要改變的音量的撥放器名稱</param>
    public void ChangeVolumn(GameObject player)
    {
        if(player.name == "BGM") musicSystem.BGMPlayer.volume = BGMSlider.value;
        else if(player.name == "effect") musicSystem.effectPlayer.volume = effectSlider.value;
    }
}
