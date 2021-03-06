using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Result
{
    [Serializable]
    public class ShiYanCeShiXiangQingJiLuYearDataResult
    {
        [SerializeField]
        public int total;

        [SerializeField]
        public ShiYanCeShiXaingQingJiLuParamResult param = new ShiYanCeShiXaingQingJiLuParamResult();

        [SerializeField]

        public List<ShiYanCeShiXiangQingJiLuYearItemResult> items = new List<ShiYanCeShiXiangQingJiLuYearItemResult>();
    }
}
