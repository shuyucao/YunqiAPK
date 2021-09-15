using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Result 
{
    [Serializable]
    public class ZiYuanQuanXianResult
    {
        [SerializeField]
        public string code;

        [SerializeField]
        public string msg;

        [SerializeField]
        public ZiYuanQuanXianDataResult data = new ZiYuanQuanXianDataResult();
    }
}

