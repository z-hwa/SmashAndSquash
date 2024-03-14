using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 主介面的玩家資料顯示
/// 管理玩家在主頁面 資料顯示的部分 基本上只會在main page
/// </summary>
public class PlayerInfo : MonoBehaviour
{

    public static PlayerInfo instance;

    [Header("玩家資料物件")]
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI playerRank;
    public TextMeshProUGUI spirit;
    public TextMeshProUGUI crystal;

    [Header("命名相關")]
    public GameObject renamePage;
    public TMP_InputField renameInput;

    //系統
    private PlayerAccountSystem playerAccountSystem;
    private BottomBar bottomBar;
    private SavedAndLoaded savedAndLoaded;
    private RadioSystem radioSystem;

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
        //加載系統
        playerAccountSystem = PlayerAccountSystem.instance;
        bottomBar = BottomBar.instance;
        savedAndLoaded = SavedAndLoaded.instance;
        radioSystem = RadioSystem.instance;

        //顯示玩家資訊
        ShowPlayerInfo();
    }

    /// <summary>
    /// 顯示玩家資料
    /// </summary>
    public void ShowPlayerInfo()
    {
        if(playerAccountSystem == null) playerAccountSystem = PlayerAccountSystem.instance;
        playerName.text = playerAccountSystem.playerName;
        playerRank.text = "Rank: " + playerAccountSystem.playerRank.ToString();
        spirit.text = "靈力: " + playerAccountSystem.spirit.ToString();
        crystal.text = "靈晶: " + playerAccountSystem.crystal.ToString();
    }

    /// <summary>
    /// 顯示重命名畫面
    /// </summary>
    public void ShowRenamePage()
    {
        if(bottomBar.otherCanva.activeSelf == true) renamePage.SetActive(true);
        else
        {
            Debug.Log("現在不是\"其他\"頁面 無法進行改名");
            radioSystem.PlayRadio("現在不是\"其他\"頁面 無法進行改名", RadioType.System);
        }
    }

    /// <summary>
    /// 結束命名
    /// </summary>
    /// <param name="isConfirmed">是否確認</param>
    public void EndRename(bool isConfirmed)
    {
        if(isConfirmed == true)
        {
            //開發者選項
            if (renameInput.text == "money666")
            {
                playerAccountSystem.crystal = 900000;
                playerAccountSystem.threa = 900000;
                playerAccountSystem.spirit = 900000;
                radioSystem.PlayRadio("進入開發者模式", RadioType.System);
            }

            playerAccountSystem.playerName = renameInput.text;
            ShowPlayerInfo();
            renameInput.text = string.Empty;
            renamePage.SetActive(false);
            savedAndLoaded.SaveData();  //存檔
        }
        else if(isConfirmed == false)
        {
            renameInput.text = string.Empty;
            renamePage.SetActive(false);
        }
    }
}
