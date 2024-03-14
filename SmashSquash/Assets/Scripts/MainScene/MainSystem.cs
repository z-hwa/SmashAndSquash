using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 主頁面的統籌系統
/// </summary>
public class MainSystem : MonoBehaviour
{
    public static MainSystem instance;

    //負責管理整個主頁面的系統
    private SavedAndLoaded savedAndLoaded;
    private StorySystem storySystem;
    private PlayerAccountSystem playerAccountSystem;
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

        //DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        //獲取系統
        savedAndLoaded = SavedAndLoaded.instance;
        storySystem = StorySystem.instance;
        playerAccountSystem = PlayerAccountSystem.instance;
        musicSystem = MusicSystem.instance;

        //載入存檔
        savedAndLoaded.LoadData();

        //撥放主頁面BGM
        musicSystem.PlayMusic(ConstantChart.backgroundBGM);
        musicSystem.ChangePitch(0.66f);

        //檢測是否撥放故事
        if(playerAccountSystem.storyRecorder.interfaceUsageTutorial == false) storySystem.LoadingStory("NoviceTeaching/interfaceUsageTutorial");
    }
}
