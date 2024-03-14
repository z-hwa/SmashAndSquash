using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

/// <summary>
/// 所有unit的模板
/// </summary>
public class A000_Default: MonoBehaviour
{
    /// <summary>
    /// 預設資料區，紀錄單位的原始資料
    /// 繼承本腳本的單位，只需要實作專屬於該單位的屬性、技能即可
    /// </summary>
    /*
    [Header("單位初始資料")]
    //單位初始的資料
    public int idDefault = 0;
    public string unitNameDefault = "unitName";
    public int spiritGradeDefault = 1;
    public SpiritControlStandard spiritControlDefault = SpiritControlStandard.ZERO;
    public int spiritControlPercentDefault = 0; //靈力掌控百分比

    [Header("單位初始屬性")]
    public int hpDefault = 1;
    public int atkDefault = 1;
    public int defDefault = 1;


    // 針對不同單位，可能有不同的屬性成長係數，就是調整這一區
    [Header("等級與屬性之成長係數")]
    public float expConstant = 13;   //經驗值常數 越大 每級需要的經驗值越多 (+-1)
    public float hpConstant = 3;  //屬性常數 越大 每級獲得的屬性越多 (+-1)
    public float atkConstant = 3;
    public float defConstant = 3;
    public float spiritControlConstant = 3; //靈力掌控程度升級所需靈粹常數

    // 紀錄單位的腳色美術
    [Header("美術")]
    public Sprite squareSprite;   //單位的圓形精靈圖片
    public Sprite circleSprite;   //單位的方形精靈圖片
    public Sprite removeSprite;  //去背的單位精靈圖片

    /// <summary>
    /// 使用資料區(public)
    /// 整個遊戲過程中會實際改動的區域
    /// 腳色的成長、升級等等
    /// </summary>
    //屬性
    [HideInInspector] public int id;
    [HideInInspector] public string unitName; //名字
    [HideInInspector] public int spiritGrade; //靈力等級
    [HideInInspector] public SpiritControlStandard spiritControl; //靈力控制階級
    [HideInInspector] public int spiritControlPercent; //靈力掌控百分比

    [HideInInspector] public int hp;  //生命
    [HideInInspector] public int atk; //攻擊
    [HideInInspector] public int def; //防禦

    //等級與屬性處理
    [HideInInspector] public int tempExp;   //儲存現在的累積經驗值

    //靈力掌控度處理
    [HideInInspector] public int tempThrea; //儲存現在的累積靈粹*/

    //資料區域
    [Header("資料區域")]
    public A000_Data default_data;  //單位的預設資料
    public A000_Data use_data;  //單位在遊戲壽命中，會使用的資料

    // 技能區域
    [Header("技能區域")]
    public Skill_Data[] skill;  //技能資料

    //系統
    [HideInInspector] public RadioSystem radioSystem;
    [HideInInspector] public MusicSystem musicSystem;

    /// <summary>
    /// 用於計算單位目前的暫存經驗，並自動升級至對應等級
    /// </summary>
    public void LevelCounter()
    {
        /* 利用經驗值函數算出，現在這個等級需要的目標經驗值
         * 判斷暫存經驗值是否超過該數字
         * 決定是否升級
         * 重複直到沒法升級
         * 計算屬性
         */

        int targetExp = SearchNextLevelExp();   //計算升級所需的經驗值

        //檢查是否能升級
        while(use_data.tempExp >= targetExp)
        {
            use_data.spiritGrade++;
            use_data.tempExp -= targetExp;
            targetExp = SearchNextLevelExp();

            //Debug.Log(((int)use_data.spiritControl).ToString() + "  " + ((int)SpiritControlStandard.Zp).ToString());
            if(((int)use_data.spiritControl) > ((int)SpiritControlStandard.Zm)) use_data.spiritControlPercent -= ConstantChart.spiritLossPercent;   //如果不是Zp才減少
        }

        use_data.spiritControl = (SpiritControlStandard)(use_data.spiritControlPercent / 100);    //更新靈力掌控程度
        AbilityCounter();   //計算屬性
    }

    /// <summary>
    /// 用於計算單位的暫存靈粹，並自動升級到對應的靈力掌控程度
    /// </summary>
    public void SpiritControlCounter()
    {
        int targetThrea = SearchNextControlLevelThrea();    //獲得升級到下一階段所需的靈粹數量

        //檢查是否能提升靈力掌控程度
        while (use_data.tempThrea >= targetThrea && use_data.spiritControl < SpiritControlStandard.Zp)
        {
            use_data.tempThrea -= targetThrea;
            targetThrea = SearchNextControlLevelThrea();

            use_data.spiritControl = (SpiritControlStandard)(use_data.spiritControlPercent / 100 + 1);    //更新靈力掌控程度
            use_data.spiritControlPercent = (int)use_data.spiritControl * 100;    //更新靈力掌控比率
        }
    }

    /// <summary>
    /// 回傳距離下一個等級所需的經驗值
    /// </summary>
    /// <returns>距離下一個等級所需的經驗值</returns>
    public int SearchNextLevelExp()
    {
        return ValueCounter(use_data.expConstant, use_data.spiritGrade);
    }

    /// <summary>
    /// 回傳距離下一個靈力掌控所需的靈粹
    /// </summary>
    /// <returns>距離下一個等級所需的靈粹數量</returns>
    public int SearchNextControlLevelThrea()
    {
        return ValueCounter(use_data.spiritControlConstant, (int)use_data.spiritControl + 27);    //讓Zm變成1
    }

    /// <summary>
    /// 用於更新這個單位的屬性
    /// </summary>
    private void AbilityCounter()
    {
        use_data.hp = (int)(use_data.hpConstant * ValueCounter(use_data.hpConstant, use_data.spiritGrade) + default_data.hp);
        use_data.atk = ValueCounter(use_data.atkConstant, use_data.spiritGrade) + default_data.atk;
        use_data.def = ValueCounter(use_data.defConstant, use_data.spiritGrade) + default_data.def;
    }

    /// <summary>
    /// 計算單位目前等級的各種數值之函數，基於常數變化
    /// </summary>
    /// <param name="constant">需要計算的屬性之係數</param>
    /// <param name="lv">目前的等級</param>
    /// <returns></returns>
    private int ValueCounter(float constant, int lv)
    {
        int exp = 0;
        const int p = 2;

        //lv+1 是到下一等級
        for(int i=1;i<=lv + 1;i++)
        {
            exp += 13 * (int)Mathf.Log(Mathf.Pow(i, constant), p);
        }

        return exp;
    }

    /// <summary>
    /// 用於從存檔中複製資料出來
    /// </summary>
    /// <param name="a000_Data">想要複製的資料</param>
    public void Copy_FromSaved(A000_Data a000_Data)
    {
        /*this.id = unit.id;
        this.unitName = unit.unitName;
        this.spiritGrade = unit.spiritGrade;
        this.spiritControl = unit.spiritControl;

        this.hp = unit.hp;
        this.atk = unit.atk;
        this.def = unit.def;*/

        use_data.Copy(a000_Data, false);   //複製單位資料
    }

    /// <summary>
    /// 初始化單位資料
    /// </summary>
    public virtual void InitUnitData() {
        /*this.id = idDefault;
        this.unitName = unitNameDefault;
        this.spiritGrade = spiritGradeDefault;
        this.spiritControl = spiritControlDefault;
        this.spiritControlPercent = spiritControlPercentDefault;

        this.hp = hpDefault;
        this.atk = atkDefault;
        this.def = defDefault;

        this.tempExp = 0;*/

        use_data.Copy(default_data, true);    //根據預設的資料 初始化使用的資料
    }

    /// <summary>
    /// 複製單位資料
    /// </summary>
    public void CopyData_Battle(A000_Default unit)
    {
        /*this.id = unit.id;
        this.unitName = unit.unitName;
        this.spiritGrade = unit.spiritGrade;
        this.spiritControl = unit.spiritControl;

        this.hp = unit.hp;
        this.atk = unit.atk;
        this.def = unit.def;*/
    }

    /// <summary>
    /// 被動技能
    /// </summary>
    /// <param name="battleSystem">戰鬥系統</param>
    protected virtual void PassiveSkill(BattleSystem battleSystem)
    {
        Debug.Log("使用被動技能");
        Init(battleSystem);
    }

    /// <summary>
    /// 主動技能1
    /// </summary>
    /// <param name="battleSystem">戰鬥系統</param>
    protected virtual void ActiveSkill_1(BattleSystem battleSystem)
    {
        Debug.Log("使用主動技能1");
        Init(battleSystem);
    }

    /// <summary>
    /// 初始化技能函數
    /// 只初始化技能相關的程式部分
    /// 不處理資料
    /// </summary>
    public virtual void InitSkill()
    {
        //設定技能的初始化為否，待會技能的實作部分，就會去初始化該技能
        for(int i = 0;i<skill.Length;i++)
        {
            skill[i].isInit = false;
        }
    }

    /// <summary>
    /// 根據index呼叫對應技能
    /// 是否能使用，由技能處決定
    /// 預設為一被動、一主動
    /// </summary>
    /// <param name="battleSystem">戰場腳本</param>
    /// <param name="index">技能index</param>
    public virtual void UsingSkill(BattleSystem battleSystem, int index, SkillType skillType)
    {
        if(skillType == SkillType.Active)
        {
            switch (index)
            {
                case 0:
                    PassiveSkill(battleSystem);
                    break;
                case 1:
                    ActiveSkill_1(battleSystem);
                    break;
                default:
                    Debug.Log("技能不存在");
                    break;
            }
        }else if(skillType == SkillType.Passive)
        {
            PassiveSkill(battleSystem);
        }
    }

    /// <summary>
    /// 技能的初始化
    /// </summary>
    /// <param name="battleSystem">戰鬥系統</param>
    protected virtual void Init(BattleSystem battleSystem)
    {
        Debug.Log("技能初始化");
        if (radioSystem == null) radioSystem = RadioSystem.instance;
        if (musicSystem == null) musicSystem = MusicSystem.instance;
    }
}