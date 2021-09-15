using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Assets.CyberCloud.Scripts;
using Assets.CyberCloud.Scripts.OpenApi;
using Assets.CyberCloud.Scripts.Tools;
using com.cybercloud.tcpTool;
using Newtonsoft.Json;
using UnityEngine;
using static Assets.CyberCloud.Scripts.OpenApi.OpenApiImp;

/// <summary>
/// 流化界面控制
/// 游戏启动以及启动时界面切换
/// </summary>
public class GameAppControl : MonoBehaviour {
    /// <summary>
    /// 左眼native控制脚本
    /// </summary>
    [SerializeField]
    private InterfaceControl leftInterface;
    [SerializeField]
    private InterfaceControl rightIntercace;

    /// <summary>
    /// 视博云播放器jar包全类名
    /// </summary>
    public static string sbyJarName = "com.cybercloud.vr.CvrPlayer";
   
    private static string unityMsgNotifyJarName = "com.cybercloud.vr.VRMsgNotify";
 
    /// <summary>
    /// 提示信息
    /// </summary>
    private TipsControl tipsControl;

    /// <summary>
    /// 左右流化显示界面
    /// </summary>
    private GameObject leftPlane;
    /// <summary>
    /// 右眼流化显示界面
    /// </summary>
    private GameObject rightPlane;
    /// <summary>
    /// 左侧菜单
    /// </summary>
    private GameObject leftMenu;
    /// <summary>
    /// portal内容展示区
    /// </summary>
    private GameObject homePage;
    //游戏是否在运行或尝试启动
    private static bool gameIsRuning = false;
    //游戏是否已启动
    private static bool gameStarted = false;
    private CommonPlane commonPlaneCom;
    public static string gatewayPort = "21700";
    /// <summary>
    /// 江西测试模式 需要将位置和应用进行映射 此id为映射后的id，如果有此id需要获取启动串后启动应用
    /// </summary>
    private String testScreenMapAppID = null;
    public static bool enableSafePanel = true;
    DataLoader.GetStartAppUrlFromCVRGatewayParam startIntentPparam;
    // Use this for initialization
    void Awake()
    {
      
        //piconeoto 禁用安全区域
        if (!enableSafePanel) {
            MyTools.setSystemProperties("persist.pvrcon.seethrough.enable","0");
        }
        leftInterface.setCenterEyeTransform(this.gameObject);
        rightIntercace.setCenterEyeTransform(this.gameObject);
        if (OpenApiImp.CyberCloudOpenApiEnable)
        {
            // CyberCloudConfig.ExportOnlyPlayer = true;
            Debug.Log("ucvr CyberCloudOpenApiEnable true");
            return;
        }
        //Debug.LogError("ucvr CyberCloudConfig.cvrScreen:" + CyberCloudConfig.cvrScreen);
        startIntentPparam = getStartParamFromAppStartIntent();
        if (startIntentPparam != null && startIntentPparam.appID != null && startIntentPparam.appID != "")
        {
            CyberCloudConfig.ExportOnlyPlayer = true;
            Debug.Log("ucvr startapp by luancher or other app****************");
        }
        if (CyberCloudConfig.tls.IndexOf(gatewayPort) > -1) {

            gatewayInit();
           
            return;
        }

        if (CyberCloudConfig.currentType == CyberCloudConfig.DeviceTypes.Pico ||
            CyberCloudConfig.currentType == CyberCloudConfig.DeviceTypes.PicoNeo2
            || CyberCloudConfig.currentType == CyberCloudConfig.DeviceTypes.Pico2)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
              String loginInfo = MyTools.getLogin_info();
            MyTools.PrintDebugLog("ucvr MyTools.getLogin_info:"+ loginInfo);
            if (CyberCloudConfig.adapt_platform != null && CyberCloudConfig.adapt_platform != "") {
                               MyTools.getStartUrlByAdaptForm(CyberCloudConfig.adapt_platform);
                CyberCloudConfig.ExportOnlyPlayer = true;
        
                Debug.Log("ucvr getStartUrlByAdaptForm loginInfo:" + loginInfo);
                return;
            }
            //CyberCloud:700001022&StartAppParam=&UserID=
            if (loginInfo != null && loginInfo != "") {
                //兼容cbmp升级后6个入口的rom无法获取应用启动串，会传入类似CyberCloud:700001022&StartAppParam=&UserID=这样的格式串
                if (loginInfo.IndexOf("CyberCloud:7")>-1||loginInfo.IndexOf("CyberCloud:9")>-1){
                   // Debug.Log("ucvr getLogin_info 1:" );
                    CyberCloudConfig.ExportOnlyPlayer = true;
                    int indexStart = loginInfo.IndexOf("CyberCloud:")+ "CyberCloud:".Length;
                    //Debug.Log("ucvr getLogin_info 2:" + indexStart);
                    int endIndex= indexStart< loginInfo.Length?loginInfo.IndexOf("&",indexStart):-1;
                    String appid = endIndex> indexStart ? loginInfo.Substring(indexStart, endIndex- indexStart) : loginInfo.Substring(indexStart);
                   // Debug.Log("ucvr getLogin_info 3:" + endIndex+ ";appid="+ appid);
                    DataLoader.Instance.startAppById(appid);                                                           
            		      Debug.Log("ucvr new cbmp jianrong :" + appid);         
            	}else{
                	CyberCloudConfig.ExportOnlyPlayer = true;
                	CyberCloudConfig.loginInfo = loginInfo;
                }
            }else{
                getStartParamFromOldLauncherIntent();
            }
           
#endif

        }
    }
    public void gatewayInit() {
        
        if (CommonPlane.is1106Test)
        {
            CyberCloudConfig.ExportOnlyPlayer = true;
            MyTools.apkUpdateCheckUp();
        }
        else {//网关模式目前只支持通过intent参数传人appid启动，无应用id时打印错误日志
      
            if (CyberCloudConfig.ExportOnlyPlayer != true)
            {
                Debug.LogError("ucvr  gateway only support AppStartByIntent，no support start portal,if need start portal please use tlsurl as config");             
            }
        }
  
    }
    //升级检测不需要升级或接口调用不通的回调
    public void apkUpdateCheckUpResult(string result)
    {
        //如果是直接启动模式需要在升级检测完成后启动游戏
        if (CyberCloudConfig.ExportOnlyPlayer == true)
        {
            Debug.Log("ucvr apkUpdateCheckUpResult over ExportOnlyPlayer startapp");
            DataLoader.GetStartAppUrlFromCVRGatewayParam param=getStartParamFromAppStartIntent();
            DataLoader.Instance.skipPortalStartAppByIdOnAppStart(param);
        }
    }
    private Boolean teststartparam = false;
    /// <summary>
    /// 从应用启动参数中获取启动信息
    /// </summary>
    /// <returns></returns>
    DataLoader.GetStartAppUrlFromCVRGatewayParam getStartParamFromAppStartIntent()
    {
        if (teststartparam) {
            DataLoader.GetStartAppUrlFromCVRGatewayParam param1 = new DataLoader.GetStartAppUrlFromCVRGatewayParam();
            param1.appID = "1";
            return param1;
        }
        
        String userName = MyTools.getIntentMapValueStringByKey("userID");        
        if(userName == null || userName == "")
            userName = MyTools.getIntentMapValueStringByKey("userName");//江西移动智能给username
        String appID = MyTools.getIntentMapValueStringByKey("appID");
        String userToken = MyTools.getIntentMapValueStringByKey("userToken");
        String authType  = MyTools.getIntentMapValueStringByKey("authType");
        AppName = MyTools.getIntentMapValueStringByKey("appName"); ;
        if (appID != null && appID != null)
        {
            DataLoader.GetStartAppUrlFromCVRGatewayParam param = new DataLoader.GetStartAppUrlFromCVRGatewayParam();
            param.appID = appID;
            DataLoader.deviceSN = param.operateUserIDUserID = (userName == null || userName == "") ? CyberCloud_UnitySDKAPI.HeadBox.getDeviceSN() : userName;//鲁艺和成研已经江西移动产品确认过不需要userid，他们不需要用户使用记录和在线统计
            
            param.userToken = userToken;
            param.authType = authType;
            param.tenantID = CyberCloudConfig.tenantID;
            Debug.Log("ucvr param userID:" + param.operateUserIDUserID + ";appID:" + appID + ";userToken:" + userToken + ";deviceSN:" + DataLoader.deviceSN+ ";authType:"+ authType);
            return param;
        }
        return null;
      
    }
    /// <summary>
    /// 通过pico早期launcher获取直接拉起应用的应用id
    /// CyberCloudConfig.ExportOnlyPlayer = true设置成true后，等GameAppControl 的start回调中会根据该参数自动选择拉起对应的应用
    /// </summary>
    void getStartParamFromOldLauncherIntent()
    {
        testScreenMapAppID = "";
        String StartAppID = MyTools.getIntentMapValueStringByKey("StartAppID");
        if (StartAppID != null && StartAppID != "")
        {
            Debug.Log("ucvr getStartParamFromOldLauncherIntent StartAppID:" + StartAppID);
            CyberCloudConfig.ExportOnlyPlayer = true;
            testScreenMapAppID = StartAppID;
            return;
        }
        String screenNo = MyTools.getIntentMapValueStringByKey("screenNo");
        if (screenNo != null && screenNo != "")
        {
            CyberCloudConfig.ExportOnlyPlayer = true;
            String indexstr = MyTools.getIntentMapValueIntByKey("index").ToString();
            MyTools.PrintDebugLog("ucvr screenNo:" + screenNo + ";indexint:" + indexstr);

            String map = CyberCloudConfig.mapapps;
            if (map != null && map != "")
            {
                string[] map_groups = map.Split(';');
                foreach (string group in map_groups)
                {
                    string[] maps = group.Split(':');
                    if (maps != null && maps.Length > 2)
                    {
                        String screenNotemp = maps[0];
                        String indextemp = maps[1];
                        String apptemp = maps[2];
                        //匹配intent参数中的场景码和位置与配置文件配置的应用id对应关系
                        if (screenNo.Equals(screenNotemp) && indexstr.Equals(indextemp))
                        {
                            testScreenMapAppID = apptemp;
                            MyTools.PrintDebugLog("ucvr testScreenMapAppID:" + testScreenMapAppID);
                            break;
                        }
                    }
                    else
                    {
                        MyTools.PrintDebugLogError("ucvr mapapps format screenNo:index:appID");
                    }
                }
            }
            else
            {
                MyTools.PrintDebugLogError("ucvr config no mapapps");
            }
        }
    }
    void Start () {
        foreach (Transform child in transform){
            if (child.gameObject.name.Equals("leftPlane")) {
                leftPlane = child.gameObject;
            }
            else if (child.gameObject.name.Equals("rightPlane"))
            {
                rightPlane = child.gameObject;
            }
        }
        if (leftPlane == null || rightPlane == null)
            MyTools.PrintDebugLogError("ucvr you can not change GamePlane children name，mast contain leftPlane and rightPlane two gameobject");
        GameObject commonPlane = GameObject.Find("CyberCloudCommonPlane");
        if (commonPlane == null)
            MyTools.PrintDebugLogError("ucvr commonPanel mast add to screen");
        commonPlaneCom = commonPlane.GetComponent<CommonPlane>();
        if (commonPlaneCom == null)
            MyTools.PrintDebugLogError("ucvr CyberCloudCommonPlane mast contain CommonPlane");
        else {
     
            tipsControl = commonPlaneCom.getTipsControl();       
        }
        if ( leftInterface==null|| rightIntercace==null)
            Debug.LogError("ucvr leftInterface and rightIntercace mast bind on left and right camera");
        toolCopyFile = new MyTools();
        //导出的是player时player启动时拉起默认的启动串
        //CyberCloudConfig.adapt_platform应用适配地址不为空时需要等待应用适配平台的系统回调
        if (CyberCloudConfig.ExportOnlyPlayer && (CyberCloudConfig.adapt_platform == null || CyberCloudConfig.adapt_platform == ""))
        {
            //导出的是player时player启动时拉起默认的启动串
            if (testScreenMapAppID != null&& testScreenMapAppID!="")
            {
                DataLoader.Instance.startAppById(testScreenMapAppID);
                MyTools.PrintDebugLog("ucvr CyberCloudCommonPlane ExportOnlyPlayer=true testScreenMapAppID:" + testScreenMapAppID);
            }
            else if (CyberCloudConfig.loginInfo != null && CyberCloudConfig.loginInfo != "")
            {
                startApp(CyberCloudConfig.loginInfo);
                MyTools.PrintDebugLog("ucvr CyberCloudCommonPlane ExportOnlyPlayer=true");
            }
            else {//
                DataLoader.GetStartAppUrlFromCVRGatewayParam param = getStartParamFromAppStartIntent();
                if (param != null && param.appID != null && param.appID != "")//新版Launcher通过intent参数启动流化应用
                {
                    Debug.Log("ucvr skipPortalStartAppByIdOnAppStart appid:" + param.appID);
                    //网关等待apkUpdateCheckUpResult
                    if (CyberCloudConfig.tls.IndexOf(gatewayPort) > -1)//如果网关接口需要等待apk检测完成后启动应用
                    {
                        Debug.Log("ucvr gateway start wait apkUpdateCheckUpResult" );
                    }
                    else
                        DataLoader.Instance.skipPortalStartAppByIdOnAppStart(param);
                }
                else {
                    Debug.LogError("ucvr skipPortalStartAppByIdOnAppStart failed appid is null");
                }
            }
        }

    }
    /// <summary>
    /// 根据左右眼索引获取左右眼流化渲染界面
    /// </summary>
    /// <param name="eye"></param>
    /// <returns></returns>
    public GameObject getRenderPlane(EyeIndex eye) {
        GameObject renderPlane= eye == EyeIndex.leftEye ? leftPlane : rightPlane;
        if(renderPlane == null)
            Debug.LogError("eyeid："+ eye+"上找不到流化渲染界面");
        return renderPlane;
    }
    // Update is called once per frame
    void Update () {
        if (exitAppByTeacher)
        {
            exitAppByTeacher = false;
            if (gameIsRuning)
                exitCyberGame();
            else
            {
                Application.Quit(0);//
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }
    }
    public static bool getGameRuning() {
        return gameIsRuning;
    }
    public static bool getGameStarted() {
        return gameStarted;
    }
    public const bool istest = false;
    //拷贝文件处理头盔返回 没有测试通过
    MyTools toolCopyFile = null;
    public static String AppName = "";
    public void startApp(string loginInfo)
    {
     
        if (CyberCloudConfig.startMode == 1)
            loginInfo = CyberCloudConfig.loginInfo;

        if (loginInfo == null || loginInfo.Equals(""))
        {
           if (CyberCloudConfig.ExportOnlyPlayer==true)
                commonPlaneCom.showDialogOneBtExitDialog("Home_NoNet", "110003");
            else
                commonPlaneCom.showHintMstByDesckey("Home_NoNet", 110004);
            startAppException(ErrorCodeOpenApi.gatewayFailed.ToString());
            MyTools.PrintDebugLogError("ucvr start game url is null");
            return;
        }
        // loginInfo=loginInfo+"&CyberZoneCode=default";

        if (gameIsRuning == true)
        {
            MyTools.PrintDebugLogError("ucvr game is runing can not change other game ");
            startAppException(ErrorCodeOpenApi.startAppDouble.ToString());
            return;
        }
        if (ServerDialog.MultipleZoneCode())
        {
            commonPlaneCom.showServerSwitchDialog(delegate (string code)
            {
                loginInfo = loginInfo + "&CyberZoneCode=" + code;
                startAppNext(loginInfo);
            });
        }
        else {
            startAppNext(loginInfo);
        }
    }
	//是否需要终端参数标识
	private string needTeminalParam = "StartAppID=61";
    public void startAppNext(string loginInfo)
    {
        gameStarted = false;
        gameIsRuning = false;
        if (CyberCloudConfig.startParam != null)
            loginInfo = loginInfo + CyberCloudConfig.startParam;
		if (loginInfo.IndexOf(needTeminalParam) > -1)
		{
			loginInfo = loginInfo + "&StartAppParam={\"operateUserID\":\"" + DataLoader.deviceSN + "\",\"deviceType\":\"" + CyberCloudConfig.currentType + "\"}";
		}
        if (OpenApiImp.CyberCloudOpenApiEnable) {
            loginInfo = OpenApiImp.getOpenApi().getStartUrl(loginInfo);
        }
        Debug.Log("ucvr startAppNext:"+ loginInfo);
        // tipsControl.dispalyCursorTips(true, TipsControl.TipesType.showExitTips);
        //  return;
        if (false) {
            tipsControl.dispalyCursorTips(true, TipsControl.TipesType.showExitTips);
            gameIsRuning = true;
            //启动前创建纹理
            leftInterface.CreateTextureAndPassToPlugin(leftPlane);

            rightIntercace.CreateTextureAndPassToPlugin(rightPlane);


            Debug.Log("ucvr call Cyber_StartPlay");
            if (CyberCloudConfig.currentType != CyberCloudConfig.DeviceTypes.Pico2)//pico2不显示外设提示,启动中需要居中
                commonPlaneCom.showHintMstByDesckey("Application_startup", -1, null, 332, true);
            else
                commonPlaneCom.showHintMstByDesckey("Application_startup", -1, null, 0, true);

            ControllerTool.CloseOrOpenAnyEnableContrlorCursor(CyberCloud_UnitySDKAPI.CursorStatus.closeCursor);
          
           // sleepOneSeconde();
            return;
        }
#if UNITY_ANDROID && !UNITY_EDITOR
            _handler = initCyberCloud();
     
            if (_handler != null) {
                gameIsRuning = true;    
                //启动前创建纹理
                leftInterface.CreateTextureAndPassToPlugin(leftPlane);
  
                rightIntercace.CreateTextureAndPassToPlugin(rightPlane);
          
              
                Debug.Log("ucvr call Cyber_StartPlay" );
                if (CyberCloudConfig.currentType != CyberCloudConfig.DeviceTypes.Pico2)//pico2不显示外设提示,启动中需要居中
                    commonPlaneCom.showHintMstByDesckey("Application_startup", -1, null, 332, true);
                else
                    commonPlaneCom.showHintMstByDesckey("Application_startup", -1, null, 0, true);
                tipsControl.dispalyCursorTips(true,TipsControl.TipesType.showExitTips);
                ControllerTool.CloseOrOpenAnyEnableContrlorCursor(CyberCloud_UnitySDKAPI.CursorStatus.closeCursor);
                int startResult = _handler.Call<int>("Cyber_StartPlay", loginInfo);
                MyTools.PrintDebugLog("ucvr native init startResult:"+startResult);
                if (startResult != 0){
                    commonPlaneCom.closeHintMsg();
                    tipsControl.dispalyCursorTips(false, TipsControl.TipesType.showExitTips);//游戏启动后隐藏提示
                    ControllerTool.CloseOrOpenAnyEnableContrlorCursor(CyberCloud_UnitySDKAPI.CursorStatus.openCursor);
                    MyTools.PrintDebugLogError("ucvr unity start failed");
                    gameIsRuning=false;
                    startAppException(startResult.ToString());
                }
            }else{
                MyTools.PrintDebugLog("ucvr native init failed");
                commonPlaneCom.showHintMstByDesckey("Application_initialization_failed", 3);
               startAppException(ErrorCodeOpenApi.initNativeCyberCloudVRSdkFailed.ToString());
             }
#else

        startAppException(ErrorCodeOpenApi.startAppInUnityEdit.ToString());
        commonPlaneCom.showHintMstByDesckey("startappinunityedit", 3);
        MyTools.PrintDebugLog("ucvr unity edit can not start app");
#endif
        if(OpenApiImp.CyberCloudOpenApiEnable==false)
            DataLoader.Instance.portalAPI.stopSessionReport();
    }
    public void startAppException(string code) {
        if (OpenApiImp.CyberCloudOpenApiEnable)
            OpenApiImp.getOpenApi().notify().systemStatusCallback(OpenApiImp.systemException, code);
    }
    public void updateControllerIndex() {
        leftInterface.toSetLeftControllerIndex();
    }
    private AndroidJavaObject _handler;
    public string Cyber_GetCastScreenPlayUrl() {
        string playurl = "";

#if UNITY_ANDROID && !UNITY_EDITOR
     
        //调用anroidjar异常日志可以搜索AndroidJavaException
        if (_handler == null)
        {
            MyTools.PrintDebugLogError("ucvr:" + sbyJarName + " is unexit");
            return null;
        }
        playurl = _handler.Call<string>("Cyber_GetCastScreenPlayUrl");
#endif
        MyTools.PrintDebugLog("ucvr Cyber_GetCastScreenPlayUrl playurl:"+ playurl);
        return playurl;
    }

    public Boolean Cyber_setAudioPlayEnable(int enable)
    {
  

#if UNITY_ANDROID && !UNITY_EDITOR
     
        //调用anroidjar异常日志可以搜索AndroidJavaException
        if (_handler == null)
        {
            MyTools.PrintDebugLogError("ucvr:" + sbyJarName + " is unexit");
            return false;
        }
        _handler.Call("audioPlayEnable", enable);
#endif
        return true;

    }
    /// <summary>
    /// 调用native接口 初始化云服务
    /// </summary>
    /// <returns></returns>
    private AndroidJavaObject initCyberCloud() {
        _handler = new AndroidJavaObject(sbyJarName);
        //调用anroidjar异常日志可以搜索AndroidJavaException
        if (_handler == null)
        {
            MyTools.PrintDebugLogError("ucvr:"+sbyJarName + " unexit");
            return null; 
        }
        bool result = regesterJavaCallBackClass();
        if (!result)
        {
            return null;
        }
        AndroidJavaObject unityActivity = MyTools.getCurrentActivity();
        //CyberCloudConfig.statisticsUpLoad 是否上传统计日志默认不上传
        if (CyberCloudConfig.statisticsUpLoad == 1 || CyberCloudConfig.statisticsUpLoad == 110)//必须放到init前面才会生效
        {
            MyTools.PrintDebugLog("ucvr statisticsUpLoad open");
            _handler.Call("logTransmissionEnable", CyberCloudConfig.statisticsUpLoad, CyberCloudConfig.statisticsUpLoadUrl);
        }
        else {
            MyTools.PrintDebugLog("ucvr statisticsUpLoad close");
            _handler.Call("logTransmissionEnable", CyberCloudConfig.statisticsUpLoad, "");
        }
        if (CyberCloudConfig.deltaTimeTest > 0)
        {
            MyTools.PrintDebugLog("ucvr startDeltaTimeTest"+CyberCloudConfig.deltaTimeTest);
            _handler.Call("startDeltaTimeTest", CyberCloudConfig.deltaTimeTest);
        }
        if (AppName != null && AppName != "") {
           _handler.Call("setAppName", AppName);
        }
        try
        {
            if (OpenApiImp.CyberCloudOpenApiEnable)
            {
                String deviceInfo = MyTools.getAndroidDevInfo();
                if (deviceInfo != null)
                {
                    string[] deviceInfos = deviceInfo.Split('&');
                    if (deviceInfos != null)
                    {
                        //string itemDevice = deviceInfos[0];
                        //
                        //string deviceName = itemDevice.Split('=')[1];
                        if (deviceInfos.Length > 2)
                        {
                            string deiveType = deviceInfos[2].Split('=')[1];
                            _handler.Call("setDeviceType", deiveType);
                            MyTools.PrintDebugLogError("ucvr deiveType:" + deiveType);
                        }
                    }
                }
            }else
                _handler.Call("setDeviceType", CyberCloudConfig.currentType.ToLower());
        }
        catch (Exception e) {
            MyTools.PrintDebugLogError("ucvr setDeviceType error");
        }
        try
        {
            _handler.Call("setOperateUserID", DataLoader.deviceSN);
        }
        catch (Exception e)
        {
            MyTools.PrintDebugLogError("ucvr setOperateUserID error");
        }
        try
        {
            _handler.Call("setApkVersion", Application.version);
        }
        catch (Exception e)
        {
            MyTools.PrintDebugLogError("ucvr ApkVersion error");
        }
        MyTools.PrintDebugLog("ucvr init ");
        //终端类型，用于判断与前端通信使用udp或tcp通信的依据，使用前需要在中心管理网站配置终端类型对应的通信协议，再将对应的终端类型配置到此处
        if (CyberCloudConfig.terminalType!=0)
            _handler.Call("init", unityActivity, CyberCloudConfig.startMode, CyberCloudConfig.terminalType);
        else
            _handler.Call("init", unityActivity, CyberCloudConfig.startMode);
        if (CyberCloudConfig.cvrScreen == CyberCloudConfig.CVRScreen.JiaoShi|| CyberCloudConfig.newScreenProtocol==1) {//教室场景和新的投屏方式需要建立直播通道与rta通讯，rta将直播消息转发给中转服务
            MyTools.PrintDebugLogError("ucvr newScreenProtocol:"+ CyberCloudConfig.newScreenProtocol );
            if (_handler != null)
            {
                _handler.Call("buildStreamLiveChannel");//
            }
        }
        return _handler;
    }
    private bool regesterJavaCallBackClass() {
        try {
            MyTools.PrintDebugLog("ucvr init handlerDevice");
            AndroidJavaObject handlerDevice = MyTools.getHandlerDevice();
            if (handlerDevice == null) {
                MyTools.PrintDebugLogError("ucvr handlerDevice null");
                return false;
            }
            MyTools.PrintDebugLog("ucvr init VRMsgNotify");
            AndroidJavaObject msg =  new AndroidJavaObject(unityMsgNotifyJarName) ;
            if (msg == null)
            {
                MyTools.PrintDebugLogError("ucvr unitymsgjar null");
                return false;
            }
            MyTools.PrintDebugLog("ucvr regester callback");
            _handler.Call("Cyber_SetMsgCallback", msg);
            _handler.Call("Cyber_SetDevStateCallback", handlerDevice);
        }
        catch (Exception e) {
            MyTools.PrintDebugLogError("ucvr regesterJavaCallBackClass message:"+e.Message+"e.StackTrace"+ e.StackTrace);
            return false;
        }
        return true;
    }
    public IEnumerator sendBroadcastOpenVRSTB()
    {
        MyTools.PrintDebugLog("ucvr sendBroadcastOpenVRSTB ");
        int times = 0;
        string playUrl = null;
        //启动成功后不一定立刻能拿到投屏地址,此处延迟100帧等待时间
        while (times<100&& (playUrl==null|| playUrl==""))
        {
            times = times + 1;
               playUrl = Cyber_GetCastScreenPlayUrl();         
            yield return new WaitForEndOfFrame();//等到该帧结束
        }
        if (playUrl == null || playUrl == "") {
            MyTools.PrintDebugLogError("ucvr sendBroadcastOpenVRSTB  playUrl=null");
        }
        else
            commonPlaneCom.sendBroadcastOpenVRSTB(playUrl);       
	}
    /// <summary>
    /// 由第三方启动游戏投屏
    /// </summary>
    public void startCastScreenByOther() {
        StartCoroutine(sendBroadcastOpenVRSTB());//
    }
    /// <summary>
    /// 为了保证可以根据投屏地址获取到直播地址，此处携程启动直播
    /// </summary>
    /// <returns></returns>
    public IEnumerator startlive()
    {
        MyTools.PrintDebugLog("ucvr startlive ");
        int times = 0;
        string playUrl = null;
        //启动成功后不一定立刻能拿到投屏地址,此处延迟100帧等待时间
        while (times < 100 && (playUrl == null || playUrl == ""))
        {
            playUrl = Cyber_GetCastScreenPlayUrl();
            yield return new WaitForEndOfFrame();//等到该帧结束
        }
        if (playUrl == null || playUrl == "")
        {
            MyTools.PrintDebugLogError("ucvr startlive  playUrl=null");
        }
        else
            StartCoroutine(streamLiveOption(1, playUrl));//教师场景启动直播
    }
    private class LiveStream {
        public string protocolType;//srt,rtmp
        public string sessionID;
        public int port;//srt 时有效表示srt监听的端口号
        public string liveUrl;//rtmp 时有效表示直播地址
        public string messge;//预留
        public int version=1;
    }
    /// <summary>
    /// cmdToServiceStartStopCastScreen 发送投屏指令
    /// streanLiveOption 发送直播指令
    /// 教室场景和新的投屏方式需要建立直播通道与rta通讯，rta将直播消息转发给中转服务
    /// 启停前端投屏指令，指令发送后中转服务会根据该指令进行启停
    /// 注意给中转服务投屏srt端口是在8001基础上加了6200变成14201
    /// </summary>
    /// <param name="onofff">1启动，0关闭</param>
    /// <param name="roomID"></param>
    /// <param name="playUrl">直播地址</param>
    /// <param name="newCastScreenMode">是否是投屏模式</param>
    public IEnumerator cmdToServiceStartStopCastScreen(int onofff,string playUrl = "")
    {
        string deviceID = CyberCloud_UnitySDKAPI.HeadBox.getDeviceSN();
        if (_handler == null)
        {
            MyTools.PrintDebugLogError("ucvr _handler  cmdToServiceStartStopCastScreen call failed onofff:" + onofff);
          
        }
        else
        {
            if (CyberCloudConfig.newScreenProtocol == 1)//新的投屏模式需要向前端发送srt启动指令
            {

                int connect = _handler.Call<int>("isConnectionStreamLiveChannel");
                MyTools.PrintDebugLog("ucvr cmdToServiceStartStopCastScreen  connect:" + connect + ";onofff:" + onofff);
                if (connect == 1)//1是成功
                {
                    LiveStream data = new LiveStream();
                    string ip = "";
                    data.protocolType = "srt";
                    data.messge = "ctime=" + DateTime.Now.ToString();
                    playUrl = commonPlaneCom.getCastScreenCmd(playUrl, ref ip, ref data.port, ref data.sessionID);
                    string jsonParas = JsonConvert.SerializeObject(data);
                    if (onofff == 1)
                    {
                        _handler.Call("startStreamLive", "roomID", jsonParas);//
                        MyTools.PrintDebugLog("ucvr startStreamLive");
                    }
                    else
                        _handler.Call("stopStreamLive", "roomID", "");//  
                }
                else
                {
                    if (onofff == 1)
                    {
                        MyTools.PrintDebugLogError("ucvr streamLiveOption  connect:" + connect);
                        yield return new WaitForSeconds(1);//未连接时需要等等一秒再开启直播
                        if (gameIsRuning)
                        {
                            StartCoroutine(cmdToServiceStartStopCastScreen(onofff, playUrl));
                        }
                    }
                }
            }
        }
    }
    /// <summary>
    /// mdToServiceStartStopCastScreen 发送投屏指令
    /// streanLiveOption 发送直播指令
    /// 如果是教室场景 应用启动成功后，教室直播方法是0、应用启动初始化时使用buildStreamLiveChannel开启直播通道，1、应用启动，2、获取投屏地址拼接rtmp地址，3、通过startStreamLive发送直播地址到中转服务
    /// 1、在上一步启动成功后通过startlive携程方法循环读取投屏地址保证投屏地址能成功获取到
    /// 2、streamLiveOption中拼接rtmp地址"rtmp://" + 服务器ip + ":15004/live/" + sessionID;
    /// 3、根据isConnectionStreamLiveChannel的java层接口获取直播通道是否已经连接，如果已连接使用startStreamLive的java层接口发送直播指令到中转服务
    /// 注：教室场景和新的投屏方式需要建立直播通道与rta通讯，rta将直播消息转发给中转服务
    /// </summary>
    /// <param name="onofff">1启动，0关闭</param>
    /// <param name="roomID"></param>
    /// <param name="playUrl">直播地址</param>
    /// <param name="newCastScreenMode">是否是投屏模式</pa
    public IEnumerator streamLiveOption(int onofff,string playUrl="") {
        string deviceID = CyberCloud_UnitySDKAPI.HeadBox.getDeviceSN();
        if (CyberCloudConfig.cvrScreen == CyberCloudConfig.CVRScreen.JiaoShi)//教师场景通过移植库上报直播启停指令
        {
            int connect = _handler.Call<int>("isConnectionStreamLiveChannel");
            MyTools.PrintDebugLog("ucvr streamLiveOption  connect:"+ connect+ ";onofff:"+ onofff);
           
            if (connect == 1)//1是成功
            {
                playUrl = commonPlaneCom.getLiveStreamUrl(playUrl);
                //为了测试多终端的场景              
                snArr = addSendMessageDevices(deviceID);
                if (onofff == 1)
                {
                    if (CyberCloudConfig.newScreenProtocol == 1) {
                        LiveStream data = new LiveStream();
                        data.protocolType = "rtmp";
                        data.liveUrl = playUrl;
                        string jsonParas = JsonConvert.SerializeObject(data);
                        _handler.Call("startStreamLive", "roomID", jsonParas);//
                    }
                    else
                        _handler.Call("startStreamLive", "roomID", playUrl);//
                    MyTools.PrintDebugLog("ucvr startStreamLive");
                    StartCoroutine(sendLiveStartToListennerTeacher(playUrl));
                }
                else
                    _handler.Call("stopStreamLive", "roomID", "");//  
            }
            else
            {
                if (onofff == 1)
                {
                    MyTools.PrintDebugLogError("ucvr streamLiveOption  connect:" + connect);
                    yield return new WaitForSeconds(1);//未连接时需要等等一秒再开启直播
                    if (gameIsRuning)
                    {
                        StartCoroutine(streamLiveOption(onofff, playUrl));
                    }
                }
            }
            //端口不用判斷是否連接成功
            if (onofff == 0)
            {
                MyTools.PrintDebugLogError("ucvr stop  zubo" );
                snArr = addSendMessageDevices(deviceID);
                sendTimes = sendMaxTimes;
                sendMessageToListennerTeacher(playUrl, false);
                UdpReceiveService.StopReceive();
            }
        }
    }
    private List<string> addSendMessageDevices(string deviceID) {
        List<string>  tempsnArr = new List<string>();
        tempsnArr.Add(deviceID);
        MyTools.PrintDebugLog("ucvr deviceTestNumJscj：" + CyberCloudConfig.deviceTestNumJscj);
        string devicestr = "";
        for (int i = 1; i < CyberCloudConfig.deviceTestNumJscj + 1; i++)//如果是测试场景把测试场景的设备也加进发送队列中
        {
            tempsnArr.Add(deviceID + i);
            devicestr += deviceID + i+";";
        }
        MyTools.PrintDebugLog("ucvr deviceTestNumJscj：" + devicestr);
        return tempsnArr;
    }
    /// <summary>
    /// 收到教师端的udp消息
    /// </summary>
    /// <param name="message"></param>
    public void udpReciveMessage(string message) {
        MyTools.PrintDebugLog("ucvr udpReciveMessage：" + message);
        //Client|receivedViveAddr|PA7910MGD5100452B
        if (message != null && message.IndexOf("receivedViveAddr") > -1)
        {
            string[] eventMessage = message.Split('|');
            string deviceID = eventMessage[2];
            liveAddrSendSuccess(deviceID);
        }
        else if (message != null && message.IndexOf("exitApk") > -1)
        {//Client|exitApk|sn 当教师端下课时需要发送该指令通知流化应用退出      
            proExitAppByTeacher();
        }
    }
    public void proExitAppByTeacher() {
        MyTools.PrintDebugLog("ucvr exitApk start exit app");
        exitAppByTeacher = true;
    }
    public void liveAddrSendSuccess(string deviceID) {
        MyTools.PrintDebugLog("ucvr deviceID：" + deviceID);
        if (snArr != null && snArr.Count > 0)
        {
            for (int i = snArr.Count - 1; i >= 0; i--)
            {
                if (snArr[i] == deviceID)
                {
                    MyTools.PrintDebugLog("ucvr remove deviceID：" + deviceID);
                    snArr.RemoveAt(i);
                }
            }
        }
    }
    private bool exitAppByTeacher = false;
    /// <summary>
    /// 需要发送的设备ID数组
    /// </summary>
    private List<string> snArr=null;
    private const int sendMaxTimes = 10;//最多发送的次数
    private int sendTimes = 0;//已发送次数
    /// <summary>
    /// 为了防止发送失败启动指令需要循环发送,sendMaxTimes最多循環的次數
    /// </summary>
    /// <param name="deviceID"></param>
    /// <param name="message"></param>
    /// <param name="start"></param>
    /// <returns></returns>
    private IEnumerator sendLiveStartToListennerTeacher(string message) {     
        sendTimes = 0;
        while (sendTimes < sendMaxTimes)
        {
            sendTimes += 1;
            MyTools.PrintDebugLog("ucvr sendLiveStartToListennerTeacher sendTimes:" + sendTimes);
            if (snArr != null && snArr.Count > 0)
            {
                sendMessageToListennerTeacher( message, true);

                yield return new WaitForSeconds(0.3f);//等等一秒再发送下一轮
            }
            else {              
                sendTimes = sendMaxTimes;
            }                     
        }
        MyTools.PrintDebugLog("ucvr sendLiveStartToListennerTeacher over");
    }
    private void sendMessageToListennerTeacher( string message, bool start)
    {
        //stop 不需要重复发送     
        for (int i = 0; i < snArr.Count; i++)
        {
            commonPlaneCom.sendMessageToListennerTeacher(snArr[i], message, start);
        }      
        MyTools.PrintDebugLog("ucvr sendMessageToListennerTeacher:"+ start);        
    }
    public void startResult(int retCode) {
        try
        {          
            if (retCode == 0)
            {
               
                updateControllerIndex();
                gameStarted = true;
                MyTools.PrintDebugLog("ucvr autoStartCastScreen："+ CyberCloudConfig.autoStartCastScreen);
                if (CyberCloudConfig.autoStartCastScreen == 1 || XMPPTool.castScreen == XMPPTool.CastScreen.castScreenTryConnect|| XMPPTool.castScreen == XMPPTool.CastScreen.castScreening)
                {
                    //游戏启动前如果处于投屏或链接中那么游戏起来后就发起游戏投屏（兼容终端投屏）
                    MyTools.PrintDebugLog("ucvr XMPPTool.castScreen：" + XMPPTool.castScreen);
                    StartCoroutine(sendBroadcastOpenVRSTB());//
                }
                if (CyberCloudConfig.cvrScreen == CyberCloudConfig.CVRScreen.JiaoShi)
                {//教师场景需要开启直播
                    StartCoroutine(startlive());                
                }
                MyTools.startDispatchEvent(_handler,1001);

                sleepOneSeconde();
                if (CyberCloudConfig.currentType == CyberCloudConfig.DeviceTypes.DaPeng)
                {

                    if (DataLoader.gameDofType3)
                    {
                        ControllerTool.Reflection("changeRenderdHeadType", new object[] { CyberCloud_UnitySDKAPI.HeadBoxDofType.Dof3 });
                    }
                    else
                    {
                        ControllerTool.Reflection("changeRenderdHeadType", new object[] { CyberCloud_UnitySDKAPI.HeadBoxDofType.Dof6 });
                    }
                }
                if (StartCloudAppTest.startTestAutoStartapp)
                    MyTools.PrintDebugLog("ucvr TestAutoStartApp currentTimes: " + StartCloudAppTest.startTimes + ";result:success");
                isShowGameingNetworkWrongError = false;
                MyTools.PrintDebugLog("ucvr startSuccess");
            }
            else
            {
                if (OpenApiImp.CyberCloudOpenApiEnable)
                    OpenApiImp.getOpenApi().notify().systemStatusCallback(OpenApiImp.systemException, retCode.ToString());
                gameStarted = false;
                gameIsRuning = false;
                commonPlaneCom.closeHintMsg();
                tipsControl.dispalyCursorTips(false, TipsControl.TipesType.showExitTips);//游戏启动后隐藏提示
                MyTools.PrintDebugLogError("ucvr startFailed");
                if (CyberCloudConfig.ExportOnlyPlayer)
                {
#if picoSdk
                    Pvr_UnitySDKAPI.Controller.UPvr_IsEnbleHomeKey(true);
#endif
                    Application.Quit();
                }
            }
        }
        catch (Exception e) {
            MyTools.PrintDebugLogError("ucvr startFailed :"+e.Message);
        }
    }
    /// <summary>
    /// 由于移植库通知流化已播出后需要一个大概1-2秒的时间流才会出来，因此此处等待2秒再显示流化界面
    /// </summary>
    /// <returns></returns>
    //IEnumerator sleepOneSeconde() {
    void sleepOneSeconde()
    {
        //yield return new WaitForSeconds(2);
        commonPlaneCom.closeHintMsg();
        tipsControl.dispalyCursorTips(false, TipsControl.TipesType.showExitTips);//游戏启动后隐藏提示
        MyTools.PrintDebugLog("ucvr hadsleepOneSeconde to show player");
        gameObjEnable(true);//游戏渲染界面是否开启， 开启游戏渲染界面后需要关闭portal界面的显示
    }
    public static bool isShowGameingNetworkWrongError=false;
    /// <summary>
    /// 云系统环境异常
    /// </summary>
    public void systemStatusCallback(string systemStatus,string desc, string errCode)
    {
        MyTools.PrintDebugLog("ucvr systemStatusCallback statusDescription:"+ systemStatus);
        if (systemStatus.Equals("networkException"))
        {
        
            tipsControl.dispalyCursorTips(false, TipsControl.TipesType.showExitTips);
            if (isShowGameingNetworkWrongError == false)
            {

                commonPlaneCom.showHintMstByDesckey("Network_connection_reconnecting", -1);
                isShowGameingNetworkWrongError = true;
            }

        }
        else if (systemStatus.Equals("networkExceptionRestore"))
        {
       
            isShowGameingNetworkWrongError = false;
            commonPlaneCom.closeHintMsg();
        }
        else if (systemStatus.Equals("resourceOverLoad")) {
            commonPlaneCom.closeHintMsg();
            tipsControl.dispalyCursorTips(false, TipsControl.TipesType.showExitTips);
            commonPlaneCom.showDialogOneBtExitDialog("resources_are_fully",errCode);
            //gameIsRuning = false;
            //gameStarted = false;
            exitCyberGame();//资源不足后需要退出游戏否则下次无法启动
        }
        else if (systemStatus.Equals(OpenApiImp.systemException))
        {
            MyTools.PrintDebugLog("ucvr errCode："+ errCode);
            if (errCode != null && errCode.Equals(CommonPlane.exitByAppSelf)&& gameStarted)//gameStarted游戏已启动，收到exitByAppSelf错误是主动退出，否则认为是异常，应用描述文件配错启动路径启动时也会返回这个错误码
            {
                MyTools.PrintDebugLog("ucvr exitByAppSelf ");
                exitCyberGame();//游戏内主动需要再主动调用exitCyberGame 否自底层库退不干净
            }
            else {
                commonPlaneCom.closeHintMsg();
                exitCyberGame();//游戏内主动需要再主动调用exitCyberGame 否自底层库退不干净
                tipsControl.dispalyCursorTips(false, TipsControl.TipesType.showExitTips);
                commonPlaneCom.showDialogOneBtExitDialog("Home_NoNet", errCode );
            }

              
        }
    }
    private bool isRunExitCyberGame = false;//exitCyberGame接口只允许一个线程调用
    /// <summary>
    /// 调用native接口退出云服务
    /// </summary>
    public void exitCyberGame() {
         if (isRunExitCyberGame)
            return;
        MyTools.PrintDebugLog("ucvr exitCyberService");
        isRunExitCyberGame = true;
        gameStarted = false;
      
        if (CyberCloudConfig.currentType == CyberCloudConfig.DeviceTypes.DaPeng)
        {
            ControllerTool.Reflection("changeRenderdHeadType", new object[] { CyberCloud_UnitySDKAPI.HeadBoxDofType.Dof3 });
        }
        commonPlaneCom.closeAllDialog();
        //退出游戏后切换回3dof

#if UNITY_ANDROID && !UNITY_EDITOR
        MyTools.stopDispatchEvent();
        if(_handler!=null)
            _handler.Call("Cyber_Stop");
        //本期牛博说无法给出退出通知所以此处省略退出回调的等待直接处理退出
        exitResult(0);
#endif
        if (OpenApiImp.CyberCloudOpenApiEnable)
        {
            OpenApiImp.getOpenApi().stopAsynNotify();
        }
        isRunExitCyberGame = false;
    }
    public void exitResult(int retCode) {

        tipsControl.dispalyCursorTips(false, TipsControl.TipesType.showExitTips);//游戏退出后隐藏提示
        commonPlaneCom.closeHintMsg();
        commonPlaneCom.closeSameScreen();
        isShowGameingNetworkWrongError = false;
        if (retCode == 0)
        {
            MyTools.PrintDebugLog("ucvr exitSuccess");
        }
        else
        {
            MyTools.PrintDebugLogError("ucvr exitFailed application will fore exit");
            #if picoSdk
            Pvr_UnitySDKAPI.Controller.UPvr_IsEnbleHomeKey(true);
#endif
            Application.Quit();
        }
        if (CyberCloudConfig.ExportOnlyPlayer && (CyberCloudConfig.adapt_platform == null || CyberCloudConfig.adapt_platform == ""))
        {
#if picoSdk
            Pvr_UnitySDKAPI.Controller.UPvr_IsEnbleHomeKey(true);
#endif
            Application.Quit();
            Debug.Log("ucvr qiangzhituichu");
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            gameIsRuning = false;
            gameStarted = false;

        }
        else
        {
            gameIsRuning = false;
            gameStarted = false;
            commonPlaneCom.getExitBt().SetActive(false);

            //情况焦点选择否自会响应两次
            UICamera.mMouseInit();
            gameObjEnable(false);
            //退出游戏后需要打开凝视或手柄操控点
            Debug.Log("ucvr openCursor");
            ControllerTool.CloseOrOpenAnyEnableContrlorCursor(CyberCloud_UnitySDKAPI.CursorStatus.openCursor);

        }
    }
    public void Cyber_HeadBoxEnable(int enable)
    {
        if (!gameIsRuning)
            return;
#if UNITY_ANDROID && !UNITY_EDITOR
        MyTools.PrintDebugLog("ucvr Cyber_HeadBoxEnable:" + enable);
        _handler.Call("Cyber_HeadBoxEnable", enable);
#endif



    }
    public void Cyber_ControlerEnable(int enable)
    {
        if (!gameIsRuning)
            return;
#if UNITY_ANDROID && !UNITY_EDITOR     
        MyTools.PrintDebugLog("ucvr Cyber_ControlerEnable:" + enable);
        _handler.Call("Cyber_ControlerEnable", enable);
#endif
    }
    /// <summary>
    /// 游戏渲染界面是否开启，
    /// 开启游戏渲染界面后需要关闭portal界面的显示
    /// </summary>
    /// <param name="enable"></param>
    private void gameObjEnable(bool enable) {
        cyberCloudPlayerPlaneShowHide(enable);
      
        if (leftMenu==null) {
            leftMenu=GameObject.Find("LeftMenu");                      
        }
        if(leftMenu!=null)
            leftMenu.SetActive(!enable);
        if (homePage == null)
        {
            homePage = GameObject.Find("HomePage");          
        }
        if (homePage != null)
        {
            if (CyberCloudConfig.adapt_platform == null || CyberCloudConfig.adapt_platform == "")//非应用适配平台才需要显示home界面
                homePage.SetActive(!enable);
        }
        commonPlaneCom.changeSkyBox(!enable);
    }
    /// <summary>
    /// 视博云播放面板的显示隐藏
    /// </summary>
    /// <param name="enable"></param>
    private void cyberCloudPlayerPlaneShowHide(bool enable) {
        leftPlane.SetActive(enable);
        rightPlane.SetActive(enable);
    }
}
