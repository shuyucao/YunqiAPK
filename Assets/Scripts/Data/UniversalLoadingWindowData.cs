using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Assets.Scripts.Windows;
using Assets.Scripts.Tool;
using Assets.Scripts.Result;
using Assets.Scripts.Constant;
using Assets.Scripts.Request;
using static Assets.Scripts.Tool.RestFulClient;
using Assets.Scripts.Manager;
namespace Assets.Scripts.Data 
{
    public class UniversalLoadingWindowData:Singleton<UniversalLoadingWindowData>
    {
        //开始实验的实验名称
        private string s_ShiYanName = "";

        public string GetShiYanName
        {
            get { return s_ShiYanName; }
            set { s_ShiYanName = value; }
        }

        //开始实验的实验URL
        private string s_ShiYanURL = "";

        public string GetShiYanURL
        {
            get { return s_ShiYanURL; }
            set { s_ShiYanURL = value; }
        }

        private string s_PlayMode = "";
        public string GetPlayMode
        {
            get { return s_PlayMode; }
            set { s_PlayMode = value; }
        }


        //从实验资源进入实验的，判断实验是否结束
        //private string s_ShiYanAppExitDone = "";
        //public string GetShiYanAppExitDone 
        //{
        //    get { return s_ShiYanAppExitDone; }
        //    set { s_ShiYanAppExitDone = value; }
        //}


        //视博云初始化状态3个状态
        private string playStatus;
        private int cyberCloudInitCode;
        private int cyberCloudPlayCode;

        //public string GetPlayStatus
        //{
        //    get { return playStatus; }
        //    set { playStatus = value; }
        //}

        public int GetCyberCloudInitCode
        {
            get { return cyberCloudInitCode; }
            set { cyberCloudInitCode = value; }
        }

        public int GetCyberCloudPlayCode
        {
            get { return cyberCloudPlayCode; }
            set { cyberCloudPlayCode = value; }
        }

        public string ResourcesType;

        //APP状态监听
        private string appStatusListen;
        public string AppStatusListen
        {
            get
            {
                return appStatusListen;
            }
            set
            {
                appStatusListen = value;
                CompareStatus();
            }
        }
        private GameObject rootObj;
        private bool isdone = false;
        private void CompareStatus()
        {
            if (appStatusListen == "appStartDone")
            {
                if (isdone)
                {
                    return;
                }
                else
                {
                    WindowManager.Close("UniversalLoadingWindow");
                    //rootObj = GameObject.Find("Root");
                    //rootObj.SetActive(false);
                    isdone = true;
                    //提交试验记录
                    SubmitJuLuByVRCourseOrVRTest();

                    //开始投屏
                    PlayCyberCloundResource.Instance.OnCastScreenClick();
                }

            }
            else if(appStatusListen == "appExitDone")
            {
                if (isdone)
                {
                    Debug.Log("退出应用");
                    //显示手柄
                    PVRControllerManager.Instance.ShowBothController();
                    //rootObj.SetActive(true);
                    WindowManager.Open<LoadingWindow>();
                    isdone = false;
                    PlayCyberCloundResource.Instance.StopCastScreen();
                }
                else
                {
                    return;
                }

            }
        }
        //投屏状态监听
        private string castScreenListen;
        public string CastScreenListen
        {
            get
            {
                return castScreenListen;
            }
            set
            {
                castScreenListen = value;
                CompareCastScreenStatus();
            }
        }

        public string checkCode ="1001011";
        private void CompareCastScreenStatus()
        {
            if (castScreenListen == "bindSTB")
            {
                //启动显示码
                WindowManager.Open<VerifyCodeTip>();

            }
            else
            {
                //隐藏手柄
                PVRControllerManager.Instance.HideBothController();
            }
        }


        RestClient client = new RestClient();

        //提交实验资源记录
        public void SubmitShiYanZiYuanJiLu(ShiYanZiYuanItemResult shiYanZiYuanItem)
        {
            client.EndPoint = CommonConstant.SERVER_URL_MEC_PROXY;
            client.Method = EnumHttpVerb.POST;
            SaveActivityRequest activityRequest = new SaveActivityRequest();
            MacInfoRequest macInfo = new MacInfoRequest();
            if (LoginWindowData.Instance.ReadAccountLoginResult() != null)
            {
                macInfo.mac_key = LoginWindowData.Instance.ReadAccountLoginResult().mac_key;
                macInfo.token = LoginWindowData.Instance.ReadAccountLoginResult().access_token;
            }
            else if (LoginWindowData.Instance.ReadToKenSwapResult() != null)
            {
                macInfo.mac_key = LoginWindowData.Instance.ReadToKenSwapResult().data.mac_key;
                macInfo.token = LoginWindowData.Instance.ReadToKenSwapResult().data.access_token;
            }
            macInfo.session_key = LoginWindowData.Instance.ReadSessionResult().session_key;
            activityRequest.macInfo = macInfo;
            activityRequest.objectId = shiYanZiYuanItem.objectId;
            activityRequest.objectName = shiYanZiYuanItem.name;
            activityRequest.type = shiYanZiYuanItem.type;
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (DateTime.Now.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位   
            activityRequest.timestampStart = t.ToString();
            activityRequest.timestampEnd = t.ToString();
            activityRequest.object_extensions = shiYanZiYuanItem;

            client.PostData = JsonUtility.ToJson(activityRequest);
            string result = client.HttpRequest(CommonConstant.GET_SAVESHIYANJILU);
            Debug.Log("提交实验资源记录：" + result);
        }

        //提交实验测试记录
        public void SubmitShiYanCeShiJiLu(ShiYanCeShiZiYuanInfoDataResult shiYanCeShiZiYuanInfoData)
        {
            client.EndPoint = CommonConstant.SERVER_URL_MEC_PROXY;
            client.Method = EnumHttpVerb.POST;
            SaveActivityRequest activityRequest = new SaveActivityRequest();
            MacInfoRequest macInfo = new MacInfoRequest();
            if (LoginWindowData.Instance.ReadAccountLoginResult() != null)
            {
                macInfo.mac_key = LoginWindowData.Instance.ReadAccountLoginResult().mac_key;
                macInfo.token = LoginWindowData.Instance.ReadAccountLoginResult().access_token;
            }
            else if (LoginWindowData.Instance.ReadToKenSwapResult() != null)
            {
                macInfo.mac_key = LoginWindowData.Instance.ReadToKenSwapResult().data.mac_key;
                macInfo.token = LoginWindowData.Instance.ReadToKenSwapResult().data.access_token;
            }
            macInfo.session_key = LoginWindowData.Instance.ReadSessionResult().session_key;
            activityRequest.macInfo = macInfo;
            activityRequest.objectId = shiYanCeShiZiYuanInfoData.id;
            activityRequest.objectName = shiYanCeShiZiYuanInfoData.name;
            activityRequest.type = shiYanCeShiZiYuanInfoData.type;
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (DateTime.Now.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位   
            activityRequest.timestampStart = t.ToString();
            activityRequest.timestampEnd = t.ToString();
            activityRequest.object_extensions = shiYanCeShiZiYuanInfoData;

            client.PostData = JsonUtility.ToJson(activityRequest);
            string result = client.HttpRequest(CommonConstant.GET_SAVESHIYANJILU);
            Debug.Log("提交实验测试记录：" + result);
        }


        public void SubmitJuLuByVRCourseOrVRTest() 
        {
            if (GetPlayMode == "VRCourse")
            {
                SubmitShiYanZiYuanJiLu(ShiYanXuanZeWindowData.Instance.GetShiYanZiYuanItemResult);
            }
            else if (GetPlayMode == "VRPractice" || GetPlayMode == "VRTest")
            {
                SubmitShiYanCeShiJiLu(ShiYanCeShiWindowData.Instance.GetShiYanCeShiZiYuanInfoDataResult);
            }
        }
    }

}
