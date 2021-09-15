using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.CyberCloud.Scripts.OpenApi;
using static Assets.CyberCloud.Scripts.OpenApi.OpenApiImp;
using Assets.CyberCloud.Scripts.DeviceController;
/// <summary>
/// portal 和 jar包通信接口 unity接口定义和jar包接口调用
/// </summary>
public class VRPortalCommunication : MonoBehaviour {
    private CommonPlane commonPlaneCom;
    private AppStartOrExitCallBack appcallback;
    private GameAppControl gameAppCtr;
    // Use this for initialization

    private string vrdevicenoconnet = "0x00A00001";//vr 设备无法上传设备信息
    void Awake() {
        DeviceInfo dev = gameObject.GetComponent<DeviceInfo>();
        if (dev == null) dev = gameObject.AddComponent<DeviceInfo>();
    }
    void Start () {

        /**
        string cname = "com.cybercloud.event.Handler";
        Debug.Log("startcall Cyber_StartPlay ");
        //AndroidJavaClass只能调用静态方法，获取静态属性 AndroidJavaObject能调用公开方法和公开属性
                //AndroidJavaClass handler = new AndroidJavaClass(cname);
        AndroidJavaObject handler = new AndroidJavaObject(cname);
        //int result= handler.Call<int>("Cyber_StartPlay", "I Am startUrl");
        AndroidJavaClass jcPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject joActivity = jcPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        handler.Call("getPkName", joActivity);
        // Debug.Log(" Cyber_StartPlay return"+result);
        **/
        comInit();

    }

    void comInit() {
        GameObject commonPlane = GameObject.Find("CyberCloudCommonPlane");
        if (commonPlane == null)
            MyTools.PrintDebugLogError("ucvr commonPanel mast be added on screne");
        commonPlaneCom = commonPlane.GetComponent<CommonPlane>();
        if (commonPlaneCom == null)
            MyTools.PrintDebugLogError("ucvr CyberCloudCommonPlane mast contain CommonPlane ");
        GameObject gamePlane = GameObject.Find("GamePlane");
        if (gamePlane != null)
        {
            appcallback = gamePlane.GetComponent<AppStartOrExitCallBack>();
            gameAppCtr = gamePlane.GetComponent<GameAppControl>();
        }

    }
	// update is called by once frame
	void Update () {

    }
    public static string defaultError = "未识别的参数错误";
    /// <summary>
    /// 应用启动状态回调
    /// </summary>
    public void appStatusCallback(string param) {
        MyTools.PrintDebugLog("ucvr statusCallback:"+ param);
        try
        {
            if (param != null)
            {
                ComunicationResult result = JsonConvert.DeserializeObject<ComunicationResult>(param);
                if (result != null && result.retData !=null )
                {
                    string statusType = result.appStatus;
                    appcallback.statusCallback(statusType, result.retData);
                    int code = 0;
                    if (OpenApiImp.CyberCloudOpenApiEnable)
                    {
                        StartStatus status = StartStatus.appExitDone;
                        if (statusType.Equals("appStarting"))
                        {//应用启动状态中
                            if (result.retData.appRetCode != 0)
                            {
                                statusType = "appStartFailed";
                                code = result.retData.appRetCode;
                               // status = StartStatus.appStartFailed;
                                if (OpenApiImp.CyberCloudOpenApiEnable)
                                {

                                    OpenApiImp.getOpenApi().notify().systemStatusCallback(OpenApiImp.systemException, code.ToString());
                                }
                            }
                            else
                                status = StartStatus.appStarting;

                        }
                        else if (statusType.Equals("appStartDone"))
                        {
                            status = StartStatus.appStartDone;
                        }
                        else
                        {//退出目前没有回调需要在应用退出stopApp时直接返回
                            Debug.LogError("ucvr appStatusCallback unknown code");
                        }

                        OpenApiImp.getOpenApi().notify().appStatusCallback(status);
                    }
                }
                else {
                    MyTools.PrintDebugLogError("ucvr statusCallback param error");
                }
               
            }
        }
        catch (Exception e) {
            MyTools.PrintDebugLogError("ucvr statusCallback statusType: param:"+ param+e.Message);
        }

    }
   
    private class DialogObj
    {

        public string dialogType;//： string
        public string content;//:string
        public string time;//：string
   }
    /// <summary>
    /// networkException：           网络状态异常
    /// networkExceptionRestore：    网络状态恢复.
    /// </summary>
    /// <param name="statusDescription"></param>
    public void systemStatusCallback(string param) {
        MyTools.PrintDebugLog("ucvr systemStatusCallback:" + param);
        try
        {
            if (param != null)
            {
                SystemStatusResult result = JsonConvert.DeserializeObject<SystemStatusResult>(param);
                if (result != null)
                {
                    string errCode = result.systemRetData==null? "110000" : result.systemRetData.errCode;
                    if (vrdevicenoconnet == errCode)
                    {//会重连
                        Debug.LogError("ucvr vrdevicenoconnet");
                        return;
                    }
                    if (errCode.Equals(CommonPlane.playTokenOutTime)) {
                        result.statusDescription = "playToken过期或无效请重新通过排队服务申请";
                    }
                    //appcallback.statusCallback(statusType, result.retData);

                    if (GameAppControl.getGameRuning())
                        gameAppCtr.systemStatusCallback(result.systemStatus, result.statusDescription, errCode);
                    else
                    {                       
                        if (errCode != null && errCode.Equals(CommonPlane.exitByAppSelf))
                        {

                            MyTools.PrintDebugLog("ucvr exitByAppSelf byself");
                            if (OpenApiImp.CyberCloudOpenApiEnable)
                            {

                                OpenApiImp.getOpenApi().notify().systemStatusCallback(OpenApiImp.exitByAppSelf, CommonPlane.exitByAppSelf);
                            }
                            gameAppCtr.exitCyberGame();//游戏内主动需要再主动调用exitCyberGame 否自底层库退不干净
                        }else
                            MyTools.PrintDebugLogError("ucvr game exited, systemStatusCallback call unenable");
                    }
                    if (OpenApiImp.CyberCloudOpenApiEnable)
                    {
                 
                        OpenApiImp.getOpenApi().notify().systemStatusCallback(result.systemStatus,errCode);
                    }
                }
                else
                {
                    MyTools.PrintDebugLogError("ucvr statusCallback param error");
                }

            }
        }
        catch (Exception e)
        {
            MyTools.PrintDebugLogError("ucvr statusCallback statusType: param:" + param+e.Message);
        }
      
    }
  
    /// <summary>
    /// 显示提示框
    /// </summary>
    /// <param name="dialogType">
    /// 提示框类型
    ///0：弹框形式显示（目前支持的）
    ///1：渐隐形式提示
    /// </param>
    /// <param name="content">需要显示的内容</param>
    /// <param name="time">如果dialogType为1时有效表示显示时间，单位是秒，默认值是3提示框显示后time秒后将被自动关闭</param>
    public void simpleShowDialog(string param) {
        try
        {
            if (param != null)
            {
                DialogObj result = JsonConvert.DeserializeObject<DialogObj>(param);
                if (OpenApiImp.CyberCloudOpenApiEnable)
                {
                    int t = result.time == null ? 0 : int.Parse(result.time);
                    OpenApiImp.getOpenApi().notify().simpleShowDialog(int.Parse( result.dialogType), result.content,t);
                }
                else
                {
                    if (result.dialogType.Equals("0"))
                    {
                        MyTools.PrintDebugLog("ucvr simpleShowDialog:" + result.content);
                        //本期异常只考虑wifi异常 下期再增加接口
                        if (result.content.IndexOf(CommonPlane.resousefull) > -1)
                            commonPlaneCom.showDialogWifiException("resources_are_fully", CommonPlane.resousefull);
                        else
                            commonPlaneCom.showDialogWifiException("Home_NoNet", null);
                    }
                    else
                    {
                        int t = result.time == null ? 0 : int.Parse(result.time);
                        MyTools.PrintDebugLog("ucvr simpleShowDialog:" + result.content);
                        if (result.content.IndexOf(CommonPlane.resousefull) > -1)
                            commonPlaneCom.showHintMstByDesckey("resources_are_fully", t, CommonPlane.resousefull);
                        else
                            commonPlaneCom.showHintMsg(result.content, t);
                    }
                }
            }
        }
        catch (Exception e)
        {
            MyTools.PrintDebugLogError("ucvr statusCallback param:" + param);
        }
  
    }

    /// <summary>
    /// 测试使用
    /// </summary>
    /// <param name="result"></param>
    public void startResult(string result) {

        Debug.Log("startResultCallBack:"+result);
    }

    //home键
    public void onSystemHome(string param) {
        commonPlaneCom.OnSystemHome();
    }
    //activity 在onstop时会调用
    public void activityStop(string param)
    {
        MyTools.PrintDebugLog("ucvr unity onStop param："+ param);
        //commonPlaneCom.onStop();
    }
    public void testf(string param) {
        MyTools.PrintDebugLog("ucvr unity testf param：" + param);
    }
    /// <summary>
    /// 安装结果
    /// </summary>
    /// <param name="result"></param>
    public void installResult(string result) {
        //0：当前已经是最新版本不需要升级
        //1：当前网络不通，不需要升级
        //2：用户点击了取消升级
        //3：下载失败apk不存在
        //4：下载失败，下载超时
        //5、其他异常
        MyTools.PrintDebugLogError("ucvr installResult:" + result);
        if (result == "0" || result == "1" || result == "2")
        {
            if(gameAppCtr==null)
                comInit();
            gameAppCtr.apkUpdateCheckUpResult(result);
        }
        else
        {
#if picoSdk
            Pvr_UnitySDKAPI.Controller.UPvr_IsEnbleHomeKey(true);
#endif
            Application.Quit();
        }        
    }
    private string terminalCode = "";
    /// <summary>
    /// 应用适配结果
    /// </summary>
    /// <param name="terminalCode">终端校验码，需要在头盔中展示</param>
    public void CSAdaptPlatformInitResult(string code)
    {
        terminalCode = code;
        Debug.Log("ucvr unity CSAdaptPlatformInitResult param：" + terminalCode);
        commonPlaneCom.showHintMsg("链接码：" + terminalCode, 0);
    }
    /// <summary>
    /// 消息回调,需要根据回调结果启停应用
    /// </summary>
    /// <param name="messageJson"></param>
    public void CSAdaptPlatformMessageCallBack(string messageJson)
    {
        MyTools.PrintDebugLog("ucvr unity CSAdaptPlatformMessageCallBack param：" + messageJson);
        //commonPlaneCom.showHintMsg(result.content);
        try
        {
            if (messageJson != null)
            {
                AdaptPlatformCommand result = JsonConvert.DeserializeObject<AdaptPlatformCommand>(messageJson);
                if (result.action == "start")
                {
                    commonPlaneCom.closeHintMsg();
                    gameAppCtr.GetComponent<GameAppControl>().startApp(result.cyberCloudEnterUrl);
                }
                else if (result.action == "stop")
                {                  
                    commonPlaneCom.cloudCyberExit();
                    commonPlaneCom.showHintMsg("链接码：" + terminalCode, 0);
                }
            }
        }
        catch (Exception e)
        {

            MyTools.PrintDebugLogError("ucvr CSAdaptPlatformMessageCallBack Exception:" + e.Message);
        }
    }
    //////////////////////////////////===============================排队相关===============
    public void initConfigServiceResult(string code) {
        OpenApiImp.getOpenApi().initResult = code=="0"?true:false;
        OpenApiImp.getOpenApi().notify().initResult(int.Parse( code));
    }
    /// <summary>
    /// 取消排队data是字符串
    /// </summary>
    public class CancleQueueBody
    {
        public int ResultCode;
        public string Description;
    }
        /// <summary>
        /// 排队结果回调
        /// </summary>
        /// <param name="type"></param>
        /// <param name="result"></param>
        public void queueResult(string data)
    {
        MyTools.PrintDebugLog("ucvr queueResult:" + data);

        if (OpenApiImp.CyberCloudOpenApiEnable)
        {
            if (data != null)
            {
                String[] param = data.Split(';');
                if (int.Parse(param[0])== QueueType.CYBER_QUEUE_CANCEL_ERROR|| int.Parse(param[0]) == QueueType.CYBER_QUEUE_CANCEL_SUCCESS) {
                    CancleQueueBody body = JsonConvert.DeserializeObject<CancleQueueBody>(param[1]);
                    param[1] = JsonConvert.SerializeObject(body);
                }
                OpenApiImp.getOpenApi().notify().queueResult(int.Parse(param[0]), param[1]);
            }
        }
    }

}
