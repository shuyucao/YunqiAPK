using UnityEngine;
using System.Collections;
public class WingScreenBase : MonoBehaviour
{
    public EWingScreen wingScreen;
    private UIPanel mPanel;
    public string PrefabName
    {
        get;
        set;
    }

    // Use this for initialization,before start,only once
    public virtual void Init()
    {
    }

    // Use this for initialization,after start
    public virtual void Show(object param)//screen入栈被调用
    {
    }


    public virtual void Destroy()//screen出栈被调用
    {
    }

    public bool Visible
    {
        get { return gameObject.activeSelf; }
    }

    public T GetNGUIComponent<T>(string name) where T : Component
    {
        GameObject gameObj = gameObject.GetNGUIComponentByID(name);
        return gameObject.GetComponent<T>();
    }
}
