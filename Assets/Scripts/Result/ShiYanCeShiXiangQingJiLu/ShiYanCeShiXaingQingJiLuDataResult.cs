using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Result
{
    [Serializable]
    public class ShiYanCeShiXaingQingJiLuDataResult
    {
        [SerializeField]
        public int total;

        [SerializeField]
        public ShiYanCeShiXaingQingJiLuParamResult param = new ShiYanCeShiXaingQingJiLuParamResult();

        [SerializeField]
        public List<ShiYanCeShiXaingQingJiLuItemsResult> items = new List<ShiYanCeShiXaingQingJiLuItemsResult>();
    }
}
