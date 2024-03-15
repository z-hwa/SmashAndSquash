using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 結算以及處理的系統
/// 也包含讓戰鬥系統處理非戰鬥相關事務的程式
/// 例如：顯示故事
/// </summary>
public class SettlementSystem : MonoBehaviour
{
    public static SettlementSystem instance;

    public GameObject settlementPage;   //結算頁面
    public TextMeshProUGUI title;    //結算頁面的顯示文字
    public TextMeshProUGUI award;   //獲取獎勵說明

    private PackageSystem packageSystem; //背包系統
    private MapSystem mapSystem;
    private StorySystem storySystem;

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
        packageSystem = PackageSystem.instance;
        mapSystem = MapSystem.instance;
        storySystem = StorySystem.instance;
    }

    /// <summary>
    /// 顯示故事
    /// </summary>
    public void ShowStory(string storyRoute)
    {
        storySystem = StorySystem.instance;

        //如果沒有設定故事路徑，且新手教學未完成，撥放新手教學
        if (storyRoute=="" && PlayerAccountSystem.Instance.storyRecorder.battleMethod == false) storySystem.LoadingStory("NoviceTeaching/battleMethod");
    }

    /// <summary>
    /// 顯示結算頁面
    /// </summary>
    /// <param name="text">結算頁面顯示的文字</param>
    /// <param name="isWin">是否勝利</param>
    public void ShowSettlementPage(string text, bool isWin)
    {
        GiveAward(isWin);
        SavedAndLoaded.Instance.SaveData();  //存檔

        settlementPage.SetActive(true);
        title.text = text;
    }

    /// <summary>
    /// 給予關卡獎勵
    /// </summary>
    /// <param name="isWin">是否勝利</param>
    private void GiveAward(bool isWin)
    {
        string awardInfo = "";

        awardInfo = mapSystem.info.GiveAward(isWin);    //給予獎勵並生成獎勵資訊

        award.text = awardInfo;
    }

    /// <summary>
    /// 回到主頁面
    /// </summary>
    public void BackToMain()
    {
        LoadingSystem.Instance.LoadTargetScene("MainScene");
    }
}
