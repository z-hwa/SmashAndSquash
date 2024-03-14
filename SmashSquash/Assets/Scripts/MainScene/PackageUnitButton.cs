using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 掛載在背包中每個物件的上面
/// 用於在按鍵被按下時 觸發
/// </summary>
public class PackageUnitButton : MonoBehaviour
{

    //單位UI以及資料
    public TextMeshProUGUI description;

    //系統
    private PackagePage packagePage;
    
    [Header("背包相關")]
    public int thisLinePos = -1;
    public int thisUnitId;  //這個物件在背包的ID

    [Header("按鍵處理")]
    public float longPressTime = 0.8f;  //長按的判定時長
    private float lastTime; //上一次按下的時間

    void Start()
    {
        packagePage = PackagePage.instance;
    }

    /// <summary>
    /// 長按檢測
    /// </summary>
    /// <param name="isDown"></param>
    public void LonePress(bool isDown)
    {
        if(isDown == true)
        {
            //按下按鍵
            lastTime = Time.time;

            //Debug.Log("press down");
        }else
        {
            //Debug.Log("release");

            //鬆開按鍵
            float ping = Time.time - lastTime;
            if(ping > longPressTime)
            {
                //載入特定單位的頁面
                packagePage.LoadUnitPage(thisUnitId);
            }
        }
    }

    /// <summary>
    /// 選擇這個出戰位置
    /// </summary>
    public void ChoosePos()
    {
        packagePage.nowChoosePos = thisLinePos; //選擇現在要設定的出戰位置

        //設定選擇效果
        packagePage.chooseEffect.SetActive(true);
        packagePage.chooseEffect.transform.SetParent(this.transform);
        packagePage.chooseEffect.transform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// 把現在選擇的單位 放進先前選擇的對列位置
    /// </summary>
    public void LineUpThisUnit()
    {
        packagePage.LiningUnit(thisUnitId);
    }

    /// <summary>
    /// 把現在選擇的對列位置的單位移除
    /// </summary>
    public void LineDownThisUnit()
    {
        packagePage.LiningDownUnit();
    }
}
