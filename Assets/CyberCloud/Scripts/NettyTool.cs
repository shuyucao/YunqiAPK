using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
/**
 * netty投屏服务
 * 目前只用到了状态常量定义其他投屏逻辑还在XMPPTool中实现
 * */
public class NettyTool : MonoBehaviour {

    // Use this for initialization
    public static string OpenVRSTB =     "VROpenSTB";
    public static string VRSTBReady =    "VRSTBReady";
    public static string lantingsx =     "VRSTBOK";//兰亭没定义清楚用VRSTBReady还是VRSTBOK 两个都解析吧 
    public static string VRSTBError =    "VRSTBError";
    public static string VRDeviceConnetError = "VRDeviceConnetError";
    public static string VRVideoInfo =   "VRVideoInfo";
    public static string VRVideoReady =  "VRVideoReady";
    public static string closeVRSTB =    "VRCloseSTB";
    public static string functionCall = "functionCall";
    public static string platform =     "VRGame";
    public static string HeartBeat =    "VRHeartBeatToHmd";//机顶盒向头盔发心跳
    public static string HeartBeatFromTerminal = "VRHeartBeatToSTB";//向机顶盒发心跳;//兰亭说HeartBeatFromTerminal会被过滤
    private CommonPlane commonPlaneCom;
    /// <summary>
    /// 尝试xmpp的连接时间
    /// </summary>
    private float tryconnectTime = 0;
    void Start ()
    {
        GameObject commonPlane = GameObject.Find("CyberCloudCommonPlane");
        if (commonPlane == null)
            MyTools.PrintDebugLogError("ucvr commonPanel mast added in screne");
        commonPlaneCom = commonPlane.GetComponent<CommonPlane>();
        if (CyberCloudConfig.currentType == CyberCloudConfig.DeviceTypes.DaPeng) {

            XMPPHelper.Instance.SetOnMessageListener(new XMPPHelper.OnMessageListener(OnMessage));
           
        }
    }
    void OnMessage(string message)
    {
        MyTools.PrintDebugLog("ucvr dpn On message" + message);
        messagePro(message);
    }
    private string serviceName = "com.cloud.HWRouteService";

    private StartServiceRepeat startServiceRepeatTimes = StartServiceRepeat.zero;//重新OpenVRSTB
    private StartServiceRepeat heartBeatRepeatTimes = StartServiceRepeat.zero;
    public enum StartServiceRepeat
    {
        zero,
        startOnce,
        noneedstart
    }
    // 
    void Update() {
        if (castScreen == CastScreen.CastScreenTryConnect)
        {
            tryconnectTime = tryconnectTime + Time.deltaTime;
            if (tryconnectTime > 5 && startServiceRepeatTimes == StartServiceRepeat.zero)
            {//5秒未响应再重试一次

                if (CyberCloudConfig.autoStartCastScreenTestService == 1 && MyTools.isServiceRunning(serviceName) == false)
                {
                    MyTools.PrintDebugLogError("ucvr xmpp CastScreen.startOnce");
                    MyTools.startService("com.cloud", LoadConfig.action);
                    startServiceRepeatTimes = StartServiceRepeat.startOnce;
                }
                else
                {
                    MyTools.PrintDebugLogError("ucvr xmpp CastScreen.noneedstart");
                    startServiceRepeatTimes = StartServiceRepeat.noneedstart;//服务还在的时候不用再启动服务
                }

            }
            else if (startServiceRepeatTimes == StartServiceRepeat.startOnce) {//5秒重启后等一帧再发广播，保证服务能起来
                sendBroadcastOpenVRSTB(playUrl);
            }
            else if (tryconnectTime > 10)//开始投屏后10秒未连接成功返回失败
            {
                castScreen = CastScreen.noCastScreen;
                startServiceRepeatTimes = StartServiceRepeat.zero;
                MyTools.PrintDebugLogError("ucvr xmpp connet time out 10 s");
                if (GameAppControl.getGameRuning())                
                    commonPlaneCom.showHintMstByDesckey("Home_NoNet", 3);
            }
        }
        else {
            tryconnectTime = 0;
            startServiceRepeatTimes = StartServiceRepeat.zero;

            if (castScreen == CastScreen.CastScreening) {

                if (Time.time - heartBeatTime > 10 && heartBeatRepeatTimes == StartServiceRepeat.zero)
                {//超过10秒未响应判断一次服务状态
                    heartBeatRepeatTimes = StartServiceRepeat.startOnce;
                }
                else if (heartBeatRepeatTimes == StartServiceRepeat.startOnce) {
                    if (CyberCloudConfig.autoStartCastScreenTestService == 1 && MyTools.isServiceRunning(serviceName) == false)
                    {
                        MyTools.PrintDebugLogError("ucvr xmpp StartServiceRepeat.startOnce");
                        MyTools.startService("com.cloud", LoadConfig.action);
                    }
                }
                else if (Time.time - heartBeatTime > 30)
                {//投屏中超过30秒未响应返回投屏失败
                    MyTools.PrintDebugLogError("ucvr heartBeat outtime ");
                    if (GameAppControl.getGameRuning())
                        commonPlaneCom.showHintMstByDesckey("Home_NoNet", 3);
                    castScreen = CastScreen.noCastScreen;
                    //sendBroadcastClose();
                }
            }
                
        }
    }

    private IEnumerator heartBeatLoop()
    {
        while (castScreen == CastScreen.CastScreening)
        {
            sendBroadcastHeartBeat();
            yield return new WaitForSeconds(0.3f);//等到该帧结束
        }
    }
    private string playUrl;
    /// <summary>
    /// 启动投屏
    /// </summary>
    /// <param name="playUrl"></param>
    public void sendBroadcastOpenVRSTB(string playUrl)
    {
        MyTools.PrintDebugLog("ucvr sendBroadcastOpenVRSTB:"+ castScreen);
        heartBeatRepeatTimes = StartServiceRepeat.zero;//开启投屏心跳次数恢复成0
        try
        {      
            
            castScreen = CastScreen.CastScreenTryConnect;
            this.playUrl = playUrl;
            XMPPData data = new XMPPData();
            data.functionType = OpenVRSTB;
            data.isVR = 1;

            data.action = functionCall;
            string jsonParas = JsonConvert.SerializeObject(data);
            sendBroadcast(jsonParas);
        }
        catch (Exception e)
        {
            castScreen = CastScreen.noCastScreen;
            MyTools.PrintDebugLogError("ucvr sendBroadcastOpenVRSTB:" + e.Message);
        }
    }
   
    public void sendBroadcastClose()
    {
        if (castScreen != CastScreen.CastScreening)
            return;
        MyTools.PrintDebugLog("ucvr sendBroadcastClose");
        try
        {
            castScreen = CastScreen.noCastScreen;
            XMPPData data = new XMPPData();
            data.functionType = closeVRSTB;
            data.isVR = 1;
    
            data.action = functionCall;
            string jsonParas = JsonConvert.SerializeObject(data);
            sendBroadcast(jsonParas);
        }
        catch (Exception e) {
            castScreen = CastScreen.noCastScreen;
            MyTools.PrintDebugLogError("ucvr sendBroadcastClose:"+e.Message);
        }
    }
    public void sendBroadcastHeartBeat()
    {
        if (castScreen != CastScreen.CastScreening)
            return;
        MyTools.PrintDebugLog("ucvr HeartBeatFromTerminal");
        try
        {
            //castScreen = CastScreen.noCastScreen;
            XMPPData data = new XMPPData();
            data.functionType = HeartBeatFromTerminal;
            data.isVR = 1;

            data.action = functionCall;
            string jsonParas = JsonConvert.SerializeObject(data);
            sendBroadcast(jsonParas);
        }
        catch (Exception e)
        {
            MyTools.PrintDebugLogError("ucvr HeartBeatFromTerminal:" + e.Message);
        }
    }
    /// <summary>
    /// 向service发送真正的播放地址
    /// </summary>
    /// <param name="playUrl"></param>
    private void sendBroadcastVRSTBVRVideoInfo(string playUrl){
        MyTools.PrintDebugLog("ucvr sendBroadcast VRVideoInfo");
        try
        {
            XMPPData data = new XMPPData();
            data.functionType = VRVideoInfo;
            data.isVR = 1;
            data.platform = platform;
            data.action = functionCall;
            data.playUrl = playUrl;
            string jsonParas = JsonConvert.SerializeObject(data);
            sendBroadcast(jsonParas);
        }
        catch (Exception e) {
            castScreen = CastScreen.noCastScreen;
            MyTools.PrintDebugLogError("ucvr sendBroadcast VRVideoInfo:" + e.Message);
        }
    }

    public void sendBroadcast(string jsonParas)
    {

        //SendXMPPMsg(jsonParas);
        if (CyberCloudConfig.currentType == CyberCloudConfig.DeviceTypes.DaPeng)
        {
            if (CyberCloudConfig.DeepoonCastScreenDemonstration == "1")
            {
                MyTools.sendBroadcast(jsonParas);
            }
            else
                XMPPHelper.Instance.SendBroadcast(jsonParas, "");
        }
        else
        {         
                MyTools.sendBroadcast(jsonParas);
        }
    }
    public enum CastScreen{
        noCastScreen,
        CastScreenTryConnect,
        CastScreening
    }
    public static CastScreen castScreen = CastScreen.noCastScreen;
 
    /// <summary>
    /// 投屏消息回调
    /// </summary>
    /// <param name="jsonStr"></param>
    public void CSMessageCallBack(string jsonStr)
    {
        //大鹏使用的自己实现的jar接口
        if (CyberCloudConfig.currentType != CyberCloudConfig.DeviceTypes.DaPeng)
            messagePro(jsonStr);
        else if (CyberCloudConfig.DeepoonCastScreenDemonstration == "1")//大鹏测试使用
        {
            messagePro(jsonStr);
        }
    }
    /// <summary>
    /// 初始化消息回调
    /// </summary>
    /// <param name="jsonStr"></param>
    public void CSInitResult(string result)
    {
  
    }
    private float heartBeatTime = 0;
    public delegate void TestCastScreenMessageCallBack(string jsonStr);
    public TestCastScreenMessageCallBack testCastScreenMessageCallBack;
    private void messagePro(string jsonStr) {
        if(jsonStr.IndexOf(HeartBeat) <0)
            MyTools.PrintDebugLog("ucvr Received Msg:" + jsonStr);
        if (testCastScreenMessageCallBack != null)
        {
            testCastScreenMessageCallBack(jsonStr);
         
            return;
        }
        else {

        }
        if (jsonStr != null && !jsonStr.Equals(""))
        {
            //XMPPData r= JsonConvert.DeserializeObject<XMPPData>(jsonStr);
            //if (r.functionType == VRSTBReady) {
            //兰亭有个bug在close的时候发送了VRSTBReady，牛博说我程序健壮性的问题 好吧我忍了 此处加个判断吧 castScreen ==CastScreen.CastScreenTryConnect;
            if ((jsonStr.IndexOf(VRSTBReady) > -1 || jsonStr.IndexOf(lantingsx) > -1) && castScreen == CastScreen.CastScreenTryConnect)
            {
                sendBroadcastVRSTBVRVideoInfo(playUrl);
                castScreen = CastScreen.CastScreenTryConnect;
            }
            else if (jsonStr.IndexOf(VRSTBError) > -1)
            //else if (r.functionType == VRSTBError)
            {
                castScreen = CastScreen.noCastScreen;
                //处理异常       
                if (GameAppControl.getGameRuning())
                    commonPlaneCom.showHintMstByDesckey("Screen_casting_failed"  , 3, VRSTBError);
            }
            else if (jsonStr.IndexOf(VRVideoReady) > -1)
            //else if (r.functionType == VRVideoReady)
            {
                repeatTimes = 0;
                castScreen = CastScreen.CastScreening;
                heartBeatTime = Time.time;
                StartCoroutine(heartBeatLoop());
            }
            else if (jsonStr.IndexOf(HeartBeat) > -1&& jsonStr.IndexOf(HeartBeatFromTerminal) == -1)
            {//HeartBeatFromTerminal也会返回
                MyTools.PrintDebugLog("ucvr Received heartBeatTime time" + Time.time);
                heartBeatTime = Time.time;
                if (castScreen == CastScreen.noCastScreen)
                {
                    repeatTimes = repeatTimes + 1;
                    //投屏中断后再收到心跳时连接投屏状态
                    if (GameAppControl.getGameRuning() && repeatTimes > 5)
                    {
                        repeatTimes = 0;
                        heartBeatRepeatTimes = StartServiceRepeat.zero;//心跳恢复重试次数恢复0
                        castScreen = CastScreen.CastScreening;
                    }
                }


            }
        }
    }
    //心跳断开后重连的次数
    private int repeatTimes = 0;

}
