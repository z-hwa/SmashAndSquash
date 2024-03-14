using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 背包系統本體
/// </summary>
public class PackageSystem : MonoBehaviour
{
    //單例模式
    public static PackageSystem instance;

    [Header("背包")]
    public GameObject[] unit_object = new GameObject[ConstantChart.PACKAGEMAXNUM];    //背包中存放的物件
    public int packageCapacity = 0; //當前背包使用的容量
    public GameObject package;  //存放單位實體的物件

    [Header("出戰單位")]
    //public A000_Default[] lineUp = new A000_Default[MAXLINEUP];   //出戰單位 目前最多四隻 紀錄物件上的腳本
    public GameObject[] lineUp = new GameObject[ConstantChart.MAXLINEUP];
    public int[] lineUpId = new int[ConstantChart.MAXLINEUP]; //紀錄出戰單位的背包ID 防止重複出戰
    public int unitNumber = 1;  //當前出戰數量

    [Header("單位資料介面")]
    public int unitId;

    //系統
    private UnitBook unitBook;  //單位資料庫
    private LoadingSystem loadingSystem;
    private RadioSystem radioSystem;

    /// <summary>
    /// 單例模式，避免重複生成的檢查
    /// </summary>
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }else
        {
            if(instance!=this)
            {
                Destroy(gameObject);
            }
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        InitPackageSystem();
        unitBook = UnitBook.instance;
        loadingSystem = LoadingSystem.instance;
        radioSystem = RadioSystem.instance;
    }

    /// <summary>
    /// 往背包中添加單位，根據單位圖鑑ID
    /// </summary>
    /// <param name="id">單位在圖鑑中的id</param>
    public void AddUnitWithID(int id)
    {
        GameObject obj = Instantiate(unitBook.unit_illustratedBook[id], package.transform); //生成對應物件，在背包底下
        obj.GetComponent<A000_Default>().InitUnitData();    //初始化該單位資料

        unit_object[packageCapacity] = obj;

        packageCapacity++;
    }

    /// <summary>
    /// 往背包中添加單位，根據單位圖鑑ID
    /// 用於讀檔 以避免錯誤的增加背包數量
    /// 以及複製存檔中的單位資料
    /// </summary>
    /// <param name="id">單位在圖鑑中的id</param>
    /// <param name="a000_Data">單位的靜態儲存格式</param>
    public void AddUnitWithID(int id, A000_Data a000_Data)
    {
        GameObject obj = Instantiate(unitBook.unit_illustratedBook[id], package.transform); //生成對應物件，在背包底下
        obj.GetComponent<A000_Default>().InitUnitData();    //初始化該單位資料
        obj.GetComponent<A000_Default>().Copy_FromSaved(a000_Data);  //初始化實體身上的單位資料
        unit_object[packageCapacity] = obj;

        packageCapacity++;
    }

    /// <summary>
    /// 設定出戰單位
    /// </summary>
    /// <param name="id">背包中的index</param>
    /// <param name="pos">對列的位置</param>
    /// <returns>上陣是否成功</returns>
    public bool LineUpUnitWithID(int id, int pos)
    {
        //檢查是否滿足上陣條件
        bool isCorrect = CheckLineUp(id, pos);
        if(isCorrect == false) return false;   //上陣失敗

        lineUp[pos] = unit_object[id];    //把背包中的單位放進對列中
        lineUpId[pos] = id;

        int unitNum = 0;
        for (int i = 0; i < 4; i++)
        {
            if (lineUp[i] == null) continue;
            else unitNum++;
        }

        unitNumber = unitNum; //更新背包中單位數量

        return true;
    }

    /// <summary>
    /// 檢查當前設置是否能上陣單位
    /// </summary>
    /// <param name="id">單位在背包中的id</param>
    /// <param name="pos">對列位置</param>
    /// <returns>是否正確</returns>
    private bool CheckLineUp(int id, int pos)
    {

        bool illegalId = false;

        //檢查是否為重複ID
        for (int i = 0; i < ConstantChart.MAXLINEUP; i++)
        {
            if (i != pos && lineUpId[i] == id) illegalId = true;
        }
        if (illegalId == true)
        {
            Debug.Log("ID is repeat");
            radioSystem.PlayRadio("無法重複上陣同一單位", RadioType.System);
            return false;
        }

        //檢查背包中該單位是否存在
        if (unit_object[id] == null)
        {
            Debug.Log("this unit do not exist");
            radioSystem.PlayRadio("目前無選擇背包單位", RadioType.System);
            return false;
        }

        //確認是否已選擇對列中的位置
        if (pos < 0 || pos >= 4)
        {
            Debug.Log("Wrond pos in line up");
            radioSystem.PlayRadio("目前無選擇對列位置", RadioType.System);
            return false;
        }

        return true;
    }

    /// <summary>
    /// 設定出戰單位
    /// </summary>
    /// <param name="pos">對列位置</param>
    /// <returns>是否成功取消上陣</returns>
    public bool LineDownUnitWithPos(int pos)
    {
        //確認是否已選擇對列中的位置
        if (pos < 0 || pos >= 4)
        {
            Debug.Log("Wrond pos in line up");
            radioSystem.PlayRadio("目前無選擇對列位置", RadioType.System);
            return false;
        }

        lineUp[pos] = null;    //把背包中的單位放進對列中
        lineUpId[pos] = -1;

        int unitNum = 0;
        for (int i = 0; i < 4; i++)
        {
            if (lineUp[i] == null) continue;
            else unitNum++;
        }

        unitNumber = unitNum; //更新背包中單位數量

        return true;
    }

    /// <summary>
    /// 獲取御靈背包
    /// </summary>
    public void InitPackageSystem()
    {
        //load data from saved data

        // 初始化對列Id
        for(int i =0;i<ConstantChart.MAXLINEUP;i++)
        {
            lineUpId[i] = -1;
        }

        //獲取背包
        GameObject[] objs = GameObject.FindGameObjectsWithTag("package");
        package = objs[0];
    }

    /// <summary>
    /// 載入單位資料介面
    /// </summary>
    /// <param name="id">背包中的單位index</param>
    public void LoadUnitPage(int id)
    {
        unitId = id;    //設定查看單位的ID

        loadingSystem.LoadTargetScene("UnitScene");
    }
}
