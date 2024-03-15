using System.Collections;
using System.Collections.Generic;
using Flower;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// 故事主系統
/// 用於載入Flower的設定
/// </summary>
public class StorySystem : MonoBehaviour
{
    public static StorySystem instance;

    private FlowerSystem flowerSystem;
    private BottomBar bottomBar;
    private MusicSystem musicSystem;
    private LoadingSystem loadingSystem;
    
    // 不希望玩家在看故事的時候亂碰，用於遮擋的物件
    public GameObject storyBackCanvaPrefab;   
    private GameObject storyBackCanva;

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
        //其他系統
        musicSystem = MusicSystem.instance;
        loadingSystem = LoadingSystem._loadingSystem;

        flowerSystem = FlowerManager.Instance.CreateFlowerSystem("default", false);    //創建flower系統
        flowerSystem.SetupDialog();     //加載預設的對話框等等

        RegisterCode(); //註冊指令
    }

    void Update()
    {
        TouchDetect();
    }

    /// <summary>
    /// 偵測玩家是否點擊，並進入下一段對話
    /// </summary>
    private void TouchDetect()
    {
        //發生了點擊
        if (Input.touchCount == 1)
        {

            Touch touch = Input.GetTouch(0);    //獲得第一個接觸的手指 點擊的事件

            //根據點擊的狀況 進入不同的操作偵測
            switch (touch.phase)
            {
                //點擊的開始
                case TouchPhase.Began:
                    flowerSystem.Next();

                    //QuickDoubleTab();   //判斷是否雙擊
                    break;

                //移動
                case TouchPhase.Moved:
                    //Hold(); 偵測是否為長按
                    break;

                //停止
                case TouchPhase.Stationary:
                    //Hold(); 偵測是否為長按
                    break;

                //離開
                case TouchPhase.Ended:
                    break;
            }
        }

        //電腦測試用
        if (Input.GetKeyDown(KeyCode.Space))
        {
            flowerSystem.Next();
        }
    }

    /// <summary>
    /// 加載指定故事
    /// </summary>
    /// <param name="route">故事的路徑</param>
    public void LoadingStory(string route)
    {
        flowerSystem.ReadTextFromResource(route);
    }

    /// <summary>
    /// 註冊代碼
    /// </summary>
    private void RegisterCode()
    {
        //註冊載入頁面的指令
        flowerSystem.RegisterCommand("LoadPage", (List<string> _params) =>
        {
            loadingSystem.LoadTargetScene(_params[0]);
        });

        //註冊某個故事放完的指令
        flowerSystem.RegisterCommand("ReadedStory", (List<string> _params) =>
        {
            PlayerAccountSystem.Instance.storyRecorder.ReadedStory(_params[0]);   //設為已經閱讀過該故事
            SavedAndLoaded.Instance.SaveData();
        });

        //註冊主頁面中 切換頁面的指令
        flowerSystem.RegisterCommand("ChangePage", (List<string> _params) =>
        {
            bottomBar = BottomBar.instance;

            if (_params[0] == "Package") bottomBar.GoToPackage();
            else if (_params[0] == "Summond") bottomBar.GoToSummonerPage();
            else if (_params[0] == "Other") bottomBar.GoToOther();
            else if(_params[0] == "Main") bottomBar.GoToMainPage();
        });

        //註冊調整音樂的指令
        flowerSystem.RegisterCommand("MusicSystem", (List<string> _params) =>
        {
            if(_params[0] == "Stop") musicSystem.StopMusic();
            else if(_params[0] == "Start") musicSystem.StartMusic();
        });

        //註冊調整遮擋背景的指令
        flowerSystem.RegisterCommand("Background", (List<string> _params) =>
        {
            if (_params[0] == "true")
            {
                storyBackCanva = Instantiate(storyBackCanvaPrefab);
            }
            else if (_params[0] == "false")
            {
                Destroy(storyBackCanva);
            }
        });
    }
}
