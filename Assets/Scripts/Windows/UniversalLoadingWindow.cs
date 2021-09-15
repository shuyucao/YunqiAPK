using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Assets.Scripts.Config;
using Assets.Scripts.Data;
using Assets.Scripts.Tool;
using System;

namespace Assets.Scripts.Windows 
{
    public enum ResPlayMode
    {
        VRCourse,
        VRPractice,
        VRExam,
        VRTest,
    }
    public class UniversalLoadingWindow : BaseWindow
    {

        private Text ResourcesType;
        //进度条
        private Slider ProgressSlider;

        //实验名称
        private Text ShiYanNameText;

        //错误提示
        private GameObject ErrorText;

        //是否加载成功
        public bool isLoadSuccfull = false;
        public override void OnInit()
        {
            base.OnInit();
            prefabType = PrefabType.Window.UniversalLoadingWindow;
            prefab = gameObject;
        }

        public override void Init()
        {
            base.Init();
            InitData();
            DefaultSeting();
        }

        public override void OnClose()
        {
            base.OnClose();
        }

        public override void OnHide()
        {
            base.OnHide();
        }

        public override void OnReShow()
        {
            base.OnReShow();
        }

        public override void OnShow(params object[] para)
        {
            base.OnShow(para);
        }

        //默认设置
        private void DefaultSeting() 
        {
            isLoadSuccfull = false;
            ShiYanNameText.text = UniversalLoadingWindowData.Instance.GetShiYanName;
            ProgressSlider.value = 0;
            //ErrorText.SetActive(false);
            ResourcesType.text = UniversalLoadingWindowData.Instance.ResourcesType;
            //判断视博云是初始化成功
            if (UniversalLoadingWindowData.Instance.GetCyberCloudInitCode != 0)
            {
                Debug.Log("视博云是初始化失败！");
                ShiYanNameText.text = "网络加载失败，点击任意位置重试";
                return;
            }
            PlayCyberCloundResource.Instance.StartPlayApp(UniversalLoadingWindowData.Instance.GetShiYanName, UniversalLoadingWindowData.Instance.GetShiYanURL,UniversalLoadingWindowData.Instance.GetPlayMode);
            if (UniversalLoadingWindowData.Instance.AppStatusListen == "appstarting")
            {
                isLoadSuccfull = true;
            }
            //else
            //{
            //    //判断是否启动成功
            //    if (UniversalLoadingWindowData.Instance.GetCyberCloudPlayCode == 0)
            //    {
            //        isLoadSuccfull = true;
            //    }
            //    else
            //    {
            //        UniversalLoadingWindowData.Instance.GetShiYanAppExitDone = "appExitDone";
            //        //ErrorText.SetActive(true);
            //    }
            //}
        }

        //初始化
        private void InitData() 
        {
            ResourcesType = GameObject.Find("ResourcesType").transform.GetChild(0).GetComponent<Text>();
            ShiYanNameText = GameObject.Find("ShiYanName").GetComponent<Text>();
            ProgressSlider = GameObject.Find("Slider").GetComponent<Slider>();
            //ErrorText = GameObject.Find("LoadError");
            //UniversalReturn = GameObject.Find("UniversalReturn").GetComponent<Button>();
            //UniversalReturn.onClick.AddListener(ReturnOnClick);
        }

        private void LateUpdate()
        {
            if (isLoadSuccfull)
            {
                if (ProgressSlider.value <= 0.9)
                {
                    ProgressSlider.value += 0.1f;
                }
                //if (UniversalLoadingWindowData.Instance.GetPlayStatus == "appStartDone")
                //{
                //    StartCoroutine(DeyleEnter());
                //}
                //else if (UniversalLoadingWindowData.Instance.GetPlayStatus == "appExitDone")
                //{
                //    UniversalLoadingWindowData.Instance.GetShiYanAppExitDone = "appExitDone";
                //    WindowManager.Close("UniversalLoadingWindow");
                //    WindowManager.Open<LoadingWindow>();
                //}
            }
        }

        //private IEnumerator DeyleEnter()
        //{
        //    ProgressSlider.value = 1;
        //    yield return new WaitForSeconds(0.5f);
        //    WindowManager.Close("UniversalLoadingWindow");
        //}


    }
}

