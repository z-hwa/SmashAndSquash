using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 儲存以及載入系統
/// </summary>
public class SavedAndLoaded: MonoBehaviour
{
    //單例模式
    private static SavedAndLoaded _savedAndLoaded;

    /// <summary>
    /// 單例模式，避免重複生成的檢查
    /// </summary>
    public static SavedAndLoaded Instance
    {
        get
        {
            if(_savedAndLoaded == null)
            {
                _savedAndLoaded = FindObjectOfType(typeof(SavedAndLoaded)) as SavedAndLoaded;
                if(_savedAndLoaded == null) Debug.LogError("There needs to be one active SavedAndLoaded script on a Gameobject"); //找不到輸出錯誤訊息
                else
                {
                    _savedAndLoaded.Init();
                }
            }

            return _savedAndLoaded;
        }
    }

    //儲存資料模板
    private PlayerData playerData;
    private A000_Data a000_Data; //單位資料
    private StoryRecorder storyRecorder;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        DontDestroyOnLoad(gameObject);
        EventManager.AddListener(EventName.LoadData, LoadData);
    }

    void Start()
    {
        
        //實例化 資料模板
        playerData = new PlayerData();  
        a000_Data = new A000_Data();
        storyRecorder = new StoryRecorder();
    }

    /// <summary>
    /// 存檔
    /// </summary>
    public void SaveData()
    {
        Debug.Log("do serialize");

        SavePlayerData();
        SavePackageData();
        SaveStoryIsReadData();

        Debug.Log("saving...");
        Debug.Log($"保存到{Application.persistentDataPath}");
        //radioSystem.PlayRadio("資料保存成功", RadioType.System);
        EventManager.Trigger(EventName.PlayRadio, "資料保存成功", RadioType.System);
    }

    /// <summary>
    /// 讀檔
    /// </summary>
    public void LoadData()
    {
        Debug.Log("do deserialize");

        LoadPlayerData();
        if(LoadingSystem.Instance.isDevelop == false) LoadStoryIsReadData();

        Debug.Log("loading...");
        Debug.Log("loaded.");
        EventManager.Trigger(EventName.PlayRadio, "資料加載成功", RadioType.System);
    }

    /// <summary>
    /// 獲取玩家資料
    /// </summary>
    private void GetPlayerData()
    {
        //獲取玩家資料
        Debug.Log("Getting player data");
        playerData.playerName = PlayerAccountSystem.Instance.playerName;
        playerData.playerRank = PlayerAccountSystem.Instance.playerRank;
        playerData.crystal = PlayerAccountSystem.Instance.crystal;
        playerData.spirit = PlayerAccountSystem.Instance.spirit;
        playerData.threa = PlayerAccountSystem.Instance.threa;

        //背包單位數量
        playerData.packageCapacity = PackageSystem.instance.packageCapacity;

        //複製出戰對列
        playerData.unitNumber = PackageSystem.instance.unitNumber;
        for(int i = 0;i<ConstantChart.MAXLINEUP;i++)
        {
            playerData.lineUpId[i] = PackageSystem.instance.lineUpId[i];
        }
    }

    /// <summary>
    /// 儲存背包資料
    /// </summary>
    private void SavePackageData()
    {
        var savePath = Application.persistentDataPath;

        //複製背包單位
        for (int i = 0; i < PackageSystem.instance.packageCapacity; i++)
        {
            //獲取背包單位資料
            a000_Data.Copy(PackageSystem.instance.unit_object[i].GetComponent<A000_Default>().use_data, false);
            var json = JsonUtility.ToJson(a000_Data);
            System.IO.File.WriteAllText($"{savePath}/packageData_{i}.json", json);

            /*a000_Saved.CopyData(packageSystem.unit_object[i].GetComponent<A000_Default>());
            var json = JsonUtility.ToJson(a000_Saved);
            System.IO.File.WriteAllText($"{savePath}/packageData_{i}.json", json);*/
        }
    }

    /// <summary>
    /// 儲存玩家資料
    /// </summary>
    private void SavePlayerData()
    {
        GetPlayerData();

        //序列化
        var json = JsonUtility.ToJson(playerData);

        //存檔
        var savePath = Application.persistentDataPath;
        System.IO.File.WriteAllText($"{savePath}/PlayerData.json", json);
    }

    /// <summary>
    /// 儲存故事紀錄資料
    /// </summary>
    private void SaveStoryIsReadData()
    {
        storyRecorder = PlayerAccountSystem.Instance.storyRecorder;  //獲取故事記錄系統

        //序列化
        var json = JsonUtility.ToJson(storyRecorder);

        //存檔
        var savePath = Application.persistentDataPath;
        System.IO.File.WriteAllText($"{savePath}/StoryIsReadData.json", json);
    }

    /// <summary>
    /// 載入玩家、背包資料
    /// </summary>
    private void LoadPlayerData()
    {
        var savePath = Application.persistentDataPath;

        try
        {
            var json = System.IO.File.ReadAllText($"{savePath}/PlayerData.json");
            playerData = JsonUtility.FromJson<PlayerData>(json);

            //載入玩家資料
            PlayerAccountSystem.Instance.playerName = playerData.playerName;
            PlayerAccountSystem.Instance.playerRank = playerData.playerRank;
            PlayerAccountSystem.Instance.spirit = playerData.spirit;
            PlayerAccountSystem.Instance.crystal = playerData.crystal;
            PlayerAccountSystem.Instance.threa = playerData.threa;

            //存檔系統不要干涉遊戲物件
            //playerAccountSystem.ShowPlayerInfo();

            //載入背包
            LoadPackageData();

            //載入對列
            PackageSystem.instance.unitNumber = playerData.unitNumber;
            for (int i = 0; i < ConstantChart.MAXLINEUP; i++)
            {
                //packageSystem.lineUpId[i] = playerData.lineUpId[i];

                int id = playerData.lineUpId[i];

                //如果該出戰位置 存在單位
                if (id != -1)
                {
                    //packageSystem.LineUpUnitWithID(id, i);
                    EventManager.Trigger_f(EventName.LineUpUnitWithID, id, i);
                }
            }
        }
        catch (System.IO.FileNotFoundException e)
        {
            Debug.Log(e.ToString());
        }
    }

    /// <summary>
    /// 載入故事記錄資料
    /// </summary>
    private void LoadStoryIsReadData()
    {
        var savePath = Application.persistentDataPath;

        try
        {
            var json = System.IO.File.ReadAllText($"{savePath}/StoryIsReadData.json");
            storyRecorder = JsonUtility.FromJson<StoryRecorder>(json);

            //載入故事資料
            PlayerAccountSystem.Instance.storyRecorder = storyRecorder;
        }
        catch (System.IO.FileNotFoundException e)
        {
            Debug.Log(e.ToString());
        }
    }

    /// <summary>
    /// 載入背包資料
    /// </summary>
    private void LoadPackageData() {

        var savePath = Application.persistentDataPath;

        //沒有載入過資料 才進行背包載入的動作
        if(PackageSystem.instance.packageCapacity == 0)
        {
            //載入背包單位
            for (int i = 0; i < playerData.packageCapacity; i++)
            {
                //獲取存檔中的單位資料
                var json = System.IO.File.ReadAllText($"{savePath}/packageData_{i}.json");
                a000_Data = JsonUtility.FromJson<A000_Data>(json);
                //packageSystem.AddUnitWithID(a000_Data.id, a000_Data);
                EventManager.Trigger(EventName.AddUnitWithID, a000_Data.id, a000_Data);

                /*var json = System.IO.File.ReadAllText($"{savePath}/packageData_{i}.json");
                a000_Saved = JsonUtility.FromJson<A000_Saved>(json);
                packageSystem.AddUnitWithID(a000_Saved.id, a000_Saved);*/
            }
        }
    }
}
