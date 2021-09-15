using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using Assets.CyberCloud.Scripts.OpenApi;
using static Assets.CyberCloud.Scripts.OpenApi.OpenApiImp;

public class XMPPTool : MonoBehaviour {

    // Use this for initialization
    public static string OpenVRSTB = "openVRSTB";
    public static string VRSTBReady = "VRSTBReady";
    public static string lantingsx = "VRSTBOK";//因兰亭未定义清楚用VRSTBReady还是VRSTBOK 所以两个都解析吧 
    public static string VRSTBError = "VRSTBError";
    public static string VRVideoInfo = "VRVideoInfo";
    public static string VRVideoReady = "VRVideoReady";
    public static string VRFirstFrame = "VRFirstFrame";//机顶盒解码首个I帧
    public static string closeVRSTB = "closeVRSTB";
    private static string functionCall = "functionCall";
    private static string platform = "VRGame";
    private static string HeartBeat = "CyberCloudHeartBeat";//CyberCloudHeartBeat
    private static string HeartBeatFromTerminal = "VRHeadInfo";//"HeartBeatFromTerminal";//兰亭说HeartBeatFromTerminal会被过滤，因此改成了VRHeadInfo
    private static string VRDeviceSetboxNeedUpdateCancel = "VRDeviceSetboxNeedUpdateCancel";//机顶盒升级窗口取消按钮
    private static string VRSetBoxUpdateStart = "VRSetBoxUpdateFailed";//机顶盒开始升级,之前只定义了升级失败因为升级成功机顶盒就不知道了
    private static string VRDeviceSetboxUpdateDialog = "VRDeviceSetboxUpdateDialog";//机顶盒弹出了升级窗口
    private CommonPlane commonPlaneCom;
    /// <summary>
    /// 尝试xmpp的连接时间
    /// </summary>
    private float tryconnectTime = 0;
    void Start()
    {
        GameObject commonPlane = GameObject.Find("CyberCloudCommonPlane");
        if (commonPlane == null)
            MyTools.PrintDebugLogError("ucvr commonPanel mast added in screne");
        commonPlaneCom = commonPlane.GetComponent<CommonPlane>();
        if (CyberCloudConfig.currentType == CyberCloudConfig.DeviceTypes.DaPeng) {

            XMPPHelper.Instance.SetOnMessageListener(new XMPPHelper.OnMessageListener(OnMessage));

        }
    }
    //投屏断开连接
    void disConnetCast() {
        castScreen = CastScreen.noCastScreen;
        if (nettyApiEnable) {//端口netty连接
            MyTools.disconnetNettyCastScreen();
        }
    }
    void OnMessage(string message)
    {
        MyTools.PrintDebugLog("ucvr dpn On message" + message);
        messagePro(message);
    }
    void Update() {
        if (OpenApiImp.getOpenApi().getInitParam() != null && OpenApiImp.getOpenApi().getInitParam().localProjectionEnable) {
            localProjectionVRGameruningSTB(Time.time);
        }
        if (nettyApiEnable)//netty 本身有超时机制
        {
            if (castScreen == CastScreen.castScreening || castScreen == CastScreen.castScreenTryConnect)//netty open後就發心跳。升级场景处于CastScreenTryConnect但是会有心跳
            {
                //如果有升级弹框的话就时时更新心跳时间
                if (ISVRDeviceSetboxUpdateDialog || (GameAppControl.getGameRuning() && GameAppControl.getGameStarted() == false))//游戏正在启动尚未启动成功
                {
                    // MyTools.PrintDebugLogError("ucvr wait app start to castscreen");
                    heartBeatTime = Time.time;
                }
                else if (Time.time - heartBeatTime > 10)
                {
                    MyTools.PrintDebugLogError("ucvr heartBeat outtime ");
                    //处理投屏异常      
                    screenCastError();
                    sendBroadcastClose();//超时也需要向机顶盒发送停止消息，否则机顶盒会在头盔超时后再等10秒再退出
                         

                }
            }
        }
        else
        {
            if (castScreen == CastScreen.castScreenTryConnect)
            {
                if (GameAppControl.getGameRuning() && GameAppControl.getGameStarted() == false)//游戏正在启动尚未启动成功
                {
                    // MyTools.PrintDebugLogError("ucvr wait app start to castscreen");
                }
                else
                    tryconnectTime = tryconnectTime + Time.deltaTime;

                if (tryconnectTime > 10)
                {
              
                    sendBroadcastClose();//超时也需要向机顶盒发送停止消息，否则机顶盒会在头盔超时后再等10秒再退出
                    MyTools.PrintDebugLogError("ucvr xmpp connet time out 10 s");
                    //处理投屏异常      
                    screenCastError();
                }
                //}
            }
            else
            {
                tryconnectTime = 0;

                if (castScreen == CastScreen.castScreening)
                {


                    if (Time.time - heartBeatTime > 10)
                    {
                        MyTools.PrintDebugLogError("ucvr heartBeat outtime ");
                        //处理投屏异常      
                        screenCastError();

                        sendBroadcastClose();//超时也需要向机顶盒发送停止消息，否则机顶盒会在头盔超时后再等10秒再退出

                    }
                }

            }
        }
    }

    private IEnumerator heartBeatLoop()
    {
        while (castScreen != CastScreen.noCastScreen)
        {
            sendBroadcastHeartBeat();
            yield return new WaitForSeconds(1f);//等到该帧结束
        }
    }
    private string playUrl;

    private bool nettyApiEnable = false;//是否使用netty Api投屏
    private const string NettyUrlISNull = "-1";
    private const string NettyLoginSuccess = "0";
    //============================================netty投屏相关开始===================================
    //netty 服务初始化
    //isSetboxUpdateCancelClick 机顶盒升级时是否是用户点击了升级取消按钮
    public void nettyApiInit(string plurl)
    {
        if (castScreen != CastScreen.noCastScreen)
        {//启动过程中不允许再投屏
            if (OpenApiImp.getOpenApi().notify() != null)
            {
                OpenApiImp.getOpenApi().notify().systemStatusCallback(OpenApiImp.systemException, ErrorCodeOpenApi.castScreenFailed.ToString());
            }
            MyTools.PrintDebugLogError("ucvr plase close before retry castScreen:"+ castScreen);
            return;
        }
        nettyApiEnable = true;
        //投屏字符串常量有變化
        OpenVRSTB = NettyTool.OpenVRSTB;
        VRSTBReady = NettyTool.VRSTBReady;
        lantingsx = NettyTool.lantingsx;
        VRSTBError = NettyTool.VRSTBError;
        VRVideoInfo = NettyTool.VRVideoInfo;
        VRVideoReady = NettyTool.VRVideoReady;
        closeVRSTB = NettyTool.closeVRSTB;
        functionCall = NettyTool.functionCall;
        platform = NettyTool.platform;
        HeartBeat = NettyTool.HeartBeat;
        HeartBeatFromTerminal = NettyTool.HeartBeatFromTerminal;
        playUrl = plurl;
 
        heartBeatTime = Time.time;
        if (OpenApiImp.getOpenApi().getInitParam().localProjectionEnable)
        {//如果使用本地投屏将使用本地消息中心MessageCenterService转发到本地投屏服务中进行处理
            localProjectionInit();
        }
        else
        {
            //调用jar初始化接口
            int nettyResult = MyTools.NettySdkInit();
            MyTools.PrintDebugLog("ucvr nettyApiInit nettyResult:" + nettyResult + ";plurl:" + playUrl);
            if (nettyResult != 0)
            {
                //处理投屏异常      
                screenCastError();
            }
        }

    }
    //====================================================处理本地投屏兼容方式开始=============
    //如果使用本地投屏将使用本地消息中心MessageCenterService转发到本地投屏服务中进行处理
    private int localProjectionInit() {
        int projection = MyTools.VRScreenAdaptInit();
        MyTools.PrintDebugLog("ucvr VRScreenAdaptInit start:" + projection);
        if (projection != 0)
        {
            castScreen = CastScreen.noCastScreen;
            MyTools.PrintDebugLogError("ucvr VRScreenAdaptInit failed:" + projection);
        }
        return projection;
    }
    private int localProjectionVRStopSTB() {
        MyTools.VRScreenAdaptSendMessage(LocalEvent.VRStopSTB, playUrl, 0);
        return 0;
    }
    float lastSendGameruningTime = 0;
    private int localProjectionVRGameruningSTB(float time)
    {
        if (time - lastSendGameruningTime > 0.2)//200ms发送一次
        {
            lastSendGameruningTime = time;
            MyTools.VRScreenAdaptSendMessage(LocalEvent.gameRuning, null, 0);
        }
        return 0;
    }
    class LocalProjectionEvent {
        public string eventName;
        public string eventBody;
        public int type;
       
    }
    class LocalEvent {
        //messageEvent
        public static string serviceConnectState = "serviceConnectState";//服务连接状态
        public static string VRDeviceConnetError = "VRDeviceConnetError";
        public static string VRCloudRepeatSTB = "VRCloudRepeatSTB";//netty服务已中断如果想投屏需要重新进行初始化
        public static string VRFirstFrame = "VRFirstFrame";
        public static string VRCloseReady = "VRCloseReady";
        //sendEvent
        public static string VRStartSTB = "VRStartSTB";
        public static string VRStopSTB = "VRStopSTB";
        public static string gameRuning = "gameRuning";//游戏运行过程中发送心跳

    }
    /// <summary>
    /// 通过本地消息中心进行投屏，这种方式支持portal本地投屏
    /// </summary>
    /// <param name="param"></param>
    public void messageCenterMessage(string param) { 
        MyTools.PrintDebugLog("ucvr messageCenterMessage param:" + param);
        LocalProjectionEvent data = JsonConvert.DeserializeObject<LocalProjectionEvent>(param);
        if (data.type !=0)
        {
            MyTools.PrintDebugLog("ucvr messageCenterMessage 非流化消息不处理");
            return;
        }
        if (data.eventName == LocalEvent.serviceConnectState) {
            if (data.eventBody == "0")//本地消息中心服务连接成功,这时通知消息中心转发投屏消息
            {
                int retcode= MyTools.VRScreenAdaptSendMessage(LocalEvent.VRStartSTB, playUrl, 0);
                if (retcode != 0)
                {
                    castScreen = CastScreen.noCastScreen;
                    MyTools.PrintDebugLogError("ucvr VRStartSTB sendfailed retcode:"+retcode);
                }
            }
            else {
                castScreen = CastScreen.noCastScreen;
                localProjectionInit();
            }
        } else if (data.eventName== LocalEvent.VRDeviceConnetError) {//设备未连接
            if (GameAppControl.getGameRuning())
                commonPlaneCom.showCheckCodeDialog("bindCheckCode", data.eventBody, dialogClickCallBack);
        }
        else if (data.eventName == LocalEvent.VRCloudRepeatSTB){
            castScreen = CastScreen.noCastScreen;//如果netty中断直接返回投屏失败就可以
        }
        else if (data.eventName == LocalEvent.VRFirstFrame)
        {//投屏成功
            castScreen = CastScreen.castScreening;
        }
        else if (data.eventName == LocalEvent.VRCloseReady)
        {//投屏结束
            castScreen = CastScreen.noCastScreen;
        }
        
    }
    //====================================================处理本地投屏兼容方式结束=============
    /// <summary>
    /// 投屏消息回调
    /// </summary>
    /// <param name="jsonStr"></param>
    public void CSMessageCallBack(string jsonStr)
    {
        messagePro(jsonStr);
    }
    /// <summary>
    /// netty投屏 初始化消息回调
    /// </summary>
    /// <param name="jsonStr"></param>
    public void CSInitResult(string result)
    {
 
        if (result == NettyLoginSuccess)
        {
           
            heartBeatTime = Time.time;
            if (castScreen == CastScreen.noCastScreen)
            {//尚未启动投屏才需要发送投屏消息
                MyTools.PrintDebugLog("ucvr netty init sucess CSInitResult 0 ");
                sendBroadcastOpenVRSTB(playUrl);
            }
            else
            {
                MyTools.PrintDebugLog("ucvr no need send sendBroadcastOpenVRSTB");
            }
        }
        else
        {
            //处理投屏异常      
            if (castScreen == CastScreen.noCastScreen)
            {
                screenCastError();
                MyTools.PrintDebugLogError("ucvr failed CSInitResult:" + result);
            }
            else//投屏过程中netty出现异常需要重新进行初始化
            {
                MyTools.PrintDebugLogError("ucvr failed repeat initNettyApi" );
                MyTools.nettyConnect();
            }
        }             
    }
    /// <summary>
    /// 投屏校验码窗口确认回调
    /// </summary>
    /// <param name="buttonIndex"></param>
    private void dialogClickCallBack(MyDialog.ButtonIndex buttonIndex)
    {
        if (buttonIndex == MyDialog.ButtonIndex.bt_ok)
        {
            MyTools.PrintDebugLog("ucvr start castscreen by checkcode callback");
            if (OpenApiImp.getOpenApi().getInitParam().localProjectionEnable)
            {//如果使用本地投屏将使用本地消息中心MessageCenterService转发到本地投屏服务中进行处理
                localProjectionInit();
            }else
                nettyApiInit(playUrl);
        }
        else {
            MyTools.PrintDebugLog("ucvr click cancel");
        }
    }
    //==============================================================netty结束====================================================
    /// <summary>
    /// 启动投屏,发送OpenVRSTB指令
    /// 等待机顶盒相应VRSTBReady后发送投屏地址（如果是新版投屏方式还需要向投屏服务发送直播指令参考sendBroadcastVRSTBVRVideoInfo）
    /// </summary>
    /// <param name="playUrl"></param>
    public void sendBroadcastOpenVRSTB(string playUrl)
    {
        MyTools.PrintDebugLog("ucvr sendBroadcastOpenVRSTB:" + castScreen);
        try
        {
            ISVRDeviceSetboxUpdateDialog = false;
            castScreen = CastScreen.castScreenTryConnect;
            this.playUrl = playUrl;
            XMPPData data = new XMPPData();
            data.functionType = OpenVRSTB;
            data.packageName = "com.cybercloud.vrcastscreen/com.cybercloud.vr.CvrActivity";
            data.isVR = 1;

            data.action = functionCall;
            string jsonParas = JsonConvert.SerializeObject(data);
            sendBroadcast(jsonParas);
            MyTools.PrintDebugLog("ucvr sendBroadcastOpenVRSTB:" + castScreen);
        }
        catch (Exception e)
        {
            //处理投屏异常      
            screenCastError();
        
            MyTools.PrintDebugLogError("ucvr sendBroadcastOpenVRSTB:" + e.Message);
           
        }
    }

    public void sendBroadcastClose()
    {          
        MyTools.PrintDebugLog("ucvr sendBroadcastClose");
        try
        {
            if (OpenApiImp.getOpenApi().getInitParam().localProjectionEnable) {
                localProjectionVRStopSTB();
                return;
            }
            XMPPData data = new XMPPData();
            data.functionType = closeVRSTB;
            data.isVR = 1;

            data.action = functionCall;
            string jsonParas = JsonConvert.SerializeObject(data);
            StartCoroutine(getGameApp().streamLiveOption(0));//教室场景启停直播
            StartCoroutine(getGameApp().cmdToServiceStartStopCastScreen(0));//停止投屏
            sendBroadcast(jsonParas);
            ISVRDeviceSetboxUpdateDialog = false;
            if (!nettyApiEnable)//老投屏方式到此投屏已結束，新投屏需要监听closeready
                disConnetCast();
        }
        catch (Exception e) {
            //处理投屏异常      
            screenCastError();          
            MyTools.PrintDebugLogError("ucvr sendBroadcastClose:" + e.Message);
        }
    }
    public void sendBroadcastHeartBeat()
    {
        if (castScreen != CastScreen.castScreening)
            return;
        MyTools.PrintDebugLog("ucvr HeartBeatFromTerminal");
        try
        {

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
    private void sendBroadcastVRSTBVRVideoInfo(string playUrl) {
        MyTools.PrintDebugLog("ucvr sendBroadcast VRVideoInfo" + ";plurl:" + playUrl);
        string tempPlayUrl = playUrl;
        if (playUrl == null || playUrl == "") {
             screenCastError();
            MyTools.PrintDebugLogError("ucvr cannot get casturl Please cast the screen later");
            return;
        }
        if(castScreen!= CastScreen.castScreenTryConnect)
            castScreen = CastScreen.castScreenTryConnect;
        if (CyberCloudConfig.newScreenProtocol == 1)
        {//使用新的投屏模式，通过终端通知前端中转服务启动单眼资源采集并通过srt推流到头盔

            StartCoroutine(getGameApp().cmdToServiceStartStopCastScreen(1, playUrl));//    

            string ip = "", session = "";
            int port = 0;
            //修改投屏串
            tempPlayUrl = commonPlaneCom.getCastScreenCmd(playUrl, ref ip, ref port, ref session);
            MyTools.PrintDebugLog("ucvr update CastScreenUrl:" + playUrl);
        }
        try
        {
            XMPPData data = new XMPPData();
            data.functionType = VRVideoInfo;
            data.isVR = 1;
            data.platform = platform;
            data.action = functionCall;
            data.playUrl = tempPlayUrl;
            string jsonParas = JsonConvert.SerializeObject(data);

            sendBroadcast(jsonParas);
        }
        catch (Exception e) {
            //处理投屏异常      
            screenCastError();
  
 
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
        { if (nettyApiEnable)//netty api 是否有效
                MyTools.sendNettySdkCastScreen(jsonParas);
            else
                MyTools.sendBroadcast(jsonParas);
        }
    }
    public enum CastScreen {
        noCastScreen,
        castScreenTryConnect,
        castScreening,
        bindSTB
    }
    public static CastScreen _castScreen = CastScreen.noCastScreen;
    public static CastScreen castScreen {
        get {
        
                return _castScreen;
         
        }
        set {
       
            _castScreen = value;
            if (OpenApiImp.CyberCloudOpenApiEnable)
            {
                OpenApiImp.getOpenApi().notify().castScreenCallback(castScreen, "");
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="jsonStr"></param>
    public void OnReceiveXMPPPMessage(string jsonStr)
    {
        //大鹏使用的自己实现的jar接口
        if (CyberCloudConfig.currentType != CyberCloudConfig.DeviceTypes.DaPeng)
            messagePro(jsonStr);
        else if (CyberCloudConfig.DeepoonCastScreenDemonstration == "1")//大鹏测试使用
        {
            messagePro(jsonStr);
        }
    }
    private float heartBeatTime = 0;
    public delegate void TestCastScreenMessageCallBack(string jsonStr);
    public TestCastScreenMessageCallBack testCastScreenMessageCallBack;
    private GameAppControl gameAppControl = null;
    private GameAppControl getGameApp (){
        if (gameAppControl == null)
        {
            GameObject gamePlane = GameObject.Find("GamePlane");
            if (gamePlane != null)
            {

                gameAppControl = gamePlane.GetComponent<GameAppControl>();
            }
        }
        return gameAppControl;
    }
    private void screenCastError() {
        if (GameAppControl.getGameRuning())
            commonPlaneCom.showHintMstByDesckey("Screen_casting_failed", 3,null);
        MyTools.PrintDebugLogError("ucvr screenCastError " );
        disConnetCast();
    }
    private bool ISVRDeviceSetboxUpdateDialog = false;
    private void messagePro(string jsonStr) {
        //
        if (jsonStr.IndexOf(HeartBeat) <0)
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
            //兰亭有个bug在close的时候发送了VRSTBReady， 此处加个判断吧 castScreen ==CastScreen.CastScreenTryConnect;
            if ((jsonStr.IndexOf(VRSTBReady) > -1 || jsonStr.IndexOf(lantingsx) > -1) && castScreen == CastScreen.castScreenTryConnect)
            {
                sendBroadcastVRSTBVRVideoInfo(playUrl);
            }
            else if (jsonStr.IndexOf(NettyTool.VRDeviceConnetError) > -1)
            {//设备未连接
                XMPPData data = JsonConvert.DeserializeObject<XMPPData>(jsonStr);
                disConnetCast();
                MyTools.PrintDebugLog("ucvr VRDeviceConnetError checkCode" + data.checkCode);
                if (GameAppControl.getGameRuning())
                    commonPlaneCom.showCheckCodeDialog("bindCheckCode", data.checkCode, dialogClickCallBack);
            }
            else if (jsonStr.IndexOf(VRSTBError) > -1 || jsonStr.IndexOf("VRBinderDeviceNotLogin") > -1)
            {
                //处理投屏异常      
                screenCastError();
            }
            else if (jsonStr.IndexOf("VRCloseReady") > -1)
            {
                disConnetCast();
                MyTools.PrintDebugLog("ucvr 关闭投屏");
            }
            else if (jsonStr.IndexOf(VRDeviceSetboxUpdateDialog) > -1)//机顶盒弹出升级窗口
            {
                ISVRDeviceSetboxUpdateDialog = true;
                castScreen = CastScreen.castScreenTryConnect; ;//弹出升级窗口后不要端口netty连接，因为如果是取消连接后还需要继续投屏，退出游戏时会判断castScreen的状态如果是castScreenTryConnect或者castScreening会在退出应用时断开netty连接
                MyTools.PrintDebugLog("ucvr setbox need update");
                if (GameAppControl.getGameRuning())
                    commonPlaneCom.showHintMstByDesckey("setboxupdate", 3, null);
            }
            else if (jsonStr.IndexOf(VRDeviceSetboxNeedUpdateCancel) > -1)//机顶盒取消升级
            {
                ISVRDeviceSetboxUpdateDialog = false;
                MyTools.PrintDebugLog("ucvr" + VRDeviceSetboxNeedUpdateCancel);
                if (GameAppControl.getGameRuning())//判断游戏是否在运行，如果游戏在运行取消升级后需要继续投屏
                {              
                    playUrl = getGameApp().Cyber_GetCastScreenPlayUrl();
                    sendBroadcastVRSTBVRVideoInfo(playUrl);
                }
            }
            else if (jsonStr.IndexOf(VRSetBoxUpdateStart) > -1) {//目前无法区分强制和非强制升级，升级失败后统一处理为关闭投屏

                ISVRDeviceSetboxUpdateDialog = false;
                sendBroadcastClose();//等closeready后再更改投屏状态
            }
            else if (jsonStr.IndexOf(VRVideoReady) > -1)
            {
                MyTools.PrintDebugLog("ucvr VRVideoReady");
                repeatTimes = 0;

                //castScreen = CastScreen.castScreening;//增加了VRSTBDecodingIFrame接口，用于判定投屏画面已出来
                heartBeatTime = Time.time;
                StartCoroutine(heartBeatLoop());
            }
            else if (jsonStr.IndexOf(VRFirstFrame) > -1)
            {
                castScreen = CastScreen.castScreening;
            }
            else if (jsonStr.IndexOf(HeartBeat) > -1 && jsonStr.IndexOf(HeartBeatFromTerminal) == -1)
            {//HeartBeatFromTerminal也会返回

                heartBeatTime = Time.time;
                if (castScreen != CastScreen.noCastScreen)
                {
                    repeatTimes = repeatTimes + 1;
                    //投屏中断后再收到心跳时连接投屏状态
                    if (GameAppControl.getGameRuning() && repeatTimes > 3)
                    {
                        repeatTimes = 0;

                        MyTools.PrintDebugLog("ucvr Received heartBeatTime time" + Time.time);//每过3秒打印一次心跳数据
                    }
                }
                else//头盔如果已经停止投屏，之后如果收到机顶盒的心跳消息告诉机顶盒停止投屏
                {
                    MyTools.PrintDebugLogError("ucvr Received heartBeatTime but castScreen had stop");
              
                    sendBroadcastClose();                   //给机顶盒发送停止投屏指令
                }
            }
        }
    }
    //心跳断开后重连的次数
    private int repeatTimes = 0;

}
