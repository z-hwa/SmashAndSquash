using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 背包介面的UI相關處理
/// </summary>
public class PackagePage : MonoBehaviour
{
    public static PackagePage instance;

    //系統
    private PackageSystem packageSystem;
    private BottomBar bottomBar;
    private RadioSystem radioSystem;

    //美術
    public GameObject chooseEffect;

    //背包物件相關
    public GameObject packageBundle;    //背包物件的父物件，grid layout的content
    public GameObject item; //背包物件預製體
    public GameObject[] itemBundle = new GameObject[ConstantChart.PACKAGEMAXNUM]; //儲存所有的背包物件美術

    //單位選擇相關
    public int nowChoosePos = -1;    //現在選擇的出戰位置
    public GameObject[] linePos = new GameObject[ConstantChart.MAXLINEUP];

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
        //加載系統
        packageSystem = PackageSystem.instance;
        bottomBar = BottomBar.instance;
        radioSystem = RadioSystem.instance;

        //如果背包介面是打開的 加載背包
        if (bottomBar.packageCanva.activeSelf == true) LoadingPackage();
    }

    /// <summary>
    /// 載入背包頁面的單位物件
    /// </summary>
    public void LoadingPackage()
    {
        //獲得背包容量
        int num = packageSystem.packageCapacity;
        packageBundle.GetComponent<AdvancedGridLayoutGroupVertical>().cellNum = num;    //設置當前的背包數量 以讓顯示區塊可以自動放大

        for(int i=0;i<num;i++)
        {
            itemBundle[i] = Instantiate(item, packageBundle.transform);    //生成實體
            itemBundle[i].transform.GetChild(0).GetComponent<Image>().sprite = packageSystem.unit_object[i].GetComponent<A000_Default>().use_data.squareSprite;    //更換圖片
            itemBundle[i].GetComponent<PackageUnitButton>().thisUnitId = i;   //設定該物品在背包中的ID
            
            //設置單位等級資訊                                                                  
            itemBundle[i].GetComponent<PackageUnitButton>().description.text = "LV:" + packageSystem.unit_object[i].GetComponent<A000_Default>().use_data.spiritGrade.ToString();
        }

        //獲得對列中出戰單位
        for(int i=0;i<ConstantChart.MAXLINEUP; i++)
        {
            if (packageSystem.lineUp[i] != null)
            {
                int id = packageSystem.lineUpId[i];
                linePos[i].transform.GetChild(0).GetComponent<Image>().sprite = packageSystem.unit_object[id].GetComponent<A000_Default>().use_data.squareSprite;   //設置對列單位的圖片

                //設置單位等級資訊                                                                  
                linePos[i].GetComponent<PackageUnitButton>().description.text = "LV:" + packageSystem.unit_object[i].GetComponent<A000_Default>().use_data.spiritGrade.ToString();
            }else
            {
                //設置單位等級資訊                                                                  
                linePos[i].GetComponent<PackageUnitButton>().description.text = "none";
            }
        }
    }

    /// <summary>
    /// 刪除所有背包物件的顯示
    /// </summary>
    public void UnLoadingPackage()
    {
        foreach (var item in itemBundle)
        {
            Destroy(item.gameObject);
        }
    }

    /// <summary>
    /// 在背包頁面中 製造設置某個單位到對列的效果
    /// </summary>
    /// <param name="id">對列ID</param>
    public void LiningUnit(int id)
    {
        int pos = nowChoosePos;
        
        bool isLined = packageSystem.LineUpUnitWithID(id, pos);

        if(isLined == true) {
            A000_Default data = packageSystem.unit_object[id].GetComponent<A000_Default>();

            //設置對列單位的圖片
            linePos[pos].transform.GetChild(0).GetComponent<Image>().sprite = data.use_data.squareSprite;

            //設置單位等級資訊                                                                  
            linePos[pos].GetComponent<PackageUnitButton>().description.text = "LV:" + data.use_data.spiritGrade.ToString();

            //廣播設置成功資訊
            radioSystem.PlayRadio($"設置{data.use_data.unitName}到對列{pos}成功", RadioType.System);

            SavedAndLoaded.Instance.SaveData();  //存檔
        }
    }

    /// <summary>
    /// 在背包頁面中 製造取消某個單位到對列的效果
    /// </summary>
    public void LiningDownUnit()
    {
        int pos = nowChoosePos;
        bool isLineDowned = packageSystem.LineDownUnitWithPos(pos);

        if (isLineDowned == true)
        {
            //設置對列單位的圖片
            linePos[pos].transform.GetChild(0).GetComponent<Image>().sprite = null;

            //廣播設置取消資訊
            radioSystem.PlayRadio($"設置對列{pos}取消成功", RadioType.System);

            SavedAndLoaded.Instance.SaveData();  //存檔
        }
    }

    /// <summary>
    /// 載入特定單位的頁面
    /// </summary>
    /// <param name="id">背包中的單位index</param>
    public void LoadUnitPage(int id)
    {
        packageSystem.LoadUnitPage(id);
    }
}
