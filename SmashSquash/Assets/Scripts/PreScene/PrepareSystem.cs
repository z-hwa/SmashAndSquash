using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 開始介面的準備系統
/// </summary>
public class PrepareSystem : MonoBehaviour
{
    public static PrepareSystem instance;

    //各種系統
    private SavedAndLoaded savedAndLoaded;
    private PlayerAccountSystem playerAccountSystem;
    private MusicSystem musicSystem;
    private LoadingSystem loadingSystem;

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

        //DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        savedAndLoaded = SavedAndLoaded.instance;
        playerAccountSystem = PlayerAccountSystem.instance;
        musicSystem = MusicSystem.instance;
        loadingSystem = LoadingSystem.instance;
        
        //撥放開始介面BGM
        musicSystem.PlayMusic(ConstantChart.backgroundBGM); 
    }

    //載入主頁面
    //加載全局資源
    public void TouchToStart()
    {
        savedAndLoaded.LoadData();  //載入存檔

        //載入場景
        if (playerAccountSystem.storyRecorder.openingIntro == false)
        {
            loadingSystem.LoadTargetScene("OpeningScene");
        }
        else
        {
            loadingSystem.LoadTargetScene("MainScene");
        }
    }
}
