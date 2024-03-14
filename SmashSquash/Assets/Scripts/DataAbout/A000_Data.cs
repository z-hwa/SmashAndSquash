using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 單位的資料類別
/// 記錄所有單位的靜態資料
/// </summary>
[System.Serializable]
public class A000_Data
{
    //單位初始的資料
    [Header("ID以及名稱")]
    public int id = 0;
    public string unitName = "unitName";

    [Header("靈力等級以及靈力掌控度")]
    public int spiritGrade = 1;
    public SpiritControlStandard spiritControl = SpiritControlStandard.ZERO;
    public int spiritControlPercent = 0; //靈力掌控百分比

    [Header("三圍屬性")]
    public int hp = 1;
    public int atk = 1;
    public int def = 1;

    // 針對不同單位，可能有不同的屬性成長係數，就是調整這一區
    [Header("等級、屬性的成長係數")]
    public float expConstant = 13;   //經驗值常數 越大 每級需要的經驗值越多 (+-1)
    public float hpConstant = 3;  //屬性常數 越大 每級獲得的屬性越多 (+-1)
    public float atkConstant = 3;
    public float defConstant = 3;
    public float spiritControlConstant = 3; //靈力掌控程度升級所需靈粹常數

    //等級與屬性處理
    public int tempExp;   //儲存現在的累積經驗值

    //靈力掌控度處理
    public int tempThrea; //儲存現在的累積靈粹

    // 紀錄單位的腳色美術
    [Header("美術")]
    public Sprite squareSprite;   //單位的圓形精靈圖片
    public Sprite circleSprite;   //單位的方形精靈圖片
    public Sprite removeSprite;  //去背的單位精靈圖片

    /// <summary>
    /// 複製運算子
    /// </summary>
    public void Copy(A000_Data a000_Data, bool isCopyArt)
    {
        this.id = a000_Data.id;
        this.unitName = a000_Data.unitName;
        
        this.spiritGrade = a000_Data.spiritGrade;
        this.spiritControl = a000_Data.spiritControl;
        this.spiritControlPercent = a000_Data.spiritControlPercent;

        this.hp = a000_Data.hp;
        this.atk = a000_Data.atk;
        this.def = a000_Data.def;

        this.expConstant = a000_Data.expConstant;
        this.hpConstant = a000_Data.hpConstant;
        this.atkConstant = a000_Data.atkConstant;
        this.defConstant = a000_Data.defConstant;
        this.spiritControlConstant = a000_Data.spiritControlConstant;

        this.tempExp = a000_Data.tempExp;
        this.tempThrea = a000_Data.tempThrea;

        if(isCopyArt == true)
        {
            this.squareSprite = a000_Data.squareSprite;
            this.circleSprite = a000_Data.circleSprite;
            this.removeSprite = a000_Data.removeSprite;
        }
    }
}
