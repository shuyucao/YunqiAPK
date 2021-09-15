using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Request 
{
    [Serializable]
    public class ShiYanCeShiJiLuRequest
    {
        [SerializeField]
        public MacInfoRequest macInfo = new MacInfoRequest();

        [SerializeField]
        public int page;

        [SerializeField]
        public int size;
    }
}

