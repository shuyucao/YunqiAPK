﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Result 
{
    [Serializable]
    public class ShiYanCeShiZiYuanResult
    {
        [SerializeField]
        public string code;

        [SerializeField]
        public string msg;

        [SerializeField]
        public ShiYanCeShiZiYuanMoKuaiDataResult data = new ShiYanCeShiZiYuanMoKuaiDataResult();
    }
}

