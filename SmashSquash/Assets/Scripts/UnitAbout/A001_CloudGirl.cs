using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 雲女的單位資料
/// </summary>
public class A001_CloudGirl : A000_Default
{
    //被動技能
    [Header("被動技能")]
    [SerializeField] private int turnCount = 0;  //經過的回合數
    public GameObject flogModeEffect;   //雲霧狀態的粒子效果 預製件
    private GameObject flog; //雲霧化的效果物件

    /// <summary>
    /// 專門for各個單位各個技能的初始化
    /// </summary>
    /// <param name="unit">單位物件</param>
    protected override void Init(BattleSystem battleSystem)
    {
        base.Init(battleSystem);

        if (skill[0].isInit == false)
        {
            GameObject unit = battleSystem.unit[battleSystem.nowTurnUnit];
            turnCount = 0;  //初始化回合數

            flog = Instantiate(flogModeEffect, unit.transform); //生成雲霧效果
            flog.transform.localPosition = Vector3.zero;
            flog.SetActive(false);

            skill[0].isInit = true;
        }
    }

    /// <summary>
    /// 雲霧化:
    /// 雲女每兩個回合會進入雲霧狀態，持續兩個回合。
    /// 雲霧狀態下，雲女的生命值上限減少50%，轉化為攻擊力。
    /// 結束雲霧狀態後，會回復生命值上限。
    /// </summary>
    /// <param name="battleSystem">戰鬥系統</param>
    protected override void PassiveSkill(BattleSystem battleSystem)
    {
        base.PassiveSkill(battleSystem);
        turnCount++;

        //獲取單位資料
        UnitData_InBattle unitData = battleSystem.unitDatas[battleSystem.nowTurnUnit];
        
        if(turnCount % 4 == 2)
        {
            radioSystem.PlayRadio("雲女被動-雲霧化觸發!", RadioType.System);
            radioSystem.PlayRadio("雲女進入雲霧狀態，最大血量下降，攻擊力上升", RadioType.System);
            
            radioSystem.PlaySkillEffect(use_data.removeSprite);  //播放技能特效
            musicSystem.PlayEffect(ConstantChart.skillAnimEffect);    //播放技能音效

            int tempValue = unitData.originalData.hp / 2;   //獲得50%的最大血量
            unitData.originalData.hp = tempValue;

            //如果當前血量多出扣減後的血量 就設置為扣減後的血量
            if(unitData.nowData.hp > tempValue) unitData.nowData.hp = tempValue;
            unitData.originalData.hp = tempValue;

            unitData.nowData.atk = unitData.nowData.atk + tempValue;    //提高攻擊力

            unitData.BloodUIUpdate(tempValue, -1);  //更新血條UI

            //設置特效
            flog.SetActive(true);
        }
        else if(turnCount % 4 == 0)
        {
            if(turnCount > 0) radioSystem.PlayRadio("雲女退出雲霧狀態，最大血量回復，攻擊力下降", RadioType.System);
            int tempValue = unitData.originalData.hp;   //獲得扣減後的血量 (相當於最大值的50%)

            //設置血量
            unitData.nowData.hp = unitData.nowData.hp + tempValue;
            unitData.originalData.hp = tempValue * 2;

            unitData.nowData.atk = unitData.originalData.atk;    //回復攻擊力

            unitData.BloodUIUpdate(tempValue, 1);

            //設置特效
            flog.SetActive(false);
        }
    }

    public override void UsingSkill(BattleSystem battleSystem, int index, SkillType skillType)
    {
        base.UsingSkill(battleSystem, index, skillType);
    }
}
