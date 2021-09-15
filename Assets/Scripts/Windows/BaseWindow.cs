using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Assets.Scripts.Manager;
using Assets.Scripts.Config;
public class BaseWindow : MonoBehaviour
{
    //窗口预制体路径
    public PrefabType.Window prefabType;
    //窗口预制体
    public GameObject prefab;
    //层级
    public WindowManager.Layer layer = WindowManager.Layer.Window;
    //初始化
    public virtual void Init()
    {
        //读取预制体
        GameObject prefabLoad = ResManager.Instance.Load<GameObject>(prefabType);
        prefab = (GameObject)Instantiate(prefabLoad);
    }
    //关闭
    public void Close()
    {
        string name = this.GetType().ToString();
        WindowManager.Close(name);
    }
    //初始化时
    public virtual void OnInit()
    {

    }
    //显示时
    public virtual void OnShow(params object[] para)
    {

    }
    //关闭时
    public virtual void OnClose()
    {

    }
    //隐藏时
    public virtual void OnHide()
    {

    }
    //面板从隐藏到显示
    public virtual void OnReShow()
    {

    }
}
