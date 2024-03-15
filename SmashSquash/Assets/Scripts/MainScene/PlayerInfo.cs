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
    private BottomBar bottomBar;
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

        EventManager.AddListener(EventName.ShowPlayerInfo, ShowPlayerInfo);
    }

    void Start()
    {
        //加載系統
        bottomBar = BottomBar.instance;
        radioSystem = RadioSystem.instance;

        //顯示玩家資訊
        ShowPlayerInfo();
    }

    /// <summary>
    /// 顯示玩家資料
    /// </summary>
    public void ShowPlayerInfo()
    {
        //Debug.Log("顯示玩家資訊");

        playerName.text = PlayerAccountSystem.Instance.playerName;
        playerRank.text = "Rank: " + PlayerAccountSystem.Instance.playerRank.ToString();
        spirit.text = "靈力: " + PlayerAccountSystem.Instance.spirit.ToString();
        crystal.text = "靈晶: " + PlayerAccountSystem.Instance.crystal.ToString();
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
                PlayerAccountSystem.Instance.crystal = 900000;
                PlayerAccountSystem.Instance.threa = 900000;
                PlayerAccountSystem.Instance.spirit = 900000;
                radioSystem.PlayRadio("進入開發者模式", RadioType.System);
            }

            PlayerAccountSystem.Instance.playerName = renameInput.text;
            ShowPlayerInfo();
            renameInput.text = string.Empty;
            renamePage.SetActive(false);
            SavedAndLoaded.Instance.SaveData();  //存檔
        }
        else if(isConfirmed == false)
        {
            renameInput.text = string.Empty;
            renamePage.SetActive(false);
        }
    }
}
