

using UnityEngine;
using System.Collections;
using BestHTTP;
using LitJson;


public class Main : MonoBehaviour, IMsgHandle
{
    //public static string StartType = "player";
    //public static string StartValue = "121,sdssdf,http://photo.picovr.com/images/thumbs/b3/25/649a73f3e973ec3d972b58736c8be40ee2ea34c0.jpg,http://photo.picovr.com/images/photos/5f/46/1d147062f3aca4f4ac82ab3bea7debbdd5b34477.jpg";
    public static string StartType = string.Empty;
    public static string StartValue = string.Empty;
    [SerializeField]
    GameObject mLoadUI;
    [SerializeField]
    public static bool test = false;
    public bool test2 = false;
    void Awake()
    {
     
    
        //test

        //Application.targetFrameRate = 10;
       // Debug.LogError("ucvr Application.targetFrameRate:"+ Application.targetFrameRate +";if -1 it is mean no limit framerate");
        if (CyberCloudConfig.ExportOnlyPlayer)
        {
            this.gameObject.SetActive(false);
            return;
        }
        if (mLoadUI == null)
            MyTools.PrintDebugLogError("param had null mLoadUI");
        //Debug.Log("Awake1");
        ExceptionEventHandler.Init();
        //Debug.Log("Awake2");
        //PicoInputManager.Init();
        //Debug.Log("Awake3");
        //获取界面数据
     
 
        MsgManager.Instance.RegistMsg(MsgID.CheckUpdateInfo, this);
    }
    void Update()
    {
        test = test2;
        /**
        if (test)
        {
            bool b= ControllerTool.getControllerBtUp(CyberCloud_UnitySDKAPI.ControllerKeyCode.APP);
           
        
            bool r = ControllerTool.getControllerBtUpDirection(CyberCloud_UnitySDKAPI.ControllerKeyCodePrivate.TOUCHPADRIGHT);
            bool d = ControllerTool.getControllerBtUpDirection(CyberCloud_UnitySDKAPI.ControllerKeyCodePrivate.TOUCHPADDOWN);
            bool u = ControllerTool.getControllerBtUpDirection(CyberCloud_UnitySDKAPI.ControllerKeyCodePrivate.TOUCHPADUP);
            bool l = ControllerTool.getControllerBtUpDirection(CyberCloud_UnitySDKAPI.ControllerKeyCodePrivate.TOUCHPADLEFT);
            //if (d||u||l||r)
            MyTools.PrintDebugLogError("ucvr ControllerKeyCode.APPup=============================d" + d+";u:"+u+";l:"+l+";r:"+r);
        }
        **/
    }
    public void loadData() {

        DataLoader.Instance.Init(mLoadUI);
    }
    void Start()
    {
        //GalleryTools.ShowLeftBar(false);
        //ScreenManager.Instance.ChangeScreen(UIScreen.Home);
        /**
#if UNITY_ANDROID && !UNITY_EDITOR
         string appSignature =MyTools.getAppSignature();
      MyTools.PrintDebugLog("ucvr appSignature:"+ appSignature);
#endif
    **/
        loadData();
    }
    public void HandleMessage(MsgID id, Bundle bundle)
    {
        if (id == MsgID.CheckUpdateInfo)
        {
            if (bundle != null && bundle.Contains<int>("VersionInfo"))
            {
                int verinfo = bundle.GetValue<int>("VersionInfo");
                if (verinfo == 3)
                {
                    ScreenManager.Instance.ChangeScreen(UIScreen.Setting);
                }
                else
                {
                    this.StartUp();
                }
            }
            else
            {
                this.StartUp();
            }
        }
    }
 
    void OnDisable()
    {
       // print("script was removed");
    }

    private void StartUp()
    {
        Debug.Log("***Recommend***" + "  starttype:  " + StartType + "  startvalue:  " + StartValue);
        if (StartType.Equals("category"))
        {
            Bundle bundle = new Bundle();
            bundle.SetValue<string>("cid", StartValue);
            ScreenManager.Instance.ChangeScreen(UIScreen.Home, bundle);
        }
        else if (StartType.Equals("player"))
        {
            string[] parm = StartValue.Split(',');
            if (parm != null && parm.Length == 4)
            {
                PhotoModel data = new PhotoModel();
                data.MID = parm[0];
                data.Title = parm[1];
                data.CoverLink = parm[2];
                data.PhotoLink = parm[3];
                GPlayerManager.Instance.PlayRecommend(data);
            }
        }
        else if (StartType.Equals("special"))
        {
            Bundle bundle = new Bundle();
            bundle.SetValue<string>("tid", StartValue);
            ScreenManager.Instance.ChangeScreen(UIScreen.Special, bundle);
        }
        else
        {
            ScreenManager.Instance.ChangeScreen(UIScreen.Home);
        }
    }
}