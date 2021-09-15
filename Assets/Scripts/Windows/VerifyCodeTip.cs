using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Scripts.Config.PrefabType;
using Assets.Scripts.Tool;
using Assets.Scripts.Data;
using Assets.Scripts.Manager;
namespace Assets.Scripts.Windows
{
    class VerifyCodeTip:BaseWindow
    {
        private Text verifyText;
        private Button CloseButton;
        private Button BeginCastScreenButton;
        public override void OnInit()
        {
            prefabType = Window.VerifyCodeTip;
            layer = WindowManager.Layer.Window;
        }
        public override void Init()
        {
            base.Init();
            InitData();
        }

        public override void OnShow(params object[] para)
        {
            base.OnShow(para);
            Debug.Log("获取到验证码数据");
            ChangeTextStr(UniversalLoadingWindowData.Instance.checkCode);
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
        private void InitData()
        {
            Debug.Log("绑定显示");
            verifyText = GameObject.Find("VerifyText").GetComponent<Text>();
            Debug.Log("绑定按钮");
            CloseButton = GameObject.Find("CloseBtn").GetComponent<Button>();
            CloseButton.onClick.AddListener(CloseBtnClick);
            BeginCastScreenButton = GameObject.Find("BeginCastScreenButton").GetComponent<Button>();
            BeginCastScreenButton.onClick.AddListener(BeginCastScreen);
        }

        public void ChangeTextStr(string str)
        {
            Debug.Log("VerifyCode"+str);
            verifyText.text ="校验码为：" +str;
        }

        public void CloseBtnClick()
        {
            //隐藏手柄
            PVRControllerManager.Instance.HideBothController();
            WindowManager.Close("VerifyCodeTip");
        }
        public void BeginCastScreen()
        {
            PlayCyberCloundResource.Instance.OnCastScreenClick();
        }

        private void Update()
        {
            if (Pvr_UnitySDKAPI.Controller.UPvr_GetKeyDown(1, Pvr_UnitySDKAPI.Pvr_KeyCode.B))
            {
                CloseBtnClick();
            }
            if (Pvr_UnitySDKAPI.Controller.UPvr_GetKeyDown(0, Pvr_UnitySDKAPI.Pvr_KeyCode.X))
            {
                CloseBtnClick();
            }
            if (Pvr_UnitySDKAPI.Controller.UPvr_GetKeyDown(1, Pvr_UnitySDKAPI.Pvr_KeyCode.A))
            {
                BeginCastScreen();
            }
            if (Pvr_UnitySDKAPI.Controller.UPvr_GetKeyDown(0, Pvr_UnitySDKAPI.Pvr_KeyCode.Y))
            {
                BeginCastScreen();
            }
        }
    }
}
