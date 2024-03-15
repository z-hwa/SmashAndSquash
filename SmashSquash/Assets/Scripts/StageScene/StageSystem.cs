using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 關卡系統
/// </summary>
public class StageSystem : MonoBehaviour
{
    public static StageSystem instance;

    //系統
    private MapBook mapBook;
    private LoadingSystem loadingSystem;

    [Header("關卡物件")]
    public GameObject content;  //關卡資訊的爸爸
    public GameObject stageInf; //關卡資訊預製件

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
        //獲取所需系統
        mapBook = MapBook.instance;
        loadingSystem = LoadingSystem._loadingSystem;
        
        //載入關卡列表
        LoadStageList();    
    }

    /// <summary>
    /// 從關卡介面，返回主介面
    /// </summary>
    public void BackToMain()
    {
        loadingSystem.LoadTargetScene("MainScene");
    }

    /// <summary>
    /// 載入關卡介面的關卡列表
    /// </summary>
    private void LoadStageList()
    {
        int num = mapBook.map_illustratedBook.Length;   //獲取關卡總數量
        content.GetComponent<AdvancedGridLayoutGroupVertical>().cellNum = num;  //更新關卡列表中的單位數量

        for(int i = 0; i < num; i++)
        {
            //生成關卡資訊物件
            GameObject obj = Instantiate(stageInf, content.transform);

            //設置關卡資訊
            SetStageInfo(obj, i);
        }
    }

    /// <summary>
    /// 設置關卡的資訊
    /// </summary>
    /// <param name="obj">關卡資訊的顯示物件</param>
    /// <param name="index">該關卡的index</param>
    private void SetStageInfo(GameObject obj, int index)
    {
        //獲取腳本組件
        StageInfo stage = obj.GetComponent<StageInfo>();
        MapInfo map = mapBook.map_illustratedBook[index].GetComponent<MapInfo>();

        //設置關卡資訊
        stage.mapIndex = map.mapIndex;
        stage.stageName.text = map.mapName;
        stage.stageDescription.text = "限制人數:" + map.playerNum;
    }
}
