using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Result;

namespace Assets.Scripts.Data
{
    public class ShiYanCeShiYiCuoXiangWindowData:Singleton<ShiYanCeShiYiCuoXiangWindowData>
    {
        private ShiYanCeShiYiCuoXiangResult s_YanCeShiYiCuoXiangResult = null;

        public void SaveShiYanCeShiYiCuoXiangResult(ShiYanCeShiYiCuoXiangResult s_ShiYanCeShiYiCuoXiangResult)
        {
            this.s_YanCeShiYiCuoXiangResult = s_ShiYanCeShiYiCuoXiangResult;
        }

        public ShiYanCeShiYiCuoXiangResult GetShiYanCeShiYiCuoXiangResult()
        {
            return s_YanCeShiYiCuoXiangResult;
        }
    }
}
