using Assets.Scripts.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data
{
    public class ShiYanCeShiXiangQingJiLuDaTiWindowData:Singleton<ShiYanCeShiXiangQingJiLuDaTiWindowData>
    {
        private string s_entryId = "";

        //保存实验标识ID
        public string GetEntryID
        {
            get { return s_entryId; }
            set { s_entryId = value; }
        }

        //保存获取的答题信息
        private ShiYanCeShiXiangQingDaTiDataResult s_XiangQingDaTiDataResult = null;

        public void SaveShiYanCeShiXiangQingDaTiDataResult(ShiYanCeShiXiangQingDaTiDataResult s_XiangQingDaTiDataResult)
        {
            this.s_XiangQingDaTiDataResult = s_XiangQingDaTiDataResult;
        }

        public ShiYanCeShiXiangQingDaTiDataResult GetShiYanCeShiXiangQingDaTiDataResult()
        {
            return s_XiangQingDaTiDataResult;
        }

    }
}
