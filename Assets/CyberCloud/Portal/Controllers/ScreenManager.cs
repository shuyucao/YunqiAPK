using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ScreenBase相互之间都是互斥的，同时只能“存在”一个
public class ScreenManager : Singleton<ScreenManager>
{
    private Transform mUIRoot = null;
    private Stack<ScreenBase> mScreenStack = new Stack<ScreenBase>();                                    //面板栈
    /// <summary>
    /// 已创建的面板字典
    /// </summary>
    private Dictionary<UIScreen, ScreenBase> mScreenDict = new Dictionary<UIScreen, ScreenBase>();       //
    private Dictionary<UIScreen, ScreenBase> mScreenPathDict = new Dictionary<UIScreen, ScreenBase>();   //剔除路径中已存在的面板，暂时无此需求，后期有再加

    public void ChangeScreen(UIScreen screen,Bundle bundle = null)
    {
        //后加载homepage组件会卡顿一下所以此处解决办法为改为预加载
        if (UIScreen.Home == screen)
            return;
        ScreenBase sc = null;
        //尝试获取数组中与key关联的value
        mScreenDict.TryGetValue(screen, out sc);
        //未找到需要创建的面板
        if (sc == null)
        {
            sc = CreateScrren(screen);
            if (sc != null)
            {
                if (mScreenDict.ContainsKey(screen))
                {
                    mScreenDict.Remove(screen);
                }
                mScreenDict.Add(screen, sc);
            }
            else
            {
                Debug.LogError("cannot find the screen :" + screen.ToString());
                return;
            }
        }
        if (mScreenStack.Count > 0)
        {
            if (mScreenStack.Peek() == sc)
            {
                //if (!UnityTools.IsActive(sc.transform))
                //{
                //    UnityTools.SetActive(sc.transform, true);
                //    sc.OprateChangeScreen(bundle);
                //}
                if (!sc.gameObject.activeInHierarchy)
                {
                    sc.gameObject.SetActive(true);
                    UnityTools.SetActive(sc.transform, true);
                    sc.OprateChangeScreen(bundle);
                    setLeftBar(screen);
                }
            }
            else
            {
                ScreenBase old = mScreenStack.Peek();
                if (old != null)
                {
                    old.OprateCloseScreen();
                    //UnityTools.SetActive(old.transform, false);
                    old.gameObject.SetActive(false);
                }
                else
                {
                    mScreenStack.Pop();
                }

                //UnityTools.SetActive(sc.transform, true);
                sc.gameObject.SetActive(true);
                mScreenStack.Push(sc);
                sc.OprateChangeScreen(bundle);
                setLeftBar(screen);
            }
        }
        else
        {
            //UnityTools.SetActive(sc.transform, true);
            sc.gameObject.SetActive(true);
            mScreenStack.Push(sc);
            sc.OprateChangeScreen(bundle);
            setLeftBar(screen);
        }

    }

    private void setLeftBar(UIScreen screen)
    {
        if (screen != UIScreen.Player)
        {
            Bundle b = new Bundle();
            b.SetValue("CurrentScreen", screen);
            MsgManager.Instance.SendMsg(MsgID.LeftMenuChange, b);
        }
        else
            GalleryTools.ShowLeftBar(false);
    }
    public void CloseScreen(UIScreen screen)
    {
        ScreenBase sc = null;
        if (mScreenDict.TryGetValue(screen, out sc) && mScreenStack.Count > 0 && mScreenStack.Peek() == sc)
        {
            if (sc != null)
            {
                //UnityTools.SetActive(sc.transform, false);
                sc.OprateCloseScreen();
                sc.gameObject.SetActive(false);
            }
            mScreenStack.Pop();
            if (mScreenStack.Count > 0)
            {
                ChangeScreen(mScreenStack.Peek().ScreenType);
            }
            else
            {
                ChangeScreen(UIScreen.Home);
            }
        }
        else
        {
            Debug.Log("Cannot close the screen:" + screen.ToString());
        }
    }

    public void CloseScreen(ScreenBase screen)
    {
        CloseScreen(screen.ScreenType);
    }

    private string GetScreenPath(UIScreen screen)
    {
        string path;
        switch (screen)
        {
            case UIScreen.Home:
                path = UIPaths.Home;
                break;
            case UIScreen.Player:
                path = UIPaths.Player;
                break;
            case UIScreen.Special:
                path = UIPaths.Special;
                break;
            case UIScreen.Setting:
                path = UIPaths.Setting;
                break;
            case UIScreen.Toast:
                path = UIPaths.Toast;
                break;
            case UIScreen.DimensionDoor:
                path = UIPaths.DimensionDoorScreen;
                break;
            case UIScreen.Local:
                path = UIPaths.Local;
                break;
            case UIScreen.None:
            default:
                path = string.Empty;
                break;
        }
        return path;
    }

    private ScreenBase CreateScrren(UIScreen screen)
    {
        ScreenBase sc = null;
        if (mUIRoot == null)
        {
            mUIRoot = Transform.FindObjectOfType<UIRoot>().transform;
        }
        string path = GetScreenPath(screen);
        if (!string.IsNullOrEmpty(path))
        {
            //向uiroot中添加homepage组件
            sc = UnityTools.CreateComptent<ScreenBase>(Resources.Load(path) as GameObject, mUIRoot);
            UnityTools.ResetTran(sc.transform, Constant.ScreenOriginPos);
            //UnityTools.ResetTran(sc.transform, Vector3.zero);
        }
        return sc;
    }

    class UIPaths
    {
        public const string Home = "Screens/HomePage";
        public const string Player = "Screens/GalleryPlayer";
        public const string Special = "Screens/SpecialScreen";
        public const string Setting = "Screens/SettingScreenUpdate";
        public const string Toast = "Screens/ToastScreen";
        public const string DimensionDoorScreen = "Screens/DimensionDoorScreen";
        public const string Local = "Screens/LocalScreen";
        public const string tableRow = "Screens/tableRow";
    }
}

public enum UIScreen
{
    None,
    Home,
    Player,
    Setting,
    DimensionDoor,
    Local,
    Toast,
    Special
}