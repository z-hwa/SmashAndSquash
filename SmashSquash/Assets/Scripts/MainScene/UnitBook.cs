using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 單位圖鑑
/// 所有單位的預製體，都必須放進圖鑑內
/// </summary>
public class UnitBook : MonoBehaviour
{
    public static UnitBook instance;

    //用於登記所有的單位
    public GameObject[] unit_illustratedBook;

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
}
