using CyberCloud.PortalSDK.Response;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using Assets.CyberCloud.Scripts.OpenApi;
/// <summary>
/// 工具类 http请求
/// apk安装
/// </summary>
public class MyTools : MonoBehaviour {
    public delegate void VoidDelegateWWWData(ToolBaseResult result);
    public VoidDelegateWWWData wwwDataLoad;

    public class ToolBaseResult {
        public int retCode = 0;
        public string desc = "";
        public string data;
    }
    // Use this for initialization
    void Start() {
        //TestHttpSend();
    }
    void Update() {
    }
    int updta;


    /// <summary>
    /// 发送post请求
    /// </summary>
    /// <param name="_url"></param>
    /// <param name="_wForm"></param>
    /// <returns></returns>
    public IEnumerator SendPost(string _url, WWWForm _wForm)
    {
        Debug.Log("SendPost:" + _url);
        WWW postData = null;
        try
        {
            postData = new WWW(_url, _wForm);
        }
        catch (System.Exception e)
        {
            MyTools.PrintDebugLogError("ucvr SendPost:" + e.Message);
        }
        yield return postData;
        resultData(postData);
    }
    public IEnumerator SendGet(string _url)
    {
        Debug.Log("SendGet:" + _url);
        WWW postData = null;
        try
        {
            postData = new WWW(_url);
        }
        catch (System.Exception e)
        {
            MyTools.PrintDebugLogError("ucvr Sendget:" + e.Message);
        }


        yield return postData;
        resultData(postData);

    }
    public IEnumerator downloadfile(string _url)
    {
        MyTools.PrintDebugLog("ucvr downloadfile:" + _url);
        WWW postData = null;
        try
        {
            postData = new WWW(_url);
        }
        catch (System.Exception e)
        {
            MyTools.PrintDebugLogError("ucvr Sendget:" + e.Message);
        }


        yield return postData;
        Stream outStream = null;
        FileInfo fi = new FileInfo(Application.persistentDataPath + "/serverConfig.ini");
        if (fi.Exists)
        {
            fi.Delete();
        }
        outStream = fi.Create();
        //string myStr = System.Text.Encoding.UTF8.GetString(heByte);
        byte[] databyte = System.Text.Encoding.UTF8.GetBytes(postData.text);
        string myStr = System.Text.Encoding.UTF8.GetString(databyte);
        outStream.Write(databyte, 0, databyte.Length);
        outStream.Close();
        resultData(postData);

    }
    private void resultData(WWW postData) {
        ToolBaseResult result = new ToolBaseResult();
        if (postData.error != null)
        {
            Debug.LogError("SendPosterror" + postData.error);
            result.retCode = -1;
        }
        else
        {
            result.retCode = 0;
            result.data = postData.text;
        }
        Debug.Log("result:" + result);
        if (wwwDataLoad != null)
            wwwDataLoad(result);
    }

    public static string configValueByKey(string key, string[] fileConfig)
    {
        try
        {
            if (fileConfig == null)
                return null;
            foreach (string st in fileConfig)
            {
                if (st.StartsWith(key))
                {
                    //string[] kv = st.Split(key.ToCharArray());
                    string[] kv = Regex.Split(st, key, RegexOptions.IgnoreCase);
                    if (kv.Length > 1)
                    {
                        return kv[1];
                    }
                    else
                        MyTools.PrintDebugLogError("ucvr configValueByKey key:" + key);
                }

            }

        }
        catch (Exception e)
        {
            MyTools.PrintDebugLogError("ucvr configValueByKey key:" + key + ";" + e.Message);
        }
        return null;
    }

    /// <summary>
    /// apk安装方法
    /// </summary>
    /// <param name="path">apk下载位置</param>
    /// <returns></returns>
    public static bool InstallAPK(string path)
    {
        try
        {/**
            if (path == null || path.Equals(""))
            {
                MyTools.PrintDebugLogError("ucvr InstallAPK path null");
                return false;
            }
            else
            {
                if (!File.Exists(path))
                {
                    MyTools.PrintDebugLogError("ucvr InstallAPK path:" + path + " not exit");
                    return false;
                }
            }
            var Intent = new AndroidJavaClass("android.content.Intent");
            var ACTION_VIEW = Intent.GetStatic<string>("ACTION_VIEW");
            var FLAG_ACTIVITY_NEW_TASK = Intent.GetStatic<int>("FLAG_ACTIVITY_NEW_TASK");
            var intent = new AndroidJavaObject("android.content.Intent", ACTION_VIEW);

            var file = new AndroidJavaObject("java.io.File", path);
            var Uri = new AndroidJavaClass("android.net.Uri");
            var uri = Uri.CallStatic<AndroidJavaObject>("fromFile", file);

            intent.Call<AndroidJavaObject>("setDataAndType", uri, "application/vnd.android.package-archive");
            intent.Call<AndroidJavaObject>("addFlags", FLAG_ACTIVITY_NEW_TASK);
            intent.Call<AndroidJavaObject>("setClassName", "com.android.packageinstaller", "com.android.packageinstaller.PackageInstallerActivity");

            AndroidJavaObject currentActivity = getCurrentActivity();
            currentActivity.Call("startActivity", intent);
            **/
            AndroidJavaObject handler = new AndroidJavaObject(CyberCloudJar);
            AndroidJavaObject currentActivity = getCurrentActivity();
            int result = handler.Call<int>("installApk2", currentActivity, path);
            MyTools.PrintDebugLog("ucvr callinstallApk result:" + result);
            return true;
        }
        catch (System.Exception e)
        {
            MyTools.PrintDebugLogError("ucvr InstallAPK:" + e.Message);
            return false;
        }
    }
    /// <summary>
    /// 判断服务是否存piconeo2头盔的服务会异常结束投屏前判断
    /// </summary>
    /// <param name="serviceName">@param serviceName 服务类的全路径名称 例如： com.jaychan.demo.service.PushService</param>
    /// <returns></returns>
    public static bool isServiceRunning(string serviceName)
    {
        try
        {
            AndroidJavaObject handler = new AndroidJavaObject(CyberCloudJar);
            AndroidJavaObject currentActivity = getCurrentActivity();
            int result = handler.Call<int>("isServiceRunning", serviceName, currentActivity);
            MyTools.PrintDebugLog("ucvr callisServiceRunning result:" + result);
            if (result == 1)
                return true;
            else
                return false;
        }
        catch (System.Exception e)
        {
            MyTools.PrintDebugLogError("ucvr isServiceRunning:" + e.Message);
            return false;
        }
    }
    /// <summary>
    /// 启动pico wifi或蓝牙
    /// </summary>
    /// <param name="actionName"> 
    /// 蓝牙 actionName：  "pui.settings.action.SETTINGS"
    /// wifi actionName：  "pui.settings.action.WIFI_SETTINGS"
    /// </param>
    public static void startPicoSystemCom(String actionName) {
        AndroidJavaObject currentActivity = getCurrentActivity();
        AndroidJavaObject joIntent = new AndroidJavaObject("android.content.Intent", actionName);
        currentActivity.Call("startActivity", joIntent);
    }
    /// <summary>
    /// 启动pico商城
    /// </summary>
    /// <returns></returns>
    public static bool startPicoStoredetail(string mid) {

        try
        {
            var Intent = new AndroidJavaClass("android.content.Intent");
            var intent = new AndroidJavaObject("android.content.Intent", "picovr.intent.action.storedetail");
            intent.Call<AndroidJavaObject>("putExtra", "intent_extra_app_id", mid);
            intent.Call<AndroidJavaObject>("putExtra", "intent_extra_type", 2);
            intent.Call<AndroidJavaObject>("putExtra", "intent_extra_app_packagename", "com.cybercloud.vr");
            AndroidJavaObject currentActivity = getCurrentActivity();
            currentActivity.Call("startActivity", intent);
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("ucvr startPicoStoredetail:" + e.Message);
            return false;
        }
    }
    private static AndroidJavaObject _currentActivity = null;
    private static Boolean CyberCloudVRActivityEnable = true;
    private static bool getCyberCloudVRActivityEnable(){
        getCurrentActivity();//必须先调用一次否则CyberCloudVRActivityEnable可能未赋值
        return CyberCloudVRActivityEnable;
    }
    public static AndroidJavaObject getCurrentActivity() {
        try
        {
            //当前activity 只需要获取一次即可不用每次使用时都进行获取
            if (_currentActivity == null)
            {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                _currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                try
                {
                    String login_info = _currentActivity.Call<String>("getLogin_info");//如果有getLogin_info方法说明CyberCloudVRActivity有效，否则表示无效
                    CyberCloudVRActivityEnable = true;
                }
                catch (Exception e) {

                    CyberCloudVRActivityEnable = false;
           
                }
            }
            return _currentActivity;
        }
        catch (Exception e) {
            //MyTools.PrintDebugLogError("ucvr getCurrentActivity:" + e.Message);
        }
        return null;
    }
    
    static AndroidJavaObject cyberCloudVRActivityMethodHandler;
    /// <summary>
    /// 用于获取CyberCloudVRActivityMetho，在不使用CyberCloudVRActivity为主入口类的场景下
    /// </summary>
    /// <returns></returns>
    public static AndroidJavaObject getCyberCloudVRActivityMethod()
    {
        try
        {
            //当前activity 只需要获取一次即可不用每次使用时都进行获取
            if (cyberCloudVRActivityMethodHandler == null)
            {
                cyberCloudVRActivityMethodHandler = new AndroidJavaObject(CyberCloudVRActivityMethod);
                AndroidJavaObject content = getCurrentActivity();
                cyberCloudVRActivityMethodHandler.Call("init", content);
            }
            return cyberCloudVRActivityMethodHandler;
        }
        catch (Exception e)
        {
            MyTools.PrintDebugLogError("ucvr getCyberCloudVRActivityMethod:" + e.Message);
        }
        return null;
    }
    public static int VERBOSE = 2;
    public static int DEBUG = 3;
    public static int INFO = 4;
    public static int WARN = 5;
    public static int ERROR = 6;
    /// <summary>
    /// 设置流化sdk日志级别
    /// </summary>
    /// <param name="level"></param>
    public static void setSdkLogLevel(int level)
    {
        PrintDebugLog("ucvr setSdkLogLevel:" + level);
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass jc = new AndroidJavaClass("com.cybercloud.vr.CvrLog");
        jc.CallStatic("SetLevel", level);
#endif
    }
  
    public static string picoDeviceJarName = "com.cybercloud.vr.player.pico.PicoVRDevice";
    
    private static string deepoonDeviceJarName = "com.cybercloud.vr.player.deepoon.DeepoonVRDevice";
    public static string otherDeviceJarName = "com.cybercloud.vr.player.pico.PicoVRDevice";
    public const string CyberCloudJar = "com.cybercloud.vr.CyberCloudJar";
    public const string AdaptivePlatformRequest = "com.cybercloud.vr.AdaptivePlatformRequest";
    public const string CastScreenTool = "com.cybercloud.vr.CastScreenTool";
    public const string VRScreenAdapt = "com.cybercloud.vr_projection.VRScreenAdapt";
    public const string AdaptPlatformCallBack = "com.cybercloud.vr.AdaptPlatformCallBack";
    public const string CastScreenCallBack = "com.cybercloud.vr.CastScreenCallBack";
    public const string InstallApiTool = "com.cybercloud.vr.client.plugin.update.wrapper.CvrUpdateTool";
    public const string InstallApiCallBack = "com.cybercloud.vr.UpdateResult";
    private const string otherDeviceJarInputName = "com.cybercloud.vr.genericdevice.VRGenericDeviceInput";
    private static string CyberCloudVRActivityMethod = "com.cybercloud.vr.CVRActivityMethod";
    private static AndroidJavaObject handlerDevice = null;
    public static AndroidJavaObject getHandlerDevice() {

#if UNITY_ANDROID && !UNITY_EDITOR
       
        if (handlerDevice == null)
        {
            if (OpenApiImp.CyberCloudOpenApiEnable)
            {
                  int devicetype = CyberCloud_UnitySDKAPI.HeadBox.getHmdDofType() == CyberCloud_UnitySDKAPI.HeadBoxDofType.Dof3 ? 0 : 1;// "PicoGoblin" : "PicoNeo";       
                if (CyberCloudConfig.currentType == CyberCloudConfig.DeviceTypes.PicoNeo2 || CyberCloudConfig.currentType == CyberCloudConfig.DeviceTypes.Pico2)
                    devicetype = 2;
                handlerDevice = new AndroidJavaObject(otherDeviceJarName, devicetype);
            }
            else
            {
                if (CyberCloudConfig.currentType == CyberCloudConfig.DeviceTypes.Pico2 || CyberCloudConfig.currentType == CyberCloudConfig.DeviceTypes.Pico || CyberCloudConfig.currentType == CyberCloudConfig.DeviceTypes.PicoNeo2)
                {
                    int devicetype = CyberCloud_UnitySDKAPI.HeadBox.getHmdDofType() == CyberCloud_UnitySDKAPI.HeadBoxDofType.Dof3 ? 0 : 1;// "PicoGoblin" : "PicoNeo";       
                    if (CyberCloudConfig.currentType == CyberCloudConfig.DeviceTypes.PicoNeo2 || CyberCloudConfig.currentType == CyberCloudConfig.DeviceTypes.Pico2)
                        devicetype = 2;
                    handlerDevice = new AndroidJavaObject(picoDeviceJarName, devicetype);
                }
                else if (CyberCloudConfig.currentType == CyberCloudConfig.DeviceTypes.DaPeng)
                    handlerDevice = new AndroidJavaObject(deepoonDeviceJarName);
                else
                    handlerDevice = new AndroidJavaObject(otherDeviceJarName);
            }
        }
#endif
        return handlerDevice;
    }

    //------------------handlerVRGenericDeviceInput
    private static AndroidJavaObject handlerVRGenericDeviceInput = null;
    public static AndroidJavaObject getVRGenericDeviceInput()
    {
        if (handlerVRGenericDeviceInput == null)
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
                handlerVRGenericDeviceInput = new AndroidJavaObject(otherDeviceJarInputName);
            #endif
       
        }
        return handlerVRGenericDeviceInput;
    }


    public static string setDevicePose(int devIndex, float[] pose)
    {
        handlerVRGenericDeviceInput = getVRGenericDeviceInput();
        if (handlerVRGenericDeviceInput != null)
        {
            try
            {
                string param = arrayToString(pose);
                handlerVRGenericDeviceInput.Call("setDevicePose", devIndex, param);
    
                return param;
            }
            catch (Exception e)
            {
                MyTools.PrintDebugLogError("ucvr setDevicePose " + e.Message);
            }
        }
        return "";
    }
    /**
   *  获取手柄获键值接口
   * @param devIndex 设备ID: 1表示手柄1， 2表示手柄2
   * @return
   *      返回结果具体请参见文档
   */
    public static void setControllerKeyEvent(int devIndex, int[] key)
    {
        handlerVRGenericDeviceInput = getVRGenericDeviceInput();
        if (handlerVRGenericDeviceInput != null)
        {
            try
            {          
                string param = arrayToString(key);
                handlerVRGenericDeviceInput.Call("setControllerKeyEvent", devIndex, param);          
            }
            catch (Exception e)
            {
                MyTools.PrintDebugLogError("ucvr  setControllerKeyEvent " + e.Message);
            }
        }
    }
    /**
  *  手柄连接状态
  * @param devIndex  设备ID：1表示手柄1，2表示手柄2
  * @return
  */
    public static void setControllerConnectionState(int devIndex, int state)
    {
        handlerVRGenericDeviceInput = getVRGenericDeviceInput();
        if (handlerVRGenericDeviceInput != null)
        {
            try
            {
                handlerVRGenericDeviceInput.Call("setControllerConnectionState", devIndex, state);
            }
            catch (Exception e)
            {
                MyTools.PrintDebugLogError("ucvr  setControllerConnectionState " + e.Message);
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="controllerType">Ctrl_None = 0    Ctrl_OculusCV1 = 1    Ctrl_Vive = 2    Ctrl_Nolo = 3    Ctrl_ViveIndex = 4    Ctrl_OculusRift = 5</param>
    public static void setControllerType(int controllerType)
    {
        handlerVRGenericDeviceInput = getVRGenericDeviceInput();
        if (handlerVRGenericDeviceInput != null)
        {
            try
            {
                handlerVRGenericDeviceInput.Call("setControllerType", controllerType);
            }
            catch (Exception e)
            {
                MyTools.PrintDebugLogError("ucvr  setControllerType " + e.Message);
            }
        }
    }
    /// <summary>
    /// 是否终端控制帧率
    /// </summary>
    /// <param name="useTerminalFrmRtCtrl">0：不控制，1：终端控制帧率</param>
    public static void setUseTerminalFrmRtCtrl(int useTerminalFrmRtCtrl)
    {
        handlerVRGenericDeviceInput = getVRGenericDeviceInput();
        if (handlerVRGenericDeviceInput != null)
        {
            try
            {
                handlerVRGenericDeviceInput.Call("setUseTerminalFrmRtCtrl", useTerminalFrmRtCtrl);
                // MyTools.PrintDebugLog("ucvr setControllerConnectionState index" + devIndex + ";value:" + state);
            }
            catch (Exception e)
            {
                MyTools.PrintDebugLogError("ucvr  setUseTerminalFrmRtCtrl " + e.Message);
            }
        }
    }
    public static void setHMDFov(float[] hmd)
    {
        handlerVRGenericDeviceInput = getVRGenericDeviceInput();
        if (handlerVRGenericDeviceInput != null)
        {
            try
            {
                handlerVRGenericDeviceInput.Call("setHMDFov", hmd);
                MyTools.PrintDebugLog("ucvr setHMDFov hmd" + hmd);
            }
            catch (Exception e)
            {
                MyTools.PrintDebugLogError("ucvr  setHMDFov " + e.Message);
            }
        }
    }
    private static string arrayToString(float[] values)
    {
        string result = "";
        if (values != null)
        {
            for (int i = 0; i < values.Length; i++)
            {
                result += values[i];
                if (i < values.Length - 1)
                    result += ",";
            }
        }
        return result;
    }
    private static string arrayToString(int[] values)
    {
        string result = "";
        if (values != null)
        {
            for (int i = 0; i < values.Length; i++)
            {
                result += values[i];
                if (i < values.Length - 1)
                    result += ",";
            }
        }
        return result;
    }
    //------------------------------handlerVRGenericDeviceInput
    public static void PrintDebugLog(string msg)
    {

        Debug.Log(msg);
    }
    public static void PrintDebugLogError(string msg)
    {

        Debug.LogError(msg);
    }
    /// <summary>
    /// 获取左右眼的fov值
    /// </summary>
    /// <param name="eyeID">0是左眼</param>
    /// <returns></returns>
    public static float[] getFov(int eyeID)
    {
        float[] fov= new float[4] { 1, 1, 1, 1 };
        #if UNITY_ANDROID && !UNITY_EDITOR
                handlerDevice=getHandlerDevice();
                if (handlerDevice != null)
                {
                    try
                    {
                        fov = handlerDevice.Call<float[]>("getHMDFov", eyeID);
                        MyTools.PrintDebugLog("ucvr get fov eyeID" + eyeID + ";fov:" + fov[0] + "," + fov[1] + "," + fov[2]+","+ fov[3] );              
                    }
                    catch (Exception e)
                    {
                        MyTools.PrintDebugLogError("ucvr  fov value error "+e.Message);
                    }
                }
                else {
                    MyTools.PrintDebugLogError("ucvr canot get fov from callbackclass ");
                }
        #endif
        return fov;
    }

    public static void setSystemProperties(string key, string value)
    {
        handlerDevice = getHandlerDevice();
        if (handlerDevice != null)
        {
            try
            {
                handlerDevice.Call("setSystemProperties", key,value);
                MyTools.PrintDebugLog("ucvr setSystemProperties key" + key + ";value:" + value);
            }
            catch (Exception e)
            {
                MyTools.PrintDebugLogError("ucvr  setSystemProperties " + e.Message);
            }
        }
        else
        {
            MyTools.PrintDebugLogError("ucvr setSystemProperties getHandlerDevice null");
        }
        
    }
    /// <summary>
    /// 手柄的连接状态，1表示已连接，0表示未连接。
    /// </summary>
    /// <param name="devIndex">devIndex表示设备ID  1表示手柄1，2表示手柄2 </param>
    /// <returns></returns>
    public static int getControllerConnectionState(int devIndex) {
        handlerDevice = getHandlerDevice();
        if (handlerDevice != null)
        {
            try
            {
                int state = handlerDevice.Call<int>("getControllerConnectionState", devIndex);

                return state;
            }
            catch (Exception e)
            {
                //e.StackTrace();
                MyTools.PrintDebugLogError("ucvr  getControllerConnectionState error " + e.Message);
            }
        }
        else
        {
            MyTools.PrintDebugLogError("ucvr canot getControllerConnectionState from callbackclass ");
        }
        return 0;
    }
    //private static float updateDataTime;//按键更新时间单位是秒
    private static int [] mUpdateFrame = { -1,-1};//上次更新帧号
    private static int[] controllerKeys1 = new int[8];
    private static int[] controllerKeys2 = new int[8];
    private static int[] preControllerKeys1 = new int[8];
    private static int[] preControllerKeys2 = new int[8];
  
  
    /// <summary>
    /// 获取上一帧数据
    /// </summary>
    /// <param name="devIndex"></param>
    /// <returns></returns>
    public static int[] getPreControllerKeyEvent(int devIndex)
    {
        if (Main.test)
            return new int[8] { 127, 255, 0, 1, 1, 0, 0, 0 };
        if (devIndex == 1)
            return preControllerKeys1;
        else
            return preControllerKeys2;
    }
    /// <summary>
    /// 获取按键
    /// 下标 名称  定义 取值范围
    /// 0	TouchPad x  touchPad X轴坐标值	0~255
    /// 1	TouchPad y  touchPad Y轴坐标值	0~255
    /// 2	Home Home 按键	0未按下，1按下
    /// 3	App App按键	0未按下，1按下
    /// 4	TouchpadClick Touch Pad按键	0未按下，1按下
    /// 5	VolumeUp 音量加键	0未按下，1按下
    /// 6	VolumeDown 音量减键	0未按下，1按下
    /// 7	Trigger Tigger 触摸/按键	0~255，( 63~255 touch， 127~255 Click)
    /// </summary>
    /// <param name="devIndex">1表示手柄1， 2表示手柄2</param>
    /// <param name="groupID">每次获取都是一组数据，手柄1和手柄2</param>
    /// <returns></returns>
    public static int[] getControllerKeyEvent(int devIndex,int groupID)
    {

        if (Main.test)
            return new int[8] { 0,0,0,0,0,0,0,0};

#if UNITY_ANDROID && !UNITY_EDITOR
        //int[] keys = new int[8];
        //缓存一帧数据
        if (Time.frameCount != mUpdateFrame[devIndex-1])
        {
            // GroupID = groupID;
            mUpdateFrame[devIndex - 1] = Time.frameCount;
           
            handlerDevice = getHandlerDevice();
          
            if (handlerDevice != null)
            {
                try
                {
                    int[] controllerKeysTemp = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
                    controllerKeysTemp = handlerDevice.Call<int[]>("getControllerKeyEvent", devIndex);
                    if (devIndex == 1)
                    {
                        preControllerKeys1 = controllerKeys1;
                        controllerKeys1 = controllerKeysTemp;
                    }
                    else
                    {
                        preControllerKeys2 = controllerKeys2;
                        controllerKeys2 = controllerKeysTemp;
                    }
                   
                 
                }
                catch (Exception e)
                {

                    MyTools.PrintDebugLogError("ucvr  getControllerKeyEvent error " + e.Message);
                }
            }
            else
            {
                MyTools.PrintDebugLogError("ucvr canot getControllerKeyEvent from callbackclass ");
            }
        }
#endif

        if (devIndex == 1)
        {
            return controllerKeys1;
        }
        else
        {
            return  controllerKeys2;
        }
  
    }
    public static float[] getDevicePose(int devIndex, float deltaTime)
    {
        if (Main.test)
            return new float[7] { 0, 0, 0, 0, 0, 0, 0 };

#if UNITY_ANDROID && !UNITY_EDITOR
        handlerDevice = getHandlerDevice();          
        if (handlerDevice != null)
        {
            try
            {
                float[] controllerKeysTemp = new float[7] { 0, 0, 0, 0, 0, 0, 0 };
                controllerKeysTemp = handlerDevice.Call<float[]>("getDevicePose", devIndex,deltaTime);
                return controllerKeysTemp;
            }
            catch (Exception e)
            {
                //e.StackTrace();
                MyTools.PrintDebugLogError("ucvr  getControllerKeyEvent error " + e.Message);
            }
        }
        else
        {
            MyTools.PrintDebugLogError("ucvr canot getControllerKeyEvent from callbackclass ");
        }
      
#endif
        return new float[7] { 0, 0, 0, 0, 0, 0, 0 };
    }
    public static void startDispatchEvent(AndroidJavaObject handler,int code)
    {
  
        try
        {


            #if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaObject handlerMethod;
            if (getCyberCloudVRActivityEnable())
            {
                handlerMethod = getCurrentActivity();
            }
            else
            {
                handlerMethod = getCyberCloudVRActivityMethod();
            }
            handlerMethod.Call("startDispatchEvent", handler, 1001);
#endif
        }
        catch (Exception e)
        {
            MyTools.PrintDebugLogError("ucvr startDispatchEvent:" + e.Message);
        }

    }
    public static void stopDispatchEvent()
    {

        try
        {

#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaObject handlerMethod;
            if (getCyberCloudVRActivityEnable())
            {
                handlerMethod = getCurrentActivity();
            }
            else
            {
                handlerMethod = getCyberCloudVRActivityMethod();
            }
            handlerMethod.Call("stopDispatchEvent");
#endif
        }
        catch (Exception e)
        {
            MyTools.PrintDebugLogError("ucvr stopDispatchEvent:" + e.Message);
        }

    }
    public static void unInitCyberCloudVRActivityMethod()
    {

        try
        {

#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaObject handlerMethod;
            if (getCyberCloudVRActivityEnable()==false)
            {         
                handlerMethod = getCyberCloudVRActivityMethod();
                handlerMethod.Call("unInit");
            }
          
#endif
        }
        catch (Exception e)
        {
            MyTools.PrintDebugLogError("ucvr unInitCyberCloudVRActivityMethod:" + e.Message);
        }

    }
    public static void registerXmppBrodcast()
    {
        try
        {

#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaObject handlerMethod;
            if (getCyberCloudVRActivityEnable()==false)
            {         
                handlerMethod = getCyberCloudVRActivityMethod();
                handlerMethod.Call("registerXmppBrodcast");
            }          
#endif
        }
        catch (Exception e)
        {
            MyTools.PrintDebugLogError("ucvr registerXmppBrodcast:" + e.Message);
        }

    }
    /// <summary>
    /// 获取androidSDKVersion
    /// </summary>
    /// <returns>当前androidSDKVersion</returns>
    public static int getAndroidSDKVersion()
    {
        int androidSDKVersion = 0;
        try
        {
           
            //AndroidJavaClass只能调用静态方法，获取静态属性 AndroidJavaObject能调用公开方法和公开属性
            //AndroidJavaClass handler = new AndroidJavaClass(cname);

            AndroidJavaObject handler = new AndroidJavaObject(CyberCloudJar);
            //调用jar包方法获取当前apk的versioncode
            androidSDKVersion = handler.Call<int>("getAndroidSDKVersion");
            MyTools.PrintDebugLog("ucvr getAndroidSDKVersion:" + androidSDKVersion);
        }
        catch (Exception e)
        {
            MyTools.PrintDebugLogError("ucvr androidSDKVersion:" + e.Message);
        }
        return androidSDKVersion;
    }
    
        public static string urlEncode(string strSource, string encode)
    {
        string encoderstr = "";
        try
        {
            AndroidJavaObject handler = new AndroidJavaObject(CyberCloudJar);
            //调用jar包方法获取当前apk的versioncode
            encoderstr = handler.Call<string>("UrlEncode", strSource, encode);
            MyTools.PrintDebugLog("ucvr UrlEncode:" + encoderstr);
        }
        catch (Exception e)
        {
            MyTools.PrintDebugLogError("ucvr androidSDKVersion:" + e.Message);
        }
        return encoderstr;
    }
    public static string urlDecoded(string strSource, string encode)
    {
        string toURLDecodedstr = "";
        try
        {
            AndroidJavaObject handler = new AndroidJavaObject(CyberCloudJar);
            //调用jar包方法获取当前apk的versioncode
            toURLDecodedstr = handler.Call<string>("toURLDecoded", strSource, encode);
            MyTools.PrintDebugLog("ucvr toURLDecoded:" + toURLDecodedstr);
        }
        catch (Exception e)
        {
            MyTools.PrintDebugLogError("ucvr androidSDKVersion:" + e.Message);
        }
        return toURLDecodedstr;
   }
    
    static AndroidJavaObject _adaptPlatformCallBackHandler;
    public static AndroidJavaObject getAdaptPlatformCallBack()
    {

        try
        {
            if (_adaptPlatformCallBackHandler == null)
                _adaptPlatformCallBackHandler = new AndroidJavaObject(AdaptPlatformCallBack);//回调类           
            MyTools.PrintDebugLog("ucvr getAdaptPlatformCallBack");
            return _adaptPlatformCallBackHandler;
        }
        catch (Exception e)
        {
            MyTools.PrintDebugLogError("ucvr getAdaptPlatformCallBack:" + e.Message);
        }
        return _adaptPlatformCallBackHandler;
    }
    private static AndroidJavaObject _handlerAdpat = null;
    /// <summary>
    /// 通过应用适配平台获取启动参数
    /// </summary>
    /// <returns></returns>
    public static void getStartUrlByAdaptForm(String adaptUrl)
    {

#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            _adaptPlatformCallBackHandler=getAdaptPlatformCallBack();
            _handlerAdpat= new AndroidJavaObject(AdaptivePlatformRequest);
            //调用jar包方法获取当前apk的versioncode
            _handlerAdpat.Call("sendRequest",adaptUrl, _adaptPlatformCallBackHandler);
            Debug.Log("ucvr getStartUrlByAdaptForm:" + adaptUrl);
        }
        catch (Exception e)
        {
            Debug.LogError("ucvr getStartUrlByAdaptForm:" + e.Message);
        }
#endif
        return;
    }
    public static void stopAdaptPlatform() {
        try
        {
            if (_handlerAdpat!=null)
            {
                //调用jar包方法获取当前apk的versioncode
                _handlerAdpat.Call("stopAdaptPlatform");
                _handlerAdpat = null;
                Debug.Log("ucvr stopAdaptPlatform ");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("ucvr stopAdaptPlatform:" + e.Message);
        }
    }
    public static void sendBroadcast(string jsonStr)
    {     
        try
        {
            AndroidJavaObject handlerMethod;
            if (getCyberCloudVRActivityEnable())
            {
                handlerMethod = getCurrentActivity();
            }
            else
            {
                handlerMethod = getCyberCloudVRActivityMethod();
            }
            handlerMethod.Call("sendBroadcast", jsonStr);

            MyTools.PrintDebugLog("ucvr sendBroadcast :" + jsonStr);
        }
        catch (Exception e)
        {
            MyTools.PrintDebugLogError("ucvr androidSDKVersion:" + e.Message);
        }
   
    }
    /// <summary>
    /// 通过升级jar包检查升级
    /// </summary>
    /// <param name="jsonStr"></param>
    public static void apkUpdateCheckUp()
    {
        try
        {
            AndroidJavaObject handler = installApiInit();
            string ucvrVersionCode = MyTools.getVersionCode();
            //updateCheck(string tenantId, string packageName, string hmdType, string versionCode, string scene, string CVRGatewayURL)
            string tenantID = CyberCloudConfig.tenantID;
            string packageName = "com.cybercloud.vr";
            string updateUrl = CyberCloudConfig.tls + "/CVRGateway/api/app/apkVersionCheck";
            int result = handler.Call<int>("updateCheck", tenantID, packageName, "PicoNeo2",
                ucvrVersionCode, CyberCloudConfig.cvrScreen, updateUrl,DataLoader.deviceSN);
            MyTools.PrintDebugLog("ucvr apkUpdateCheckUp reult:" + result+ ";DataLoader.deviceSN:"+ DataLoader.deviceSN);
        }
        catch (Exception e)
        {
            MyTools.PrintDebugLogError("ucvr apkUpdateCheckUp:" + e.Message);
        }

    }
    private static AndroidJavaObject updateHandler;//通过升级jar包检查升级
    private static AndroidJavaObject installApiInit()
    {
        try
        {
            if (updateHandler == null)
            {
                AndroidJavaObject currentActivity = getCurrentActivity();
                AndroidJavaObject callback = new AndroidJavaObject(InstallApiCallBack);//回调类
                updateHandler = new AndroidJavaObject(InstallApiTool);               
                int result = updateHandler.Call<int>("init", callback, 1, currentActivity);
                MyTools.PrintDebugLog("ucvr installApiInit reult:" + result);
            }
            return updateHandler;
        }
        catch (Exception e)
        {
            MyTools.PrintDebugLogError("ucvr installApiInit:" + e.Message);
        }
        return null;
    }

        /// <summary>
        /// 通过netty发送投屏消息
        /// </summary>
        /// <param name="jsonStr"></param>
     public static void sendNettySdkCastScreen(string jsonStr)
    {
        try
        {
            NettySdkHandler=getNettyHandler();
            //
            int sendmessageResult = NettySdkHandler.Call<int>("sendMessage", jsonStr);
            MyTools.PrintDebugLog("ucvr sendNettySdkCastScreen reult:" + sendmessageResult);
        }
        catch (Exception e)
        {
            MyTools.PrintDebugLogError("ucvr sendNettySdkCastScreen:" + e.Message);
        }

    }
    public static void disconnetNettyCastScreen()
    {
        try
        {
            NettySdkHandler = getNettyHandler();
            //
            NettySdkHandler.Call("disconnect");
            MyTools.PrintDebugLog("ucvr disconnetNettyCastScreen " );
        }
        catch (Exception e)
        {
            MyTools.PrintDebugLogError("ucvr disconnetNettyCastScreen:" + e.Message);
        }

    }
    private static AndroidJavaObject NettySdkHandler = null;
    public static AndroidJavaObject  getNettyHandler() {
        try { 
            if (NettySdkHandler == null)
            {
             
                NettySdkHandler = new AndroidJavaObject(CastScreenTool);
       
            }
           
        }
        catch (Exception e)
        {
            MyTools.PrintDebugLogError("ucvr getNettyHandler:" + e.Message);
        }
        return NettySdkHandler;
    }
    //本地投屏适配层
    private static AndroidJavaObject VRScreenAdaptHandler = null;
    public static AndroidJavaObject getVRScreenAdaptHandler()
    {
        try
        {
            if (VRScreenAdaptHandler == null)
            {

                VRScreenAdaptHandler = new AndroidJavaObject(VRScreenAdapt);

            }

        }
        catch (Exception e)
        {
            MyTools.PrintDebugLogError("ucvr getNettyHandler:" + e.Message);
        }
        return VRScreenAdaptHandler;
    }

    public static void nettyConnect()
    {
        try
        {
            AndroidJavaObject CastScreen = new AndroidJavaObject(CastScreenCallBack);//回调类
            NettySdkHandler = getNettyHandler();
            //调用jar包方法获取当前apk的versioncode
            NettySdkHandler.Call<AndroidJavaObject>("nettyConnect");
            MyTools.PrintDebugLog("ucvr nettyConnect repeat==");
        }
        catch (Exception e)
        {
            MyTools.PrintDebugLogError("ucvr NettySdkInit:" + e.Message);
        }
    }
    public static int NettySdkInit()
    {
        int result = 100;
        try
        {
            AndroidJavaObject CastScreen = new AndroidJavaObject(CastScreenCallBack);//回调类
            NettySdkHandler = getNettyHandler();
            //调用jar包方法获取当前apk的versioncode
            result = NettySdkHandler.Call<int>("init", CastScreen, CyberCloudConfig.tls, "Game", DataLoader.deviceSN,"Hmd","");
            MyTools.PrintDebugLog("ucvr NettySdkInit reult:" + result+";dSN:"+ DataLoader.deviceSN);
         
            return result;
        }
        catch (Exception e)
        {
            MyTools.PrintDebugLogError("ucvr NettySdkInit:" + e.Message);
        }
        return result;
    }
    private static String  messageCenter = "com.cybercloud.vr_projection.MessageCenterService";
    private static String packageName = "com.cybercloud.vr_projection";
    //private static String packageName = "com.cybercloud.cybercloudtest";
    public static int VRScreenAdaptInit()
    {
        int result = 100;
        try
        {

            VRScreenAdaptHandler = getVRScreenAdaptHandler();
            AndroidJavaObject handlerMethod;
          
           handlerMethod = getCurrentActivity();//获取activity上下文
         
    
            result = VRScreenAdaptHandler.Call<int>("init", handlerMethod, packageName, messageCenter);
            MyTools.PrintDebugLog("ucvr VRScreenAdaptInit reult:" + result + ";dSN:" + DataLoader.deviceSN);

            return result;
        }
        catch (Exception e)
        {
            MyTools.PrintDebugLogError("ucvr VRScreenAdaptInit:" + e.Message);
        }
        return result;
    }
    public static int VRScreenAdaptSendMessage(String key, String body, int type)
    {
        int result = 100;
        try
        {

            VRScreenAdaptHandler = getVRScreenAdaptHandler();
      

            result = VRScreenAdaptHandler.Call<int>("sendMessage", key, body,type);
            if (key != "gameRuning")
                MyTools.PrintDebugLog("ucvr VRScreenAdaptSendMessage key:" + key + ";body:" + body+ ";type:"+ type);

            return result;
        }
        catch (Exception e)
        {
            MyTools.PrintDebugLogError("ucvr VRScreenAdaptSendMessage:" + e.Message);
        }
        return result;
    }
    public static String getLogin_info()
    {
        MyTools.PrintDebugLog("ucvr getLogin_info ");
  
        AndroidJavaObject handlerMethod;
        if (getCyberCloudVRActivityEnable())
        {
            handlerMethod = getCurrentActivity();
        }
        else
        {
            handlerMethod = getCyberCloudVRActivityMethod();
        }
        //AndroidJavaObject handler = new AndroidJavaObject(CyberCloudJar);
        //调用jar包方法获取当前apk的versioncode
        String login_info = handlerMethod.Call<String>("getLogin_info");
        return login_info;
    }
    //注意此方法不支持在線程中調用
    public static String getIntentMapValueStringByKey(String key)
    {
        if (OpenApiImp.CyberCloudOpenApiEnable)
            return null;
        Debug.Log("ucvr getIntentMapValueStringByKey key" + key);
        try
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaObject handlerMethod;
            if (getCyberCloudVRActivityEnable())
            {
                handlerMethod = getCurrentActivity();
            }
            else
            {
                handlerMethod = getCyberCloudVRActivityMethod();
            }
            //获取启动参数
            String value = handlerMethod.Call<string>("getIntentMapValueStringByKey", key);
        return value;
#endif
        }
        catch (Exception e){
            Debug.LogError("ucvr getIntentMapValueStringByKey failed key" + key+";"+e.StackTrace);
        }
        return null;
    }
    public static int getIntentMapValueIntByKey(String key)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Debug.Log("ucvr getIntentMapValueIntByKey key" + key);
        AndroidJavaObject handlerMethod;
        if (getCyberCloudVRActivityEnable())
        {
            handlerMethod = getCurrentActivity();
        }
        else
        {
            handlerMethod = getCyberCloudVRActivityMethod();
        }
        //调用jar包方法获取当前apk的versioncode
        int value = handlerMethod.Call<int>("getIntentMapValueIntByKey", key);
        return value;
#endif
        return -1;
    }
    public static string readNoSectionFromIni(string fileName,string key)
    {
        string res=null;
        try
        {
            //AndroidJavaClass只能调用静态方法，获取静态属性 AndroidJavaObject能调用公开方法和公开属性
            //AndroidJavaClass handler = new AndroidJavaClass(cname);       
            AndroidJavaObject handler = new AndroidJavaObject(CyberCloudJar);
            //调用jar包方法获取当前apk的versioncode
            res=handler.Call<string>("readNoSectionFromIni", fileName,key);
            MyTools.PrintDebugLog("ucvr readNoSectionFromIni :"+res );
        }
        catch (Exception e)
        {
            MyTools.PrintDebugLogError("ucvr readNoSectionFromIni:" + e.Message);
        }
        return res;

    }
    /// <summary>
    /// 获取getAndroidDevInfo
    /// </summary>
    /// <returns>当前getAndroidDevInfo</returns>
    public static String getAndroidDevInfo()
    {
        string androidDevInfo = "";
        try
        {
            AndroidJavaObject handler = new AndroidJavaObject(CyberCloudJar);
            //调用jar包方法获取当前apk的versioncode
            androidDevInfo = handler.Call<string>("getAndroidDevInfo");
            Debug.Log("ucvr getAndroidDevInfo:" + androidDevInfo);
        }
        catch (Exception e)
        {
            MyTools.PrintDebugLogError("ucvr getAndroidDevInfo:" + e.Message);
        }
        return androidDevInfo;
    }
    private static string versionCode = "3";

    public static string getwifiProxy() {
        string wifiProxy="";
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            //AndroidJavaClass只能调用静态方法，获取静态属性 AndroidJavaObject能调用公开方法和公开属性
            AndroidJavaObject currentActivity = getCurrentActivity();
            AndroidJavaObject handler = new AndroidJavaObject(CyberCloudJar);
            //调用jar包方法获取当前apk的versioncode
            wifiProxy = handler.Call<string>("wifiProxy", currentActivity);
            Debug.Log("ucvr wifiProxy wifiProxy:"+ wifiProxy);
        }
        catch (Exception e) {
            Debug.LogError("ucvr wifiProxy:" + e.Message);
        }
#endif
        return wifiProxy;
    }
    /// <summary>
    /// 获取apk的versioncode
    /// </summary>
    /// <returns>当前apk的versioncode</returns>
    public static String getVersionCode()
    {

        // if (!string.IsNullOrEmpty(versionCode)) {
        //     return versionCode;
        // }
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
  
            //AndroidJavaClass只能调用静态方法，获取静态属性 AndroidJavaObject能调用公开方法和公开属性
            AndroidJavaObject currentActivity = getCurrentActivity();
            AndroidJavaObject handler = new AndroidJavaObject(CyberCloudJar);
            //调用jar包方法获取当前apk的versioncode
            versionCode = handler.Call<string>("getVersionCode", currentActivity);
            MyTools.PrintDebugLog("ucvr getVersionCode versionCode:"+ versionCode);
        }
        catch (Exception e) {
            MyTools.PrintDebugLogError("ucvr getVersionCode:" + e.Message);
        }
#endif

        return versionCode;
    }
    
    /// <summary>
    /// 获取apk的versioncode
    /// </summary>
    /// <returns>当前apk的versioncode</returns>
    public static String getAppSignature(string apkPackage="")
    {
        if (string.IsNullOrEmpty(apkPackage))
            apkPackage = CyberCloudConfig.ApkPackage;
        string versionCode = "";
        try
        {
    
            //AndroidJavaClass只能调用静态方法，获取静态属性 AndroidJavaObject能调用公开方法和公开属性
            //AndroidJavaClass handler = new AndroidJavaClass(cname);
            AndroidJavaObject currentActivity = getCurrentActivity();
            AndroidJavaObject handler = new AndroidJavaObject(CyberCloudJar);
            //调用jar包方法获取当前apk的versioncode
            versionCode = handler.Call<string>("getAppSignature", currentActivity, apkPackage);
            Debug.Log("ucvr getAppSignature appSignature:" + versionCode);
        }
        catch (Exception e)
        {
            Debug.LogError("ucvr getVersionCode:" + e.Message);
        }
        return versionCode;
    }
    /// <summary>
    /// 获取apk的versioncode
    /// </summary>
    /// <returns>当前apk的versioncode</returns>
    public static String getDeviceSN()
    {
        string deviceSN = "";
        try
        {

            //AndroidJavaClass只能调用静态方法，获取静态属性 AndroidJavaObject能调用公开方法和公开属性
            //AndroidJavaClass handler = new AndroidJavaClass(cname);
            AndroidJavaObject currentActivity = getCurrentActivity();
            AndroidJavaObject handler = new AndroidJavaObject(CyberCloudJar);
            //调用jar包方法获取当前apk的versioncode
            deviceSN = handler.Call<string>("getDeviceSN");
            MyTools.PrintDebugLog("ucvr getDeviceSN:" + deviceSN);
        }
        catch (Exception e)
        {
            MyTools.PrintDebugLogError("ucvr getDeviceSN:" + e.Message);
        }
        return deviceSN;
    }
    /// <summary>
    /// 获取wifi信号 状态信息
    /// </summary>
    /// <param name="apkPackage"></param>
    /// <returns></returns>
    public static String getWifiInfo()
    {
        string wifiInfo = "";
        try
        {
            AndroidJavaObject handler = new AndroidJavaObject(CyberCloudJar);
            //调用jar包方法获取当前apk的versioncode
            wifiInfo = handler.Call<string>("getWifiInfo" );
   
        }
        catch (Exception e)
        {
            MyTools.PrintDebugLogError("ucvr getWifiInfo exception:" + e.Message);
        }
        return wifiInfo;
    }
    /// <summary>
    /// 启动apk
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool startActivity(string pakageName,string classname, Dictionary<string, string> intentParm)
    {
        try
        {
            var Intent = new AndroidJavaClass("android.content.Intent");
            var ACTION_MAIN = Intent.GetStatic<string>("ACTION_MAIN");
            var CATEGORY_LAUNCHER = Intent.GetStatic<string>("CATEGORY_LAUNCHER");
            var intent = new AndroidJavaObject("android.content.Intent", ACTION_MAIN);
            var cn = new AndroidJavaObject("android.content.ComponentName", pakageName, classname);
         
            if (intentParm != null) {
                foreach (KeyValuePair<string, string> kvp in intentParm) {
                    intent.Call<AndroidJavaObject>("putExtra", kvp.Key, kvp.Value);
                }
            }
            intent.Call<AndroidJavaObject>("addCategory", CATEGORY_LAUNCHER);
            intent.Call<AndroidJavaObject>("setComponent",cn);
            AndroidJavaObject currentActivity = getCurrentActivity();
            currentActivity.Call("startActivity", intent);
            return true;
        }
        catch (System.Exception e)
        {
            MyTools.PrintDebugLogError("ucvr startActivity:" + e.Message);
            return false;
        }
    }
    public static bool startService(string pakageName, string action)
    {
        try
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaObject handlerMethod;
            if (getCyberCloudVRActivityEnable())
            {
                handlerMethod = getCurrentActivity();
            }
            else
            {
                handlerMethod = getCyberCloudVRActivityMethod();
            }
            handlerMethod.Call("startMyService", pakageName, action);
    
#endif
            return true;
        }
        catch (System.Exception e)
        {
            MyTools.PrintDebugLogError("ucvr startService:" + e.Message);
            return false;
        }
    }
    private static string KEY_CONFIG_PATH = "/data/local/tmp/SystemKeyConfig.prop";
    public void copyConfigTodata(string sourceSrc) {
#if UNITY_EDITOR
            StartCoroutine(LoadXml(sourceSrc));
#endif
    }
    IEnumerator LoadXml(string sourceSrc)
    {
        WWW www = null;
        try
        {
            www = new WWW(sourceSrc);
        }
        catch (Exception e)
        {
            MyTools.PrintDebugLogError("ucvr LoadXml:" + e.Message + ";sourceSrc=" + sourceSrc);
        }
        yield return www;
     
        string content= www.text;
        int  r=writeFile(KEY_CONFIG_PATH, content);

        ToolBaseResult result = new ToolBaseResult();
        result.retCode = r;
        Debug.Log("result:" + result);
        if (wwwDataLoad != null)
            wwwDataLoad(result);

    }
    int writeFile(string path,string content)
    {
        try
        {
            FileStream fs = new FileStream(path, FileMode.Create);   //打开一个写入流
            string str = content;
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            fs.Write(bytes, 0, bytes.Length);
            fs.Flush();     //流会缓冲，此行代码指示流不要缓冲数据，立即写入到文件。
            fs.Close();     //关闭流并释放所有资源，同时将缓冲区的没有写入的数据，写入然后再关闭。
            fs.Dispose();   //释放流所占用的资源，Dispose()会调用Close(),Close()会调用Flush();    也会写入缓冲区内的数据。
        }
        catch (Exception e) {
            MyTools.PrintDebugLogError("ucvr writeFile:" + e.Message);
            return 2000;
        }
        return 0;
    }

    float LoadTime = 0;
    /// <summary>
    /// 下载文件
    /// </summary>
    /// <param name="url">下载服务器地址</param>
    /// <param name="file_SaveUrl">本地存储地址</param>
    /// <returns></returns>
    public IEnumerator DownFile(string url, string file_SaveUrl)
    {
        MyTools.PrintDebugLog("ucvr start download url:"+ url);
        LoadTime = 0;
        //防止中文无法加载的问题
        url = System.Uri.EscapeUriString(url);
        WWW www=null;
        try
        {
             www = new WWW(url);
        }
        catch (Exception e)
        {
            MyTools.PrintDebugLogError("ucvr DownFile:" + e.Message+ ";url="+ url);
        }
        //yield return www;


        while (true)
        {
            Debug.Log("ucvr:"+www.progress / 1f * 100);
            if (www.isDone)
            {
                break;
            }
            if (www.error != null)
            {
                Debug.LogError("ucvr downloaderror www.error 1:" + www.error);
                break;
            }
                yield return null;
        }



        if (www.error != null)
        {
            Debug.LogError("ucvr downloaderror www.error:" + www.error);
            ToolBaseResult result = new ToolBaseResult();
            result.retCode = -2;
            if (wwwDataLoad != null)
                wwwDataLoad(result);
        }
        else
        {

            if (www.isDone)
            {
                MyTools.PrintDebugLog("ucvr down over");
                byte[] bytes = www.bytes;
                CreatFile(bytes, file_SaveUrl);
            }


        }
    }
  
public IEnumerator DownApk(string url, string file_SaveUrl)
    {
        string url1 = url;
        ToolBaseResult result = new ToolBaseResult();
        FileInfo file = new FileInfo(file_SaveUrl);
        if (file.Directory.Exists)
        {
            if (file.Exists)
            {
                Debug.Log("ucv delete old update file:" + file_SaveUrl);
                file.Delete();
            }
        }
        else
        {
            Debug.Log("ucv create directory:" + file.Directory);
            file.Directory.Create();
        }
        {
            UnityWebRequest request = UnityWebRequest.Get(url1);
            request.timeout = 10;
            yield return request.SendWebRequest();
            if (request.isHttpError || request.isNetworkError)
            {
                MyTools.PrintDebugLog("ucvr download  isNetworkError");
                result.retCode = 114;
            }
            else if (request.isDone)
            {
                int packLength = 1024 * 2048;//2M   new byte[1024 * 1024]; //  1字节*1024 = 1k 1k*1024 = 1M内存
                byte[] data = request.downloadHandler.data;
                int nReadSize = 0;
                byte[] nbytes = new byte[packLength];
                using (FileStream fs = new FileStream(file_SaveUrl, FileMode.Create))
                using (Stream netStream = new MemoryStream(data))
                {
                    nReadSize = netStream.Read(nbytes, 0, packLength);
                    while (nReadSize > 0)
                    {
                        fs.Write(nbytes, 0, nReadSize);
                        nReadSize = netStream.Read(nbytes, 0, packLength);
                        double dDownloadedLength = fs.Length * 1.0 / (1024 * 1024);
                        double dTotalLength = data.Length * 1.0 / (1024 * 1024);
                        string ss = string.Format("ucvr down apk progress {0:F}M / {1:F}M", dDownloadedLength, dTotalLength);
                        //if (OnDownloadProgressEvent != null)
                        // {
                        //     OnDownloadProgressEvent.Invoke(ss);
                        // }
                        Debug.Log(ss);
                        yield return null;
                    }

                }
            }

        }
        if (wwwDataLoad != null)
        {
            MyTools.PrintDebugLog("ucvr download  finished");
            // OnDownloadCompleteEvent.Invoke();
            //result.retCode = 0;     
         
        }
        wwwDataLoad(result);
    }
    /// <summary>
    /// 缓存到本地文件
    /// </summary>
    /// <param name="bytes"></param>
    void CreatFile(byte[] bytes,string file_SaveUrl)
    {
        try
        {
            Stream stream;
            FileInfo file = new FileInfo(file_SaveUrl);
            if (file.Directory.Exists)
            {
                if (file.Exists)
                {
                    Debug.Log("ucv delete old update file:" + file_SaveUrl);
                    file.Delete();
                }
            }
            else
            {
                Debug.Log("ucv create directory:" + file.Directory);
                file.Directory.Create();
            }

            Debug.Log("ucv bytes:" + bytes.Length+ ";file_SaveUrl:"+ file_SaveUrl);
            stream = file.Create();
            stream.Write(bytes, 0, bytes.Length);
            stream.Close();
            stream.Dispose();
            ToolBaseResult result = new ToolBaseResult();
            result.retCode = 0;
            if (wwwDataLoad != null)
                wwwDataLoad(result);
            else {
                result.retCode = 110;
                wwwDataLoad(result);
            }

            //if (file_SaveUrl.IndexOf(".apk")>0)
            //    InstallAPK(file_SaveUrl);
            
        }
        catch (Exception e)
        {
            MyTools.PrintDebugLogError("ucvr CreatFile:" + e.Message);
            ToolBaseResult result = new ToolBaseResult();
            result.retCode = -2;
            if (wwwDataLoad != null)
                wwwDataLoad(result);
           
        }
    }
  

    public void CaptureScreen(int picnum)
    {
   
        Rect CaptureScreenrect = new Rect(0, 0, 1280, 720);
        Texture2D screenShot = new Texture2D((int)CaptureScreenrect.width, (int)CaptureScreenrect.height, TextureFormat.RGB24, false);

        screenShot.ReadPixels(CaptureScreenrect, 0, 0);

        screenShot.Apply();

        byte[] bytes = screenShot.EncodeToPNG();
        //picnum = picnum + 1;
        string CaptureScreenpicName = "";
        CaptureScreenpicName = "/leftScreenShot" + picnum + ".png";
        string filename = Application.persistentDataPath + CaptureScreenpicName;
        Debug.Log("filename:"+ filename);
        //System.IO.File.WriteAllBytes(filename, bytes);

        ContentWritePicToFile obj = new ContentWritePicToFile(bytes, filename);
        Thread thread1 = new Thread( obj.writePicToFile);
        thread1.Start();

    }
    class ContentWritePicToFile
    {
        public string filename;
        public byte[] bytes;
        public ContentWritePicToFile(byte[] bytes, string filename) {
            this.filename = filename;
            this.bytes = bytes;
        }
        public void writePicToFile()
        {
            System.IO.File.WriteAllBytes(filename, bytes);
        }
    }
    public static void DeleteFolder(string dir)
    {
        foreach (string d in Directory.GetFileSystemEntries(dir))
        {
            if (File.Exists(d))
            {
                FileInfo fi = new FileInfo(d);
                if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                    fi.Attributes = FileAttributes.Normal;
                File.Delete(d);//直接删除其中的文件  
            }
            else
            {
                DirectoryInfo d1 = new DirectoryInfo(d);
                if (d1.GetFiles().Length != 0)
                {
                    DeleteFolder(d1.FullName);////递归删除子文件夹
                }
                Directory.Delete(d);
            }
        }
    }        /// <summary>
             /// 删除文件夹及其内容
             /// </summary>
             /// <param name="dir"></param>
    public static void DeleteFolder1(string dir)
    {
        foreach (string d in Directory.GetFileSystemEntries(dir))
        {
            if (File.Exists(d))
            {
                FileInfo fi = new FileInfo(d);
                if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                    fi.Attributes = FileAttributes.Normal;
                File.Delete(d);//直接删除其中的文件  
            }
            else
                DeleteFolder(d);////递归删除子文件夹
            Directory.Delete(d);
        }
    }
}
