using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.CyberCloud.Scripts;
using Assets.CyberCloud.Scripts.OpenApi;
using Newtonsoft.Json;
using UnityEngine;
using static Assets.CyberCloud.Scripts.OpenApi.OpenApiImp;
using static XMPPTool;

public class TestDialog : MonoBehaviour
{

    [SerializeField]
    private List<UIEventListener> btList;
    CyberCloudOpenApi openApi;

    public UILabel requestResultText;

    public GameObject containerView;
    // Use this for initialization
    void Start() {
        //bt_ok.onClick = OnButtonClickOK;
        initParam.notify.setUILabel(requestResultText);//
        readConfig("sdkconfig.txt");//可以将测试测试参数放到sdcard的配置文件中，方便测试人员测试
        for (int i = 0; i < btList.Count; i++) {
            btList[i].onClick = onClick;
        }       
        openApi = new CyberCloudOpenApi();
    }

    // Update is called once per frame
    void Update() {

      //  playToken.text = CVRMsgNotify.playtoken!=null? CVRMsgNotify.playtoken:"";
    }
    //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<配置参数<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
    /// <summary>
    /// 初始化参数
    /// </summary>
    public class InitParam
    {
        /// <summary>
        /// 网关地址用于用户二次认证，以及获取应用启动地址，由视博云运营提供
        /// </summary>
       public string gatewayUrl = "http://10.10.6.63:10801/";
        /// <summary>
        /// 配置服务地址用于申请排队，由视博云运营提供
        /// </summary>
        public string configServer = "http://192.168.16.191:80";
        /// <summary>
        /// 终端编码格式、通讯协议、分辨率： VR_H265_UDP_3k = 624631811, VR_H265_UDP_4k = 624631809,  VR_H265_UDP_2k = 621748226，由视博云运营提供 
        /// </summary>
        public OpenApiImp.TerminalType terminalType = OpenApiImp.TerminalType.VR_H265_UDP_3k;
        /// <summary>
        /// 租户id，由视博云运营提供
        /// </summary>
        public string tenantID = "cybercloud";
        /// <summary>
        /// 测试用的userID
        /// </summary>
        public string userID = "userIDTest";

        /// <summary>
        /// 设备适配java类用于获取设备信息
        /// </summary>
        public string deviceinfoClassName = "com.cybercloud.vr.player.pico.PicoVRDevice";//piconeo2头盔使用的外设回调类，对接其他头盔后需要修改该设备信息实现类

        public CVRMsgNotify notify = new CVRMsgNotify();
        /**终端手柄类型Ctrl_None = 0    Ctrl_OculusCV1 = 1    Ctrl_Vive = 2    Ctrl_Nolo = 3    Ctrl_ViveIndex = 4    Ctrl_OculusRift = 5**/
        public TerminalControllerType controllerType = TerminalControllerType.Ctrl_Vive;
        /** 是否由终端控制帧率 **/
        public int useTerminalFrmRtCtrl=0;
        /// <summary>
        /// 是否使用终端投屏
        /// </summary>
        public int localProjectionEnable = 0;
        /// <summary>
        /// 日志级别  VERBOSE:2，  DEBUG:3， INFO:4， WARN:5， ERROR:6
        /// </summary>
        public int logOutLevel = 2;
    }
    /// <summary>
    /// 应用启动参数
    /// </summary>
    public class AppStartParam {
        /// <summary>
        /// 需要用于启动的应用id*
        /// </summary>
        public string appID = "60001110";
        /// <summary>
        /// 用户token，用于用户二次认证
        /// </summary>
        public string userToken = "userToken";
        /// <summary>
        /// 流化资源认证token，租户配置如果配置了key请在此处配置对应的authtoken否则无法启动游戏 
        /// </summary>
        public string authToken = "";
        public Dictionary<string, string> exParam = new Dictionary<string, string> { };
    }
    /// <summary>
    /// 排队参数
    /// </summary>
    public class QueueParam
    {
        /// <summary>
        /// 用户级别
        /// </summary>
        public int userLevel = 0;
        /// <summary>
        /// 扩展参数，如需指定帧率码率分辨率需传此参数，key示例:Resolution，FrameRate，BitRate
        /// </summary>
        public Dictionary<string, string> ext = new Dictionary<string, string> { };
    }
    public class StatisticsParam {
        public bool statisticsEnabel = false;
        public string statisticsAppName ="";
    }
    InitParam initParam = new InitParam();
    AppStartParam startParm = new AppStartParam();
    QueueParam queueParam = new QueueParam();
    StatisticsParam statisticsParam = new StatisticsParam();
    private int rightHandIndex = 1;//右手手柄索引
    private int audioEnable = 1;//是否开启声音
    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>配置参数>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
    void onClick(GameObject obj)
    {
        Debug.Log("ucvr onClick==:" + obj.name);
        initParam.notify.setUILabel(requestResultText);//测试时用于显示回调结果
        readConfig("sdkconfig.txt");//可以将测试测试参数放到sdcard的配置文件中，方便测试人员测试
  
        //初始化接口测试
        if (obj.name == "init")
        {
            //configServer = "";
            int code = openApi.init(initParam.gatewayUrl, initParam.configServer, initParam.terminalType, initParam.tenantID, initParam.logOutLevel, initParam.deviceinfoClassName, initParam.notify,
                initParam.useTerminalFrmRtCtrl==1?true:false,initParam.controllerType,initParam.localProjectionEnable==1?true:false);
            initParam.notify.resetUiLable("init return :" + code);
        }
        else if (obj.name == "startapp")
        {//启动应用接口测试
            string pt = CVRMsgNotify.playtoken;//获取缓存的playtoken，没有playtoken可以传空
            int code = openApi.startApp(startParm.appID, initParam.userID, startParm.userToken, pt, startParm.authToken, startParm.exParam);
            initParam.notify.resetUiLable("startapp return :" + code);
            CVRMsgNotify.playtoken = "";
        }
        else if (obj.name == "stopapp")
        {//停止应用接口测试
            int code = openApi.stopApp();
            initParam.notify.resetUiLable("stopapp return :" + code);
        }
        else if (obj.name == "castscreen")
        {//开启投屏接口测试
            int code = openApi.startCastScreen(true);
            initParam.notify.resetUiLable("castscreen return :" + code);
        }
        else if (obj.name == "castscreen_stop")
        {//停止投屏接口测试
            int code = openApi.stopCastScreen();
            initParam.notify.resetUiLable("stopCastScreen return :" + code);
        }
        else if (obj.name == "applyQueue")
        {//排队接口测试
            CVRMsgNotify.playtoken = "";
            CVRMsgNotify.QueueCode = "";
            int code = openApi.applyQueue(startParm.appID, initParam.userID, queueParam.userLevel, queueParam.ext);
            initParam.notify.resetUiLable("applyQueue return :" + code);
            // String result = "{\"ResultCode\":0,\"Description\":\"资源申请成功\",\"Data\":{ \"UserCode\":\"cyberclouduserid\",\"QueueCode\":null,\"CyberZoneCode\":\"55\",\"CyberEdgeCode\":1,\"UserLevel\":0,\"AppID\":\"700001076\",\"PlayToken\":\"eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpYXQiOiIyMDIxMDEwNSdUJzE2NDU0MCdaJyIsImV4cCI6IjIwMjEwMTA1J1QnMTY0NjAwJ1onIiwianRpIjoiNzQ2MzliNzc4NzE1NDQ4ZGE0ZTA2MmJkZGI4NjJiYTMiLCJub25jZSI6IjE1NTg0IiwiYXVkIjoiY3liZXJjbG91ZHVzZXJpZCIsImFwcGlkIjo3MDAwMDEwNzYsInRlcmlkIjo2MjQ2MzE4MTEsInJzY2lkIjoiMTkyLjE2OC4xNi4xODctcnRhLTIiLCJyaWQiOiIxLTE5Mi4xNjguMTYuMTg3LTIifQ == .4dfb50b6c10fbdb86b4750f3dd85e06dac15acccd02dd99d10496259ab3398f7\",\"QueueFlag\":\"QueueComplete\",\"QueueInfo\":null,\"OnlineInfo\":null,\"TermTypeID\":null,\"ExtInfo\":null,\"TenantID\":null,\"EventTime\":\"2021 - 01 - 05 16:45:44\",\"EnterQTime\":\"2021 - 01 - 05 16:45:44\",\"LastQueryTime\":\"2021 - 01 - 05 16:45:44\",\"QueryTime\":0}}";      
        }
        else if (obj.name == "queryQueue")
        {//查询排队接口测试
            // CVRMsgNotify.playtoken = "";
            int code = openApi.queryQueue(CVRMsgNotify.QueueCode);
            initParam.notify.resetUiLable("queryQueue return :" + code);
        }
        else if (obj.name == "cancelQueue")
        {//取消排队接口测试
            int code = openApi.cancelQueue(CVRMsgNotify.QueueCode);
            initParam.notify.resetUiLable("cancelQueue return :" + code);
            CVRMsgNotify.playtoken = "";
            CVRMsgNotify.QueueCode = "";
        }
        else if (obj.name == "dataStatisticsOption")
        {//时延数据统计接口测试
            int code = openApi.dataStatistics(statisticsParam.statisticsEnabel);
            initParam.notify.resetUiLable("dataStatisticsOption return:" + code);
        }
        else if (obj.name == "handDeviceInfoEnable")
        {//恢复手柄上信息上报到云端的接口测试
            int code = openApi.handDeviceInfoEnable(true);
            initParam.notify.resetUiLable("handDeviceInfoEnable return:" + code);
        }
        else if (obj.name == "handDeviceInfoUnEnable")
        {//停止手柄上报接口测试
            int code = openApi.handDeviceInfoEnable(false);
            initParam.notify.resetUiLable("handDeviceInfoUnEnable return:" + code);
        }
        else if (obj.name == "changeRightHandIndex")
        {//修改右手手柄索引接口测试
            int code = openApi.changeRightHandIndex(rightHandIndex);
            initParam.notify.resetUiLable("changeRightHandIndex return:" + code);
        }
        else if (obj.name == "quitapplication")
        {//退出apk
            openApi.stopApp();
            Application.Quit();
        }
        else if (obj.name == "on-off")
        {
            if (containerView.activeSelf)
            {
                containerView.SetActive(false);
            }
            else
                containerView.SetActive(true);
        }
        else if (obj.name == "audioEnable") {
            int code = openApi.audioEnable(audioEnable==1?true:false);
            initParam.notify.resetUiLable("audioEnable return:" + code);

        }
    }
    /// <summary>
    /// 排队接口返回json结构体
    /// </summary>
    public class QueueBody {
      

        public int ResultCode;
        public string Description;
        public DataBody Data;
        public class DataBody
        {
            public string UserCode;
            public string CyberZoneCode;
            public int CyberEdgeCode;
            public int UserLevel;
            public string AppID;
            public string QueueFlag;
            public string PlayToken;
            public QueueInfoBody QueueInfo;
        }
        public class QueueInfoBody
        {
            public string QueueCode;  
            public int AllQueueLength;
            public int AllQueuePosition;
        }
    }
    public class CVRMsgNotify : ICVRMsgNotify
    {
        public UILabel requestResultText;
        //private int uiRowNum = 0;
        /// <summary>
        /// 緩存playtoken
        /// </summary>
        public static string playtoken;//
        /// <summary>
        /// 排队队列标识
        /// </summary>
        public static string QueueCode;//
        public void setUILabel(UILabel ui) {
            requestResultText = ui;
        }
        public void initResult(int code) {
            string param = "asyn initResult code:" + code;
            resetUiLable(param);
        }
        public void appStatusCallback(StartStatus appStatus)
        {
            string param = "ucvr asyn appStatusCallback appStatus:" + appStatus;

            resetUiLable(param);
        }

        public void castScreenCallback(XMPPTool.CastScreen status,string checkCode)
        {
            string param = "ucvr asyn castScreenCallback status:" + status;//
            if (status == CastScreen.bindSTB) {
                param += ";checkCode:" + checkCode;
            }

            resetUiLable(param);
        }

        public void queueResult(int type, string result)
        {
            string param = "ucvr asyn queueResult type:" + type + ";result:" + result;
            Debug.Log("ucvr param:"+ param);
   
            QueueBody body = JsonConvert.DeserializeObject<QueueBody>(result);
            if (body.ResultCode!=0) {
                param = "asyn 接口调用失败 errorCode:" + body.ResultCode;
                resetUiLable(param);
                return;
            }
            if (QueueType.CYBER_QUEUE_APPLY_SUCCESS == type) {
              
                if (body.Data.QueueFlag == QueueFlag.QueueComplete)
                {
                    playtoken = body.Data.PlayToken;

                }
                if (body.Data.QueueFlag == QueueFlag.OnQueue)
                    QueueCode = body.Data.QueueInfo.QueueCode;
                param = "asyn 申请排队成功 QueueCode:" + QueueCode + " 如果QueueCode不为空需要调用查询排队接口";
               
            }
            else if (QueueType.CYBER_QUEUE_QUERY_SUCCESS == type)
            {
                if (body.Data.QueueFlag == QueueFlag.QueueComplete)
                    playtoken = body.Data.PlayToken;
                else if (body.Data.QueueFlag == QueueFlag.OnQueue)
                    QueueCode = body.Data.QueueInfo.QueueCode;
                param = "asyn 查询排队成功 QueueCode:" + QueueCode;
            }
            else if (QueueType.CYBER_QUEUE_CANCEL_SUCCESS == type)
            {
                param = "asyn 取消排队成功";

                playtoken = "";
            }else if (QueueType.CYBER_QUEUE_APPLY_ERROR == type)
            {
                param = "asyn 申请排队失败";
                playtoken = "";
            }else if (QueueType.CYBER_QUEUE_CANCEL_ERROR == type)
            {
                param = "asyn 取消排队失败";
                playtoken = "";
            }else if (QueueType.CYBER_QUEUE_QUERY_ERROR == type)
            {
                param = "asyn 查询排队失败";
                playtoken = "";
            }
            resetUiLable(param);
        }

        public void simpleShowDialog(int dialogType, string content, int time)
        {
            string param = "ucvr asyn simpleShowDialog dialogType:" + dialogType + ";content:" + content + ";time:" + time;

            resetUiLable(param);

        }

        public void systemStatusCallback(string systemStatus, string errCode)
        {
            string param = "ucvr asyn systemStatusCallback systemStatus:" + systemStatus + ";errCode:" + errCode;

            resetUiLable(param);

        }
        public void resetUiLable(string param)
        {
            string text2;
            NGUIText.WrapText(requestResultText.text, out text2);
            string[] lines = text2.Split('\n');
            //Debug.LogError("lines = " + lines.Length);
            int hour = DateTime.Now.Hour;
            int minute = DateTime.Now.Minute;
            int second = DateTime.Now.Second;
            string logtime = string.Format("{0:D2}:{1:D2}:{2:D2} ", hour, minute, second);
    
            if (lines.Length == 10)
            {
           
                requestResultText.text = "请求结果"+ logtime+" " + param + "\n"; ;
            } else
                requestResultText.text = requestResultText.text + logtime + " " + param + "\n";
            Debug.Log(param);
     
        }
    }
    /// <summary>
    /// 从sdcard中读取参数配置文件
    /// </summary>
    /// <param name="fileName"></param>
    private void readConfig(string fileName)
    {
        string cybertlsPath = Application.persistentDataPath + "/" + fileName;//
        bool localLoadSuceess = false;
#if UNITY_ANDROID && !UNITY_EDITOR
        cybertlsPath = "sdcard/"+fileName;
#endif
        MyTools.PrintDebugLog("ucvr loadsdkconfig " + cybertlsPath);
        localLoadSuceess = readLocalConfig(cybertlsPath);
        if (localLoadSuceess)//如果配置文件存在读取配置文件
        {
            try
            {
                CVRMsgNotify notify = new CVRMsgNotify();
                initParam.gatewayUrl = configValueByKey("gatewayUrl=");
                initParam.configServer = configValueByKey("configServer=");
                initParam.terminalType = (OpenApiImp.TerminalType)int.Parse(configValueByKey("terminalType="));
                initParam.tenantID = configValueByKey("tenantID="); ;
                initParam.userID = configValueByKey("userID=");

                initParam.deviceinfoClassName = configValueByKey("deviceinfoClassName=");//头盔使用的外设回调类，对接其他头盔后需要修改该设备信息实现类
                try
                {
                    initParam.controllerType = (TerminalControllerType)int.Parse(configValueByKey("controllerType=")); ;
                    initParam.localProjectionEnable =int.Parse(configValueByKey("localProjectionEnable=")); ;
                    initParam.useTerminalFrmRtCtrl = int.Parse(configValueByKey("useTerminalFrmRtCtrl=")); ;
                }
                catch (Exception ex2)
                {
                    MyTools.PrintDebugLogError("ucvr sdkconfig parse init error"+ex2.StackTrace);
                    initParam.notify.resetUiLable("sdkconfig 解析初始化参数失败请确认controllerType，useTerminalFrmRtCtrl，localProjectionEnable配置是否正确");
                }
                startParm.appID = configValueByKey("appID=");
                startParm.userToken = configValueByKey("userToken=");
                startParm.authToken = configValueByKey("authToken=");
                string exParamstr = configValueByKey("exParam=");
                if (exParamstr != null && exParamstr != "")
                {
                    string[] Items = exParamstr.Split(';');
                    startParm.exParam = new Dictionary<string, string>();
                    foreach (string item in Items)
                    {
                        string[] keyv = item.Split(':');
                        if (keyv != null && keyv.Length > 1)
                            startParm.exParam.Add(keyv[0], keyv[1]);
                    }
                }
                if (configValueByKey("logOutLevel=") != null)
                    initParam.logOutLevel = int.Parse(configValueByKey("logOutLevel="));
                string ext = configValueByKey("ext=");
                if (ext != null && ext != "")
                {
                    string[] Items = ext.Split(';');
                    queueParam.ext = new Dictionary<string, string>();
                    foreach (string item in Items)
                    {
                        string[] keyv = item.Split(':');
                        if (keyv != null && keyv.Length > 1)
                            queueParam.ext.Add(keyv[0], keyv[1]);
                    }
                }
                queueParam.userLevel = int.Parse(configValueByKey("userLevel="));
                statisticsParam.statisticsAppName = configValueByKey("statisticsAppName=");
                if (configValueByKey("statisticsEnabel=") != null)
                    statisticsParam.statisticsEnabel = int.Parse(configValueByKey("statisticsEnabel=")) == 1 ? true : false;
                if (configValueByKey("rightHandIndex=") != null)
                    rightHandIndex = int.Parse(configValueByKey("rightHandIndex="));
                if (configValueByKey("audioEnable=") != null)
                    audioEnable = int.Parse(configValueByKey("audioEnable="));
                
            
            }
            catch (Exception ex) {
                MyTools.PrintDebugLogError("ucvr sdkconfig parse error");
                initParam.notify.resetUiLable("sdkconfig 解析失败请确认文件配置是否出错 错误详情："+ex.StackTrace);
            }
        }
        else
        {
            MyTools.PrintDebugLogError("ucvr sdkconfig not exit use default starturl");
            initParam.notify.setUILabel(requestResultText);//测试时用于显示回调结果
            initParam.notify.resetUiLable("请将sdkconfig 放到sdcard目录后再测试");
        }
    }
    private string configValueByKey(string key)
    {
        try
        {
            return MyTools.configValueByKey(key, fileConfig);
        }
        catch (Exception e)
        {
            MyTools.PrintDebugLogError("ucvr configValueByKey key:" + key + ";" + e.Message);
        }
        return null;
    }
    /// <summary>
    /// 配置文件信息
    /// </summary>
    private string[] fileConfig = null;
    private bool readLocalConfig(string fileName)
    {
        try
        {
            if (File.Exists(fileName))
            {
                fileConfig = File.ReadAllLines(fileName);
                return true;
            }
        }
        catch (Exception e)
        {
            MyTools.PrintDebugLogError("ucvr readLocalConfig fileName:" + fileName + ";" + e.Message);
        }
        return false;
    }

    void OnEnable()
    {


    }
    void OnDisable()
    {

    }
 }
