using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUi : MonoBehaviour
{
    private PlayerUI playerUI;
    private bool isInit = false;

    //血條相關
    [Header("血條相關物件")]    
    public GameObject healthBarPrefab;   //血條預制物件
    public Transform initPoint; //血條生成位置
    private GameObject healthBarCanvas;    //血條生成父物件

    public GameObject healthUI;   //血條物件
    public GameObject statusContent;    //狀態欄物件
    private Image fill; //血條填充物

    //傷害相關
    [Header("傷害相關物件")]
    public GameObject hitDamage;    //傷害預製件

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        if(isInit == false)
        {
            //獲取系統
            playerUI = PlayerUI.instance;

            //抓取血條生成畫布
            healthBarCanvas = GameObject.FindGameObjectsWithTag("Health Bar Canvas")[0];

            //生成血條物件
            healthUI = Instantiate(healthBarPrefab, healthBarCanvas.transform);
            healthUI.transform.position = initPoint.position;

            //初始化血輛顯示
            fill = healthUI.transform.GetChild(0).GetChild(0).GetComponent<Image>();   //設置血條填充物
            ChangeUnitHp(1, CampGroup.enemy, -1);

            //初始化狀態欄
            statusContent = healthUI.transform.GetChild(1).GetChild(0).GetChild(0).gameObject;

            isInit = true;
        }
    }

    void Update()
    {
        //實時更新血條的位置
        if(healthUI.transform.position != initPoint.position) healthUI.transform.position = initPoint.position;
    }

    /// <summary>
    /// 如果發生單位的血量改變，對應的UI會出現變化
    /// </summary>
    /// <param name="rate">當前血量比率</param>
    /// <param name="type">單位是哪個陣營的</param>
    /// <param name="lineUpPos">當前的index為何，玩家陣營才需要使用</param>
    public void ChangeUnitHp(float rate, CampGroup type, int lineUpPos)
    {
        if(type == CampGroup.enemy)
        {
            fill.fillAmount = rate;  //血條變化
        }else if(type == CampGroup.player)
        {
            fill.fillAmount = rate;  //血條變化
            playerUI.ChangeUnitHp(lineUpPos, rate);   //改變單位資料的血條UI
        }

        if (rate <= 0)
        {
            healthUI.transform.GetChild(0).gameObject.SetActive(false);   //沒血 就把血條關掉
            healthUI.transform.GetChild(1).gameObject.SetActive(false);   //沒血 就把狀態欄關掉
        }
    }

    /// <summary>
    /// 顯示碰撞傷害
    /// </summary>
    /// <param name="damage">傷害數字</param>
    public void ShowDamage(int damage)
    {
        Init();

        //生成傷害物件
        GameObject obj = Instantiate(hitDamage, healthUI.transform);

        //隨機生成位置
        Vector3 pos = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-2f, 1f), 0);
        pos += initPoint.transform.position;

        obj.transform.position = pos; //定位出現位置
        obj.GetComponent<HitDamage>().ShowDamage(damage);   //顯示傷害
    }
}
