using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Result 
{
    [Serializable]
    public class JiaoCaiZhangJieResult
    {
        [SerializeField]
        public string code;

        [SerializeField]
        public string msg;

        [SerializeField]
        public List<JiaoCaiZhangJieDataResult> data = new List<JiaoCaiZhangJieDataResult>();
    }
}

