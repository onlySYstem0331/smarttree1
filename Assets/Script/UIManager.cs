using System;
using UnityEngine;

public class UIManager
{
    private static UIManager instance;

    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new UIManager();
            }
            return instance;
        }
    }

    private GameObject exitGameObject;
    private bool _init = false;
    public bool Init
    {
        get { return _init; }
        set
        {
            if (_init != value)
            {
                _init = value;
                if (_init)
                {
                    InitChanged?.Invoke();
                }
            }
        }
    }

    public event Action InitChanged;

    // 存储每个回调函数的执行标志
    private System.Collections.Generic.Dictionary<System.Action, bool> callbackExecuted = new System.Collections.Generic.Dictionary<System.Action, bool>();

    public void UiControl(System.Action callback = null)
    {
        if (exitGameObject == null)
        {
            exitGameObject = GameObject.Find("Knowledge");
        }

        if (Init)
        {
            Init = false;
            if (callback != null)
            {
                callback();
                // 标记回调已执行
                if (!callbackExecuted.ContainsKey(callback))
                {
                    callbackExecuted.Add(callback, true);
                }
                else
                {
                    callbackExecuted[callback] = true;
                }
            }
        }
        else
        {
            if (callback != null)
            {
                // 注册回调函数
                InitChanged += () =>
                {
                    if (!callbackExecuted.ContainsKey(callback) || !callbackExecuted[callback])
                    {
                        callback();
                        // 标记回调已执行
                        if (!callbackExecuted.ContainsKey(callback))
                        {
                            callbackExecuted.Add(callback, true);
                        }
                        else
                        {
                            callbackExecuted[callback] = true;
                        }
                        Init = false;
                    }
                };
            }
        }
    }
}