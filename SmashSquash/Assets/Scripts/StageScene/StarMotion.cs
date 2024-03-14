using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 星空撥放動畫系統
/// </summary>
public class StarMotion : MonoBehaviour
{
    //星空速度與星空物件
    public float velocity;
    public Transform star;
    public int dir = 1; //方向

    //星空的邊界
    public Transform myLeft;
    public Transform myRight;

    //螢幕的邊界
    public Transform boundaryLeft;
    public Transform boundaryRight;

    void Start()
    {
        star = this.transform;
    }

    void Update()
    {
        StarAnim();
    }

    /// <summary>
    /// 播放星空移動動畫
    /// </summary>
    private void StarAnim()
    {
        //變換方向
        if (myLeft.transform.position.x > boundaryLeft.transform.position.x) dir = -1;
        else if (myRight.transform.position.x < boundaryRight.transform.position.x) dir = 1;

        star.transform.localPosition = new Vector3(star.transform.localPosition.x + dir * velocity, star.transform.localPosition.y, 0);
    }
}
