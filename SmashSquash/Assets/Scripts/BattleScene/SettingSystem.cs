using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 戰鬥介面中的設定系統
/// 可以用來退出關卡
/// </summary>
public class SettingSystem : MonoBehaviour
{
    public static SettingSystem instance;

    public GameObject settingPage;

    private LoadingSystem loadingSystem;

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
    /// 設定介面的開關
    /// </summary>
    /// <param name="isShow">是否顯示</param>
    public void SetSetting(bool isShow)
    {
        if(isShow==true) settingPage.SetActive(true);
        else if(isShow == false) settingPage.SetActive(false);
    }

    /// <summary>
    /// 返回主介面
    /// </summary>
    public void GoToMain()
    {
        loadingSystem = LoadingSystem.instance;
        loadingSystem.LoadTargetScene("MainScene");
    }
}
