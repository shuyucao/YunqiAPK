using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Result 
{
    [Serializable]
    public class ZiYuanQuanXianDataResult
    {
        [SerializeField]
        public string id;

        [SerializeField]
        public string type;

        [SerializeField]
        public Dictionary<string, string> resource_operations = new Dictionary<string, string>();
    }
}


