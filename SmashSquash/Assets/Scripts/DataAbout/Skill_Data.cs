using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 技能類別
/// </summary>
public enum SkillType
{
    Passive,
    Active
}

/// <summary>
/// 技能的描述類別
/// </summary>
[System.Serializable]
public class Skill_Data
{
    [TextArea] public string skillName;    //技能名稱
    [TextArea] public string skillIntro;   //技能描述

    public SkillType skillType; //技能類別
    public bool isInit; //是否完成技能的初始化
}
