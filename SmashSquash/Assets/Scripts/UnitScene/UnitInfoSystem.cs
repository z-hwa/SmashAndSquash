using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 單位資料介面
/// </summary>
public class UnitInfoSystem : MonoBehaviour
{
    public static UnitInfoSystem instance;
    private bool isInit = false;

    private PackageSystem packageSystem;
    private PlayerAccountSystem playerAccountSystem;
    private SavedAndLoaded savedAndLoaded;
    private StorySystem storySystem;
    private LoadingSystem loadingSystem;

    [Header("UI物件")]
    [Header("單位intro")]
    A000_Default unit;  //這個單位
    A000_Data data;  //這個單位的靜態資料
    public TextMeshProUGUI id;
    public TextMeshProUGUI unitName;
    public TextMeshProUGUI spiritGrade;

    [Header("屬性")]
    public TextMeshProUGUI atk;
    public TextMeshProUGUI hp;
    public TextMeshProUGUI def;

    [Header("經驗值相關")]
    public TextMeshProUGUI tempExp;
    public TMP_InputField changeSpiritAmount;
    public TextMeshProUGUI spiritRemain;
    public TextMeshProUGUI toNextLevelExp;

    [Header("靈力掌控程度相關")]
    public TextMeshProUGUI spiritControl;
    public TMP_InputField addThreaAmount;   //添加靈粹數量
    public TextMeshProUGUI tempThrea; //單位已獲得的靈粹數量
    public TextMeshProUGUI threaRemain; //剩餘靈粹數量
    public TextMeshProUGUI toNextControlLevel;  //距離下一致掌控等級所需靈粹數量
    public TextMeshProUGUI spiritControlInfluence;  //靈力掌控程度影響倍率

    [Header("美術")]
    public Image unitArt;

    [Header("技能描述")]
    public GameObject skillContent; //技能的甲板父物件
    public GameObject skillTextPrefabs; //技能預製體

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
        Init();
    }

    /// <summary>
    /// 初始化函數
    /// </summary>
    private void Init()
    {
        if(isInit == false)
        {
            //加載系統
            packageSystem = PackageSystem.instance;
            playerAccountSystem = PlayerAccountSystem.instance;
            savedAndLoaded = SavedAndLoaded.instance;
            loadingSystem = LoadingSystem.instance;

            //獲得該單位的id
            int idInPackage = packageSystem.unitId;
            unit = packageSystem.unit_object[idInPackage].GetComponent<A000_Default>(); //設定這個單位的資料
            data = unit.use_data;
            LoadingInfo();  //載入這個單位的資料

            //檢測是否需要教學故事
            if (playerAccountSystem.storyRecorder.unitPageTutorial == false)
            {
                storySystem = StorySystem.instance;
                storySystem.LoadingStory("NoviceTeaching/unitPageTutorial");
            }

            isInit = true;
        }
    }

    /// <summary>
    /// 添加經驗值給該單位
    /// </summary>
    public void ChangeSpiritToExp()
    {
        int amount = int.Parse(changeSpiritAmount.text);    //獲得添加的數量

        //確認玩家靈力數量足夠
        if(playerAccountSystem.spirit >= amount)
        {
            playerAccountSystem.spirit -= amount;
            data.tempExp += amount;

            unit.LevelCounter();    //計算等級

            LoadingInfo();  //重新載入單位資料

            savedAndLoaded.SaveData();  //存檔
        }else
        {
            Debug.Log("靈力數量不足");
        }
    }

    /// <summary>
    /// 添加靈粹給該單位
    /// </summary>
    public void AddThreaToUnit()
    {
        int amount = int.Parse(addThreaAmount.text);

        if (playerAccountSystem.threa >= amount)
        {
            playerAccountSystem.threa -= amount;
            data.tempThrea += amount;

            unit.SpiritControlCounter();    //計算靈力掌控程度

            LoadingInfo();  //重新載入單位資料

            savedAndLoaded.SaveData();  //存檔
        }
        else
        {
            Debug.Log("靈粹數量不足");
        }
    }

    /// <summary>
    /// 載入該單位的資料
    /// </summary>
    public void LoadingInfo()
    {
        //描述相關
        id.text = "ID " + data.id.ToString();
        unitName.text = data.unitName;
        spiritGrade.text = "靈力等級: " + data.spiritGrade.ToString();

        //屬性相關
        atk.text = "Atk: " + data.atk.ToString();
        hp.text = "Hp: " + data.hp.ToString(); 
        def.text = "Def: " + data.def.ToString();

        //經驗值相關
        tempExp.text = "當前經驗值: " + data.tempExp.ToString();
        spiritRemain.text = "背包剩餘靈力數量: " + playerAccountSystem.spirit.ToString();
        toNextLevelExp.text = "提升至下一等級所需靈力: " + unit.SearchNextLevelExp().ToString();

        //靈力相關
        spiritControl.text = "靈力掌控程度: " + data.spiritControl.ToString();
        tempThrea.text = "單位已累計靈粹數量: " + data.tempThrea.ToString();
        threaRemain.text = "背包剩餘靈粹數量: " + playerAccountSystem.threa.ToString();
        if(data.spiritControl == SpiritControlStandard.Zp)
        {
            toNextControlLevel.color = Color.red;
            toNextControlLevel.text = "以達到最高級靈力掌控程度";
        }
        else
        {
            toNextControlLevel.color = Color.black;
            toNextControlLevel.text = "提升下一掌控程度所需靈粹數量: " + unit.SearchNextControlLevelThrea();
        }
        spiritControlInfluence.text = "能發揮的最終靈力倍率: " + ((int)(Mathf.Pow(ConstantChart.spiritInfluence, (int)data.spiritControl) * 100)).ToString() + "%";

        //美術相關
        unitArt.sprite = data.squareSprite;

        //技能相關
        skillContent.GetComponent<AdvancedGridLayoutGroupVertical>().cellNum = unit.skill.Length;   //設定技能數量
        for(int i =0;i<unit.skill.Length;i++)
        {
            //生成技能描述
            GameObject obj = Instantiate(skillTextPrefabs, skillContent.transform);
            obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = unit.skill[i].skillName + ":\n" + unit.skill[i].skillIntro;
        }
    }

    /// <summary>
    /// 返回主介面
    /// </summary>
    public void BackToMain()
    {
        loadingSystem.LoadTargetScene("MainScene");
    }
}
