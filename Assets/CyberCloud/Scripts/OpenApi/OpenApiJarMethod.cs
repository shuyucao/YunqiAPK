using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.CyberCloud.Scripts.OpenApi
{
    public class OpenApiJarMethod
    {
        static AndroidJavaObject handlerCyberCloudJar;
        private static AndroidJavaObject getCyberCloudJar() {
            if(handlerCyberCloudJar==null)
                handlerCyberCloudJar = new AndroidJavaObject(MyTools.CyberCloudJar);
            return handlerCyberCloudJar;
        }
        /// <summary>
        /// 初始化配置服务
        /// </summary>
        /// <param name="baseUrl">配置服务地址，由视博云提供</param>
        /// <param name="terminalType">参考附属目录-终端类型</param>
        /// <param name="tenantID">租户id,iptv项目需传入此参数</param>
        /// <returns>初始化结果，是否真的初始化成功需要监听回调结果</returns>
        public static int initConfigService(String baseUrl, int terminalType, String tenantID)
        {
         
            int code = 0;
            try
            {

                //AndroidJavaClass只能调用静态方法，获取静态属性 AndroidJavaObject能调用公开方法和公开属性
                //AndroidJavaClass handler = new AndroidJavaClass(cname);
                AndroidJavaObject currentActivity = MyTools.getCurrentActivity();
                AndroidJavaObject handler = getCyberCloudJar();
                //调用jar包方法获取当前apk的versioncode
                code = handler.Call<int>("init", currentActivity, baseUrl, terminalType, tenantID);
                MyTools.PrintDebugLog("ucvr initConfigService baseUrl:" + baseUrl+ ";terminalType:"+ terminalType+ ";tenantID:"+ tenantID);
            }
            catch (Exception e)
            {
                MyTools.PrintDebugLogError("ucvr initConfigService:" + e.Message);
            }
            return code;
        }
        /// <summary>
        /// 通过配置服务获取gds地址
        /// </summary>
        /// <returns></returns>
        public static string getGDSUrlByConfigService()
        {

            string gdsurl = "";
            try
            {

                //AndroidJavaClass只能调用静态方法，获取静态属性 AndroidJavaObject能调用公开方法和公开属性
                //AndroidJavaClass handler = new AndroidJavaClass(cname);
   
                AndroidJavaObject handler = getCyberCloudJar();
                //调用jar包方法获取当前apk的versioncode
                gdsurl = handler.Call<string>("getGdsUrl");
                MyTools.PrintDebugLog("ucvr getGDSUrlByConfigService:" + gdsurl);
            }
            catch (Exception e)
            {
                MyTools.PrintDebugLogError("ucvr getGDSUrlByConfigService:" + e.Message);
            }
            return gdsurl;
        }
        /// <summary>
        /// 通过配置服务获取gds地址
        /// </summary>
        /// <returns></returns>
        public static int applyQueue(string appID, string userID, int userLevel, string ext)
        {

            int code = 0;
            try
            {
                AndroidJavaObject handler = getCyberCloudJar();
                code = handler.Call<int>("applyQueue", appID, userID, userLevel, ext);
                MyTools.PrintDebugLog("ucvr applyQueue:" + code);
            }
            catch (Exception e)
            {
                MyTools.PrintDebugLogError("ucvr applyQueue:" + e.Message);
            }
            return code;
        }
        /// <summary>
        /// 通过配置服务获取gds地址
        /// </summary>
        /// <returns></returns>
        public static int queryQueue(string queueCode)
        {
            int code = 0;
            try
            {
                AndroidJavaObject handler = getCyberCloudJar();
                code = handler.Call<int>("queryQueue", queueCode);
                MyTools.PrintDebugLog("ucvr queryQueue:" + code);
            }
            catch (Exception e)
            {
                MyTools.PrintDebugLogError("ucvr queryQueue:" + e.Message);
            }
            return code;
        }
        /// <summary>
        /// 通过配置服务获取gds地址
        /// </summary>
        /// <returns></returns>
        public static int cancelQueue(string queueCode)
        {

            int code = 0;
            try
            {
                AndroidJavaObject handler = getCyberCloudJar();
                code = handler.Call<int>("cancelQueue", queueCode);
                MyTools.PrintDebugLog("ucvr cancelQueue:" + code);
            }
            catch (Exception e)
            {
                MyTools.PrintDebugLogError("ucvr cancelQueue:" + e.Message);
            }
            return code;
        }
        //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<日志收集开始<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
        static AndroidJavaObject instanceCyberInfoService;
        /// <summary>
        /// 日志收集
        /// </summary>
        /// <returns></returns>
        private static AndroidJavaObject getCyberInfoServiceInistance()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            MyTools.PrintDebugLog("ucvr getCyberInfoServiceInistance");
            try
            {
                if (instanceCyberInfoService == null)
                {
                    AndroidJavaObject handler = getCyberCloudJar();

                    instanceCyberInfoService = handler.Call<AndroidJavaObject>("getCyberInfoService");
                   

                }
                return instanceCyberInfoService;
            }
            catch (Exception e)
            {
                MyTools.PrintDebugLogError("ucvr getCyberInfoServiceInistance:" + e.Message);
            }
#endif
            return null;
        }
        public static void cyberInfoService_resetMap()
        {
            MyTools.PrintDebugLog("ucvr cyberInfoService_resetMap");
#if UNITY_ANDROID && !UNITY_EDITOR
            try { 
                 getCyberInfoServiceInistance().Call("resetMap");
            }
            catch (Exception e)
            {
                MyTools.PrintDebugLogError("ucvr cyberInfoService_resetMap:" + e.Message);
            }
#endif
        }
        public static void cyberInfoService_updateInfo(String appID, String userID, String zone, String edgeCode, String cloudUrl)
        {

#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {               
                getCyberInfoServiceInistance().Call("updateInfo", appID, userID, zone, edgeCode,cloudUrl);
            }
            catch (Exception e)
            {
                MyTools.PrintDebugLogError("ucvr cyberInfoService_updateInfo:" + e.Message);
            }
#endif
        }
        public static void cyberInfoService_uploadAction(Boolean isLastRetry, String ctrl, String deskIP, String sessionID, String retCode, String retMessage, String starturl)
        {


#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                getCyberInfoServiceInistance().Call("uploadAction", isLastRetry,deskIP,sessionID, ctrl, retCode, retMessage,starturl);
            }
            catch (Exception e)
            {
                MyTools.PrintDebugLogError("ucvr cyberInfoService_uploadAction:" + e.Message);
            }
#endif
        }
        public static void cyberInfoService_changeStreamState(Boolean isRunning)
        {

#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                getCyberInfoServiceInistance().Call("changeStreamState", isRunning);
            }
            catch (Exception e)
            {
                MyTools.PrintDebugLogError("ucvr cyberInfoService_changeStreamState:" + e.Message);
            }
#endif
        }
        public static void cyberInfoService_addExtParam( string ext)
        {

#if UNITY_ANDROID && !UNITY_EDITOR

         
            
           AndroidJavaObject handler = getCyberCloudJar();
           handler.Call<int>("cyberInfoService_addExtParam", ext);
#endif
        }

        //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>日志收集结束>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
    }
}
