using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DevelopSystem : MonoBehaviour
{
    public static DevelopSystem instance;

    [Header("系統")]
    private UnitBook unitBook;   //unitBook的系統
    private PackageSystem packageSystem;
    public GameObject package;

    [Header("輔助元件")]
    public TMP_InputField inputId;
    public TMP_InputField packageId;
    public TMP_InputField linePos;

    //單例模式
    //避免重複生成的檢查
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
        //獲取系統
        packageSystem = PackageSystem.instance;
        unitBook = UnitBook.instance;
    }

    //往背包中添加單位，根據單位圖鑑ID
    public void AddUnitWithID()
    {
        int id;
        id = int.Parse(inputId.text);   //獲取ID

        packageSystem.AddUnitWithID(id);
    }

    //設定出戰單位
    public void LineUpUnitWithID()
    {
        int id, pos;
        id = int.Parse(packageId.text); //獲取背包中的id
        pos = int.Parse(linePos.text);  //獲取對列中的位置

        packageSystem.LineUpUnitWithID(id, pos);
    }

    //儲存測試
    public void Save()
    {
        SavedAndLoaded.Instance.SaveData();
    }

    //載入測試
    public void Load()
    {
        SavedAndLoaded.Instance.LoadData();
    }

    // test stage function
    public void TestStage()
    {
        // 載入測試場景
        SceneManager.LoadScene("BattleScene");
    }
}
