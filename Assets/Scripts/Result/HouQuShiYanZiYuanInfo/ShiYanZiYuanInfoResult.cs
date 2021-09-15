using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Result 
{
    [Serializable]
    public class ShiYanZiYuanInfoResult
    {
        [SerializeField]
        public string code;

        [SerializeField]
        public string msg;

        [SerializeField]
        public ShiYanZiYuanInfoDataResult data = new ShiYanZiYuanInfoDataResult();
    }
}

