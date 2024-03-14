using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 召喚池的相關設定
/// </summary>
public class SummonPool : MonoBehaviour
{
    private PackageSystem packageSystem;
    private PlayerAccountSystem playerAccountSystem;
    private SavedAndLoaded savedAndLoaded;
    private RadioSystem radioSystem;

    [Header("召喚池設定")]
    public int[] unitNum;   //各個單位在這個池中的數量
    [SerializeField] private int num;   //這個池中的單位總數量
    [SerializeField] private float[] unitRate;    //各個單位的命中率 會各個單位在池中的數量去計算這個
    public int[] poolUnit;  //可以抽到的單位ID

    void Start()
    {
        packageSystem = PackageSystem.instance;
        playerAccountSystem = PlayerAccountSystem.instance;
        savedAndLoaded = SavedAndLoaded.instance;
        radioSystem = RadioSystem.instance;

        CountRate();
    }

    /// <summary>
    /// 計算命中率
    /// </summary>
    private void CountRate()
    {
        unitRate = new float[poolUnit.Length];

        for(int i = 0; i < poolUnit.Length; i++)
        {
            num += unitNum[i];
        }

        //計算單位命中率
        for(int i=0;i<poolUnit.Length; i++)
        {
            unitRate[i] = unitNum[i] / (float)num * 100;
        }
    }

    /// <summary>
    /// 一抽
    /// </summary>
    public void Summon1()
    {
        if (playerAccountSystem.crystal >= 30)
        {
            playerAccountSystem.crystal -= 30;
            Summon(1, false);
            savedAndLoaded.SaveData();  //存檔
        }
        else
        {

            Debug.Log("靈晶數量不足");
            radioSystem.PlayRadio("靈晶數量不足", RadioType.System);
        }
    }

    /// <summary>
    /// 十抽
    /// </summary>
    public void Summon10() {
        if (playerAccountSystem.crystal >= 300)
        {
            playerAccountSystem.crystal -= 300;
            Summon(10, false);
            savedAndLoaded.SaveData();  //存檔
        }
        else
        {

            Debug.Log("靈晶數量不足");
            radioSystem.PlayRadio("靈晶數量不足", RadioType.System);
        }
    }

    /// <summary>
    /// 命運召喚 必定獲得該池限定單位
    /// </summary>
    public void SummonDestiny() {
        Summon(1, true);
    }

    /// <summary>
    /// 召喚函數
    /// </summary>
    /// <param name="times">召喚次數</param>
    /// <param name="isDestiny">是否為命運召喚</param>
    private void Summon(int times, bool isDestiny)
    {
        if(isDestiny == true)
        {
            //
        }
        else
        {
            for (int j = 0; j < times; j++)
            {
                int unitIndex = -1;
                int unitPos = Random.Range(0, num);

                //計算出抽到的單位是哪隻
                for (int i = 0; i < poolUnit.Length; i++)
                {
                    unitPos -= unitNum[i];
                    if (unitPos <= 0)
                    {
                        unitIndex = i;
                        break;
                    }
                }

                Debug.Log($"抽中了單位ID為{poolUnit[unitIndex]}");
                packageSystem.AddUnitWithID(poolUnit[unitIndex]);
            }
        }
    }
}
