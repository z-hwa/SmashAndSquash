using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

/// <summary>
/// 訊息類型
/// </summary>
public enum RadioType
{
    System,
    UnitSkill
}

/// <summary>
/// 用於廣播提示資訊以及腳色資訊
/// </summary>
public class RadioSystem : MonoBehaviour
{
    public static RadioSystem instance;

    public GameObject radioContent; //畫布中scroll view的content
    public GameObject radioPrefab; //訊息預製件

    public GameObject imageInitPos; //技能動畫觸發生成 的父物件
    public GameObject imagePrefab; //技能動畫 預製件

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

        EventManager.AddListener(EventName.PlayRadio, PlayRadio);
    }

    /// <summary>
    /// 播放廣播
    /// </summary>
    /// <param name="message">廣播訊息</param>
    public void PlayRadio(string message, RadioType type)
    {
        if (type == RadioType.System) PlayRadio_System(message);
        else if(type == RadioType.UnitSkill) PlayRadio_UnitSkill(message);
    }

    /// <summary>
    /// 播放廣播
    /// </summary>
    /// <param name="_message">廣播訊息</param>
    /// <param name="_type">廣播類別</param>
    public void PlayRadio(object _message, object _type)
    {
        string message = _message.ToString();
        RadioType type = (RadioType)_type;

        if (type == RadioType.System) PlayRadio_System(message);
        else if (type == RadioType.UnitSkill) PlayRadio_UnitSkill(message);
    }

    /// <summary>
    /// 系統廣播
    /// </summary>
    /// <param name="message">訊息</param>
    private void PlayRadio_System(string message)
    {
        GameObject obj = Instantiate(radioPrefab, radioContent.transform);
        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "system." + message;
        Destroy(obj, 1f);
    }

    /// <summary>
    /// 單位技能廣播
    /// </summary>
    /// <param name="message">訊息</param>
    private void PlayRadio_UnitSkill(string message)
    {
        GameObject obj = Instantiate(radioPrefab, radioContent.transform);
        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "skill." + message;
        Destroy(obj, 1.8f);
    }

    /// <summary>
    /// 撥放技能觸發特效
    /// </summary>
    /// <param name="unitImage">單位的圖片 會休的噴出來</param>
    public void PlaySkillEffect(Sprite unitImage)
    {
        GameObject obj = Instantiate(imagePrefab, imageInitPos.transform);
        obj.GetComponent<Image>().sprite = unitImage;

        float deltaX = Random.Range(-50f, 50f);
        float deltaY = Random.Range(-50f, 50f);
        obj.transform.localPosition = new Vector3(deltaX, deltaY, 0);


        Vector3 endPos = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y + 150f, 0);

        Destroy(obj, 1.8f);
        StartCoroutine(ImageUp(obj, endPos));  //圖片上升效果
    }

    IEnumerator ImageUp(GameObject obj, Vector3 endPos)
    {
        while (obj != null)
        {
            obj.transform.localPosition = Vector3.Lerp(obj.transform.localPosition, endPos, 0.1f);
            yield return new WaitForSeconds(0.02f);
        }
    }
}
