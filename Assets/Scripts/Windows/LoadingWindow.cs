using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Config;
using Assets.Scripts.Data;
using Assets.Scripts.Tool;
namespace Assets.Scripts.Windows
{
    public class LoadingWindow : BaseWindow
    {
        public override void OnInit()
        {
            base.OnInit();
            prefabType = PrefabType.Window.LoadingWindow;
            prefab = gameObject;
        }

        //初始化
        public override void Init()
        {
            base.Init();
            
            //OpenMainWindow();

        }

        //打开主界面
        IEnumerator OpenMainWindow()
        {
            yield return new WaitForSeconds(1f);
            if (UniversalLoadingWindowData.Instance.AppStatusListen == "appExitDone")
            {
                //停止APP
                PlayCyberCloundResource.Instance.StopApp();
                Debug.Log("显示上一个Window");
                WindowManager.ReShow();
            }
            else if (PlayerPrefs.HasKey("account") && PlayerPrefs.HasKey("password")) 
            {
                WindowManager.Open<MainWindow>();
            }
            else
            {
                if (string.IsNullOrEmpty(BaseInfoWindowData.Instance.GradeName))
                {
                    WindowManager.Open<BaseInfoWindow>();
                    BaseInfoWindowData.Instance.FromWindow = "LoadingWindow";
                }
            }
            
            Debug.Log("关闭加载界面");
            WindowManager.Close("LoadingWindow");
            //if (UniversalLoadingWindowData.Instance.GetShiYanAppExitDone =="")
            //{
            //    WindowManager.Open<BaseInfoWindow>();
            //    BaseInfoWindowData.Instance.FromWindow = "LoadingWindow";
            //}
            //else if (UniversalLoadingWindowData.Instance.GetShiYanAppExitDone == "appExitDone")
            //{
            //    UniversalLoadingWindowData.Instance.GetShiYanAppExitDone = "";
            //    WindowManager.ReShow();
            //}
        }

        public override void OnShow(params object[] para)
        {
            base.OnShow(para);
            Debug.Log("打开加载界面");
            StartCoroutine(OpenMainWindow());
        }

        public override void OnClose()
        {
            base.OnClose();
        }
    }
}
