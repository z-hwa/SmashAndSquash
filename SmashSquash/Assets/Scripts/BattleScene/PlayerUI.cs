using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 處理戰鬥界面底下的單位UI
/// </summary>
public class PlayerUI : MonoBehaviour
{
    public static PlayerUI instance;
    
    //對列單位物件
    public GameObject[] unitInfo = new GameObject[ConstantChart.MAXLINEUP];

    //技能觸發物件
    [Header("技能")]
    public GameObject skillDect;    //技能甲板
    public GameObject skillContent; //技能資訊父物件
    public GameObject skillInfoPrefabs; //技能資訊 預製件

    private PackageSystem packageSystem;    //背包系統
    private BattleSystem battleSystem;  //戰鬥系統
    private RadioSystem radioSystem;    //廣播系統

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

        //DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        packageSystem = PackageSystem.instance;
        battleSystem = BattleSystem.instance;
        radioSystem = RadioSystem.instance;

        InitUnitBottomBar();
    }

    /// <summary>
    /// 如果發生單位的血量改變，對應的UI會出現變化
    /// </summary>
    /// <param name="index">單位的index位置</param>
    /// <param name="rate">單位的血量變化</param>
    public void ChangeUnitHp(int index, float rate)
    {
        //對列index在正確範圍內
        if(index >= 0 && index < ConstantChart.MAXLINEUP) {
            if (rate <= 0)
            {
                unitInfo[index].transform.GetChild(0).gameObject.SetActive(false);  //血條歸0 直接關掉遊戲物件
            }
            else
            {
                unitInfo[index].transform.GetChild(0).GetComponent<Slider>().value = rate;  //血條變化
            }
        }
    }

    /// <summary>
    /// 初始化戰鬥界面底下的單位資料
    /// </summary>
    public void InitUnitBottomBar()
    {
        int unitNumber = packageSystem.unitNumber;  //單位數量
        for(int i =0;i<unitNumber;i++)
        {
            A000_Default a000_Default = packageSystem.lineUp[i].GetComponent<A000_Default>();

            //加載單位的方形圖片
            unitInfo[i].GetComponent<Image>().sprite = a000_Default.use_data.squareSprite;

            //初始化血量
            unitInfo[i].transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
            unitInfo[i].transform.GetChild(0).GetComponent<Slider>().value = 1;
        }
    }

    /// <summary>
    /// 顯示技能甲板
    /// </summary>
    /// <param name="index">單位的對列index</param>
    public void ShowSkillDeck(int index)
    {
        if(battleSystem.nowState == BattleState.playerTurn && battleSystem.IsAllUnitStop() == true)
        {
            Debug.Log($"顯示單位{index}的技能甲板");
            skillDect.SetActive(true);  //顯示技能甲板

            if (battleSystem.unitLineUp[index] != null)
            {
                //獲得單位資訊
                A000_Default unit = battleSystem.unitLineUp[index].GetComponent<A000_Default>();
                for (int i = 0; i < unit.skill.Length; i++)
                {
                    GameObject obj = Instantiate(skillInfoPrefabs, skillContent.transform); //生成技能資訊物件

                    //設置單位ID、技能ID、技能資訊
                    SkilIntro skillIntro = obj.GetComponent<SkilIntro>();
                    skillIntro.unitIndex = index;
                    skillIntro.skillIndex = i;
                    skillIntro.skillName = unit.skill[i].skillName;
                    skillIntro.skillIntro = unit.skill[i].skillIntro;
                    skillIntro.skillType = unit.skill[i].skillType;

                    //如果是被動技能 就無法被按件觸發
                    if (unit.skill[i].skillType == SkillType.Passive) skillIntro.activateButton.enabled = false;

                    skillIntro.ChangeShow();
                }
            }
        }else
        {
            Debug.Log("現在無法使用技能");
            radioSystem.PlayRadio("現在無法使用技能", RadioType.System);
        }
    }

    /// <summary>
    /// 關閉技能甲板
    /// </summary>
    public void CloseSkillDeck()
    {
        /*
        while(skillContent.transform.childCount > 0)
        {
            Debug.Log(skillContent.transform.childCount);
            Destroy(skillContent.transform.GetChild(0));
        }*/

        int childCount = skillContent.transform.childCount;
        for(int i=0;i<childCount;i++)
        {
            Destroy(skillContent.transform.GetChild(i).gameObject);
        }
        skillDect.SetActive(false);
    }

    public void ActivateSkill(int unitIndex, int skillIndex)
    {
        //獲得單位資訊
        A000_Default unit = battleSystem.unitLineUp[unitIndex].GetComponent<A000_Default>();

        unit.UsingSkill(battleSystem, skillIndex, SkillType.Active);
    }
}
