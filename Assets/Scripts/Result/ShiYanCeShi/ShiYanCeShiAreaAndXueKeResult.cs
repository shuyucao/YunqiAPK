using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Result 
{
    [Serializable]
    public class ShiYanCeShiAreaAndXueKeResult
    {
        [SerializeField]
        public string code;

        [SerializeField]
        public string msg;

        [SerializeField]
        public ShiYanCeShiAreaAndXueKeDataResult data = new ShiYanCeShiAreaAndXueKeDataResult();
    }
}

