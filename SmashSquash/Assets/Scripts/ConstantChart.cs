using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 常數表
/// </summary>
public class ConstantChart
{
    //戰鬥相關
    [Header("戰鬥")]
    public const float dragMulti = 1.5f;

    //背包相關
    [Header("背包相關")]
    public const int PACKAGEMAXNUM = 100;   //背包最大數量
    public const int MAXLINEUP = 4; //出戰單位最大數量

    //靈力相關
    [Header("靈力相關")]
    public const float spiritInfluence = 1.03f;  //靈力影響倍率
    public const int spiritLossPercent = 40;    //靈力掌控衰減百分比

    //算術相關
    [Header("算數相關")]
    public const float diviSupport = 0.000001f; //除法補0
    public const float floatSuppirt = 0.000001f; //浮點數比較

    //音效
    [Header("音效")]
    public const string shootEffect = "ShootingSound/crossbow"; //射擊音效
    public const string hitEffect = "ShootingSound/Grenade6Short";  //碰撞音效
    public const string skillAnimEffect = "SFX/Skill SFX";  //技能動畫音效

    //BGM
    [Header("BGM")]
    public const string battleBGM = "Audio/StreetLove_Audio Trimmer";
    public const string backgroundBGM = "Audio/casualFantasy";
}
