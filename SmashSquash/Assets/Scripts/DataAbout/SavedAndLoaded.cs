using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 儲存以及載入系統
/// </summary>
public class SavedAndLoaded: MonoBehaviour
{
    //單例模式
    public static SavedAndLoaded instance;

    //儲存資料模板
    public PlayerData playerData;
    //public A000_Saved a000_Saved;
    public A000_Data a000_Data; //單位資料
    public StoryRecorder storyRecorder;

    private PackageSystem packageSystem;
    private PlayerAccountSystem playerAccountSystem;

    //系統
    private LoadingSystem loadingSystem;
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

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        //加載系統
        loadingSystem = LoadingSystem.instance;
        packageSystem = PackageSystem.instance;
        playerAccountSystem = PlayerAccountSystem.instance;
        radioSystem = RadioSystem.instance;
        
        //實例化 資料模板
        playerData = new PlayerData();  
        //a000_Saved = new A000_Saved();
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
        radioSystem.PlayRadio("資料保存成功", RadioType.System);
    }

    /// <summary>
    /// 讀檔
    /// </summary>
    public void LoadData()
    {
        Debug.Log("do deserialize");

        LoadPlayerData();
        if(loadingSystem.isDevelop == false) LoadStoryIsReadData();

        Debug.Log("loading...");
        Debug.Log("loaded.");
        radioSystem.PlayRadio("資料加載成功", RadioType.System);
    }

    /// <summary>
    /// 獲取玩家資料
    /// </summary>
    private void GetPlayerData()
    {
        //獲取玩家資料
        Debug.Log("Getting player data");
        playerData.playerName = playerAccountSystem.playerName;
        playerData.playerRank = playerAccountSystem.playerRank;
        playerData.crystal = playerAccountSystem.crystal;
        playerData.spirit = playerAccountSystem.spirit;
        playerData.threa = playerAccountSystem.threa;

        //背包單位數量
        playerData.packageCapacity = packageSystem.packageCapacity;

        //複製出戰對列
        playerData.unitNumber = packageSystem.unitNumber;
        for(int i = 0;i<ConstantChart.MAXLINEUP;i++)
        {
            playerData.lineUpId[i] = packageSystem.lineUpId[i];
        }
    }

    /// <summary>
    /// 儲存背包資料
    /// </summary>
    private void SavePackageData()
    {
        var savePath = Application.persistentDataPath;

        //複製背包單位
        for (int i = 0; i < packageSystem.packageCapacity; i++)
        {
            //獲取背包單位資料
            a000_Data.Copy(packageSystem.unit_object[i].GetComponent<A000_Default>().use_data, false);
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
        storyRecorder = playerAccountSystem.storyRecorder;  //獲取故事記錄系統

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
            playerAccountSystem.playerName = playerData.playerName;
            playerAccountSystem.playerRank = playerData.playerRank;
            playerAccountSystem.spirit = playerData.spirit;
            playerAccountSystem.crystal = playerData.crystal;
            playerAccountSystem.threa = playerData.threa;

            //存檔系統不要干涉遊戲物件
            //playerAccountSystem.ShowPlayerInfo();

            //載入背包
            LoadPackageData();

            //載入對列
            packageSystem.unitNumber = playerData.unitNumber;
            for (int i = 0; i < ConstantChart.MAXLINEUP; i++)
            {
                //packageSystem.lineUpId[i] = playerData.lineUpId[i];

                int id = playerData.lineUpId[i];

                //如果該出戰位置 存在單位
                if (id != -1)
                {
                    packageSystem.LineUpUnitWithID(id, i);
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
            playerAccountSystem.storyRecorder = storyRecorder;
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
        if(packageSystem.packageCapacity == 0)
        {
            //載入背包單位
            for (int i = 0; i < playerData.packageCapacity; i++)
            {
                //獲取存檔中的單位資料
                var json = System.IO.File.ReadAllText($"{savePath}/packageData_{i}.json");
                a000_Data = JsonUtility.FromJson<A000_Data>(json);
                packageSystem.AddUnitWithID(a000_Data.id, a000_Data);

                /*var json = System.IO.File.ReadAllText($"{savePath}/packageData_{i}.json");
                a000_Saved = JsonUtility.FromJson<A000_Saved>(json);
                packageSystem.AddUnitWithID(a000_Saved.id, a000_Saved);*/
            }
        }
    }
}
