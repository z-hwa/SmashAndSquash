using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 狀態機
/// 移動、靜止
/// </summary>
public enum UnitMovingState
{
    Moving,
    Stop
}

/// <summary>
/// 處理Unit會發生在遊戲中的實際行為，造成的數據變化，會記錄到UnitData中
/// 每個Unit都應該有一個UnitBehavior
/// </summary>
public class UnitBehavior : MonoBehaviour
{
    public UnitMovingState nowState = UnitMovingState.Stop;   //紀錄現在的單位靜止狀態

    private float defaultMass = 1f;  //單位質量
    private float defaultDrag = 0.5f;  //單位阻尼
    private float endVelocity = 1.2f; //下限速度，速度強度低於該數值 就停止

    private Rigidbody2D rigi2D;    //該unit的鋼體
    public UnitData_InBattle unitData;  //該unit的資料
    private Vector2 inVelocity; //進入碰撞前的速度向量

    private float lastRec = 0f; //上一次紀錄速度的時間
    private float freOfRecordVelocity = 0.5f;   //紀錄速度的頻率
    private float lastVec = 0f; //上一次紀錄的速度強度

    //拉動時的箭頭ui
    public GameObject arrorUi;

    private MusicSystem musicSystem;
    private VFXSystem vFXSystem;

    //用於記錄當前unit移動的速度、方向 (向量)
    private void LateUpdate()
    {
        inVelocity = rigi2D.velocity;  //實時更新速度向量 (DEBUG: 如果進入碰撞才獲取速度 就會變成0
        
        //如果是靜止的 速度大於0 則狀態設為移動
        if(nowState == UnitMovingState.Stop && rigi2D.velocity.magnitude > 0f)
        {
            nowState = UnitMovingState.Moving;
        } 

        //如果在移動 則開始記錄前0.5秒的速度強度
        if(nowState == UnitMovingState.Moving && Time.time - lastRec > freOfRecordVelocity)
        {
            lastRec = Time.time;
            lastVec = rigi2D.velocity.magnitude;
        }
        
        //移動 且 在減速中 那速度小於一定程度 直接停止
        if(nowState == UnitMovingState.Moving && lastVec > rigi2D.velocity.magnitude && rigi2D.velocity.magnitude < endVelocity)
        {
            rigi2D.velocity = Vector2.zero;
            nowState = UnitMovingState.Stop;  //進入靜止狀態
        }
    }

    void Start()
    {
        //獲取系統
        musicSystem = MusicSystem.instance;
        vFXSystem = VFXSystem.instance;
    }

    /// <summary>
    /// 初始化，這個Unit的物理性質
    /// </summary>
    public void InitUnitBehavior()
    {
        //component
        rigi2D = GetComponent<Rigidbody2D>();  //獲得鋼體
        unitData = GetComponent<UnitData_InBattle>();    //獲取這個單位的資料
        
        //賦予單位 質量與阻尼(之後從單位資料獲取
        rigi2D.mass = defaultMass;
        rigi2D.drag = defaultDrag;

        //剛開始為靜止
        nowState = UnitMovingState.Stop;

        //速度紀錄相關
        lastRec = 0f;
        lastVec = 0f;
    }

    /// <summary>
    /// 根據力量 將單位射擊出去
    /// 只有發射的時候 才會使用
    /// 反彈的部分，則是直接賦予unit速度
    /// </summary>
    /// <param name="_force">發射的力度</param>
    public void ShootUnit(Vector2 _force)
    {
        //撥放射擊音效
        musicSystem.PlayEffect(ConstantChart.shootEffect);

        rigi2D.AddForce(_force);   //施加 力的向量
        nowState = UnitMovingState.Moving;    //進入移動
    }

    /// <summary>
    /// 與牆壁以及單位的反彈檢測
    /// </summary>
    /// <param name="collision">碰撞到的物體</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D rigidOfColli = collision.gameObject.GetComponent<Rigidbody2D>();    //獲取被撞者的rigid

        //獲取撞擊事件發生的位置
        Vector3 pos = (this.gameObject.transform.position + collision.gameObject.transform.position) / 2;
        Quaternion rot = this.gameObject.transform.rotation;

        if (collision.gameObject.tag == "wall")
        {
            //傳入進入向量、法向量 計算射出向量
            Vector2 reflexAngle = Vector2.Reflect(inVelocity, collision.GetContact(0).normal);

            rigi2D.velocity = reflexAngle.normalized * inVelocity.magnitude * 0.6f;   //施加新的速度
        }
        else if ((collision.gameObject.tag == "unit" || collision.gameObject.tag == "enemy")
            && (inVelocity.magnitude - rigidOfColli.velocity.magnitude) > ConstantChart.floatSuppirt)
        {
            //計算碰撞後新速度的強度
            float newMagnitude = inVelocity.magnitude - rigidOfColli.velocity.magnitude;    //計算新的速度強度

            //傳入進入向量、法向量 計算射出向量
            Vector2 reflexAngle = Vector2.Reflect(inVelocity, collision.GetContact(0).normal);
            rigi2D.velocity = reflexAngle.normalized * newMagnitude;   //施加新的速度

            rigidOfColli.velocity = inVelocity.normalized * newMagnitude; //對被撞擊的物體 施加新的速度

            //雙方標籤不同 才扣血 且速度慢的會被攻擊
            if(gameObject.tag != collision.gameObject.tag)
                unitData.SmashSettlement(collision.gameObject);  //碰撞結算

            //碰撞音效
            musicSystem.PlayEffect(ConstantChart.hitEffect);
            vFXSystem.MakeEffect(0, pos, rot);  //碰撞效果
        }
    }
}
