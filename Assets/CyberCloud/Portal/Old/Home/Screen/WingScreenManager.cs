using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DG.Tweening;
using System;

public class WingScreenManager : MonoBehaviour
{
    public static WingScreenManager Instance { get; private set; }
    private Stack<WingScreenBase> mScreenStack;
    private List<WingScreenBase> mScreenList;
    private Transform mParent;
    private bool mTransporting = false;
    public UICamera uiCamera;
    public CameraFader mCameraFader;
    public static EWingScreen FirstScreen;
    public EWingScreen CurrenScreen = EWingScreen.none;

    void Awake()
    {
        Instance = this;
        mParent = this.gameObject.transform;
        mScreenStack = new Stack<WingScreenBase>();
        mScreenList = new List<WingScreenBase>();
    }
    // Use this for initialization
    void Start()
    {
        //NetConstant.setWebServer();
        //uiCamera.eventReceiverMask = 1 << (int)EWingLayer.UI;

        FirstScreen = EWingScreen.home;
        RunScreen(FirstScreen);
        CameraFader.FadeOutDone += FadeOutDone;
    }

    void OnDestroy()
    {
        CameraFader.FadeOutDone -= FadeOutDone;
        for (int i = 0; i < mScreenList.Count; i++)
        {
            WingScreenBase screen = mScreenList[i];
            if (screen.gameObject != null)
            {
                screen.Destroy();
            } 
        }
    }

    public Stack<WingScreenBase> ScreenStatck
    {
        get { return mScreenStack; }
    }

    public bool IsTransporting
    {
        get { return mTransporting; }
    }

    public WingScreenBase GetWingScreen(EWingScreen eScreen)
    {
        foreach (WingScreenBase screen in mScreenList)
        {
            if (screen.wingScreen == eScreen)
            {
                return screen;
            }
        }
        return null;
    }

    public void PopAllScreen()
    {
        while (mScreenStack.Count > 0)
        {
            WingScreenBase screen = mScreenStack.Pop();
            screen.gameObject.SetActive(false);
            screen.Destroy();
        }
    }
    public WingScreenBase GetWingScreen(string prefabName)
    {
        foreach (WingScreenBase screen in mScreenList)
        {
            if (screen.PrefabName == prefabName)
            {
                return screen;
            }
        }
        return null;
    }

    public void RunScreen(EWingScreen screen, object param = null,string transitionName = null)
    {
        RunScreen(GetPrefabNameByEWingScreen(screen), param,transitionName);
    }
    public void RunScreen(string prefabName,object param = null,string transitionName = null)
    {
        if (string.IsNullOrEmpty(prefabName))
        {
            Debug.LogError(prefabName + " IsNullOrEmpt");
            return;
        }
        if (mScreenStack.Count != 0)
        {
            Debug.LogError("had screen is runing");
            return;
        }
        mTransporting = true;
        mCameraFader.FadeTo(1.0f,0,1.0f);
        StartScreen(prefabName,param);
    }

    public WingScreenBase GetRunningScreen()
    {
        if (mScreenStack.Count > 0)
        {
            WingScreenBase screen = mScreenStack.Peek();
            if (screen.gameObject.activeSelf)
            { 
                return screen;
            }
        }
        return null;
    }
    public WingScreenBase PushScreen(EWingScreen screen, object param = null, bool inactivePre = true/*是否隐藏上一个screen*/,string transitionName = null)
    {
        return PushScreen(GetPrefabNameByEWingScreen(screen),param,inactivePre, transitionName);
    }

    //run new screen,will not pop the last screen
    public WingScreenBase PushScreen(string prefabName, object param = null,bool inactivePre = true/*是否隐藏上一个screen*/,string transitionName = null)
    {
        if (mScreenStack.Count == 0)
        {
            Debug.LogError("no screen is runing,should run a screen");
            return null;
        }
        mTransporting = true;
        mCameraFader.FadeTo(1.0f, 0, 1.0f);
        if (inactivePre) {

            WingScreenBase screen = mScreenStack.Peek();
            //CameraFader.Instance.FadeOut(1.0f);
            screen.gameObject.SetActive(false);
        }
        return StartScreen(prefabName,param);
    }

    //close the last screen
    public void PopScreen(string transitionName = null)
    {
        if (mScreenStack.Count == 0)
        {
            Debug.LogError("not screen can pop");
            
        }
        else {

            mTransporting = true;
            mCameraFader.FadeTo(1.0f, 0, 1.0f);
            WingScreenBase screen = mScreenStack.Pop();
            screen.gameObject.SetActive(false);
            screen.Destroy();
            if (mScreenStack.Count > 0)
            {
                screen = mScreenStack.Peek();
                screen.gameObject.SetActive(true);
                CurrenScreen = screen.wingScreen;
                //CameraFader.Instance.FadeTo(1,0,1.0f);
            }
            else {

                //PicoUnityActivity.CallObjectMethod("exitFromPico");//栈里没有screen,退出
            }
        }
    }

    public WingScreenBase ReplaceScreen(EWingScreen screen, object param = null,string transitionName = null)
    {
        return ReplaceScreen(GetPrefabNameByEWingScreen(screen), param,transitionName);
    }

    //replace the last screen
    public WingScreenBase ReplaceScreen(string prefabName,object param = null,string transitionName = null)
    {
        if (string.IsNullOrEmpty(prefabName))
        {
            Debug.LogError("PrefabName Is NullOrEmpt");
            return null;
        }
        if (mScreenStack.Count == 0)
        {
            Debug.LogError("no screen can replace");
            return null;
        }
        mTransporting = true;
        mCameraFader.FadeTo(1.0f, 0, 1.0f);
        WingScreenBase screen = mScreenStack.Pop();
        screen.gameObject.SetActive(false);
        screen.Destroy();
        return StartScreen(prefabName,param);
    }

    private WingScreenBase StartScreen(string prefabName,object param)
    {
        if (string.IsNullOrEmpty(prefabName))
        {
            Debug.LogError(prefabName + " IsNullOrEmpt");
            return null;
        }
        WingScreenBase screen = GetWingScreen(prefabName);
        if (screen != null)
        {
            GameObject prefab = screen.gameObject;
            Vector3 position = prefab.transform.localPosition;
            Vector3 scale = prefab.transform.localScale;
            Quaternion quaternion = prefab.transform.localRotation;
            prefab.transform.parent = mParent;
            prefab.transform.localPosition = position;
            prefab.transform.localScale = scale;
            prefab.transform.localRotation = quaternion;

            screen.gameObject.SetActive(true);
            screen.Show(param);
            mScreenStack.Push(screen);
            CurrenScreen = screen.wingScreen;
            return screen;
        }
        else
        {
            screen = LoadWingScreenPrefab(prefabName);
            if (screen != null)
            {
                GameObject prefab = screen.gameObject;
                //reset position scale rotation
                Vector3 position = prefab.transform.localPosition;
                Vector3 scale = prefab.transform.localScale;
                Quaternion quaternion = prefab.transform.localRotation;
                prefab.transform.parent = mParent;
                prefab.transform.localPosition = position;
                prefab.transform.localScale = scale;
                prefab.transform.localRotation = quaternion;

                screen.gameObject.SetActive(true);
                screen.Show(param);
                mScreenStack.Push(screen);
                mScreenList.Add(screen);
                CurrenScreen = screen.wingScreen;
                return screen;
            }
        }
        return null;
    }

    private WingScreenBase LoadWingScreenPrefab(string prefabName)
    {
        if(string.IsNullOrEmpty(prefabName))
        {
            Debug.LogError(prefabName + " IsNullOrEmpt");
            return null;
        }
        string path = prefabName;
        GameObject prefab = (GameObject)Resources.Load(path);
        if (prefab == null)
        {
            Debug.LogError(prefabName + " not exist");
            throw new System.ArgumentException("no prefab found at path " + prefabName);
        }
        GameObject prefabObj = (GameObject)GameObject.Instantiate(prefab);
        WingScreenBase screen = prefabObj.GetComponent<WingScreenBase>();
        screen.Init();
        if (screen == null)
        {
            Debug.LogWarning("no use WingScreenBase or implment");
            screen = prefabObj.AddComponent<WingScreenBase>();
        }
        screen.PrefabName = prefabName;
        return screen;
    }

    private string GetPrefabNameByEWingScreen(EWingScreen screen)
    {
        string prefabName = string.Empty;
        switch (screen)
        {
            case EWingScreen.home:
                {
                    prefabName = WingScreenNames.HOME;
                    break;
                }
            case EWingScreen.setting:
                {
                    prefabName = WingScreenNames.SETTING;
                    break;
                }
            case EWingScreen.toast:
                {
                    prefabName = WingScreenNames.TOAST;
                    break;
                }
        }
        return prefabName;
    }

    private void FadeOutDone()
    {
        mTransporting = false;
    }
}

public class WingScreenNames
{
    public const string HOME = "Screens/HomeScreen";
    public const string SETTING = "Screens/SettingScreen";
    public const string TOAST = "Screens/ToastScreen";
}

public enum EWingScreen
{
    none,
    home,
    setting,
    toast
}

public enum EWingLayer
{
    UI = 5,
}