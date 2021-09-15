using UnityEngine;

public class SettingScreen_New : ScreenBase
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

    //when this screen move to the top
    public override void OprateChangeScreen(Bundle bundle)
    {
       // InputManager.OnBack += OnBack;
        UpdateCacheSize();
    }

    //when this screen closed
    public override void OprateCloseScreen()
    {
       // InputManager.OnBack -= OnBack;
    }

    void OnButtonClick(GameObject obj)
    {
        Debug.Log("OnButtonClick : " + obj.name);

        if (obj == mBackButton)
        {
            //返回Home
            OnBack();
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
        TweenScale ts = TweenScale.Begin(obj.transform.parent.gameObject, 0.6f, status ? 1.1f * Vector3.one : Vector3.one);
        ts.PlayForward();
    }

    void OnBack()
    {
        Debug.Log("SettingScreen : OnBack");
        ScreenManager.Instance.CloseScreen(this);
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