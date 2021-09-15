using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class WindowManager : MonoBehaviour
{
    //Layer
    public enum Layer
    {
        Window,
        Tip,
    }
    //层级列表
    private static Dictionary<Layer, Transform> layers = new Dictionary<Layer, Transform>();
    //窗口列表
    public static Dictionary<string, BaseWindow> windows = new Dictionary<string, BaseWindow>();
    //窗口隐藏入栈
    public static Stack<BaseWindow> hideWindows = new Stack<BaseWindow>();
    //结构
    public static Transform root;
    public static Transform canvas;
    
    //初始化(所有Manager类都需要在使用前初始化。！！！！)
    public static void Init()
    {
        root = GameObject.Find("Root").transform;
        canvas = root.Find("Canvas");
        Transform window = canvas.Find("Window");
        Transform tip = canvas.Find("Tip");
        layers.Add(Layer.Window, window);
        layers.Add(Layer.Tip, tip);
    }
    //打开面板
    public static void Open<T>(params object[] para) where T : BaseWindow
    {
        //已经打开
        string[] names = typeof(T).ToString().Split('.');
        string name = names[names.Length - 1];
        //string name = typeof(T).ToString();
        if (windows.ContainsKey(name))
        {
            Close(name);
            //Open<>
            //return;
        }
        //组件
        BaseWindow window = root.gameObject.AddComponent<T>();
        window.OnInit();
        window.Init();
        //父容器
        Transform layer = layers[window.layer];
        window.prefab.transform.SetParent(layer, false);
        //列表
        windows.Add(name, window);
        //OnShow
        window.OnShow(para);
    }
    //关闭面板
    public static void Close(string name)
    {
        //name = "Code.Windows." + name;
        //没有打开
        if (!windows.ContainsKey(name))
        {
            Debug.LogError("没有该窗口打开");
            return;
        }
        BaseWindow window = windows[name];
        if (hideWindows.Contains(window))
        {
            hideWindows.Pop();
        }
        //OnClose
        window.OnClose();
        //列表
        windows.Remove(name);
        //销毁
        GameObject.Destroy(window.prefab);
        Component.Destroy(window);
    }
    //隐藏面板
    public static void Hide(string name)
    {
        if (!windows.ContainsKey(name))
        {
            Debug.LogError("没有该窗口打开");
            return;
        }
        BaseWindow window = windows[name];
        //OnHide
        window.OnHide();
        //列表
        hideWindows.Push(window);
        //销毁
        window.prefab.SetActive(false);
    }
    //显示面板
    public static void ReShow()
    {
        if (hideWindows.Count==0)
        {
            Debug.LogError("栈内没有窗口");
            return;
        }
        BaseWindow window = hideWindows.Pop();
        //OnReShow
        window.OnReShow();
        //隐藏
        window.prefab.SetActive(true);
    }
}
