using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Request 
{
    [Serializable]
    public class ShiYanCeShiZiYuanRequest
    {
        [SerializeField]
        public int offset;

        [SerializeField]
        public int limit;

        [SerializeField]
        public string tags;

        [SerializeField]
        public MacInfoRequest macInfo = new MacInfoRequest();
    }
}

