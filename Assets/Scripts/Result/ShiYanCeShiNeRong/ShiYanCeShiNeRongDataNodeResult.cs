using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Result 
{
    [Serializable]
    public class ShiYanCeShiNeRongDataNodeResult
    {
        [SerializeField]
        public int order_no;

        [SerializeField]
        public ShiYanCeShiNeRongNodeStudyDataResult study = new ShiYanCeShiNeRongNodeStudyDataResult();

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

