using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家帳號系統
/// </summary>
public class PlayerAccountSystem : MonoBehaviour
{
    public static PlayerAccountSystem instance;

    private PlayerInfo playerInfo;

    [Header("玩家資料")]
    public string playerName = "暫無名稱";
    public int playerRank = 0;
    public int spirit = 0;  //靈力用於提升等級
    public int crystal = 0; //靈晶用於抽獎

    public int threa = 0;   //靈粹 用於維持靈力掌控程度

    [Header("故事播放記錄")]
    public StoryRecorder storyRecorder = new StoryRecorder();

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
    }

    void Start()
    {
        playerInfo = PlayerInfo.instance;
    }

    /// <summary>
    /// 展示玩家資料
    /// </summary>
    public void ShowPlayerInfo()
    {
        playerInfo.ShowPlayerInfo();
    }
}
