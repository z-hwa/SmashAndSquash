using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 戰鬥過程中只會有一個，處理戰鬥中，玩家操控的事件
/// </summary>
public class PlayerControlSystem : MonoBehaviour
{
    public static PlayerControlSystem instance;

    private float beganTime = 0f;   //點擊開始的時間
    private float interval = 0f;    //間隔的時間
    public float swipeMagnitudeLower = 120f;    //滑動力度的下限 (標準
    public float swipeMagnitudeUpper = 3000f;    //滑動力度的上限 (標準
    public float arrorStandardMagnitude = 200f; //箭頭UI每次變大的基準

    private Vector2 startPos = Vector2.zero;    //點擊初始點
    private Vector2 endPos = Vector2.zero;  //點擊結束點
    private Vector2 direction = Vector2.zero;   //紀錄滑動的方向

    private GameObject controlUnit; //控制中的單位
    public bool manipulateAvailable = false;  //操控可用性

    public GameObject playerUi; //玩家的單位UI介面

    //測試資料區
    public Vector2 DirMinRange, DirMaxRange;    //測試向量的區間

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

    void Update()
    {
        //電腦測試
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector2 testDir = Vector2.zero;
            testDir.x = Random.Range(DirMinRange.x, DirMaxRange.x);
            testDir.y = Random.Range(DirMinRange.y, DirMaxRange.y);
            Swipe(testDir);
        }

        //手機操作
        TouchDetect();
    }

    /// <summary>
    /// 偵測對於遊戲的點擊
    /// </summary>
    private void TouchDetect()
    {
        //發生了點擊
        if(Input.touchCount == 1)
        {

            Touch touch = Input.GetTouch(0);    //獲得第一個接觸的手指 點擊的事件

            bool isTouchUIElement = EventSystem.current.IsPointerOverGameObject(touch.fingerId);    //檢查是否點擊到UI

            //如果沒點到UI
            if(!isTouchUIElement)
            {
                //根據點擊的狀況 進入不同的操作偵測
                switch(touch.phase)
                {
                    //點擊的開始
                    case TouchPhase.Began:
                        startPos = touch.position;  //紀錄點擊開始的位置
                        beganTime = Time.realtimeSinceStartup;  //不受timescale影響的時間

                        playerUi.SetActive(false);
                        //QuickDoubleTab();   //判斷是否雙擊
                        break;

                    //移動
                    case TouchPhase.Moved:
                        direction = touch.position - startPos;  //獲得往哪個方向滑動
                        interval = Time.realtimeSinceStartup - beganTime;   //獲得間隔時間

                        ShowArrow(direction);

                        //Hold(); 偵測是否為長按
                        break;

                    //停止
                    case TouchPhase.Stationary:
                        interval = Time.realtimeSinceStartup - beganTime;   //獲得間隔時間

                        //Hold(); 偵測是否為長按
                        break;

                    //離開
                    case TouchPhase.Ended:
                        interval = Time.realtimeSinceStartup - beganTime;   //獲得間隔時間
                        endPos = direction + startPos;  //計算結束點

                        Swipe(direction);    //判斷是否滑動
                        playerUi.SetActive(true);
                        break;
                }
            }
        }
    }

    /// <summary>
    /// 顯示箭頭輔助UI
    /// </summary>
    /// <param name="_direction">拉動的方向</param>
    private void ShowArrow(Vector2 _direction)
    {
        Vector2 shootingDir = _direction;

        //判斷滑動距離 且 存在控制物體
        if (shootingDir.magnitude > swipeMagnitudeLower && controlUnit != null && manipulateAvailable == true)
        {
            UnitBehavior unitBehavior = controlUnit.GetComponent<UnitBehavior>();
            if(unitBehavior.arrorUi.activeSelf == false) unitBehavior.arrorUi.SetActive(true);   //激活arror ui

            float rate = (shootingDir.magnitude - swipeMagnitudeLower) / (swipeMagnitudeUpper - swipeMagnitudeLower) * 3 + 1;
            if(rate > 1) {
                unitBehavior.arrorUi.transform.localScale = new Vector3(rate, 2 * rate, 0);   //放大箭頭
            }

            float rotate = Mathf.Atan2(shootingDir.y, shootingDir.x);
            float angle = rotate * 180 / Mathf.PI;
            unitBehavior.arrorUi.transform.rotation = Quaternion.Euler(0, 0, angle + 90);   //旋轉
        }
    }

    /// <summary>
    /// 手指滑動後的行為
    /// </summary>
    /// <param name="_direction">拉動的方向</param>
    private void Swipe(Vector2 _direction)
    {
        Vector2 shootingDir = -_direction;  //射擊方向和拉動方向 相反

        //關閉箭頭UI
        UnitBehavior unitBehavior = controlUnit.GetComponent<UnitBehavior>();
        if (unitBehavior.arrorUi.activeSelf == true) unitBehavior.arrorUi.SetActive(false);   //激活arror ui

        //判斷滑動距離 且 存在控制物體
        if (shootingDir.magnitude > swipeMagnitudeLower && controlUnit != null && manipulateAvailable == true)
        {
            manipulateAvailable = false;    //完成操作 操控可用設為false
            controlUnit.GetComponent<UnitBehavior>().ShootUnit(shootingDir * ConstantChart.dragMulti);    //彈射單位出去
        }
    }

    /// <summary>
    /// 更換控制的單位
    /// </summary>
    /// <param name="gameObject"></param>
    public void ChangeControlUnit(GameObject gameObject)
    {
        controlUnit = gameObject; 
    }

    /// <summary>
    /// 初始化玩家操控系統
    /// </summary>
    public void InitPlayerControlSystem()
    {
        manipulateAvailable = false;
    }
}
