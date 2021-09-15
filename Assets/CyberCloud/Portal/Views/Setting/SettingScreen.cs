using UnityEngine;

public class SettingScreen : ScreenBase,IMsgHandle
{
    [SerializeField]
    GameObject ForceUpdatePage;
    [SerializeField]
    GameObject UpdatingPage;
    [SerializeField]
    GameObject NormalPage;
    GameObject CurrentPage;
    public static bool isForce = false;
    public static string versionCode = "1";
    public static string versionName = null;
    public static string releaseNote = null;
    public static string url = null;
    void Start()
    {
        MsgManager.Instance.RegistMsg(MsgID.SettingNormal, this);
        MsgManager.Instance.RegistMsg(MsgID.SettingForceUpdate, this);
        MsgManager.Instance.RegistMsg(MsgID.SettingUpdating, this);
        Debug.Log("Setting screen start !!!");
    }

    //when this screen move to the top
    public override void OprateChangeScreen(Bundle bundle)
    {
        //InputManager.OnBack += OnBack;
        if (SettingScreen.isForce)
            CurrentPage = ForceUpdatePage;
        else
            CurrentPage = NormalPage;

        Debug.Log("OprateChangeScreen : " + CurrentPage.name);

        show(CurrentPage);
        if (CurrentPage == NormalPage)
            MsgManager.Instance.SendMsg(MsgID.SettingNormalFreshLabel, null);
    }

    //when this screen closed
    public override void OprateCloseScreen()
    {
        //InputManager.OnBack -= OnBack;
    }

    void OnButtonClick(GameObject obj)
    {
        Debug.Log("OnButtonClick : " + obj.name);
    }

    void OnButtonHover(GameObject obj, bool status)
    {
    }

    void OnBack()
    {
        Debug.Log("SettingScreen : OnBack " + CurrentPage);
        if (CurrentPage == UpdatingPage)
            MyTools.PrintDebugLog("ucvr this is update");
        else
        {
            Bundle b = new Bundle();
            b.SetValue("CurrentScreen", UIScreen.None);
            MsgManager.Instance.SendMsg(MsgID.LeftMenuChange, b);
            ScreenManager.Instance.CloseScreen(this);
        }
    }

    public void HandleMessage(MsgID id, Bundle bundle)
    {
        //Debug.Log("@@@@@@@@@@@@@@@@  " + id.ToString());
        if (id == MsgID.SettingNormal)
        {
            show(NormalPage);
        }
        else if (id == MsgID.SettingForceUpdate)
        {
            show(ForceUpdatePage);
        }
        else if (id == MsgID.SettingUpdating)
        {
            show(UpdatingPage);
        }
    }

    void  show(GameObject obj)
    {
        CurrentPage.SetActive(false);
        CurrentPage = obj;
        CurrentPage.SetActive(true);
    }
}