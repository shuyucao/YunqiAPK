using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyberCloud.PortalSDK.vo;
using UnityEngine;
using static XMPPTool;

namespace Assets.CyberCloud.Scripts.OpenApi
{
    /// <summary>
    /// 1、增加OpenApiImp用于实现CyberCloudOpenApi方法，CyberCloudOpenApiEnable用于标识是否是sdk方式对外提供，sdk中默认是true不需要显示交互界面，视博云本身需要自身显示解码中为false。
    /// 2、GameAppControl中判断CyberCloudOpenApiEnable模式不再执行Awake方法
    /// 3、dataloader中判断CyberCloudOpenApiEnable为false才初始化portal
    /// 4、OpenApiImp启动应用a、调用DataLoader的skipPortalStartAppByIdOnAppStart通过网关获取启动地址和统计服务地址。 b、DataLoader调用GameAppControl的startApp进行应用启动，启动流程没有变
    /// </summary>
    public class OpenApiImp: MonoBehaviour
    {
        /// <summary>
        /// sdk中默认是true不需要显示交互界面，视博云本身需要自身显示时为false
        /// </summary>
        public static bool CyberCloudOpenApiEnable
        {
            get
            {
#if PicoSDK
                return false;
#else
                return true;
#endif
            }

        }
        private static OpenApiImp instance=null;
        public class ErrorCodeOpenApi {
            public static int success = 0;
            public static int canshucuowu = 20029001;      //参数错误
            public static int gamePlaneNull = 20029002;    //gamePlane找不到导致，请确保GamePlane组件已经添加到场景中
            public static int commonPlaneNull = 20029003;  // commonPlane 预设组件未加入场景中
            public static int initFrist = 20029004;        //尚未初始化
            public static int gamenotruning = 20029005;    //游戏未启动，方法需要在应用启动后调用
            public static int gamehadruning = 20029006;    //游戏已启动，方法需要在应用启动前调用
            public static int startAppInUnityEdit = 20029007;    //游戏无法在unityedit中启动请使用真机测试
            public static int gatewayFailed = 20029008;    //网关初始化失败
            public static int initNativeCyberCloudVRSdkFailed = 20029009;    //视博云navtive层sdk初始化失败
            public static int castScreenFailed = 20029010;    //投屏失败
            public static int configServiceInitErrorJar = 20029011;    //配置服务初始化错误
            public static int queueNeedConfigUrl = 20029012;    //排队接口需要配置服务地址
            public static int startAppDouble = 20029013;//重复启动应用
            public static int initDouble = 20029014;//请不要重复初始化
            public static int networkExceptionGameExit = 20029015;//网络超时应用退出
            public static int nettyJsonError = 20029016;
            public static int netWorkErrorGetNettyUrl = 20029017;

            public static int unknown = 20029030;    //未知异常
            public static int configInitFailed = 20029031;//配置服务初始化失败，会重新进行初始化直到初始化完成
        }
        private InitParam param;
        public InitParam getInitParam() {
            return param;
        }
        private GameAppControl gameAppControl;
        private CommonPlane commonPlaneCom;
        public static string systemException = "systemException";
        public static string castScreenFailed = "castScreenFailed";
        public static string exitByAppSelf = "exitByAppSelf";//下面四个原因会触发1:退出通知, 用户主动游戏内退出或游戏闪退

        public class InitParam {
            public string gatewayUrl;
            public int terminalType;
            public string tenantID;
            public int logOutLevel;
            public string deviceinfoClassName;
            public ICVRMsgNotify notify;
            public string configServiceUrl;
            public bool localProjectionEnable;
            public TerminalControllerType terminalControllerType = TerminalControllerType.Ctrl_Vive;
            public bool useTerminalFromRtCtrl = false;
        }
        public class QueueType
        {
            public static int CYBER_QUEUE_APPLY_ERROR = 517;
            public static  int CYBER_QUEUE_APPLY_SUCCESS = 518;
            public static  int CYBER_QUEUE_QUERY_ERROR = 520;
            public static  int CYBER_QUEUE_QUERY_SUCCESS = 521;
            public static  int CYBER_QUEUE_CANCEL_ERROR = 529;
            public static  int CYBER_QUEUE_CANCEL_SUCCESS = 530;
        }
        public class QueueFlag
        {
            public static string NotQueue = "NotQueue";//: 不在队列；
            public static string QueueComplete = "QueueComplete";//：排队完成，已经获取到资源
            public static string OnQueue = "OnQueue";//：在普通队列排队；
        }

        /// <summary>
        /// VR_H265_UDP_4k =624631809,// 0x253B2001
        /// VR_H265_UDP_3k= 624631811,// 0x253B2003
        /// VR_H265_UDP_2k =621748226 // 0x250F2002
        /// </summary>
        public enum TerminalType { VR_H265_UDP_3k = 624631811, VR_H265_UDP_4k = 624631809,  VR_H265_UDP_2k = 621748226 }
        public enum TerminalControllerType {Ctrl_None = 0,Ctrl_OculusCV1 = 1,Ctrl_Vive = 2,Ctrl_Nolo = 3,Ctrl_ViveIndex = 4,Ctrl_OculusRift = 5}
        /// <summary>
        /// appStarting： 应用启动中
        ///appStartDone：应用启动结束.收到此状态后终端需要将流化界面送显示（在此之前送显示会显示黑屏）
        ///appExiting：  应用开始退出。（本期为假异步）
        ///appExitDone： 应用退出结束。（本期为假异步）
        ///appStartFailed:应用启动失败
        /// </summary>
        public enum StartStatus {
            appStarting,//启动中
            appStartDone,//启动成功
            appExiting,//退出中
            appExitDone,//退出结束
           
        }

        private DelayCallBack delayCallBack;
         
        void Awake()
        {
            instance = this;
            delayCallBack = new DelayCallBack();
            MyTools.PrintDebugLog("ucvr OpenApiImp Awake");
        }

        void Start()
        {
        }

        //exitByReason1,//退出通知,用户主动游戏内退出或游戏闪退   0x34319005
        //exitByReason2,//长时间无操作退出                        0x34319015
        //exitByReason3,//用户被强制踢出                          0x34319012
        //exitByReason4,//账号异地登录                            0x34319009
        public static OpenApiImp getOpenApi() {
           // if (instance == null)
           //     MyTools.PrintDebugLogError("ucvr please wait a frame then get instance");
            return instance;
        }
        public bool initResult = false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gatewayUrl"></param>
        /// <param name="configServiceUrl"></param>
        /// <param name="terminalType"></param>
        /// <param name="tenantID"></param>
        /// <param name="logOutLevel">VERBOSE:2，DEBUG:3，INFO:4，WARN:5，ERROR:6请在商用模式下将日志级别调整到4或以上，否则由于日志输出太多会影响头盔性能哦</param>
        /// <param name="deviceinfoClassName"></param>
        /// <param name="notify"></param>
        /// <returns></returns>
        public int init(string gatewayUrl, string configServiceUrl, TerminalType terminalType, string tenantID, int logOutLevel, string deviceinfoClassName, ICVRMsgNotify notify,
            bool useTerminalFromRtCtrl = false, TerminalControllerType terminalControllerType = TerminalControllerType.Ctrl_Vive, bool localProjectionEnable=false)
        {
            if (initResult)
            {
                MyTools.PrintDebugLogError("ucvr 请不要重复初始化");
                return ErrorCodeOpenApi.initDouble;
            }
            MyTools.PrintDebugLog("ucvr gatewayUrl:" + gatewayUrl + ";terminalType:" + terminalType + ";tenantID:" + tenantID + ";logOutLevel:" + logOutLevel + ";deviceinfoClassName:" + deviceinfoClassName);
            if (gatewayUrl == null || gatewayUrl == "" || terminalType == 0 ||
                tenantID == null || tenantID == "" ||
                deviceinfoClassName == null || deviceinfoClassName == ""|| notify==null) {
                MyTools.PrintDebugLogError("ucvr init param error  **"); ;
                return ErrorCodeOpenApi.canshucuowu;
            }
            param = new InitParam();
             param.useTerminalFromRtCtrl = useTerminalFromRtCtrl;
            CyberCloudConfig.useTerminalFrmRtCtrl = useTerminalFromRtCtrl ? 1 : 0;
            CyberCloudConfig.controllerType =(int) terminalControllerType;
            MyTools.setControllerType(CyberCloudConfig.controllerType);
            MyTools.setUseTerminalFrmRtCtrl(CyberCloudConfig.useTerminalFrmRtCtrl);

            param.terminalControllerType = terminalControllerType;
            param.localProjectionEnable = localProjectionEnable;

            param.notify = notify;
            param.gatewayUrl = gatewayUrl;
            param.terminalType = (int)terminalType;
            
            CyberCloudConfig.tenantID = param.tenantID = tenantID;
            param.logOutLevel = logOutLevel;
            logoLevel(logOutLevel);
            MyTools.otherDeviceJarName = param.deviceinfoClassName = deviceinfoClassName;
            param.configServiceUrl = configServiceUrl;
            CyberCloudConfig.tls = param.gatewayUrl;//用于网关请求
            CyberCloudConfig.terminalType = param.terminalType;
            GameObject gamePlane = GameObject.Find("GamePlane");
            GameObject commonPlane = GameObject.Find("CyberCloudCommonPlane");
            if (commonPlane != null)
            {
                commonPlaneCom = commonPlane.GetComponent<CommonPlane>();
            }
            else
            {
                MyTools.PrintDebugLogError("ucvr CyberCloudCommonPlane mast contain CommonPlane");
                return ErrorCodeOpenApi.commonPlaneNull;
            }
            if (gamePlane != null)
            {
                gameAppControl = gamePlane.GetComponent<GameAppControl>();
            }
            else
                return ErrorCodeOpenApi.gamePlaneNull;
            DataLoader.Instance.onGetTenantList = OnGetTenantList;
            DataLoader.Instance.getTenantList(param.tenantID);
            return 0;
        }
        /// <summary>
        /// 获取到租户列表
        /// </summary>
        /// <param name="tenantList"></param>
        public void OnGetTenantList(RctTenantList tenantList) {
  
            bool en=tenantIDEnable(tenantList);
           // MyTools.PrintDebugLogError("ucvr========================== 2asynResult.OnGetTenantList:" + en);
            if (en)
            {
                if (param.configServiceUrl != "" && param.configServiceUrl != null)
                {
                    //初始化配置服务
#if UNITY_ANDROID && !UNITY_EDITOR
                int configResult= OpenApiJarMethod.initConfigService(param.configServiceUrl, param.terminalType, param.tenantID);
                    if (configResult != 0) {
                        MyTools.PrintDebugLogError("ucvr 配置服务初始化失败");
                        StartCoroutine(delayCallBack.initResult(ErrorCodeOpenApi.gatewayFailed));
                    }
#else
                    initResult = true;//unity 编辑器直接跳过初始化配置服务
#endif
                }
                else
                {
                  //  MyTools.PrintDebugLogError("ucvr========================== 2asynResult.OnGetTenantList:success");
                    StartCoroutine(delayCallBack.initResult(ErrorCodeOpenApi.success));
                }
            }
          
        }
        /// <summary>
        /// 判断租户id是否可用
        /// </summary>
        /// <param name="tenantList"></param>
        /// <returns></returns>
        private bool tenantIDEnable(RctTenantList tenantList) {
            bool tenantIDEnable = false;
            if (tenantList != null && tenantList.retCode == 0)
            {
                if (tenantList.dataList == null)
                {
                    Debug.LogError("ucvr 网关未配置租户");
                    StartCoroutine(delayCallBack.initResult(ErrorCodeOpenApi.gatewayFailed));
                    return false;
                }
                else
                {
                    foreach (TenantItem item in tenantList.dataList)
                    {
                        if (param.tenantID == item.tenantId)
                        {
                            tenantIDEnable = true;
                            break;
                        }
                    }
                    if (tenantIDEnable == false)
                    {
                        Debug.LogError("ucvr 租户id错误");
                        StartCoroutine(delayCallBack.initResult(ErrorCodeOpenApi.gatewayFailed));
                        return false;
                    }
                }
            }
            else
            {
                Debug.LogError("ucvr 网络问题或者网关未部署");
                StartCoroutine(delayCallBack.initResult(ErrorCodeOpenApi.gatewayFailed));
                return false;
            }
            return true;
        }
        /// <summary>
        /// 设置日志级别
        /// </summary>
        /// <param name="level">日志级别：VERBOSE:2，  DEBUG:3， INFO:4， WARN:5， ERROR:6</param>
        /// <returns>  </returns>
        private int logoLevel(int level) {
            CyberCloudConfig.logOutLevel = level;
            MyTools.setSdkLogLevel(CyberCloudConfig.logOutLevel);
            return 0;
        }
        TempCVRMsgNotify tempNotify;
        public ICVRMsgNotify notify()
        {
            if (tempNotify == null)
                tempNotify = new TempCVRMsgNotify();

            return tempNotify;
        }
        /// <summary>
        /// 中间回调类用于在上报第三方前做一些内部处理
        /// </summary>
        public class TempCVRMsgNotify : ICVRMsgNotify
        {

            public TempCVRMsgNotify()
            {

            }
            private ICVRMsgNotify getNotify()
            {
                if (OpenApiImp.getOpenApi() != null && OpenApiImp.getOpenApi().param != null && OpenApiImp.getOpenApi().param.notify != null)
                    return OpenApiImp.getOpenApi().param.notify;
                return null;
            }
            public void initResult(int code)
            {
                if (getNotify() != null)
                    getNotify().initResult(code);
            }
            private const string appStartDoneDesc = "启动流化成功";
            private const string appExitDoneDesc = "用户主动退出";
            public void appStatusCallback(StartStatus appStatus)
            {
                if (getNotify() != null)
                    getNotify().appStatusCallback(appStatus);
                if (appStatus == StartStatus.appStartDone) {
                    if (OpenApiImp.instance.param.configServiceUrl != "" && OpenApiImp.instance.param.configServiceUrl != null)
                    {
                        SessionParam.castScreenUrl = OpenApiImp.instance.gameAppControl.Cyber_GetCastScreenPlayUrl();
                     
                        OpenApiJarMethod.cyberInfoService_uploadAction(true, SessionParam.ctrl, OpenApiImp.instance.getDeskIp(SessionParam.castScreenUrl), OpenApiImp.instance.getSession(SessionParam.castScreenUrl), "0x00000001", appStartDoneDesc, OpenApiImp.instance.tempStartURl);
                        SessionParam.ctrl = "end";
                    }
                    MyTools.registerXmppBrodcast();//注册广播监听用于直连投屏
                }

                if (appStatus == StartStatus.appExitDone)
                {
                    if (OpenApiImp.instance.param.configServiceUrl != "" && OpenApiImp.instance.param.configServiceUrl != null)
                    {
                        if (SessionParam.errorCode == "")
                            OpenApiJarMethod.cyberInfoService_uploadAction(true, SessionParam.ctrl, OpenApiImp.instance.getDeskIp(SessionParam.castScreenUrl), OpenApiImp.instance.getSession(SessionParam.castScreenUrl), "0x00000000", appExitDoneDesc, OpenApiImp.instance.tempStartURl);
                        else {
                            MyTools.PrintDebugLog("ucvr game had trigger error no need uploadAction 用户主动退出");
                        }
                    }
                    MyTools.unInitCyberCloudVRActivityMethod();//注销广播
                }
            }

            public void castScreenCallback(XMPPTool.CastScreen status, string checkCode)
            {
                if (getNotify() != null)
                    getNotify().castScreenCallback(status, checkCode);

                
            }

            public void queueResult(int type, string result)
            {
                if (getNotify() != null)
                    getNotify().queueResult(type, result);
            }

            public void simpleShowDialog(int dialogType, string content, int time)
            {
                if (getNotify() != null)
                    getNotify().simpleShowDialog(dialogType, content, time);
            }

            public void systemStatusCallback(string systemStatus, string errCode)
            {//系统异常回调
                if (getNotify() != null)
                    getNotify().systemStatusCallback(systemStatus, errCode);
                if (systemStatus == "networkExceptionRestore")
                {
                    if (OpenApiImp.instance.param.configServiceUrl != "" && OpenApiImp.instance.param.configServiceUrl != null)
                    {
                        OpenApiJarMethod.cyberInfoService_changeStreamState(true);
                    }
                  
                }
                else if (systemStatus == "networkException")
                {
                    if (OpenApiImp.instance.param.configServiceUrl != "" && OpenApiImp.instance.param.configServiceUrl != null)
                        OpenApiJarMethod.cyberInfoService_changeStreamState(false);
                  
                }
                else {
                    SessionParam.errorCode = errCode;
                }
                if (OpenApiImp.instance.param.configServiceUrl != "" && OpenApiImp.instance.param.configServiceUrl != null)                
                    OpenApiJarMethod.cyberInfoService_uploadAction(true, SessionParam.ctrl, OpenApiImp.instance.getDeskIp(SessionParam.castScreenUrl), OpenApiImp.instance.getSession(SessionParam.castScreenUrl), errCode, systemStatus, OpenApiImp.instance.tempStartURl);
            }

        }
        private string getDeskIp(string playurl) {
    
            string ip="";
            int port=0;
            string sessionID="";
            parsePlayUrl(playurl, ref ip, ref port, ref sessionID);
            string deskip = port>0?ip + "-" + (port - 8000):ip;
            MyTools.PrintDebugLog("ucvr getDeskIp" + deskip);
            return deskip;
        }
        private string getSession(string playurl)
        {
      
            string ip = "";
            int port = 0;
            string sessionID = "";
            parsePlayUrl(playurl, ref ip, ref port, ref sessionID);
            return sessionID;
        }
        public void parsePlayUrl(string playurl, ref string ip, ref int port, ref string session)
        {
            // string str = "10.10.6.67:8001:973646265883563225:[1.213, 1.213, 1.213, 1.213]:[1.213, 1.213, 1.213, 1.213]:2";
            if (playurl != null && playurl.Length > 0)
            {
                string[] strs = playurl.Split(':');
                if (strs != null && strs.Length > 2)
                {
                    ip = strs[0];
                    port = int.Parse(strs[1]);
                    session = strs[2];                          
                }
            }
        
        }
        /// <summary>
        /// 用于启动流化应用。
        /// 1、调用DataLoader的skipPortalStartAppByIdOnAppStart通过网关获取启动地址和统计服务地址
        /// 2、DataLoader调用GameAppControl的startApp进行应用启动，启动流程没有变
        /// </summary>
        /// <param name="appID"></param>
        /// <param name="playToken">该token由排队完成时的json传返回，申请排队成功或者查询排队当排队结束时都会返回。 如果传空时表示不使用排队服务。</param>
        /// <param name="authToken">流化认证token。由租户后台通过jwt的方式生成，生成authToken的key由视博云提供，每次启动应用时请生成一个新的authToken，可以防止非法启动应用。中心网站后台可以针对每个租户配置前面用的key，如果key为空就不对authToken做校验否则需要对authToken做校验</param>
        /// <param name="ex">扩展参数，注意其中StartExtParam将被记录在使用记录中，所以可以用于订购账单对账使用</param>
        /// <returns></returns>
        public int startApp(string appID, string userID,string userToken,string playToken, string authToken, Dictionary<string, string> ex)
        {
            string exstr = "";
            //每次启动应用时都需要调用，如果不调用默认为不开启时延统计服务<<<<<<<<<<<<<<<<<<<<<<<<<<
            if (paramStatisticsOption != null)
            {
                CyberCloudConfig.statisticsUpLoad = paramStatisticsOption.statisticsUpLoad;
                GameAppControl.AppName = paramStatisticsOption.appName;
                paramStatisticsOption = new StatisticsOption();
            }
            else {
                CyberCloudConfig.statisticsUpLoad = 0;
                GameAppControl.AppName = "";
            }
            SessionParam.ctrl = "start";
            SessionParam.errorCode ="";
            SessionParam.castScreenUrl = "";
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            if (param == null|| !initResult)
            {
                MyTools.PrintDebugLogError("ucvr plase init frist**");
                return ErrorCodeOpenApi.initFrist;
            }
            else {
                if (GameAppControl.getGameRuning()) {
                    return ErrorCodeOpenApi.startAppDouble;
                }
                DataLoader.GetStartAppUrlFromCVRGatewayParam startparam = new DataLoader.GetStartAppUrlFromCVRGatewayParam();
                DataLoader.deviceSN = userID;
                startparam.appID = appID;
                startparam.operateUserIDUserID = DataLoader.deviceSN = userID;//
                startparam.userToken = userToken;
                startparam.authType = param.tenantID;//
                startparam.tenantID = param.tenantID;
            
                if (ex!=null) {                  
                          foreach (KeyValuePair<string, string> kv in ex) {
                       // Debug.Log();
                        if (kv.Key == "StartAppParam") {
                            exstr = exstr + "&" + kv.Key + "=" +MyTools.urlEncode(kv.Value,"GB2312");// WWW.EscapeURL(kv.Value,Encoding.GetEncoding("GB2312"));//必须用gb2312如果用utf8rta会出现乱码 不知道原因
                        }
                        else
                            exstr = exstr + "&" + kv.Key + "=" + kv.Value;
                    }
                }
                // _startParam = "UserID=" + userID + "&StartAppID=" + appID + "&TenantID=" + param.tenantID + "&playToken=" + playToken + "&BizAuthToken=" + authToken + exstr;
                _startParam = "UserID=" + userID + "&StartAppID=" + appID + "&TenantID=" + param.tenantID + "&StartTenantID=" + param.tenantID + "&playToken=" + playToken + "&BizAuthToken=" + authToken + exstr;
                MyTools.PrintDebugLog("ucvr param :" +_startParam);
                                           
                DataLoader.Instance.skipPortalStartAppByIdOnAppStart(startparam);//使用兼容api进行用户认证,并通过网关获取统计服务地址
            }
            SessionParam.ctrl = "start";
            if (param.configServiceUrl != "" && param.configServiceUrl != null)
            {
                OpenApiJarMethod.cyberInfoService_resetMap();
                OpenApiJarMethod.cyberInfoService_updateInfo(appID, userID, "", "", "");
               
                OpenApiJarMethod.cyberInfoService_addExtParam(exstr);
            }
            return 0;
        }
      
        /// <summary>
        /// 	用于停止流化应用。
        /// </summary>
        /// <param name="appID"></param>
        /// <returns></returns>
        public int stopApp()
        {
            if (param == null || !initResult)
            {
                MyTools.PrintDebugLogError("ucvr plase init frist**");
                return ErrorCodeOpenApi.initFrist;
            }
            gameAppControl.exitCyberGame();            
            return 0;
        }
        /// <summary>
        /// 游戏退出时异步通知第三方退出结果
        /// </summary>
        public void stopAsynNotify() {
            StartCoroutine(delayCallBack.appStatusCallback(StartStatus.appExiting));
            StartCoroutine(delayCallBack.appStatusCallback(StartStatus.appExitDone));
        }
        /// <summary>
        /// 开启投屏请在应用启动成功后调用。
        /// </summary>
        /// <param name="appID"></param>
        /// <returns></returns>
        public int startCastScreen(bool encodeStandAlone)
        {
            if (param == null || !initResult)
            {
                MyTools.PrintDebugLogError("ucvr plase init frist**");
                return ErrorCodeOpenApi.initFrist;
            }
            //是否使用新的投屏方式开启独立投屏编码，开启独立编码后会导致每个投屏多消耗游戏编码能力的1/4左右，但是会降低带宽，并且可以适配更多的机顶盒。建议用true
            CyberCloudConfig.newScreenProtocol = encodeStandAlone ? 1 : 0;
         
            if (GameAppControl.getGameStarted())
            {
                //1、gameAppControl中获取playerurl，CommonPlane中转掉XmppToll投屏
                gameAppControl.startCastScreenByOther();
            }
            else {
                MyTools.PrintDebugLogError("ucvr startCastScreen need gameruning");
                return ErrorCodeOpenApi.gamenotruning;
            }
            return 0;
        }
        /// <summary>
        /// 用于停止应用投屏。
        /// </summary>
        /// <param name="appID"></param>
        /// <returns></returns>
        public int stopCastScreen()
        {
            if (param == null || !initResult)
            {
                MyTools.PrintDebugLogError("ucvr plase init frist**");
                return ErrorCodeOpenApi.initFrist;
            }
            commonPlaneCom.closeSameScreen();
            return 0;
        }
        //<<<<<<<<<<<<<<<<<<<<<<<<排队<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<   
        public int applyQueue(string appID, string userID, int userLevel, Dictionary<string, string> list)
        {
            //注意Dictionary无法传递给android此处需要转换成字符串进行传递
            if (param == null || !initResult)
            {
                MyTools.PrintDebugLogError("ucvr plase init frist**");
                return ErrorCodeOpenApi.initFrist;
            }
            else if (appID == null || appID == "" || userID == null || userID == "")
            {
                MyTools.PrintDebugLogError("ucvr applyQueue param error  **"); ;
                return ErrorCodeOpenApi.canshucuowu;
            }
            else if (param.configServiceUrl == null || param.configServiceUrl == "") {
                MyTools.PrintDebugLogError("ucvr applyQueue need configServiceUrl  **"); ;
                return ErrorCodeOpenApi.queueNeedConfigUrl;
            }
            
            string ext = "";
            if (list != null && list.Count > 0) {//--------------
            
                foreach (KeyValuePair<string, string> kv in list)
                {
                    if (kv.Value != null&&kv.Value!="")
                    {
                        if (ext == "")
                        {
                            ext = kv.Key + ":" + kv.Value;
                        }
                        else
                        {
                            ext += ";" + kv.Key + ":" + kv.Value;
                        }
                    }
                }
            }
          
            MyTools.PrintDebugLog("ucvr appID:"+appID+ ";userID:"+ userID+ ";userLevel:"+ userLevel + " applyQueue:" +ext);
            return OpenApiJarMethod.applyQueue(appID, userID, userLevel, ext);
          
        }
        public int queryQueue(string queueCode)
        {
            if (param == null || !initResult)
            {
                MyTools.PrintDebugLogError("ucvr plase init frist**");
                return ErrorCodeOpenApi.initFrist;
            }else if (queueCode == null || queueCode == "")
            {
                MyTools.PrintDebugLogError("ucvr queryQueue param error  **"); ;
                return ErrorCodeOpenApi.canshucuowu;
            }else if (param.configServiceUrl == null || param.configServiceUrl == "")
            {
                MyTools.PrintDebugLogError("ucvr queryQueue  need configServiceUrl  **"); ;
                return ErrorCodeOpenApi.queueNeedConfigUrl;
            }
            MyTools.PrintDebugLog("ucvr queryQueue:"+ queueCode);
            return OpenApiJarMethod.queryQueue(queueCode);
        }
        public int cancelQueue(string queueCode)
        {
            if (param == null || !initResult)
            {
                MyTools.PrintDebugLogError("ucvr plase init frist**");
                return ErrorCodeOpenApi.initFrist;
            }else if (queueCode == null || queueCode == "")
            {
                MyTools.PrintDebugLogError("ucvr cancelQueue param error  **"); ;
                return ErrorCodeOpenApi.canshucuowu;
            }
            else if (param.configServiceUrl == null || param.configServiceUrl == "")
            {
                MyTools.PrintDebugLogError("ucvr cancelQueue need configServiceUrl  **"); ;
                return ErrorCodeOpenApi.queueNeedConfigUrl;
            }
            MyTools.PrintDebugLog("ucvr cancelQueue:" + queueCode);
            return OpenApiJarMethod.cancelQueue(queueCode);
        }
        //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

        public int handDeviceInfoEnable(bool enable)
        {
            MyTools.PrintDebugLogError("ucvr handDeviceInfoEnable enable:" + enable);
            if (GameAppControl.getGameRuning()==false)
            {
                MyTools.PrintDebugLogError("ucvr need runing game before handDeviceInfoEnable");
                return ErrorCodeOpenApi.gamenotruning;
            }
            int en = enable ? 1 : 0;
            gameAppControl.Cyber_ControlerEnable(en);
            return 0;
        }

        /// <summary>
        /// 统计日志上报开关，如果需要上报统计数据请调用该接口
        /// </summary>
        /// <param name="enable"></param>
        /// <param name="appName"></param>
        /// <param name="setOperateUserID"></param>
        /// <param name="setApkVersion"></param>
        /// <returns></returns>
        public int dataStatisticsOption(bool enable)
        {
            if (param == null || !initResult)
            {
                MyTools.PrintDebugLogError("ucvr plase init frist**");
                return ErrorCodeOpenApi.initFrist;
            }
            if (GameAppControl.getGameRuning()) {

                MyTools.PrintDebugLogError("ucvr 请在启动应用之前调用该方法");
                return ErrorCodeOpenApi.gamehadruning;
            }
            MyTools.PrintDebugLog("ucvr dataStatisticsOption:" + enable);
            paramStatisticsOption = new StatisticsOption();
            paramStatisticsOption.statisticsUpLoad = enable ? 1 : 0; ;
            //paramStatisticsOption.appName = appName;
            
            return 0;
        }
        /// <summary>
        /// 音频禁用和恢复
        /// </summary>
        /// <param name="enable"></param>
        /// <returns></returns>
        public int audioEnable(bool enable) {
            MyTools.PrintDebugLog("ucvr audioEnable enable:" + enable);
            if (param == null || !initResult)
            {
                MyTools.PrintDebugLogError("ucvr plase init frist**");
                return ErrorCodeOpenApi.initFrist;
            }
            if (!GameAppControl.getGameStarted())
            {

                MyTools.PrintDebugLogError("ucvr please call method after game startdone ");
                return ErrorCodeOpenApi.gamenotruning;
            }
            gameAppControl.Cyber_setAudioPlayEnable(enable?1:0);
            return 0;
        }
        
        /// <summary>
        /// 一次session期间有效参数
        /// </summary>
        private class SessionParam {
            public static string ctrl = "start";
            public static string errorCode = "";//启动过程中是否有错误发生
            public static string castScreenUrl = "";
        }
        StatisticsOption paramStatisticsOption;
        private class StatisticsOption {
            public int statisticsUpLoad=0;
            public string appName="";
        }
        /// <summary>
        /// 该接口需要在应用启动成功后调用,用于设置右手手柄的索引
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int changeRightHandIndex(int rightIndex)
        {
            MyTools.PrintDebugLog("ucvr rightIndex:" + rightIndex);
            if (param == null || !initResult)
            {
                MyTools.PrintDebugLogError("ucvr plase init frist**");
                return ErrorCodeOpenApi.initFrist;
            }
            int leftRightMode = rightIndex == 1 ? 1 : 0;//如果右手手柄是1就是右手模式否则就是左手模式，因为默认1是主手柄
            PlayerPrefs.SetInt(SetingDialog.leftRightMode, leftRightMode);
            //InterfaceControl.setLeftControllerIndex(leftindex);
            if(GameAppControl.getGameStarted())
                gameAppControl.updateControllerIndex();
            return 0;
        }
        /// <summary>
        /// 保证异步回调结果在同步回调之后调用，为此使用该延迟回调类
        /// </summary>
        private class DelayCallBack
        {          
            public IEnumerator initResult(int code) {
                yield return new WaitForEndOfFrame();
                instance.notify().initResult(code);
                if (code == 0)
                    instance.initResult = true;
            }
            /// <summary>
            /// 应用启停状态回调
            /// </summary>
            /// <param name="statusDescription"></param>
            public IEnumerator appStatusCallback(StartStatus appStatus) {
                yield return new WaitForEndOfFrame();
                instance.notify().appStatusCallback(appStatus);
            }
     
        }
        private string tempStartURl;
        /// <summary>
        /// 应用启动接口拼接的临时启动串
        /// </summary>
        private string _startParam;
        /// <summary>
        /// 如果有配置服务使用配置服务的gds，否则使用网关返回的gds地址
        /// </summary>
        /// <param name="tempStartUrl"></param>
        /// <returns></returns>
        public string getStartUrl(string tempStartUrl)
        {
            string startUrl="";
            string headstr = "CyberCloud";
            if (param.configServiceUrl != null && param.configServiceUrl != "")//如果有配置服务通过配置服务获取启动地址
            {
                string gds = OpenApiJarMethod.getGDSUrlByConfigService();

                if (gds!=null&&gds.IndexOf(headstr) <0) {
                    gds = headstr + "://" + gds;
                }
                startUrl = gds + "?" + _startParam;
            }
            else {
                if (tempStartUrl != "" && tempStartUrl != null)
                {
                    string[] arrGdsTemp = tempStartUrl.Split('?');
                    if (arrGdsTemp[0].IndexOf(headstr) < 0)
                    {
                        arrGdsTemp[0] = headstr + "://" + arrGdsTemp[0];
                    }
                    startUrl = arrGdsTemp[0] + "?" + _startParam;
                }
            }
            tempStartURl = startUrl;
            OpenApiJarMethod.cyberInfoService_uploadAction(true, SessionParam.ctrl, "", "", "0x00000002", "开始流化", tempStartURl);
        
            return startUrl;
        }
    }
}
