using UnityEngine;
using System.Collections;

public class SettingNormal : MonoBehaviour, IMsgHandle
{
    [SerializeField]
    UILabel mVersion;
    [SerializeField]
    GameObject mUpdateButton;
    [SerializeField]
    GameObject mClearButton;
    [SerializeField]
    UILabel mClearText;
    [SerializeField]
    GameObject noUpdate;
    [SerializeField]
    GameObject toUpdate;
    [SerializeField]
    GameObject clearToast;

    string cacheSize = "0.0Byte";

    // Use this for initialization
    void Start () {
        MsgManager.Instance.RegistMsg(MsgID.SettingNormalFreshLabel, this);
#if UNITY_EDITOR
        mVersion.text = Localization.Get("Version") + "0.01";
#elif UNITY_ANDROID
        string version = "1.0";
        PicoUnityActivity.CallObjectMethod<string>(ref version, "getVersionName");
        mVersion.text = Localization.Get("Version") + version;
        Debug.Log("version:" + version);
#endif

#if UNITY_EDITOR
        mUpdateButton.SetActive(false);
        noUpdate.SetActive(true);
#elif UNITY_ANDROID
        //判断是否有更新
        int versionCode = 0;
        PicoUnityActivity.CallObjectMethod<int>(ref versionCode, "getVersionCode");
        Debug.Log(SettingScreen.versionCode +" || "+ versionCode);
        if(int.Parse(SettingScreen.versionCode) > versionCode) {
            mUpdateButton.SetActive(true);

            if (mUpdateButton != null)
            {
                UIEventListener.Get(mUpdateButton).onClick = OnButtonClick;
            }
            else
                Debug.Log("mUpdateButton is not found !!!");

            noUpdate.SetActive(false);
            toUpdate.SetActive(true);
            toUpdate.transform.Find("Content").GetComponent<UITextList>().Add(SettingScreen.releaseNote);
            if (toUpdate.transform.Find("Scrollbar").GetComponent<UIScrollBar>().barSize == 1)
                toUpdate.transform.Find("Scrollbar").gameObject.SetActive(false);
        }
        else{
            mUpdateButton.SetActive(false);
            noUpdate.SetActive(true);
            toUpdate.SetActive(false);
        }
#endif
        if (mClearButton != null)
            UIEventListener.Get(mClearButton).onClick = OnButtonClick;
        else
            Debug.Log("mClearButton is not found !!!");

        UpdateCacheSize();
    }

    // Update is called once per frame
    void Update () {
	
	}

    void OnButtonClick(GameObject obj)
    {
        Debug.Log("OnButtonClick : " + obj.name);
        if (obj == mUpdateButton)
        {
            MsgManager.Instance.SendMsg(MsgID.SettingUpdating,null);
            GalleryTools.ShowLeftBar(false);
        }
        if (obj == mClearButton)
        {
            if (clearToast.activeInHierarchy)
            {
                //do nothing
            }
            else
            {
                if(cacheSize.Equals("0.0Byte"))
                {
                    clearToast.SetActive(true);
                    Invoke("hideClearToast",1.5f);
                }
                else
                { 
                    //调用jar里的函数清空缓存
                    ClearCache();
                    //UpdateCacheSize();
                }
            }

        }
    }

    void hideClearToast()
    {
        clearToast.SetActive(false);
    }
    void UpdateCacheSize()
    {
        if (mClearText != null)
        {
#if UNITY_EDITOR
#elif UNITY_ANDROID
        cacheSize = GalleryActivity.Instance.GetCacheSize_Activity();
#endif
            mClearText.text = Localization.Get("Player_Memory_Clear") + cacheSize;
        }
    }

    void ClearCache()
    {
#if UNITY_ANDROID
        GalleryActivity.Instance.ClearCache_Activity();
#endif
    }

    public void HandleMessage(MsgID id, Bundle bundle)
    {
        if (id == MsgID.SettingNormalFreshLabel)
            UpdateCacheSize();
    }

    public void OnCacheCleared(string str)
    {
        Debug.Log("SettingNormal - OnCacheCleared - str");
        UpdateCacheSize();
        if (this.gameObject.activeInHierarchy)
        {
            clearToast.SetActive(true);
            Invoke("hideClearToast", 1.5f);
        }
    }
}
