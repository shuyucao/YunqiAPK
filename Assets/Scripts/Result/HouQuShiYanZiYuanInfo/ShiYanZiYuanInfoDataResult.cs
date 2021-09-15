using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Result 
{
    [Serializable]
    public class ShiYanZiYuanInfoDataResult
    {
        [SerializeField]
        public List<ShiYanZiYuanItemResult> chapter = new List<ShiYanZiYuanItemResult>();

        [SerializeField]
        public int total;
    }
}

