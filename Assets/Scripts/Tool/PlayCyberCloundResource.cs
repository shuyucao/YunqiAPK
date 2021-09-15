using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.CyberCloud.Scripts;
using Assets.CyberCloud.Scripts.OpenApi;
using UnityEngine;
using static Assets.CyberCloud.Scripts.OpenApi.OpenApiImp;
using static XMPPTool;
using UnityEngine.UI;
using Assets.Scripts.Util;
using Assets.Scripts.Constant;
using Assets.Scripts.Data;

namespace Assets.Scripts.Tool
{
    class PlayCyberCloundResource : MonoSingleton<PlayCyberCloundResource>
    {
        CyberCloudOpenApi openApi;
        public bool isInit = false;
        void Start()
        {
            openApi = new CyberCloudOpenApi();
        }
        private class InitParam
        {
            /// <summary>
            /// 网关地址用于用户二次认证，以及获取应用启动地址，由视博云运营提供
            /// </summary>
            public string gatewayUrl = CommonConstant.SERVER_URL_SBY_PROXY;
            /// <summary>
            /// 配置服务地址用于申请排队，由视博云运营提供
            /// </summary>
            public string configServer = "";
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
            public string userID/* = CommonUtil.DecryptDes(LoginWindowData.Instance.ReadAccountLoginResult().user_id, LoginWindowData.Instance.ReadSessionResult().session_key)*/;

            /// <summary>
            /// 设备适配java类用于获取设备信息
            /// </summary>
            public string deviceinfoClassName = "com.cybercloud.vr.player.pico.PicoVRDevice";//piconeo2头盔使用的外设回调类，对接其他头盔后需要修改该设备信息实现类

            public CVRMsgNotify notify = new CVRMsgNotify();

            /**终端手柄类型Ctrl_None = 0    Ctrl_OculusCV1 = 1    Ctrl_Vive = 2    Ctrl_Nolo = 3    Ctrl_ViveIndex = 4    Ctrl_OculusRift = 5**/
            public TerminalControllerType controllerType = TerminalControllerType.Ctrl_Vive;
            /** 是否由终端控制帧率 **/
            public int useTerminalFrmRtCtrl = 0;
            /// <summary>
            /// 是否使用终端投屏
            /// </summary>
            public int localProjectionEnable = 0;
            /// <summary>
            /// 日志级别  VERBOSE:2，  DEBUG:3， INFO:4， WARN:5， ERROR:6
            /// </summary>
            public int logOutLevel = 6;
        }
        private class QueueBody
        {


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
        private class CVRMsgNotify : ICVRMsgNotify
        {
            public Text requestResultText = GameObject.Find("ShowMsg").GetComponent<Text>();
            /// <summary>
            /// 緩存playtoken
            /// </summary>
            public static string playtoken;
            /// <summary>
            /// 排队队列标识
            /// </summary>
            public static string QueueCode;
            public void appStatusCallback(StartStatus appStatus)
            {
                string param = "ucvr asyn appStatusCallback appStatus:" + appStatus;
                UniversalLoadingWindowData.Instance.AppStatusListen = appStatus.ToString();
                //UniversalLoadingWindowData.Instance.GetPlayStatus = appStatus.ToString();
                resetUiLable(param);
            }

            public void castScreenCallback(XMPPTool.CastScreen status, string checkCode)
            {
                string param = "ucvr asyn castScreenCallback status:" + status;//
                Debug.Log("投屏状态"+ status.ToString());
                UniversalLoadingWindowData.Instance.checkCode = checkCode;
                UniversalLoadingWindowData.Instance.CastScreenListen = status.ToString();

                if (status == CastScreen.bindSTB)
                {
                    param += ";checkCode:" + checkCode;
                }

                resetUiLable(param);
            }

            public void initResult(int code)
            {
                string param = "asyn initResult code:" + code;
                resetUiLable(param);
            }

            public void queueResult(int type, string result)
            {
                string param = "ucvr asyn queueResult type:" + type + ";result:" + result;
                Debug.Log("ucvr param:" + param);

                QueueBody body = JsonUtility.FromJson<QueueBody>(result);
                if (body.ResultCode != 0)
                {
                    param = "asyn 接口调用失败 errorCode:" + body.ResultCode;
                    resetUiLable(param);
                    return;
                }
                if (QueueType.CYBER_QUEUE_APPLY_SUCCESS == type)
                {

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
                }
                else if (QueueType.CYBER_QUEUE_APPLY_ERROR == type)
                {
                    param = "asyn 申请排队失败";
                    playtoken = "";
                }
                else if (QueueType.CYBER_QUEUE_CANCEL_ERROR == type)
                {
                    param = "asyn 取消排队失败";
                    playtoken = "";
                }
                else if (QueueType.CYBER_QUEUE_QUERY_ERROR == type)
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
                int hour = DateTime.Now.Hour;
                int minute = DateTime.Now.Minute;
                int second = DateTime.Now.Second;
                string logtime = string.Format("{0:D2}:{1:D2}:{2:D2} ", hour, minute, second);
                //requestResultText.text = "请求结果" + logtime + "结果：" + param + "End";
                Debug.Log("请求结果" + logtime + "结果：" + param + "End");
            }
        }
        private class AppStartParam
        {
            /// <summary>
            /// 需要用于启动的应用id*
            /// </summary>
            public string appID = "60000001";
           // public string appID = "700001076";
            /// <summary>
            /// 用户token，用于用户二次认证
            /// </summary>
            public string userToken  /*=LoginWindowData.Instance.ReadAccountLoginResult().access_token*/;
            /// <summary>
            /// 流化资源认证token，租户配置如果配置了key请在此处配置对应的authtoken否则无法启动游戏 
            /// </summary>
            public string authToken = "";
            public Dictionary<string, string> exParam = new Dictionary<string, string> { };

        }

        InitParam initParam;
        AppStartParam startParm = new AppStartParam();
        public void InitCyberCloudApp()
        {
            if (isInit)
            {
                Debug.Log("已经初始化成功，请勿重复初始化");
                return;
            }
            else
            {
                openApi = new CyberCloudOpenApi();
                initParam = new InitParam();
                //Debug.Log("SBY:" + CommonConstant.SERVER_URL_SBY_PROXY);
                //Debug.Log("initParam:" + initParam.gatewayUrl);
                int code = openApi.init(initParam.gatewayUrl, initParam.configServer, initParam.terminalType, initParam.tenantID, initParam.logOutLevel, initParam.deviceinfoClassName, initParam.notify,
                    initParam.useTerminalFrmRtCtrl == 1 ? true : false, initParam.controllerType, initParam.localProjectionEnable == 1 ? true : false);
                UniversalLoadingWindowData.Instance.GetCyberCloudInitCode = code;
                initParam.notify.resetUiLable("init return :" + code);
                if (code == 0)
                {
                    Debug.Log("初始化成功，可以启动APP了");
                    isInit = true;
                }
                else
                {
                    return;
                }
            }
        }

        public void StartPlayApp(string resName,string resSrc,string resPlayMode)
        {
            int code2 = openApi.dataStatistics(true);
            initParam.notify.resetUiLable("dataStatisticsOption return:" + code2);
            startParm.exParam.Clear();
            startParm.exParam.Add("StartAppParam", GetTotalSrc(resName, resSrc, resPlayMode));
            string pt = CVRMsgNotify.playtoken;//获取缓存的playtoken，没有playtoken可以传空
            if (LoginWindowData.Instance.ReadAccountLoginResult()!=null)
            {
                startParm.userToken = LoginWindowData.Instance.ReadAccountLoginResult().access_token;
                initParam.userID = CommonUtil.DecryptDes(LoginWindowData.Instance.ReadAccountLoginResult().user_id, LoginWindowData.Instance.ReadSessionResult().session_key);
            }
            else
            {
                startParm.userToken = LoginWindowData.Instance.ReadToKenSwapResult().data.access_token;
                initParam.userID = CommonUtil.DecryptDes(LoginWindowData.Instance.ReadToKenSwapResult().data.user_id, LoginWindowData.Instance.ReadSessionResult().session_key);
            }
            int code = openApi.startApp(startParm.appID, initParam.userID, startParm.userToken, pt, startParm.authToken, startParm.exParam);
            UniversalLoadingWindowData.Instance.GetCyberCloudPlayCode = code;
            initParam.notify.resetUiLable("startapp return :" + code);
            CVRMsgNotify.playtoken = "";
        }

        public void StopApp()
        {
            int code = openApi.stopApp();
            initParam.notify.resetUiLable("stopapp return :" + code);
        }


        private string GetTotalSrc(string name, string src,string playMode)
        {
            string totalSrc = "";
            totalSrc = "ncetlabshell:channel=NCET&playmode="+ playMode + "&name=" + name + "&url=" + src;
            Debug.Log("启动串："+totalSrc);
            return totalSrc;
        }

        public void OnCastScreenClick()
        {
            int code = openApi.startCastScreen(true);
            initParam.notify.resetUiLable("castscreen return :" + code);
        }

        public void StopCastScreen()
        {
            int code = openApi.stopCastScreen();
            initParam.notify.resetUiLable("stopCastScreen return :" + code);
        }
    }
}
