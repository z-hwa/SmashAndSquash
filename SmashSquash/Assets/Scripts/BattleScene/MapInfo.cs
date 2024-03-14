using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 每一張地圖的資料設定
/// 如果有特殊需求的地圖，可以繼承該地圖資料
/// </summary>
public class MapInfo : MonoBehaviour
{
    //地圖資訊
    [Header("地圖資訊")]
    public int mapIndex;    //關卡ID
    public string mapName; //關卡名稱
    public int enemyNum;    //敵人數量
    public GameObject[] enemy;  //敵人實體

    [Header("玩家設定")]
    public int playerNum;   //限制玩家數量
    public GameObject InitPosParent;    //玩家出生位置的父物件

    [Header("故事設定")]
    public string storyRoute = "";   //故事路徑

    [Header("固定獎勵")]
    public int spiritNum = 1000;
    public int crystalNum = 6;
    public int threaNum = 100;

    //系統
    private SettlementSystem settlementSystem;
    private PlayerAccountSystem playerAccountSystem;

    /// <summary>
    /// 撥放這張地圖的故事
    /// </summary>
    public void ShowStory()
    {
        settlementSystem = SettlementSystem.instance;
        settlementSystem.ShowStory(storyRoute);
    }

    /// <summary>
    /// 玩家結束關卡後 給予之獎勵
    /// </summary>
    /// <param name="isWin">是否勝利</param>
    /// <returns>獎勵資訊</returns>
    public string GiveAward(bool isWin)
    {
        playerAccountSystem = PlayerAccountSystem.instance;

        string awardInfo = "";

        if (isWin == true)
        {
            playerAccountSystem.spirit += spiritNum;
            playerAccountSystem.crystal += crystalNum;
            playerAccountSystem.threa += threaNum;
            awardInfo = awardInfo + "靈力*" + spiritNum + "\n";
            awardInfo = awardInfo + "靈晶*" + crystalNum + "\n";
            awardInfo = awardInfo + "靈粹*" + threaNum + "\n";
        }
        else
        {
            awardInfo = "None";
        }

        return awardInfo;
    }
}
