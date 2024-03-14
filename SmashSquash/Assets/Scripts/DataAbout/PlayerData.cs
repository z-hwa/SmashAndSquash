using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用於儲存玩家資料的靜態類別
/// </summary>
public class PlayerData
{
    //玩家資訊
    public string playerName;
    public int playerRank;
    public int crystal;
    public int spirit;
    public int threa;

    //背包資料
    public int packageCapacity; //背包中物品數量

    //出戰資料
    public const int MAXLINEUP = 4;
    public int unitNumber;  //當前出戰數量
    public int[] lineUpId = new int[ConstantChart.MAXLINEUP]; //紀錄出戰單位的背包ID 防止重複出戰
}

/*
/// <summary>
/// 用於轉存單位資料的靜態類別
/// </summary>
public class A000_Saved
{

    [Header("屬性")]
    public int id;
    public string unitName; //名字
    public int spiritGrade; //靈力等級
    public SpiritControlStandard spiritControl; //靈力控制階級

    public int hp;  //生命
    public int atk; //攻擊
    public int def; //防禦

    //複製單位資料 用於儲存
    public void CopyData(A000_Default unit)
    {
        this.id = unit.id;
        this.unitName = unit.unitName;
        this.spiritGrade = unit.spiritGrade;
        this.spiritControl = unit.spiritControl;

        this.hp = unit.hp;
        this.atk = unit.atk;
        this.def = unit.def;
    }
}

*/

/// <summary>
/// 用於儲存玩家是否已經看過某個故事的靜態類別
/// </summary>
public class StoryRecorder
{
    public bool openingIntro = false;
    public bool interfaceUsageTutorial = false;
    public bool unitPageTutorial = false;
    public bool battleMethod = false;

    public void ReadedStory(string storyName)
    {
        if (storyName == "openingIntro") openingIntro = true;
        else if(storyName == "interfaceUsageTutorial") interfaceUsageTutorial = true;
        else if (storyName == "unitPageTutorial") unitPageTutorial = true;
        else if (storyName == "battleMethod") battleMethod = true;
    }
}