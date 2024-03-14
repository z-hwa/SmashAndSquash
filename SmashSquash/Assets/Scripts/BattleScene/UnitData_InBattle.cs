using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 當前單位的存活狀態
/// </summary>
public enum LifeState
{
    died,
    life
}

/// <summary>
/// 紀錄單位的陣營
/// 分為玩家和敵人
/// </summary>
public enum CampGroup
{
    player,
    enemy
}

/// <summary>
/// 每個戰鬥中的單位都應該有一個 用於處理那個單位的資料管理
/// 因為是透過prefab生成 故應該得到初始化
/// </summary>
public class UnitData_InBattle : MonoBehaviour
{
    private bool isInit = false;

    [Header("單位當前屬性")]
    public A000_Data nowData;
    //public A000_Default nowData;

    [Header("單位原始屬性")]
    public A000_Data originalData;
    //public A000_Default originalData;

    //額外資料
    [Header("其他資料")]
    public LifeState isLife = LifeState.died;    //單位現在狀態，預設為死亡狀態
    public int lineUpPos = -1;   //對列中的ID
    public CampGroup group = CampGroup.enemy;  //屬於哪一類
    public EA_NAME EA_name = EA_NAME.EA_default;  //如果是敵人 使用的AI

    //系統
    private PlayerUI playerUI;  //處理玩家的對列單位 狀態欄的UI
    public HealthBarUi healthBarUI;    //處理玩家的對列單位 遊戲物件頭上的UI

    void Start()
    {
        Init();
    }

    /*
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("更新單位資料");
            origianlHp_show = originalData.hp;
            originalAtk_show = originalData.atk;
            originalDef_show = originalData.def;

            hp_show = nowData.hp;
            atk_show = nowData.atk;
            def_show = nowData.def;
        }
    }*/

    public void Init()
    {
        if(isInit == false)
        {
            //獲取系統
            playerUI = PlayerUI.instance;
            healthBarUI = this.gameObject.GetComponent<HealthBarUi>();
            healthBarUI.Init(); //初始化血量條UI
        }
    }

    /// <summary>
    /// 當碰撞發生時，呼叫者與被動者的資料結算
    /// </summary>
    /// <param name="passive">被碰撞者</param>
    public virtual void SmashSettlement(GameObject passive)
    {
        //獲取被碰撞者的資料
        UnitData_InBattle paData = passive.GetComponent<UnitData_InBattle>();

        //靈力控制力 影響倍率
        float ac_SpiritRate = Mathf.Pow(ConstantChart.spiritInfluence, (int)nowData.spiritControl);
        float pa_SpiritRate = Mathf.Pow(ConstantChart.spiritInfluence, (int)paData.nowData.spiritControl);

        //受傷率 = ((攻擊者的攻擊力+靈力影響) / (被攻擊者的防禦力+靈力影響))
        float damageRate = (nowData.atk + nowData.spiritGrade * ac_SpiritRate) 
            / (paData.nowData.def + paData.nowData.spiritGrade * pa_SpiritRate);

        /*
         套用最終單位技能 or 其他最終效果
         */

        float damage = nowData.atk * damageRate;   //得到最終傷害

        paData.HpChange(damage, -1);    //扣除被碰撞者血量
        paData.DiedJudge(passive);  //檢測被碰撞者是否死亡
    }

    /// <summary>
    /// 呼叫者的unitData的生命改變
    /// </summary>
    /// <param name="value">改變純量</param>
    /// <param name="state">改變方向 決定扣血或是回血</param>
    public void HpChange(float value, int state)
    {
        Debug.Log("造成傷害: " + value);
        nowData.hp = nowData.hp + (int)value * state;   //計算傷害

        BloodUIUpdate(value, state);    
    }

    /// <summary>
    /// 更新血條相關的UI
    /// </summary>
    /// <param name="value">改變純量</param>
    /// <param name="state">改變方向 決定扣血或是回血</param>
    public void BloodUIUpdate(float value, int state)
    {
        Init();

        //展示傷害數字
        healthBarUI.ShowDamage((int)value * state);

        //血條UI更新
        healthBarUI.ChangeUnitHp((float)nowData.hp / (float)originalData.hp, group, lineUpPos);   //改變屬於group之單位的血條資料的UI
    }

    /// <summary>
    /// 檢測呼叫單位是否死亡
    /// </summary>
    /// <param name="unit">為呼叫單位的遊戲物件 用以deactive</param>
    public void DiedJudge(GameObject unit)
    {
        if(nowData.hp <= 0)
        {
            isLife = LifeState.died;
            unit.SetActive(false);  //關閉單位
        }
    }

    /// <summary>
    /// 初始化 呼叫者的單位資料
    /// </summary>
    /// <param name="original">單位的預設資料</param>
    public void InitUnitData_InBattle(A000_Data original, A000_Data now, CampGroup campGroup, int pos)
    {
        group = campGroup;  //設置陣營
        lineUpPos = pos;   //設置單位的位置

        nowData.Copy(now, false);    //獲取單位現在的屬性，後續戰鬥中會修改這個
        originalData.Copy(original, false);   //獲取單位原始屬性

        isLife = LifeState.life;    //設定為存活狀態
    }

    /// <summary>
    /// 複製運算子 單位資料
    /// </summary>
    /// <param name="unitData_InBattle">單位資料</param>
    public virtual void Copy(UnitData_InBattle unitData_InBattle)
    {
        //實例化
        nowData = new A000_Data();
        originalData = new A000_Data();

        group = unitData_InBattle.group;
        lineUpPos = unitData_InBattle.lineUpPos;

        nowData.Copy(unitData_InBattle.nowData, false);
        originalData.Copy(unitData_InBattle.originalData, false);

        isLife = unitData_InBattle.isLife;
    }
}
