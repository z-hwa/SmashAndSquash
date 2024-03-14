using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 翠色獵手的單位資料
/// </summary>
public class A002_GreenHunter : A000_Default
{
    private MapSystem mapSystem;
    private CameraSystem cameraSystem;

    private UnitData_InBattle_GreenHunter unitData_InBattle_GreenHunter;
    public GameObject lockEffectPrefabs;   //鎖定效果預製體
    [HideInInspector] public GameObject lockEffect;   //鎖定效果物件

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="battleSystem">戰鬥系統</param>
    protected override void Init(BattleSystem battleSystem)
    {
        base.Init(battleSystem);

        if(mapSystem == null) mapSystem = MapSystem.instance;
        if(cameraSystem == null) cameraSystem = CameraSystem.instance;

        if (skill[0].isInit == false)
        {
            GameObject unit = battleSystem.unit[battleSystem.nowTurnUnit];    //觸發被動的翠色獵手
            unitData_InBattle_GreenHunter = unit.AddComponent<UnitData_InBattle_GreenHunter>();   //添加翠色獵手獨有的單位資料

            //複製單位資料
            UnitData_InBattle unitData_InBattle = battleSystem.unitDatas[battleSystem.nowTurnUnit];
            unitData_InBattle_GreenHunter.Copy(unitData_InBattle);

            //修改單位使用的資料
            unit.GetComponent<UnitBehavior>().unitData = unitData_InBattle_GreenHunter;
            battleSystem.unitDatas[battleSystem.nowTurnUnit] = unitData_InBattle_GreenHunter;

            skill[0].isInit = true;
        }
    }

    /// <summary>
    /// 被動技能:狩獵之眼
    /// 每回合，翠色獵手會隨機標記敵方的其中一名單位。
    /// 被標記的單位，遭到翠色獵手攻擊時，會額外受到50%的追加傷害
    /// </summary>
    /// <param name="battleSystem"></param>
    protected override void PassiveSkill(BattleSystem battleSystem)
    {
        base.PassiveSkill(battleSystem);
        int targetIndex = 0;

        //隨機鎖定目標
        targetIndex = Random.Range(0, mapSystem.enemyNum);
        while (mapSystem.enemy[targetIndex].gameObject.activeSelf == false)
        {
            targetIndex++;
            targetIndex = targetIndex%mapSystem.enemyNum;
        }

        //設置目標
        unitData_InBattle_GreenHunter.targetLocked = mapSystem.enemy[targetIndex];

        radioSystem.PlayRadio("翠色獵手被動-狩獵之眼觸發", RadioType.UnitSkill);
        radioSystem.PlayRadio($"鎖定單位-{mapSystem.enemy[targetIndex].GetComponent<UnitData_InBattle>().nowData.unitName}", RadioType.UnitSkill);

        radioSystem.PlaySkillEffect(use_data.removeSprite);  //播放技能特效
        musicSystem.PlayEffect(ConstantChart.skillAnimEffect);    //播放技能音效

        //視角鎖定敵人
        GameObject origianlTarget = cameraSystem.target;
        StartCoroutine(WaitToResumeTrackTarget(0.3f, origianlTarget, targetIndex));}

    IEnumerator WaitToResumeTrackTarget(float time, GameObject originalTarget, int targetIndex)
    {
        yield return new WaitForSeconds(time);
        cameraSystem.TargetChange(mapSystem.enemy[targetIndex]);

        //生成鎖定效果
        if (lockEffect != null) Destroy(lockEffect);
        lockEffect = Instantiate(lockEffectPrefabs, mapSystem.enemyDatas[targetIndex].healthBarUI.statusContent.transform);

        yield return new WaitForSeconds(3 * time);
        cameraSystem.TargetChange(originalTarget);
    }
}

public class UnitData_InBattle_GreenHunter: UnitData_InBattle
{
    public GameObject targetLocked; //當前鎖定的目標

    public override void SmashSettlement(GameObject passive)
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
        if (passive == targetLocked) paData.HpChange(damage * 0.5f, -1);

        paData.DiedJudge(passive);  //檢測被碰撞者是否死亡
    }
}
