

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.CyberCloud.Scripts.Tools;
using Assets.CyberCloud.Scripts;
using Assets.CyberCloud.Scripts.OpenApi;
using static Assets.CyberCloud.Scripts.OpenApi.OpenApiImp;
using static XMPPTool;
#if PicoSDK
using Pvr_UnitySDKAPI;
#endif
/// <summary>
/// 视博云通用组件
/// 对外提供获取子组件的接口
/// </summary>
public class CommonPlane: MonoBehaviour {
    [SerializeField]
    /// <summary>
    /// 弱提示
    /// </summary>
    private Hint hint;
    [SerializeField]
    /// <summary>
    /// 弹框提示
    /// </summary>
    private ServerDialog serverDialog;
    [SerializeField]
    /// <summary>
    /// 弹框提示
    /// </summary>
    private MyDialog mydialog;
    [SerializeField]
    /// <summary>
    /// 提示信息
    /// </summary>

    private TipsControl tipsControl;
    [SerializeField]
    /// <summary>
    /// 无手柄时的退出提示
    /// </summary>
    private GameObject setingBt;
    [SerializeField]
    /// <summary>
    /// 设置界面
    /// </summary>
    private SetingDialog setingDialog;
    [SerializeField]
    private XMPPTool xmpptool;
    [SerializeField]
    private TestCastScreen testCastScreen;
  
    private GameAppControl gameAppControl;
	

	public static bool is1106Test=true;//1106号版本支持升级自检测和新功能投屏，测试阶段所以加此字段测试稳定后需要去掉
    // Use this for initialization
    public static string exitByAppSelf = "0x0000FF02";//应用内主动退出错误码
    
    private static string networkErrorCode = "0x00702001";
    public static string resousefull = "0x34321002";
    public static string resousefull2 = "0x34321006";
    public static string playTokenOutTime = "0x34312018";//playToken过期或无效请重新通过排队服务申请
    private Material newSkybox;
    void Awake()
    {


        GameObject gamePlane = GameObject.Find("GamePlane");
        if (gamePlane != null)
        {

            gameAppControl = gamePlane.GetComponent<GameAppControl>();
        }
     
    }
    LiveManager liveManager;
    void Start()
    {
        if (CyberCloudConfig.usePingCmd==1) {
			if (CyberCloudConfig.adapt_platform != null && CyberCloudConfig.adapt_platform != "") {
				Debug.Log("ucvr current is adapt_platform no need PingConnect ");
			} else if (CyberCloudConfig.ExportOnlyPlayer==true|| OpenApiImp.CyberCloudOpenApiEnable == true) {//快速启动模式不需要展示portal也不用ping
				Debug.Log("ucvr current is ExportOnlyPlayer no need PingConnect ");
			} else
				StartCoroutine(PingConnect());
        }
#if ISPICO
        MyTools.PrintDebugLogError("ucvr ispico");
#endif


        //StartMulticast() .SendBroadcastMessage(string message) 
        if (CyberCloudConfig.cvrScreen == CyberCloudConfig.CVRScreen.JiaoShi)
		{
			MyTools.PrintDebugLog("===ucvr init MyMulticastFinder===");
            liveManager = new LiveManager();
            liveManager.startConnect(gameAppControl);
           
			//finder.SendBroadcastMessage("ClientVideoStreaming|我是SN|10.10.6.70:8001:123456:[无用数据]");
		}
		newSkybox = RenderSettings.skybox;
        //u+ app不需要天空盒子
        if (CyberCloudConfig.cvrScreen == CyberCloudConfig.CVRScreen.LGUJa)
        {
            newSkybox = null;
            changeSkyBox(false);
        }
        else if (newSkybox == null) {
            MyTools.PrintDebugLogError("ucvr newSkybox is null" );
        }
      
        //初始化时隐藏手柄显示凝视点 pico内部代码已处理,如果提前关掉会导致手柄无法检测
        if (CyberCloudConfig.currentType != CyberCloudConfig.DeviceTypes.Pico&& CyberCloudConfig.currentType != CyberCloudConfig.DeviceTypes.PicoNeo2)
        {
            connectStatusChange();
        }
        updateControllerConnected();
        //mydialog.OnButtonClickCancel(null);
        //CyberCloud_UnitySDKAPI.HeadBox.changeRenderdHeadType(CyberCloud_UnitySDKAPI.HeadBoxDofType.Dof3);
   

        //CyberCloud_UnitySDKAPI.HeadBox.changeRenderdHeadType(CyberCloud_UnitySDKAPI.HeadBoxDofType.Dof6);
        //showSetingDialog();
    }
    public void sendMessageToListennerTeacher(string deviceID, string message, bool start) {
        liveManager.sendMessageToListennerTeacher(deviceID,message,start);
    }
    /// <summary>
    /// 根据投屏地址获取直播地址
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public string getLiveStreamUrl(string message) {
       // string str = "10.10.6.67:8001:973646265883563225:[1.213, 1.213, 1.213, 1.213]:[1.213, 1.213, 1.213, 1.213]:2";
        if (message != null && message.Length > 0)
        {
            string[] strs = message.Split(':');
            if (strs != null && strs.Length > 2)
                message = "rtmp://" + strs[0] + ":15004/live/" + strs[2];
        }
        MyTools.PrintDebugLog("ucvr getLiveStreamUrl:" + message);
        //     rtmp://ip:15004/live/123456
        return message;
    }
    public string getSessionIDByCastScreenStreamUrl(string message) {
        if (message != null && message.Length > 0)
        {
            string[] strs = message.Split(':');
            if (strs != null && strs.Length > 2)
                return strs[2]; 
        }
        return "";
    }
    /// <summary>
    /// 根据投屏地址获取投屏指令
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public string getCastScreenCmd(string message, ref string ip ,ref int port, ref string session)
    {
        // string str = "10.10.6.67:8001:973646265883563225:[1.213, 1.213, 1.213, 1.213]:[1.213, 1.213, 1.213, 1.213]:2";
        //10.10.6.70:8001:979414624449332873:[1.213, 1.213, 1.213, 1.213]:[1.213, 1.213, 1.213, 1.213]:2
        //MyTools.PrintDebugLog("ucvr getCastScreenCmdplayurl:" + message);
        if (message != null && message.Length > 0)
        {
            string[] strs = message.Split(':');
            if (strs != null && strs.Length > 2) {
                ip = strs[0];
                port = int.Parse(strs[1])+6200;//14201. 6200=14201-8001
                session = strs[2];          
                ip = strs[0];
                message =  strs[0] + ":" + port + ":" + session;
                if (strs.Length > 3) {
                    for (int i = 3; i < strs.Length; i++) {
                        message = message + ":" + strs[i];
                    }
                }
            }
        }
        MyTools.PrintDebugLog("ucvr getLiveStreamUrl=" + message);
        //     rtmp://ip:15004/live/123456
        return message;
    }
    bool checkNetWorkEnable = true;
    bool isTest = false;
    // Update is called once per frame
    bool initContolerType = false;
    /// <summary>
    /// 更新手柄连接状态
    /// </summary>
    /// <returns></returns>
    private List<int> updateControllerConnected() {
        handlerList = CyberCloud_UnitySDKAPI.ControllerManager.getControllerConnected();
        //Debug.LogError(" ucvr getRaycastHit true========================================handlerList:"+ handlerList.Count+"");
        if (handlerList != null && currentConnectContrlor == false) {
            currentConnectContrlor = true;
     
                connectStatusChange();
       
        }
        else if(handlerList == null&& currentConnectContrlor==true){
            currentConnectContrlor = false;
   
                connectStatusChange();
        
        }
  
        return handlerList;
    }
    public void changeSkyBox(bool addSkyBox) {
        if (addSkyBox)
            RenderSettings.skybox = newSkybox;
        else
            RenderSettings.skybox = null;
       
    }
    /// <summary>
    /// 检测手柄连接状态，不在游戏中时如果手柄有效需要开启手柄显示，手柄无效时开启凝视点
    /// </summary>
    private void connectStatusChange() {
        MyTools.PrintDebugLog("ucvr currentConnectContrlor:"+ currentConnectContrlor);
        if (!GameAppControl.getGameRuning())
        {
        	 
             ControllerTool.CloseOrOpenAnyEnableContrlorCursor(CyberCloud_UnitySDKAPI.CursorStatus.openCursor);
        }
    }
    private bool currentConnectContrlor=false;
    public static List<int> handlerList = null;
    private float networkErrorTime = 0;//网络异常时间

    //portal中确认键按下的时间
    private float keyEnterDownStartTime = 0;
    private int frameNum = 0;
    void Update()
    {
     
        //老版本的aar 位置高度只按照头盔启动时的高度为0，berton修改后的版本以apk进入时头的高度为0
        if (CyberCloudConfig.currentType == CyberCloudConfig.DeviceTypes.PicoNeo2&& frameNum<9)//调用一次unity位置有时不对需要调用多次unitysdk返回的高度才对不知道为啥(从数据看前4次都没有获取到姿态不确定和这个是否有关系)
        {
            frameNum++;
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass devive = new AndroidJavaClass(MyTools.picoDeviceJarName);
        devive.CallStatic("setTrackingOrigin", 0);//0:eyelevel,1:floorlevel
#endif
        }

        if (!GameAppControl.getGameRuning())
        {
            updateSesion();
            if (CyberCloud_UnitySDKAPI.HeadBox.getKeyDown(CyberCloud_UnitySDKAPI.ControllerKeyCode.HmdOK))
            {
                keyEnterDownStartTime = Time.time;
            }
            else if (CyberCloud_UnitySDKAPI.HeadBox.getKeyUp(CyberCloud_UnitySDKAPI.ControllerKeyCode.HmdOK))
            {
                float t = Time.time - keyEnterDownStartTime;
                if (t > 8)
                {
                    MyTools.PrintDebugLog("ucvr load ConfigPage");
                    Application.LoadLevel("ConfigPage");
                }
                else
                {
                    MyTools.PrintDebugLog("ucvr load ConfigPage t:" + t);                   
                }
                keyEnterDownStartTime = Time.time;
            }
        }else
            keyEnterDownStartTime = Time.time;
        updateControllerConnected();
        if (!initContolerType)
        {
            if (CyberCloudConfig.currentType == CyberCloudConfig.DeviceTypes.DaPeng)
            {
              
                ControllerTool.Reflection("changeRenderdHeadType", new object[] { CyberCloud_UnitySDKAPI.HeadBoxDofType.Dof3 });
                initContolerType = true;
            }
            else {
                //十秒内检测手柄状态,如果有手柄连接根据手柄类型显示对应的模型
                if (Time.time < 10 && handlerList != null)
                {

                    CyberCloud_UnitySDKAPI.HeadBoxDofType boxType = CyberCloud_UnitySDKAPI.ControllerManager.getControlerDofType();
                    ///3dof pico头盔需要在此处切换否则手柄会显示在头盔上面
                    if ( boxType == CyberCloud_UnitySDKAPI.HeadBoxDofType.Dof3)
                        ControllerTool.Reflection("changeRenderdHandType", new object[] { CyberCloud_UnitySDKAPI.HeadBoxDofType.Dof3 }, "CyberCloud_UnitySDKAPI.ControllerManager");
                    else
                        MyTools.PrintDebugLog("ucvr type:" + CyberCloudConfig.currentType + "dof:" + boxType);
                    initContolerType = true;
                }

            }
          
        }
        networkCheck();
      /****/
        //统计帧率 50fps
        {
            FrameNum++;
            TimeNow = Time.time;
            if (TimeNow - TimeLast > 5)
            {
                MyTools.PrintDebugLog("ucvr fps " + FrameNum/5);
                FrameNum = 0;
                TimeLast = TimeNow;                
            }
        }
    }
    private void updateSesion()
    {//在portal中并且初始化成功后调用更新session
        int timeM = (int)(Time.time * 1000);

        if (DataLoader.Instance.portalIintRet)
        {
            DataLoader.Instance.portalAPI.updateSession(timeM);
        }
    }
    int FrameNum;
    float TimeNow;
    float TimeLast;
   
    private void networkCheck() {
        //游戏中时以游戏状态为准不用再判断系统wifi状态（因系统wifi状态不及时 20秒左右才有状态更新 很影响效果）
        if (GameAppControl.getGameRuning())
        {//如果游戏中 网络中断超过2分钟后恢复 弹出错题提示框提示用户退出，2分钟内网络能恢复就不再弹退出提示框

            //判断流化系统网络状态是否异常
            if (GameAppControl.isShowGameingNetworkWrongError)
            {
                if (checkNetWorkEnable == true)
                {
                    MyTools.PrintDebugLogError("ucvr isShowGameingNetworkWrongError wait time:"+Time.time);
                    networkErrorTime = networkErrorTime + Time.deltaTime;
                    if (networkErrorTime > 30)
                    {//如果游戏内网络中断后30秒给出错误提示框（）
                      
                        closeHintMsg();
                      
                        gameAppControl.exitCyberGame();
                        showDialogWifiException("network_error_exit", networkErrorCode);
                        checkNetWorkEnable = false;
                    }
                }
            }
            else
            {
                networkErrorTime = 0;
                if (checkNetWorkEnable == false)
                {
                    MyTools.PrintDebugLogError("ucvr GameingNetworkResume");
                    checkNetWorkEnable = true;
              
                    if (DialogBase.isShow && mydialog != null)
                    {
                        string desc=mydialog.getDesc();
                        //判断当前提示框是否是网络异常提示框
                        if(desc.IndexOf(networkErrorCode)>-1)
                            mydialog.closeByOther();
                    }
                }
            }

        }
        else
        {
            if (CyberCloudConfig.usePingCmd == 1)
            {
                if (internetReachabilityToTls == false)
                {
                    if (checkNetWorkEnable)
                    {

                        MyTools.PrintDebugLogError("ucvr netwok error");
                        closeHintMsg();
                        showDialogWifiException("network_error_exit", networkErrorCode);
                        checkNetWorkEnable = false;
                    }
                }
                else {
                    networkErrorTime = 0;
                    if (checkNetWorkEnable == false)
                    {
                        //如果初始化完成但是初始化失败再次连接上网时需要重新进行初始
                        //此恢复只是判断wifi是否有信号，不代表和服务器的网络是通的
                        if (DataLoader.Instance.initStatus != DataLoader.InitStatus.initing && DataLoader.Instance.portalIintRet == false)
                        {
                            Debug.LogError("ucvr wifi network recovery ");

                          DataLoader.Instance.reInit();
                        }
                        checkNetWorkEnable = true;
                        Debug.Log("ucvr wifi network recovery initStatus:" + DataLoader.Instance.initStatus+ ";portalIintRet:" + DataLoader.Instance.portalIintRet); 
                       if (DialogBase.isShow && mydialog != null)
                       {
                           mydialog.closeByOther();

                       }
                    }
                }
                return;
            }
            //checkNetworkInPortal();
           // if (true)
           //     return;
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                if (checkNetWorkEnable)
                {

                    MyTools.PrintDebugLogError("ucvr netwok error");

                    closeHintMsg();
                    showDialogWifiException("network_error_exit", networkErrorCode);
                    checkNetWorkEnable = false;
                }

            }
            else
            {
                networkErrorTime = 0;
                if (checkNetWorkEnable == false)
                {
                    //如果初始化完成但是初始化失败再次连接上网时需要重新进行初始
                    //此恢复只是判断wifi是否有信号，不代表和服务器的网络是通的
                    if (DataLoader.Instance.initStatus != DataLoader.InitStatus.initing && DataLoader.Instance.portalIintRet == false)
                    {
                        DataLoader.Instance.reInit();
                    }
                    //此恢复只是判断wifi是否有信号，不代表和服务器的网络是通的所以不能关闭网络异常提示
                   
                    checkNetWorkEnable = true;
                    Debug.Log("ucvr wifi network recovery");
                    /**
                   if (DialogBase.isShow && mydialog != null)
                   {
                       mydialog.closeByOther();

                   }
                   **/
                }
            }
        }
    }
    //private bool usePingCmd = true;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="isDone"></param>
    /// <param name="time">ping 的时间长度单位是毫秒</param>
    private void checkNetworkInPortal(bool isDone, int time,string severIp) {
        //

        if (GameAppControl.getGameRuning())
        {
            MyTools.PrintDebugLog("ucvr checkNetworkInPortalgetGameRuning isDone:"+ isDone+";time" + time);
            return;
        }
        //wifi不通时会返回-1
        if (time <1000&&time>-1)
        {
            MyTools.PrintDebugLog("ucvr =================================network Reachable time："+ time+ "ms;isDone:"+ isDone);
            if (internetReachabilityToTls == false) {//网络恢复
                internetReachabilityToTls = true;
            }
        }
        else {
            internetReachabilityToTls = false;
            Debug.LogError("ucvr =================================network error time："+ time+ "ms;isDone:"+ isDone);
        }
    }
    private bool internetReachabilityToTls = true;//和tls网络连通情况
    IEnumerator PingConnect()
    {
        if (CyberCloudConfig.ExportOnlyPlayer)//快速启动模式不需要检测业务接口是否通
            yield return new WaitForSeconds(2f);
        string severIp = CyberCloudConfig.tls;
        if (severIp == null || severIp == "" || severIp.IndexOf("http") < 0)
        {
            Debug.LogError("ucvr CyberCloudConfig.tls is : " + severIp);
            yield return new WaitForSeconds(2f); ;
        }
        else {
            string[] strarr= CyberCloudConfig.tls.Split(':');
            severIp = strarr[1].Substring(2);
        }

        // Ping网站
      
        int addTime = 0;
        int waittime = 1;//0.1秒请求一次
        int outTime = (int)(waittime * 3000); 

        while (true)
        {
         
            Ping ping = new Ping(severIp);
            yield return new WaitForSeconds(waittime);
            // 等待请求返回
            while (!ping.isDone)
            {
                yield return new WaitForSeconds(waittime);
                addTime = addTime + (waittime * 1000);
                // 链接失败
                if (addTime >= outTime)
                {
                    checkNetworkInPortal(false, addTime, severIp);
                    addTime = 0;                   
                    break;
                }            
            }
            // 链接成功
            if (ping.isDone)
            {
                checkNetworkInPortal(true, ping.time, severIp);              
            }      
        }
    }
    private bool oneBtExitDialog=false;
    /// <summary>
    /// 显示一个按钮的退出确认框
    /// </summary>
    /// <param name="msg"></param>
    public void showDialogOneBtExitDialog(string key, string errorcode)
    {
        if (!OpenApiImp.CyberCloudOpenApiEnable)
        {
            oneBtExitDialog = true;
            mydialog.closeByOther();
            showDialogOneBt(key, errorcode);
        }
    }
    public void closeAllDialog()
    {
        if (mydialog)
            mydialog.closeByOther();
        if (setingDialog)
            setingDialog.closeByOther();
    }
    /// <summary>
    /// 显示一个按钮的dialog
    /// </summary>
    /// <param name="msg"></param>
    private void showDialogOneBt(string key,string errorcode)
    {
        string msg = Localization.Get(key);
        if (errorcode != null)
            msg += "(" + errorcode + ")";

        mydialog.gameObject.SetActive(true);
        mydialog.ClickCallBackDelegate = dialogClickCallBack;
        mydialog.changeDialogText(MyDialog.BtType.oneBt,msg);
    }
    /// <summary>
    /// 弹出分前端服务器选择窗口
    /// </summary>
    /// <param name="clickfun"></param>
    public void showServerSwitchDialog(ServerDialog.ClickDelegate clickfun)
    {
        serverDialog.gameObject.SetActive(true);
        serverDialog.clickDelegate = delegate (string obj)
        {

            clickfun(obj);
            serverDialog.gameObject.SetActive(false);
        };
    }
    public void showupdateapkdialog(string key, string errorcode, MyDialog.ClickDelegate updateDialogClickCallBack)
    {
        string msg = Localization.Get(key);
  

        mydialog.gameObject.SetActive(true);
        mydialog.ClickCallBackDelegate = updateDialogClickCallBack;
        mydialog.changeDialogText(MyDialog.BtType.oneBt, msg);
    }
    /// <summary>
    /// 显示投屏校验码弹框
    /// </summary>
    /// <param name="key"></param>
    /// <param name="checkCode"></param>
    /// <param name="updateDialogClickCallBack"></param>
    public void showCheckCodeDialog(string key, string checkCode, MyDialog.ClickDelegate updateDialogClickCallBack)
    {
        string msg = Localization.Get(key);
        msg = msg.Replace("checkCode", checkCode);
        if (OpenApiImp.CyberCloudOpenApiEnable)
        {
            MyTools.PrintDebugLog("ucvr showCheckCodeDialog checkCode:" + checkCode);
            OpenApiImp.getOpenApi().notify().castScreenCallback(CastScreen.bindSTB, checkCode);
        }
        else
        {
            mydialog.gameObject.SetActive(true);
            mydialog.ClickCallBackDelegate = updateDialogClickCallBack;
            mydialog.changeDialogText(MyDialog.BtType.twoBt, msg);
        }
    }
    public void showDialogWifiException(string key, string errorcode)
    {
      
        //unity的wifi检测高于游戏内的wifi网络检测所以一旦unity检测出wifi异常弹出对话框
        if (checkNetWorkEnable == false)
            return;
        if ("network_error_exit"== key&& OpenApiImp.CyberCloudOpenApiEnable&& OpenApiImp.getOpenApi()!=null) {
            if (OpenApiImp.getOpenApi().notify() != null)
                OpenApiImp.getOpenApi().notify().systemStatusCallback(OpenApiImp.systemException, ErrorCodeOpenApi.castScreenFailed.ToString());
            return;
        }
        string msg = Localization.Get(key);
        if (errorcode != null)
            msg += "(" + errorcode + ")";
        mydialog.gameObject.SetActive(true);
        mydialog.ClickCallBackDelegate += showDialogWifiExceptionCallBack;
        mydialog.changeDialogText(MyDialog.BtType.oneBt, msg);
    }
    /// <summary>
    /// 显示两个按钮退出二次确认
    /// </summary>
    public void showExitDialog(string key, string errorcode)
    {
        string msg = Localization.Get(key);
        if (errorcode != null)
            msg += "(" + errorcode + ")";
        mydialog.gameObject.SetActive(true);
        mydialog.ClickCallBackDelegate += dialogClickCallBack;
        mydialog.changeDialogText(MyDialog.BtType.twoBt, msg);
    }
    //比如按home键回到launcher如果在流化中需要退出流化
    void OnDisable()
    {
        MyTools.PrintDebugLog("ucvr cybercloud window OnDisable");
        //退出游戏后需要打开凝视或手柄操控点
        ControllerTool.CloseOrOpenAnyEnableContrlorCursor(CyberCloud_UnitySDKAPI.CursorStatus.openCursor);
        if (GameAppControl.getGameRuning()) {
            MyTools.PrintDebugLog("ucvr exitCyberCloud OnDisable");
            gameAppControl.exitCyberGame();
        }
    }

    public MyDialog getDialog() {
        return mydialog;
    }
    /// <summary>
    /// wifi连接异常
    /// </summary>
    /// <param name="buttonIndex"></param>
    private void showDialogWifiExceptionCallBack(MyDialog.ButtonIndex buttonIndex) {
        if (GameAppControl.getGameRuning())
            gameAppControl.exitCyberGame();
        #if PicoSDK
        Controller.UPvr_IsEnbleHomeKey(true);
#endif
        Application.Quit();
    }
    private void dialogClickCallBack(MyDialog.ButtonIndex buttonIndex) {
        if (buttonIndex == MyDialog.ButtonIndex.bt_ok)
        {
            MyTools.PrintDebugLog("ucvr click exit");
            if (GameAppControl.getGameRuning())            
                gameAppControl.exitCyberGame();
            else//退出游戏后需要打开凝视或手柄操控点
                ControllerTool.CloseOrOpenAnyEnableContrlorCursor(CyberCloud_UnitySDKAPI.CursorStatus.openCursor);
            if (CyberCloudConfig.ExportOnlyPlayer == true)
            {
                 #if PicoSDK
                Controller.UPvr_IsEnbleHomeKey(true);
#endif
                Application.Quit();
            }
        }
        else if (buttonIndex == MyDialog.ButtonIndex.bt_center) {
            MyTools.PrintDebugLog("ucvr click exit bt_center");
            if (oneBtExitDialog) {


                if (GameAppControl.getGameRuning())
                    gameAppControl.exitCyberGame();
                else
                {
                 
                        //退出游戏后需要打开凝视或手柄操控点
                        ControllerTool.CloseOrOpenAnyEnableContrlorCursor(CyberCloud_UnitySDKAPI.CursorStatus.openCursor);
                }
            }
            if (CyberCloudConfig.ExportOnlyPlayer == true)
            {
#if PicoSDK
                Controller.UPvr_IsEnbleHomeKey(true);
#endif
                Application.Quit();
            }
        }
        else
        {
            if (GameAppControl.getGameRuning())
            {  //关闭弹框返回游戏
                ControllerTool.CloseOrOpenAnyEnableContrlorCursor(CyberCloud_UnitySDKAPI.CursorStatus.closeCursor);
            }
        }
        oneBtExitDialog = false;
    }
    /// <summary>
    /// 显示4个菜单的设置窗口
    /// </summary>
    public void showSetingDialog()
    {
        setingDialog.gameObject.SetActive(true);
        setingDialog.ClickCallBackDelegate = setingDialogClickCallBack;

    }
    public SetingDialog getSetingDialog() {
        return setingDialog;
    }
    private void setingDialogClickCallBack(SetingDialog.ButtonIndex buttonIndex)
    {
        if (buttonIndex == SetingDialog.ButtonIndex.bt_Exit)
        {
            Debug.Log("click exit");
        }
        else
        {
            if (GameAppControl.getGameRuning())//游戏中关闭弹框需返回游戏时要关闭凝视点
            {
                //退出游戏后需要打开凝视或手柄操控点
                ControllerTool.CloseOrOpenAnyEnableContrlorCursor(CyberCloud_UnitySDKAPI.CursorStatus.closeCursor);
            }
            if (buttonIndex == SetingDialog.ButtonIndex.bt_ClickSameScreen)
            {//点击同屏按钮
                Debug.Log("ucvr click ClickSameScreen");
                string APP_BIND_STATE = "1";
                if(CyberCloudConfig.cvrScreen == CyberCloudConfig.CVRScreen.FuJian)
                    APP_BIND_STATE = MyTools.readNoSectionFromIni("/sdcard/FJCMCCCloudVR/Config.ini", "APP_BIND_STATE");

                if (APP_BIND_STATE == "1" )
                {
                    string playUrl = gameAppControl.Cyber_GetCastScreenPlayUrl();

                    if (CyberCloudConfig.castScreenTestConfigFileUrl != null && CyberCloudConfig.castScreenTestConfigFileUrl != "")
                    {
                        //开启投屏测试
                        testCastScreen.gameObject.SetActive(true);
                    }
                    else {
                        if (XMPPTool.castScreen == XMPPTool.CastScreen.castScreening)
                        {
                      
                            closeSameScreen();
                        }
                        else if (XMPPTool.castScreen == XMPPTool.CastScreen.castScreenTryConnect)
                        {
                            MyTools.PrintDebugLog("ucvr runing castScreen repeat click castScreen is unenable");
                        }
                        else
                        {
                            if (GameAppControl.getGameStarted())
                            {
                             
                                sendBroadcastOpenVRSTB(playUrl);
                            }
                            else
                            {
                                XMPPTool.castScreen = XMPPTool.CastScreen.noCastScreen;
                                showHintMstByDesckey("startAppFirst", 3);
                                MyTools.PrintDebugLog("ucvr game is not runing cannot openVRSTB,等待游戏启动后再投屏");
                            }

                        }

                    }


                }
                else
                {
                    MyTools.PrintDebugLog("ucvr userloginstatus:" + APP_BIND_STATE);              
                    showHintMstByDesckey("user_not_logged", 3);
                    
                }


            } else if (buttonIndex == SetingDialog.ButtonIndex.bt_SwitchControl) {
                gameAppControl.updateControllerIndex();
            }
        }
    }
    /// <summary>
    /// 通过国际化配置码显示弱提示
    /// </summary>
    /// <param name="key"></param>
    /// <param name="showtime"></param>
    public void showHintMstByDesckey(string key, int showtime,string errorcode=null, int hintY = 0, bool needFollowCamera = false) {
        //startappinunityedit
        string msg = Localization.Get(key);
        if (errorcode != null)
            msg += "(" + errorcode + ")";
        if (OpenApiImp.CyberCloudOpenApiEnable)
        {
            // OpenApiImp.getOpenApi().notify().simpleShowDialog(1,msg, showtime);        
            // 若提示上层自己控制否则有可能会重
            if ("Screen_casting_failed" == key)
                OpenApiImp.getOpenApi().notify().systemStatusCallback(OpenApiImp.castScreenFailed, ErrorCodeOpenApi.castScreenFailed.ToString());
            else if ("setboxupdate" == key) { 
                OpenApiImp.getOpenApi().notify().simpleShowDialog(1, msg,3);
            }
        }
        else
            showHintMsg(msg, showtime, hintY,needFollowCamera);
    }
    public void showHintMsg(string msg, int showtime, int hintY = 0, bool needFollowCamera = false) {
        //positionCenter(hint.gameObject);
        Transform cameraCenter = gameAppControl.gameObject.transform;
        if (needFollowCamera)
        {
        
            Quaternion q = cameraCenter.rotation;          
            Vector3 direction = q * Vector3.forward;//相对场景中心点，相机前方（内部z轴）的单位向量                                                                    //this.transform.LookAt(center);
            Vector3 newPos = direction * CyberCloudConfig.DialogDistance;//相对场景中心点
            Vector3 temp = cameraCenter.position + newPos;
            
            hint.gameObject.transform.rotation = Quaternion.Euler(q.eulerAngles.x, q.eulerAngles.y, 0);      
            hint.gameObject.transform.position = temp;
        }
        Vector3 v3 = hint.gameObject.transform.localPosition;// .position;
        v3.y = hintY+ cameraCenter.localPosition.y;
        hint.gameObject.transform.localPosition = v3;


        hint.gameObject.SetActive(true);
      

        hint.hintMsg(msg, showtime);
    }
    /// <summary>
    /// 关闭弱提示
    /// </summary>
    public void closeHintMsg()
    {
        hint.gameObject.SetActive(false);
    }
    public TipsControl getTipsControl() {
        if (tipsControl == null)
        {
            MyTools.PrintDebugLogError("ucvr commonPanel can not find TipsControl please make sure this is added on AppTips");
        }
        else
            return tipsControl;
        return null;
    }
    public GameObject getExitBt()
    {
     
 

            if (tipsControl == null)
        {
            MyTools.PrintDebugLogError("ucvr commonPanel can not find ExitBt please make sure this is added on exitCyberCloud");
        }
        else
            return setingBt;
        return null;
    }
    /// <summary>
    /// 方法：当程序获得焦点（即程序没有后台时）bool为true，当程序失去焦点（即程序后台）时，bool为false；
    /// 此方法会自动执行，只需要在方法体内写好自己根据bool来执行的逻辑就可以了。
    ///  pico neo 有时出现OnApplicationFocus（false）和OnApplicationFocus（true）瞬间切换导致游戏退出 所以禁用
    /// </summary>
    /// <param name="isFocus"></param>
    void OnApplicationFocus(bool isFocus)
    {
	
        if (CyberCloudConfig.currentType != CyberCloudConfig.DeviceTypes.Pico2)
        return;
        if (OpenApiImp.CyberCloudOpenApiEnable)
            return;
        //创维没有home键，锁屏键会触发OnApplicationFocus false 所以此处需要过滤掉
        if (CyberCloudConfig.currentType != CyberCloudConfig.DeviceTypes.skyworth)
        {
            
              if (isFocus)
              {
                MyTools.PrintDebugLog("ucvr OnApplicationFocus true");  //  

                  OnResume();
              }
              else
              {
                  OnPause();

                MyTools.PrintDebugLog("ucvr  OnApplicationFocus false");  //  
              }
            
        }
    }
    void OnApplicationQuit()
    {
        Debug.Log("ucvr OnApplicationQuit");
        if(GameAppControl.getGameRuning())
            cloudCyberExit();
    }
    /// <summary>
    /// activity的激活和暂停, 
    /// 这个方法游戏电源休眠时也会触发 pause（6dof摘下时电源休眠有问题）,OnApplicationFocus不会??????????????
    /// 注意如果pause不停止游戏经常会导致apk卡死
    /// </summary>
    /// <param name="pause"></param>
    private void OnApplicationPause(bool pause)
    {
        MyTools.PrintDebugLog("ucvr OnApplicationPause::" + (pause ? "true" : "false"));
        //if (OpenApiImp.CyberCloudOpenApiEnable)
        //    return;
   
        if (CyberCloudConfig.currentType != CyberCloudConfig.DeviceTypes.skyworth) { 
      
#if UNITY_ANDROID && !UNITY_EDITOR
                    if (pause)
                    {
                        OnPause();
                     
                    }
                    else
                    {
                           OnResume();
                    }
#endif
        }
    }
    public void closeSameScreen() {
        if (gameAppControl)
        {
            StartCoroutine(gameAppControl.streamLiveOption(0));
            StartCoroutine(gameAppControl.cmdToServiceStartStopCastScreen(0));//
        }
        if (XMPPTool.castScreen == XMPPTool.CastScreen.castScreening|| XMPPTool.castScreen == XMPPTool.CastScreen.castScreenTryConnect)
            xmpptool.sendBroadcastClose();
    }
    public void sendBroadcastOpenVRSTB(string playUrl)
    {
        //string playUrl = gameAppControl.Cyber_GetCastScreenPlayUrl();
        //xmpptool.sendBroadcastOpenVRSTB(playUrl);

        if (is1106Test)
        {
            //有网关时使用netty方式投屏
            if (CyberCloudConfig.tls.IndexOf(GameAppControl.gatewayPort) > -1||OpenApiImp.CyberCloudOpenApiEnable==true)
                xmpptool.nettyApiInit(playUrl);
            else
                xmpptool.sendBroadcastOpenVRSTB(playUrl);
        }
        else
            xmpptool.sendBroadcastOpenVRSTB(playUrl);
    }
    private void OnPause()
    {

        MyTools.PrintDebugLog("ucvr try exitCyberCloud OnPause");
        closeSameScreen();
        //if(CyberCloud_UnitySDKAPI.ControllerManager.isTest==false)
        cloudCyberExit();
        UdpReceiveService.StopReceive();
        if (DataLoader.Instance.portalAPI != null)
            DataLoader.Instance.portalAPI.stopSessionReport();
        if (CyberCloudConfig.ExportOnlyPlayer == true)
        {
             #if PicoSDK
            Controller.UPvr_IsEnbleHomeKey(true);
#endif
            Debug.Log("ucvr try exitCyberCloud OnPause ExportOnlyPlayer true");
    
            Application.Quit(0);
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }
    void OnDestroy()
    {
        MyTools.PrintDebugLog("ucvr try exitCyberCloud OnDestroy");

	}
    /// <summary>
    /// OnSystemHome??????无法触发 home键回调
    /// 此jar回调未使用
    /// </summary>
    public void OnSystemHome() {
        MyTools.PrintDebugLog("ucvr try exitCyberCloud OnSystemHome");
    }
    /// <summary>
    /// 此jiar回调未使用，因为stop时使用unitysendmessage无法发送消息到unity
    /// </summary>
    public void onStop() {
        MyTools.PrintDebugLog("ucvr try exitCyberCloud onStop");
  
    }
    public bool cloudCyberExit() {
        //退出游戏后需要打开凝视或手柄操控点
        ControllerTool.CloseOrOpenAnyEnableContrlorCursor(CyberCloud_UnitySDKAPI.CursorStatus.openCursor);
        //应用正在运行中或者应用适配平台方式启动，退出时只退出游戏
        if (GameAppControl.getGameRuning()|| (CyberCloudConfig.adapt_platform != null && CyberCloudConfig.adapt_platform != ""))
        {
            MyTools.PrintDebugLog("ucvr exitCyberCloud ");
            if(GameAppControl.getGameRuning())
                gameAppControl.exitCyberGame();
            return true;
        }else if (CyberCloudConfig.ExportOnlyPlayer)
        {
            if(DataLoader.Instance.portalAPI!=null)
                DataLoader.Instance.portalAPI.stopSessionReport();
            MyTools.PrintDebugLog("ucvr ExportOnlyPlayer true quit apk ");
 #if PicoSDK
            Controller.UPvr_IsEnbleHomeKey(true);
#endif
            Application.Quit(0);//unity 弹出系统升级时有時會報錯模式不對

            System.Diagnostics.Process.GetCurrentProcess().Kill();
  
        }
        return false;
    }
    private void OnResume()
    {
        Debug.Log("sbyCommonPlane OnResume");
    
    }

}
