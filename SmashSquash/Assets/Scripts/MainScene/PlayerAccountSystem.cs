using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家帳號系統
/// </summary>
public class PlayerAccountSystem : MonoBehaviour
{
    private static PlayerAccountSystem _playerAccountSystem;

    /// <summary>
    /// 單例模式，避免重複生成的檢查
    /// </summary>
    public static PlayerAccountSystem Instance
    {
        get
        {
            if(_playerAccountSystem == null)
            {
                _playerAccountSystem = FindObjectOfType(typeof(PlayerAccountSystem)) as PlayerAccountSystem;
                if(_playerAccountSystem == null) { Debug.LogError("no object with PlayerAccountSystem"); }
                else
                {
                    _playerAccountSystem.Init();
                }
            }

            return _playerAccountSystem;
        }
    }

    [Header("玩家資料")]
    public string playerName = "暫無名稱";
    public int playerRank = 0;
    public int spirit = 0;  //靈力用於提升等級
    public int crystal = 0; //靈晶用於抽獎

    public int threa = 0;   //靈粹 用於維持靈力掌控程度

    [Header("故事播放記錄")]
    public StoryRecorder storyRecorder = new StoryRecorder();

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 展示玩家資料
    /// </summary>
    public void ShowPlayerInfo()
    {
        //playerInfo.ShowPlayerInfo();
        EventManager.Trigger(EventName.ShowPlayerInfo);
    }
}
