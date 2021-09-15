using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Result
{
    [Serializable]
    public class ShiYanCeShiXaingQingJiLuResult
    {
        [SerializeField]
        public string code;

        [SerializeField]
        public string msg;

        [SerializeField]
        public ShiYanCeShiXaingQingJiLuDataResult data = new ShiYanCeShiXaingQingJiLuDataResult();
    }
}
