using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 開始介面的準備系統
/// </summary>
public class PrepareSystem : MonoBehaviour
{
    private static PrepareSystem _prepareSystem;

    /// <summary>
    /// 單例模式，避免重複生成的檢查
    /// </summary>
    public static PrepareSystem Instance
    {
        get
        {
            if(_prepareSystem == null)
            {
                _prepareSystem = FindObjectOfType(typeof(PrepareSystem)) as PrepareSystem;
                if (_prepareSystem == null) Debug.LogError("no object with PrepareSystem");
                else
                {
                }
            }

            return _prepareSystem;
        }
    }


    void Start()
    {
        //撥放開始介面BGM
        MusicSystem.instance.PlayMusic(ConstantChart.backgroundBGM); 
    }

    //載入主頁面
    //加載全局資源
    public void TouchToStart()
    {
        SavedAndLoaded.Instance.LoadData();  //載入存檔

        //載入場景
        if (PlayerAccountSystem.Instance.storyRecorder.openingIntro == false)
        {
            LoadingSystem.Instance.LoadTargetScene("OpeningScene");
        }
        else
        {
            LoadingSystem.Instance.LoadTargetScene("MainScene");
        }
    }
}
