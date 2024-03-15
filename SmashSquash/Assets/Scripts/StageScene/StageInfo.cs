using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 關卡的資訊腳本
/// </summary>
public class StageInfo : MonoBehaviour
{
    //每個關卡資訊物件 都有這個腳本 用於獲取資料
    public int mapIndex;
    public TextMeshProUGUI stageName;   //關卡名稱
    public TextMeshProUGUI stageDescription;    //關卡描述

    //獲取系統
    private MapBook mapBook;
    private PackageSystem packageSystem;
    private LoadingSystem loadingSystem;
    private RadioSystem radioSystem;

    //用於給每個關卡物件 載入對應的關卡
    public void LoadStage()
    {
        //獲取系統
        mapBook = MapBook.instance;
        packageSystem = PackageSystem.instance;
        radioSystem = RadioSystem.instance;
        
        //設定地圖能激活的地圖index
        mapBook.mapIndex = mapIndex;

        //載入關卡確認
        bool isError = LoadingStageCheck();
        if (isError == true) return;

        loadingSystem = LoadingSystem._loadingSystem;
        loadingSystem.LoadTargetScene("BattleScene");  //載入場景
    }

    /// <summary>
    /// 關卡載入是否發生錯誤的確認函數
    /// </summary>
    /// <returns>是否發生問題</returns>
    private bool LoadingStageCheck()
    {
        //出戰單位檢查
        if (packageSystem.unitNumber <= 0)
        {
            radioSystem.PlayRadio("請配置出戰單位", RadioType.System);
            return true;
        }

        //人數限制檢查
        if (packageSystem.unitNumber > mapBook.map_illustratedBook[mapIndex].GetComponent<MapInfo>().playerNum)
        {
            Debug.Log("人數超過限制");
            radioSystem.PlayRadio("對列單位人數超過限制", RadioType.System);
            return true;
        }

        //單位排序檢查
        for (int i = 0; i < packageSystem.unitNumber; i++)
        {
            if (packageSystem.lineUp[i] == null)
            {
                Debug.Log("必須由最前方開始配置單位");
                radioSystem.PlayRadio("必須由最前方開始配置單位", RadioType.System);
                return true;
            }
        }

        return false;
    }
}
