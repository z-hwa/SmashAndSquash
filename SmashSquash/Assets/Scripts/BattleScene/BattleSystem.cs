using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 戰鬥中的狀態，用於確認當前戰鬥情況
/// </summary>
public enum BattleState
{
    start,
    playerTurn,
    enemyTurn,
    end,
}

/// <summary>
/// 每個回合後，當前戰鬥結果的檢測
/// </summary>
public enum CheckReult
{
    win,
    lose,
    none
}

/// <summary>
/// 主要戰鬥系統，掌握整個戰鬥過程中所有的資訊
/// </summary>
public class BattleSystem : MonoBehaviour
{
    public static BattleSystem instance;
    
    //玩家選擇出戰的unit對列
    [Header("背包相關")]
    public GameObject[] unitLineUp = new GameObject[ConstantChart.MAXLINEUP];  //玩家的單位原始資料腳本，用於抓取腳色的技能

    //地圖相關
    [Header("地圖相關")]
    public int mapIndex = -1;    ///這場戰鬥中使用的地圖的ID

    //戰鬥相關設定
    [Header("戰鬥前置相關設定")]
    public BattleState nowState = BattleState.start;    //當前戰鬥回合的狀態為 開始
    public GameObject unitPrefab;   //單位預制體
    public Transform unitInitPoint;     //單位生成位置，從這場戰鬥的地圖獲取
    public int turnNum = 0; //當前戰鬥回合數
    public int halfTurn = 0;    //當前半場回合數

    //目前最大四隻腳色進場
    [Header("單位相關")]
    public GameObject[] unit = new GameObject[ConstantChart.MAXLINEUP];   //單位的遊戲物件，生成實體後放在這裡面，用於管理單位，包括behavior、unitdata
    public UnitData_InBattle[] unitDatas = new UnitData_InBattle[ConstantChart.MAXLINEUP];    //單位在戰鬥中，使用的temp遊戲資料，背包中的物件
    public UnitBehavior[] unitBehaviors = new UnitBehavior[ConstantChart.MAXLINEUP];    //單位的行為腳本
    public Color camp_represent = new Color(168 / 255, 255 / 255, 136 / 255, 255 / 255);  //玩家單位的陣營表示顏色
    public int unitNumber;  //玩家的出戰單位數量，從背包中獲取

    [Header("敵人相關")]
    public GameObject enemy;    //當前操控的敵人單位實體

    [Header("操作相關")]
    public int nowTurnUnit; //這個回合操控的玩家單位
    public int nowTurnEnemy;    //這個回合操控的敵方單位

    //System
    private PlayerControlSystem playerControlSystem;
    private CameraSystem cameraSystem;
    private MapSystem mapSystem;
    private PackageSystem packageSystem;
    private SettlementSystem settlementSystem;
    private MapBook mapBook;
    private MusicSystem musicSystem;
    private EnemyAI enemyAI;
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
    }

    // Start is called before the first frame update
    void Start()
    {
        //獲取系統
        packageSystem = PackageSystem.instance;
        settlementSystem = SettlementSystem.instance;
        mapSystem = MapSystem.instance;
        cameraSystem = CameraSystem.instance;
        playerControlSystem = PlayerControlSystem.instance;
        mapBook = MapBook.instance;
        musicSystem = MusicSystem.instance;
        enemyAI = EnemyAI.instance;
        radioSystem = RadioSystem.instance;

        InitLineUp();   //初始化玩家的出戰列表

        InitBattle();   //初始化戰鬥相關設置

        mapSystem.info.ShowStory();  //檢測是否需要播放地圖故事

        musicSystem.PlayMusic(ConstantChart.battleBGM);    //撥放戰鬥音樂
        
        StartCoroutine(PlayerTurn());   //進入玩家回合
    }

    /// <summary>
    /// 初始化出戰列表
    /// 並載入玩家出戰數量
    /// </summary>
    private void InitLineUp()
    {
        unitLineUp = packageSystem.lineUp;  //載入單位列表
        unitNumber = packageSystem.unitNumber;  //載入出戰單位數量
    }

    /// <summary>
    /// 玩家回合
    /// </summary>
    /// <returns></returns>
    IEnumerator PlayerTurn()
    {
        nowState = BattleState.playerTurn;
        radioSystem.PlayRadio("玩家回合", RadioType.System);
        CountTurn();    //計算回合數

        //攝影機給到單位
        //設置這回合操控的單位
        cameraSystem.TargetChange(unit[nowTurnUnit]);
        playerControlSystem.ChangeControlUnit(unit[nowTurnUnit]);

        //被動觸發階段
        unitLineUp[nowTurnUnit].GetComponent<A000_Default>().UsingSkill(instance, 0, SkillType.Active);

        //啟動玩家控制系統的 操控可用
        playerControlSystem.manipulateAvailable = true;

        //等待玩家操控
        yield return new WaitUntil(() => playerControlSystem.manipulateAvailable == false);

        yield return new WaitUntil(() => IsAllUnitStop());  //等待單位停下來

        //玩家贏或輸的判斷 如果都是錯的 會確保其他狀況中一定能找到存活的目標
        //判斷遊戲狀態
        if (CheckGameState() == CheckReult.win)
        {
            //勝利結算畫面
            ShowSettlement(true);
            radioSystem.PlayRadio("任務成功", RadioType.System);
        }
        else if (CheckGameState() == CheckReult.lose)
        {
            //失敗結算畫面
            ShowSettlement(false);
            radioSystem.PlayRadio("任務失敗", RadioType.System);
        }
        else
        {
            nowTurnUnit = (nowTurnUnit + 1) % unitNumber;   //下回合 輪到下一個單位

            PreventReuseOfDeadUnit();
            StartCoroutine(EnemyTurn());    //敵人回合
        }
    }

    /// <summary>
    /// 敵人回合
    /// </summary>
    /// <returns></returns>
    IEnumerator EnemyTurn()
    {
        nowState = BattleState.enemyTurn;   //敵人回合
        radioSystem.PlayRadio("敵人回合", RadioType.System);
        CountTurn();    //計算回合數

        enemy = mapSystem.enemy[nowTurnEnemy];  //獲得當前操控的敵人單位

        //攝影機給到敵人 (DEBUG:以不同的攝影機移動方式
        cameraSystem.TargetChange(enemy);

        yield return new WaitForSeconds(2f);  //等待攝影機移動

        //執行對應敵人agent的互動(攻擊
        //mapSystem.settingData[nowTurnEnemy].EnemyAgent(enemy, unit, unitNumber);
        //mapSystem.settingData[nowTurnEnemy].EnemyAgentNEW(this);
        enemyAI.SetEA(this, enemy.GetComponent<UnitData_InBattle>().EA_name);

        yield return new WaitUntil(() => IsAllUnitStop());  //等待單位停下來

        //玩家贏或輸的判斷 如果都是錯的 會確保其他狀況中一定能找到存活的目標
        //判斷遊戲狀態
        if (CheckGameState() == CheckReult.win)
        {
            //勝利結算畫面
            ShowSettlement(true);
            radioSystem.PlayRadio("任務成功", RadioType.System);
        }
        else if (CheckGameState() == CheckReult.lose)
        {
            //失敗結算畫面
            ShowSettlement(false);
            radioSystem.PlayRadio("任務失敗", RadioType.System);
        }
        else
        {
            nowTurnEnemy = (nowTurnEnemy + 1) % mapSystem.enemyNum;   //下回合 輪到下一個敵人單位

            PreventReuseOfDeadUnit();
            StartCoroutine(PlayerTurn()); //玩家回合
        }
    }

    /// <summary>
    /// 計算當前回合數
    /// 每半場呼叫一次
    /// </summary>
    private void CountTurn()
    {
        halfTurn++;
        turnNum = halfTurn / 2 + 1;
    }

    /// <summary>
    /// 顯示結算畫面
    /// 用於遊戲結束後，進行遊戲的結算
    /// </summary>
    /// <param name="isPlayerWin">傳入玩家是否勝利</param>
    private void ShowSettlement(bool isPlayerWin)
    {
        if (isPlayerWin == true)
        { 
            settlementSystem.ShowSettlementPage("任務成功", true);
        }else
        {
            settlementSystem.ShowSettlementPage("任務失敗", false);
        }
    }

    /// <summary>
    /// 再次檢測敵我雙方的出戰單位，確保死亡單位不會被重複使用
    /// </summary>
    private void PreventReuseOfDeadUnit()
    {
        //確保不會讓死亡單位被重複使用
        while (mapSystem.enemy[nowTurnEnemy].gameObject.activeSelf == false)
        {
            nowTurnEnemy = (nowTurnEnemy + 1) % mapSystem.enemyNum;   //這個單位死亡 輪到下一個敵人單位
        }

        //確保不會讓死亡單位被重複使用
        while (unitDatas[nowTurnUnit].gameObject.activeSelf == false)
        {
            nowTurnUnit = (nowTurnUnit + 1) % unitNumber;   //這個單位死亡 輪到下一個單位
        }
    }

    /// <summary>
    /// 判斷場上狀況是否都停止，以進入到下一個回合
    /// </summary>
    /// <returns>場中狀況是否停止</returns>
    public bool IsAllUnitStop()
    {
        bool isStop = true;

        for(int i=0;i<unitNumber;i++)
        {
            //遊戲物件激活中 且正在移動 => 彈珠們還在動
            if (unitDatas[i].gameObject.activeSelf == true &&
                unitBehaviors[i].nowState == UnitMovingState.Moving) isStop = false;
        }

        for (int i = 0; i < mapSystem.enemyNum; i++)
        {
            //遊戲物件激活中 且正在移動 => 彈珠們還在動
            if (mapSystem.enemy[i].gameObject.activeSelf == true &&
                mapSystem.enemyBehaviors[i].nowState == UnitMovingState.Moving) isStop = false;
        }

        return isStop;
    }

    /// <summary>
    /// 判斷該回合結束後，整場戰鬥的結果是否出現
    /// </summary>
    /// <returns>戰鬥的結果</returns>
    private CheckReult CheckGameState()
    {
        bool isWin = true;
        bool isLose = true;

        //如果有一個敵方單位存活
        for (int i = 0; i < mapSystem.enemyNum; i++)
        {
            if (mapSystem.enemy[i].gameObject.activeSelf == true) isWin = false;
        }

        //如果有一個玩家單位存活
        for (int i = 0; i < unitNumber; i++)
        {
            if (unit[i].gameObject.activeSelf == true) isLose = false;
        }

        if (isWin == true) return CheckReult.win;
        else if (isLose == true) return CheckReult.lose;
        else return CheckReult.none;
    }

    /// <summary>
    /// 初始化戰鬥中的所有設置
    /// </summary>
    private void InitBattle()
    {
        nowState = BattleState.start;   //當前狀態: 開始狀態

        mapIndex = mapBook.mapIndex;    //獲得關卡ID
        mapSystem.InitMap(mapIndex);    //根據關卡設定的地圖ID 初始化關卡

        nowTurnUnit = 0;    //第一回合 玩家可以使用的單位index 0
        nowTurnEnemy = 0;   //敵人第一回合使用的單位

        //載入玩家單位的數據
        for (int i = 0; i < ConstantChart.MAXLINEUP; i++)
        {
            //如果這個位置不存在 則跳過
            if (unitLineUp[i] == null) continue;

            //gameobject
            unit[i] = Instantiate(unitPrefab, mapSystem.playerInitPos[i].position, Quaternion.identity); //生成遊戲單位實體 放進Unit
            
            //行為、物理性質
            unitBehaviors[i] = unit[i].GetComponent<UnitBehavior>();    //獲得單位實體的行為腳本
            unitBehaviors[i].InitUnitBehavior();    //初始化實體的行為腳本

            //獲取玩家單位的資料腳本
            A000_Default a000_Default = unitLineUp[i].GetComponent<A000_Default>();
            a000_Default.InitSkill();    //初始化單位的資料腳本-技能實作部分

            //單位資料
            unitDatas[i] = unit[i].GetComponent<UnitData_InBattle>();    //獲得單位實體的資料腳本
            unitDatas[i].InitUnitData_InBattle(a000_Default.use_data, a000_Default.use_data, CampGroup.player, i);  //根據玩家的單位資料，初始化實體的資料腳本

            //美術
            //初始化單位美術
            unit[i].transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = a000_Default.use_data.circleSprite; //獲取單位圖片
            unit[i].transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().color = camp_represent; //設置陣營圓環
        }

        //初始化玩家操控系統
        playerControlSystem.InitPlayerControlSystem();

        //初始化cameraSystem, PlayerSystem的鎖定單位為，現在回合的玩家操控單位
        playerControlSystem.ChangeControlUnit(unit[nowTurnUnit]);
        cameraSystem.TargetChange(unit[nowTurnUnit]);
    }
}
