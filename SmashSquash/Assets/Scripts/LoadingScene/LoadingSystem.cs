using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 各種場景轉換或載入 使用的載入系統
/// </summary>
public class LoadingSystem : MonoBehaviour
{
    public static LoadingSystem instance;

    [Header("加載介面設定")]
    public GameObject LoadingCanvas;
    public Slider loadingBar;
    public TextMeshProUGUI loadingText;

    [Header("開發者模式")]
    public bool isDevelop;

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

    void Start()
    {
        //打開遊戲後 自動載入開始準備介面
        LoadTargetScene("PreScene");
    }

    /// <summary>
    /// 載入指定場景
    /// </summary>
    /// <param name="sceneName">場景名稱</param>
    public void LoadTargetScene(string sceneName)
    {
        LoadingCanvas.SetActive(true);
        StartCoroutine(LoadLevel(sceneName));
    }
    
    /// <summary>
    /// 異步加載場景
    /// </summary>
    /// <param name="sceneName">場景名稱</param>
    /// <returns></returns>
    IEnumerator LoadLevel(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);  //預加載指定場景

        while(operation.isDone == false)
        {
            loadingBar.value = operation.progress;
            loadingText.text = "Loading......" + operation.progress * 100 + "%";

            yield return null;
        }

        LoadingCanvas.SetActive(false);
    }
}
