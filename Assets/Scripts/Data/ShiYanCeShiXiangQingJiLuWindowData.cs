using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Result;
using Assets.Scripts.Windows;
using UnityEngine;

namespace Assets.Scripts.Data
{
    public class ShiYanCeShiXiangQingJiLuWindowData:Singleton<ShiYanCeShiXiangQingJiLuWindowData>
    {
        //保存实验测试详情的测试记录弹窗的信息
        private ShiYanCeShiXaingQingJiLuResult s_yanCeShiXaingQingJiLuResult = null;

        public void SaveShiYanCeShiXaingQingJiLuResult(ShiYanCeShiXaingQingJiLuResult shiYanCeShiXaingQingJiLuResult)
        {
            this.s_yanCeShiXaingQingJiLuResult = shiYanCeShiXaingQingJiLuResult;
        }

        public ShiYanCeShiXaingQingJiLuResult GetShiYanCeShiXaingQingJiLuResult()
        {
            return s_yanCeShiXaingQingJiLuResult;
        }


        //保存实验测试详情的测试记录弹窗年月日
        private ShiYanCeShiXiangQingJiLuYearResult s_yanCeShiXiangQingJiLuYearResult  = null;

        public void SaveShiYanCeShiXiangQingJiLuYearResult(ShiYanCeShiXiangQingJiLuYearResult s_yanCeShiXiangQingJiLuYearResult)
        {
            this.s_yanCeShiXiangQingJiLuYearResult = s_yanCeShiXiangQingJiLuYearResult;
        }

        public ShiYanCeShiXiangQingJiLuYearResult GetShiYanCeShiXiangQingJiLuYearResult()
        {
            return s_yanCeShiXiangQingJiLuYearResult;
        }

        //次数的信息面板
        private string s_shiYanTimeDateInfo = "";
        private string s_shiYanTimeInfo = "";
        private string s_shiYanScroeInfo = "";
        private string s_shiYanYongShiInfo = "";
        private int s_trainingMode;

        public string GetShiYanTimeDateInfo 
        {
            get { return s_shiYanTimeDateInfo; }
            set { s_shiYanTimeDateInfo = value; }
        }


        public string GetShiYanTimeInfo 
        {
            get { return s_shiYanTimeInfo; }
            set { s_shiYanTimeInfo = value; }
        }

        public string GetShiYanScroeInfo
        {
            get { return s_shiYanScroeInfo; }
            set { s_shiYanScroeInfo = value; }
        }

        public string GetShiYanYongShiInfo 
        {
            get { return s_shiYanYongShiInfo; }
            set { s_shiYanYongShiInfo = value; }
        }

        public int GetTrainingMode 
        {
            get { return s_trainingMode; }
            set { s_trainingMode = value; }
        }

        //是否点击的是次数
        private bool isClickCount = false;

        public bool IsClickCount 
        {
            get { return isClickCount; }
            set { isClickCount = value; }
        }

        //存储当前的对象
        private ShiYanCeShiXiangQingJiLuWindow s_YanCeShiXiangQingJiLuWindow = null;

        public ShiYanCeShiXiangQingJiLuWindow GetShiYanCeShiXiangQingJiLuWindow 
        {
            get { return s_YanCeShiXiangQingJiLuWindow; }
            set { s_YanCeShiXiangQingJiLuWindow = value; }
        }


        //存储折现图的数据Item
        private Dictionary<GameObject, int> s_ZheXianItemsDic = new Dictionary<GameObject, int>();

        public Dictionary<GameObject, int> ZheXianItemsDic 
        {
            get { return s_ZheXianItemsDic; }
        }

        //选中折线图数据的圆圈图片物体
        private GameObject s_TempSelectNodeObj = null;

        public GameObject TempSelectNodeObj 
        {
            get { return s_TempSelectNodeObj; }
            set { s_TempSelectNodeObj = value; }
        }

        private GameObject s_SelectLien = null;

        public GameObject SelectLien 
        {
            get { return s_SelectLien; }
            set { s_SelectLien = value; }
        }
    }
}
