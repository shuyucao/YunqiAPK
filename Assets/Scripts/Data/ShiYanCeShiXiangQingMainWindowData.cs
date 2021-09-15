using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Result;

namespace Assets.Scripts.Data
{
    public class ShiYanCeShiXiangQingMainWindowData:Singleton<ShiYanCeShiXiangQingMainWindowData>
    {
        //从实验测试列表点击的实验名称
        private string s_ShiYanCeShiName = "";

        public string ShiYanCeShiName
        {
            get { return s_ShiYanCeShiName; }
            set { s_ShiYanCeShiName = value; }
        }


        //选择的实验测试的ID
        private string s_ShiYanCeShiId = "";

        public string ShiYanCeShiId
        {
            get { return s_ShiYanCeShiId; }
            set { s_ShiYanCeShiId = value; }
        }

        //保存实验测试内容
        private ShiYanCeShiNeRongResult s_ShiYanCeShiNeRongResult = null;

        public void SaveShiYanCeShiNeRongResult(ShiYanCeShiNeRongResult shiYanCeShiNeRongResult)
        {
            this.s_ShiYanCeShiNeRongResult = shiYanCeShiNeRongResult;
        }

        public ShiYanCeShiNeRongResult GetShiYanCeShiNeRongResult()
        {
            return s_ShiYanCeShiNeRongResult;
        }

        //保存实验测试内容
        private ShiYanCeShiNeRongOneResult s_ShiYanCeShiNeRongOneResult = null;

        public void SaveShiYanCeShiNeRongOneResult(ShiYanCeShiNeRongOneResult shiYanCeShiNeRongOneResult)
        {
            this.s_ShiYanCeShiNeRongOneResult = shiYanCeShiNeRongOneResult;
        }

        public ShiYanCeShiNeRongOneResult GetShiYanCeShiNeRongOneResult()
        {
            return s_ShiYanCeShiNeRongOneResult;
        }

        //是否显示上一次的实验的时长，掌握度，易错项的记录还是显示本次的实验的信息
        private bool s_isLastShiYanCeShiInfo = false;

        public bool IsLastShiYanCeShiMingXiInfo 
        {
            get { return s_isLastShiYanCeShiInfo; }
            set { s_isLastShiYanCeShiInfo = value; }
        }

        //选择的实验的instanceId
        private string s_instanceId = "";

        public string GetInstanceId 
        {
            get { return s_instanceId; }
            set { s_instanceId = value; }
        }


        //是否有实验记录
        private bool s_IsNoJiLuData = false;

        public bool IsNoJiLuData 
        {
            get { return s_IsNoJiLuData; }
            set { s_IsNoJiLuData = value; }
        }

        //是否有易错项数据
        private bool s_IsNoYiCuoXiangData = false;

        public bool IsNoYiCuoXiangData
        {
            get { return s_IsNoYiCuoXiangData; }
            set { s_IsNoYiCuoXiangData = value; }
        }
    }
}
