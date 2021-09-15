using Assets.Scripts.Result;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Data 
{
    public class MainWindowData : Singleton<MainWindowData>
    {
        //用户名称
        private string s_UserName;

        public string GetUserName
        {
            get { return s_UserName; }
            set { s_UserName = value; }
        }

        //用户图像
        private Image s_UserIcon;

        public Image GetUserIcon
        {
            get { return s_UserIcon; }
            set { s_UserIcon = value; }
        }

        //保存用户选择列表
        private UserChooseInfoResult s_UserChooseInfoResult = null;
        public void SaveUserChooseInfoResult(UserChooseInfoResult userChooseInfo)
        {
            this.s_UserChooseInfoResult = userChooseInfo;
        }

        public UserChooseInfoResult GetUserChooseInfoResult()
        {
            return s_UserChooseInfoResult;
        }
    }
}

