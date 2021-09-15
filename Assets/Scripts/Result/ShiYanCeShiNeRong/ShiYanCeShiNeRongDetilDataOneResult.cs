using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Result
{
    [Serializable]
    public class ShiYanCeShiNeRongDetilDataOneResult
    {
        [SerializeField]
        public int order_no;

        [SerializeField]
        public ShiYanCeShiNeRongStudyDataOneResult study = new ShiYanCeShiNeRongStudyDataOneResult();

        [SerializeField]
        public string instance_id;

        [SerializeField]
        public string node_name;

        [SerializeField]
        public ShiYanCeShiNeRongNodeTestResult testResult = new ShiYanCeShiNeRongNodeTestResult();

        [SerializeField]
        public string node_id;
    }
}
