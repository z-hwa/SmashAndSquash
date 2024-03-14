using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 地圖圖鑑
/// 所有地圖預製體，都需要放進這裡面
/// </summary>
public class MapBook : MonoBehaviour
{
    public static MapBook instance;

    public int mapIndex;

    //用於登記所有的單位
    public GameObject[] map_illustratedBook;

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

        DontDestroyOnLoad(gameObject);
    }
}
