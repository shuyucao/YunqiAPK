using Assets.Scripts.Config;
using Assets.Scripts.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Windows 
{
    public class YinSiZhengCeAndXieYiWindow : BaseWindow
    {
        //返回按钮
        private Button Return;

        //用户协议和隐私政策内容
        private GameObject YinSiZhengCeText;
        private GameObject YongHuXieYiText;
        public override void OnInit()
        {
            prefabType = PrefabType.Window.YinSiZhengCeAndXieYiWindow;
            prefab = gameObject;
        }

        public override void Init()
        {
            base.Init();
            InitData();
            DefulatSetting();
        }

        private void InitData() 
        {
            Return = GameObject.Find("ReturnYiSi").GetComponent<Button>();
            Return.onClick.AddListener(ReturnOnClick);

            YinSiZhengCeText = GameObject.Find("YinSiZhengCeText");
            YongHuXieYiText = GameObject.Find("YongHuXieYiText");
        }

        private void DefulatSetting() 
        {
            if (YinSiZhengCeAndXieYiWindowData.Instance.GetYinSiOrXieYi == "YongHuXieYi")
            {
                YinSiZhengCeText.SetActive(false);
                YongHuXieYiText.SetActive(true);
            }
            else if (YinSiZhengCeAndXieYiWindowData.Instance.GetYinSiOrXieYi == "YinSiZhengCe")
            {
                YinSiZhengCeText.SetActive(true);
                YongHuXieYiText.SetActive(false);
            }
        }

        //返回
        private void ReturnOnClick()
        {
            YinSiZhengCeAndXieYiWindowData.Instance.GetYinSiOrXieYi = "";
            WindowManager.ReShow();
            WindowManager.Close("YinSiZhengCeAndXieYiWindow");
        }

        public override void OnShow(params object[] para)
        {
            base.OnShow(para);
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
    }
}

