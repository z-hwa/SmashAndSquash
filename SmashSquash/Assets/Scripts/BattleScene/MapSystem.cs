using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 負責處理戰鬥中，跟地圖以及敵人有關的資料
/// </summary>
public class MapSystem : MonoBehaviour
{
    public static MapSystem instance;

    [Header("地圖資訊")]
    //地圖遊戲物件實體
    public GameObject nowMap;   //當前的地圖遊戲物件
    public MapInfo info;    //當前地圖資料

    //地圖生成位置
    public Transform mapInitPoint;

    //會從地圖資訊獲取以下資料 並透過init設定
    //敵人設定
    public int enemyNum;    //敵人數量
    [HideInInspector] public GameObject[] enemy;  //敵人物件實體
    [HideInInspector] public UnitData_InBattle[] enemyDatas;    //敵人實體使用的資料腳本
    [HideInInspector] public UnitBehavior[] enemyBehaviors;    //敵人實體使用的行為腳本

    public Color camp_represent = new Color(255 / 255, 127 / 255, 137 / 255, 255 / 255);  //敵人單位的陣營表示顏色

    //玩家設定
    public int playerNum;
    [HideInInspector] public Transform[] playerInitPos; //玩家出生位置

    //系統
    public MapBook mapBook;

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

    /// <summary>
    /// 初始化當前地圖，根據地圖index
    /// 包括敵人資料、玩家生成位置
    /// </summary>
    /// <param name="mapIndex">需要初始化的地圖index</param>
    public void InitMap(int mapIndex)
    {
        mapBook = MapBook.instance; //獲取地圖系統

        //根據給的id 生成地圖
        nowMap = Instantiate(mapBook.map_illustratedBook[mapIndex], mapInitPoint.position, Quaternion.identity);

        //從地圖獲取敵人相關資訊
        info = nowMap.GetComponent<MapInfo>();
        enemyNum = info.enemyNum; //敵人數量
        enemy = info.enemy;   //敵人實體

        //生成敵人實體的腳本
        enemyBehaviors = new UnitBehavior[enemyNum];
        enemyDatas = new UnitData_InBattle[enemyNum];

        //初始化敵人 行為、資料腳本
        for(int i = 0;i < enemyNum;i++)
        {
            //行為、物理性質
            enemyBehaviors[i] = enemy[i].GetComponent<UnitBehavior>();  //獲得單位實體的行為腳本
            enemyBehaviors[i].InitUnitBehavior();    //初始化實體的行為腳本

            //獲取敵人單位的資料腳本
            A000_Default a000_Default = enemy[i].transform.GetChild(2).GetComponent<A000_Default>();    //獲取敵人的全局資料腳本
            a000_Default.InitUnitData(); //初始化地圖設定的資料，到敵人單位資料中
            
            //敵人資料
            enemyDatas[i] = enemy[i].GetComponent<UnitData_InBattle>();  //獲得單位實體的資料腳本
            enemyDatas[i].InitUnitData_InBattle(a000_Default.use_data, a000_Default.use_data, CampGroup.enemy, -1);    //根據敵人的單位資料，初始化實體的資料腳本
            enemyDatas[i].Init();

            //美術
            //初始化單位美術
            enemy[i].transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = a000_Default.use_data.circleSprite; //獲取單位圖片
            enemy[i].transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().color = camp_represent;    //設置陣營圓環
        }

        //初始化玩家出生位置
        playerNum = info.playerNum; //獲取玩家數量
        playerInitPos = new Transform[ConstantChart.MAXLINEUP]; //生成出生位置實體
        GameObject obj = info.InitPosParent;

        for(int i=0;i<playerNum;i++)
        {
            playerInitPos[i] = obj.transform.GetChild(i).transform; //獲取玩家單位出生位置
        }
    }
}
