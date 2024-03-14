using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkilIntro : MonoBehaviour
{
    private PlayerUI playerUI;  //玩家UI介面

    [Header("index")]
    public int unitIndex = -1; //單位index
    public int skillIndex = -1;    //技能的index

    [Header("遊戲物件")]
    public TextMeshProUGUI showText;
    public Button activateButton;
    public TextMeshProUGUI activateText;

    [Header("技能資料")]
    public SkillType skillType;
    private bool isNamed = false;
    [TextArea] public string skillName;
    [TextArea] public string skillIntro;

    /// <summary>
    /// 激活技能
    /// </summary>
    public void ActivateSkill()
    {
        playerUI = PlayerUI.instance;
        playerUI.ActivateSkill(unitIndex, skillIndex);
    }

    /// <summary>
    /// 改變顯示資訊
    /// </summary>
    public void ChangeShow()
    {
        isNamed = !isNamed;

        if(isNamed == true)
        {
            showText.text = skillName;
        }else
        {
            showText.text = skillIntro;
        }

        if(skillType == SkillType.Passive)
        {
            activateText.text = "無法主動使用";
        }
    }
}
