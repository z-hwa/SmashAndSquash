using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 主界面的各種頁面切換
/// </summary>
public class BottomBar : MonoBehaviour
{
    public static BottomBar instance;

    //bottom bar canvas
    [Header("Canvas")]
    public GameObject mainCanva;
    public GameObject packageCanva;
    public GameObject summonerCanva;
    public GameObject otherCanva;
    public GameObject developCanva;

    //系統
    private PackagePage packagePage;
    private PlayerInfo playerInfo;
    private LoadingSystem loadingSystem;

    //特殊物件
    public GameObject stageDefault; //用於在非主頁 代替關卡選項

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

    void Start()
    {
        //設置系統
        packagePage = PackagePage.instance;
        playerInfo = PlayerInfo.instance;
        loadingSystem = LoadingSystem.instance;

        //預設進入main page
        GoToMainPage();
    }

    /// <summary>
    /// Go to main page
    /// </summary>
    public void GoToMainPage()
    {
        ClosedNowPage();
        stageDefault.SetActive(false);  //關閉代替物件

        mainCanva.SetActive(true);
        //mainSystem.Init();  //init main system
    }

    /// <summary>
    /// go to unit package
    /// </summary>
    public void GoToPackage()
    {
        ClosedNowPage();
        packageCanva.SetActive(true);

        packagePage.LoadingPackage();   //載入背包內容
    }

    /// <summary>
    /// go to summoner page
    /// </summary>
    public void GoToSummonerPage()
    {
        ClosedNowPage();
        summonerCanva.SetActive(true);
        //summonerSystem.Init();
    }

    /// <summary>
    /// go to other page
    /// </summary>
    public void GoToOther()
    {
        ClosedNowPage();
        otherCanva.SetActive(true);
        //otherSystem.Init();
    }

    /// <summary>
    /// 前往關卡介面
    /// </summary>
    public void GoToStageInfo()
    {
        loadingSystem.LoadTargetScene("StageScene");
    }

    /// <summary>
    /// 前往開發窗口
    /// </summary>
    public void GoToDevelop()
    {
        ClosedNowPage();
        developCanva.SetActive(true);
    }

    /// <summary>
    /// closed now using page
    /// </summary>
    void ClosedNowPage()
    {
        if (mainCanva.activeSelf == true)
        {
            mainCanva.SetActive(false);
        }
        if (packageCanva.activeSelf == true)
        {
            packageCanva.SetActive(false);
            packagePage.UnLoadingPackage();
        }
        if (summonerCanva.activeSelf == true) summonerCanva.SetActive(false);
        if (otherCanva.activeSelf == true) otherCanva.SetActive(false);
        if (developCanva.activeSelf == true) developCanva.SetActive(false);
        if (playerInfo.renamePage.activeSelf == true) playerInfo.EndRename(false);

        stageDefault.SetActive(true);   //激活代替物件
    }
}
