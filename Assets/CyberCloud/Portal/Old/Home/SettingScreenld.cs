using UnityEngine;
using System.Collections;

public class SettingScreenOld : WingScreenBase
{
    public GameObject mBackButton;
    public GameObject mClearSprite;
    public UILabel mClearText;
    public UILabel mVersion;

    void Start()
    {
        if (mBackButton != null)
        {
            UIEventListener.Get(mBackButton).onClick = OnButtonClick;
        }

        if (mClearSprite != null)
        {
            UIEventListener.Get(mClearSprite).onClick = OnButtonClick;
            UIEventListener.Get(mClearSprite).onHover = OnButtonHover;
        }
#if UNITY_ANDROID
        Debug.Log("getVersionName");
        string version = "1.0";
        PicoUnityActivity.CallObjectMethod<string>(ref version, "getVersionName");
        mVersion.text = "V" + version;
        Debug.Log("version:" + version);
#endif
    }

    public override void Init()
    {
        base.Init();
    }

    public override void Show(object param)
    {
        base.Show(param);
      //  InputManager.OnBack += OnBack;


        UpdateCacheSize();
    }

    public override void Destroy()
    {
        base.Destroy();
      //  InputManager.OnBack -= OnBack;
    }

    void OnButtonClick(GameObject obj)
    {
        Debug.Log("OnButtonClick : " + obj.name);

        if (obj == mBackButton)
        {
            //返回Home
        }

        if (obj == mClearSprite)
        {
            //调用jar里的函数清空缓存
            ClearCache();
            UpdateCacheSize();
        }
    }

    void OnButtonHover(GameObject obj, bool status)
    {
        if (obj == mClearSprite)
        {
            Vector3 pos = obj.transform.parent.localPosition;
            obj.transform.parent.localPosition = new Vector3(pos.x, pos.y, status ? -3 : 0);
        }
    }

    void OnBack()
    {
        Debug.Log("SettingScreen : OnBack");
        if (Visible)
        {
            WingScreenManager.Instance.PopScreen();
        }
    }

    void UpdateCacheSize()
    {
        string size = "0.0Byte";
#if UNITY_ANDROID
        size = GalleryActivity.Instance.GetCacheSize_Activity();
#endif
        if (mClearText != null)
        {
            mClearText.text = Localization.Get("Player_Memory_Clear") + size;
        }
    }

    void ClearCache()
    {
#if UNITY_ANDROID

        GalleryActivity.Instance.ClearCache_Activity();

#endif
    }
}