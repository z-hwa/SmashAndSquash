using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 特效系統
/// </summary>
public class VFXSystem : MonoBehaviour
{
    public static VFXSystem instance;

    //特效預製體，特效列表
    public GameObject[] effect;

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

    /// <summary>
    /// 製造特效
    /// </summary>
    /// <param name="index">特效index</param>
    /// <param name="pos">特效生成位置</param>
    /// <param name="rot">特效生成角度</param>
    public void MakeEffect(int index, Vector3 pos, Quaternion rot)
    {
        Instantiate(effect[index], pos, rot);
    }
}
