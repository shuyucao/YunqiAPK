using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Result;
using UnityEngine.UI;

namespace Assets.Scripts.Data
{
    public class ShiYanCeShiWindowData:Singleton<ShiYanCeShiWindowData>
    {
        //保存实验测试地区和学科
        private ShiYanCeShiAreaAndXueKeResult s_AreaAndXueKeResult = null;

        public void SaveShiYanCeShiAreaAndXueKeResult(ShiYanCeShiAreaAndXueKeResult areaAndXueKeResult)
        {
            this.s_AreaAndXueKeResult = areaAndXueKeResult;
        }

        public ShiYanCeShiAreaAndXueKeResult GetShiYanCeShiAreaAndXueKeResult()
        {
            return s_AreaAndXueKeResult;
        }

        //保存实验测试资源
        private ShiYanCeShiZiYuanResult s_CeShiZiYuanResult = null;

        public void SaveShiYanCeShiZiYuanResult(ShiYanCeShiZiYuanResult ceShiZiYuanResult)
        {
            this.s_CeShiZiYuanResult = ceShiZiYuanResult;
        }

        public ShiYanCeShiZiYuanResult GetShiYanCeShiZiYuanResult()
        {
            return s_CeShiZiYuanResult;
        }

        //实验课程分类的名称，必修，技能提升等
        private Text s_courseNameText = null;

        public Text GetFenLeiMoKuaiNameText
        {
            get { return s_courseNameText; }
            set { s_courseNameText = value; }
        }

        //滑动条滑动的值
        private float s_scrollRectPosValue = -1f;

        public float ScrollRectPosValue 
        {
            get { return s_scrollRectPosValue; }
            set { s_scrollRectPosValue = value; }
        }


        //地区名称
        private string AreaName = "";
        public string GetAreaName {
            get { return AreaName; }
            set { AreaName = value; }
        }

        //存储每个实验资源课件的信息
        private ShiYanCeShiZiYuanInfoDataResult ceShiZiYuanInfoDataResult = null;

        public ShiYanCeShiZiYuanInfoDataResult GetShiYanCeShiZiYuanInfoDataResult
        {
            get { return ceShiZiYuanInfoDataResult; }
            set { ceShiZiYuanInfoDataResult = value; }
        }
    }
}
