using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 擊中的傷害物件腳本
/// </summary>
public class HitDamage : MonoBehaviour
{
    public float keepTime = 0.8f;   //傷害存活時間
    public TextMeshProUGUI damageText;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, keepTime);
    }

    /// <summary>
    /// 顯示傷害資料
    /// </summary>
    /// <param name="damage"></param>
    public void ShowDamage(int damage)
    {
        damageText.text = damage.ToString();
    }
}
