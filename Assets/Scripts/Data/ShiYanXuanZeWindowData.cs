using Assets.Scripts.Result;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Data 
{
    public class ShiYanXuanZeWindowData : Singleton<ShiYanXuanZeWindowData>
    {
        //存储射线选中可以摇杆滑动的物体
        private GameObject tempObject = null;
        public GameObject TempObject
        {
            get { return tempObject; }
            set { tempObject = value; }
        }

        //保存教材章节内容

        private JiaoCaiZhangJieResult s_JiaoCaiZhangJie = null;

        public void SaveJiaoCaiZhangJieResult(JiaoCaiZhangJieResult JiaoCaiZhangJie)
        {
            this.s_JiaoCaiZhangJie = JiaoCaiZhangJie;
        }

        public JiaoCaiZhangJieResult GetJiaoCaiZhangJieResult()
        {
            return s_JiaoCaiZhangJie;
        }

        //保存实验资源信息
        private ShiYanZiYuanInfoResult s_ShiYanZiYuanInfoResult = null;
        public void SaveShiYanZiYuanInfoResult(ShiYanZiYuanInfoResult shiYanZiYuanInfo)
        {
            this.s_ShiYanZiYuanInfoResult = shiYanZiYuanInfo;
        }

        public ShiYanZiYuanInfoResult GetShiYanZiYuanInfoResult()
        {
            return s_ShiYanZiYuanInfoResult;
        }

        //存储用户点击主界面的3本近期教材的哪一本ID
        private string s_RecentTextbooksId = "";

        public string GetRecentTextbooksID
        {
            get { return s_RecentTextbooksId; }
            set { s_RecentTextbooksId = value; }
        }

        //滑动条滑动的值
        private float s_scrollRectPosValue = -1f;

        public float ScrollRectPosValue
        {
            get { return s_scrollRectPosValue; }
            set { s_scrollRectPosValue = value; }
        }

        //存储每个实验资源课件的信息
        private ShiYanZiYuanItemResult ziYuanItemResult = null;
         
        public ShiYanZiYuanItemResult GetShiYanZiYuanItemResult 
        {
            get { return ziYuanItemResult; }
            set { ziYuanItemResult = value; }
        }
    }
}

