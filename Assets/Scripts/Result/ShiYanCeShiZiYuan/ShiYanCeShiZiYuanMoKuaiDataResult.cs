using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Result
{
    [Serializable]
    public  class ShiYanCeShiZiYuanMoKuaiDataResult
    {
        [SerializeField]
        public int total;

        [SerializeField]
        public ShiYanCeShiZiYuanDataResult requireData = new ShiYanCeShiZiYuanDataResult();

        [SerializeField] 
        public ShiYanCeShiZiYuanDataResult courseData = new ShiYanCeShiZiYuanDataResult();

        [SerializeField]
        public ShiYanCeShiZiYuanDataResult skillData = new ShiYanCeShiZiYuanDataResult();
    }
}
