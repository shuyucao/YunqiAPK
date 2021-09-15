using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Result
{
    [Serializable]
    public class ShiYanCeShiZiYuanDataResult
    {
        [SerializeField]
        public int total;

        [SerializeField]
        public List<ShiYanCeShiZiYuanInfoDataResult> resource_info = new List<ShiYanCeShiZiYuanInfoDataResult>();
    }
}
