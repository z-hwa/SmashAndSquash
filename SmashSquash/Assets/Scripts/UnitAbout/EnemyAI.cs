using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EA_NAME
{
    EA_default
}

/// <summary>
/// 敵人的AI系統
/// </summary>
public class EnemyAI : MonoBehaviour
{
    public static EnemyAI instance;

    /// <summary>
    /// 單例模式、避免重複生成的檢查
    /// </summary>
    void Awake()
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

    /// <summary>
    /// 設置敵人AI
    /// </summary>
    /// <param name="battleSystem">當前的戰鬥主系統，用於獲取AI計算需要的資料</param>
    /// <param name="_name">想要使用的AI名稱</param>
    public void SetEA(BattleSystem battleSystem, EA_NAME _name)
    {
        if (_name == EA_NAME.EA_default) EA_default(battleSystem);
    }

    /// <summary>
    /// 預設的敵人AI智能，更新過後的版本，將參數的獲取更改為battle system
    /// </summary>
    /// <param name="battleSystem">當前的戰鬥主系統，用於獲取AI計算需要的資料</param>
    private void EA_default(BattleSystem battleSystem)
    {
        //獲取該AI需要的資料
        GameObject enemy = battleSystem.enemy;
        GameObject[] playerUnit0 = battleSystem.unit;
        int unitNum = battleSystem.unitNumber;

        //隨機射擊力度
        float shootMagnitude = Random.Range(500f, 1500f);

        //設定初始攻擊目標
        GameObject target = playerUnit0[0];

        //找到玩家存活的第一個單位 從0~unitNum
        for (int i = 0; i < unitNum; i++)
        {
            if (playerUnit0[i].GetComponent<UnitData_InBattle>().nowData.hp > 0)
            {
                target = playerUnit0[i];
                break;
            }
        }

        //獲得應該攻擊的方向
        //指向傳入的player unit
        Vector2 shootDir = (target.transform.position - enemy.transform.position).normalized;

        //彈射攻擊
        enemy.GetComponent<UnitBehavior>().ShootUnit(shootDir * shootMagnitude);
    }
}
