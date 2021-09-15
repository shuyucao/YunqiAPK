using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Result
{
    [Serializable]
    class TextbookSelectTotalResult
    {
        [SerializeField]
        public string code;
        [SerializeField]
        public string msg;
        [SerializeField]
        public List<JiaoCaiDataInfoResult> data = new List<JiaoCaiDataInfoResult>();
    }
}
