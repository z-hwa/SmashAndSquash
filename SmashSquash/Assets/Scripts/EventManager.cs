using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 記錄各種事件的名稱
/// </summary>
public enum EventName
{
    //SaveAndLoaded
    LoadData,

    //LoadingSystem
    LoadTargetScene,

    //PlayerInfo
    ShowPlayerInfo,

    //RadioSystem
    PlayRadio,

    //PackageSystem
    LineUpUnitWithID,
    AddUnitWithID
}

/// <summary>
/// 事件管理腳本
/// </summary>
public class EventManager : MonoBehaviour
{  
    //事件字典
    private Dictionary<EventName, Action> _events_0a;
    private Dictionary<EventName, Action<object>> _events_1a;
    private Dictionary<EventName, Action<object, object>> _events_2a;
    private Dictionary<EventName, Func<object, object, object>> _events_2f;
    
    private static EventManager _eventManager;  //儲存字典實體

    /// <summary>
    /// 用於拿取的字典實體
    /// </summary>
    public static EventManager Instance
    {
        get
        {
            if (!_eventManager)
            {
                _eventManager = FindObjectOfType(typeof(EventManager)) as EventManager; //尋找場景中帶有EventManager type的物件
                if (!_eventManager) Debug.LogError("There needs to be one active EventManager script on a Gameobject"); //找不到輸出錯誤訊息
                else
                {
                    _eventManager.Init();  //找到 進行事件管理器的初始化
                }

            }
            
            return _eventManager; //回傳事件管理器
        }
    }

    void Awake()
    {
        Init();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    void Init()
    {
        DontDestroyOnLoad(gameObject);

        if (_events_0a == null) _events_0a = new Dictionary<EventName, Action>();
        if (_events_1a == null) _events_1a = new Dictionary<EventName, Action<object>>();
        if (_events_2a == null) _events_2a = new Dictionary<EventName, Action<object, object>>();

        if (_events_2f == null) _events_2f = new Dictionary<EventName, Func<object, object, object>>();
    }

    /// <summary>
    /// 訂閱事件
    /// </summary>
    /// <param name="evtName">事件名稱</param>
    /// <param name="listener">訂閱者</param>
    public static void AddListener(EventName evtName, Action listener)
    {
        Action evt = null;
        if(Instance._events_0a.TryGetValue(evtName, out evt))
        {
            evt += listener;  //事件已被創建 訂閱該事件
        }else
        {
            evt += listener;  //訂閱該事件
            Instance._events_0a.Add(evtName, evt); //於字典中添加該事件
        }
    }

    /// <summary>
    /// 移除訂閱
    /// </summary>
    /// <param name="evtName">事件名稱</param>
    /// <param name="listener">訂閱者</param>
    public static void RemoveListener(EventName evtName, Action listener) {
        if (_eventManager == null) return;  //事件管理器為空 直接回傳

        Action evt = null;
        if(Instance._events_0a.TryGetValue(evtName,out evt)) evt -= listener; //如果該事件存在，移除訂閱
    }

    /// <summary>
    /// 激活事件
    /// </summary>
    /// <param name="evtName">事件名稱</param>
    public static void Trigger(EventName evtName)
    {
        Action evt = null;
        if (Instance._events_0a.TryGetValue(evtName, out evt)) evt.Invoke(); //事件存在 激活該事件
    }

    /// <summary>
    /// 訂閱事件
    /// </summary>
    /// <param name="evtName">事件名稱</param>
    /// <param name="listener">訂閱者</param>
    public static void AddListener(EventName evtName, Action<object> listener)
    {
        Action<object> evt = null;
        if (Instance._events_1a.TryGetValue(evtName, out evt))
        {
            evt += listener;  //事件已被創建 訂閱該事件
        }
        else
        {
            evt += listener;  //訂閱該事件
            Instance._events_1a.Add(evtName, evt); //於字典中添加該事件
        }
    }

    /// <summary>
    /// 移除訂閱
    /// </summary>
    /// <param name="evtName">事件名稱</param>
    /// <param name="listener">訂閱者</param>
    public static void RemoveListener(EventName evtName, Action<object> listener)
    {
        if (_eventManager == null) return;  //事件管理器為空 直接回傳

        Action<object> evt = null;
        if (Instance._events_1a.TryGetValue(evtName, out evt)) evt -= listener; //如果該事件存在，移除訂閱
    }

    /// <summary>
    /// 激活事件
    /// </summary>
    /// <param name="evtName">事件名稱</param>
    public static void Trigger(EventName evtName, object arg1)
    {
        Action<object> evt = null;
        if(Instance._events_1a.TryGetValue(evtName,out evt)) evt.Invoke(arg1); //事件存在 激活該事件
    }

    /// <summary>
    /// 訂閱事件
    /// </summary>
    /// <param name="evtName">事件名稱</param>
    /// <param name="listener">訂閱者</param>
    public static void AddListener(EventName evtName, Action<object, object> listener)
    {
        Action<object, object> evt = null;
        if (Instance._events_2a.TryGetValue(evtName, out evt))
        {
            evt += listener;  //事件已被創建 訂閱該事件
        }
        else
        {
            evt += listener;  //訂閱該事件
            Instance._events_2a.Add(evtName, evt); //於字典中添加該事件
        }
    }

    /// <summary>
    /// 移除訂閱
    /// </summary>
    /// <param name="evtName">事件名稱</param>
    /// <param name="listener">訂閱者</param>
    public static void RemoveListener(EventName evtName, Action<object, object> listener)
    {
        if (_eventManager == null) return;  //事件管理器為空 直接回傳

        Action<object, object> evt = null;
        if (Instance._events_2a.TryGetValue(evtName, out evt)) evt -= listener; //如果該事件存在，移除訂閱
    }

    /// <summary>
    /// 激活事件
    /// </summary>
    /// <param name="evtName">事件名稱</param>
    public static void Trigger(EventName evtName, object arg1, object arg2)
    {
        Action<object, object> evt = null;
        if (Instance._events_2a.TryGetValue(evtName, out evt)) evt.Invoke(arg1,  arg2); //事件存在 激活該事件
    }

    /// <summary>
    /// 訂閱事件
    /// </summary>
    /// <param name="evtName">事件名稱</param>
    /// <param name="listener">訂閱者</param>
    public static void AddListener(EventName evtName, Func<object, object, object> listener)
    {
        Func<object, object, object> evt = null;
        if (Instance._events_2f.TryGetValue(evtName, out evt))
        {
            evt += listener;  //事件已被創建 訂閱該事件
        }
        else
        {
            evt += listener;  //訂閱該事件
            Instance._events_2f.Add(evtName, evt); //於字典中添加該事件
        }
    }

    /// <summary>
    /// 移除訂閱
    /// </summary>
    /// <param name="evtName">事件名稱</param>
    /// <param name="listener">訂閱者</param>
    public static void RemoveListener(EventName evtName, Func<object, object, object> listener)
    {
        if (_eventManager == null) return;  //事件管理器為空 直接回傳

        Func<object, object, object> evt = null;
        if (Instance._events_2f.TryGetValue(evtName, out evt)) evt -= listener; //如果該事件存在，移除訂閱
    }

    /// <summary>
    /// 激活事件
    /// </summary>
    /// <param name="evtName">事件名稱</param>
    public static object Trigger_f(EventName evtName, object arg1, object arg2)
    {
        Func<object, object, object> evt = null;
        if (Instance._events_2f.TryGetValue(evtName, out evt)) return evt.Invoke(arg1, arg2); //事件存在 激活該事件
        else return null;
    }
}
