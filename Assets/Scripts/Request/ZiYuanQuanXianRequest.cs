using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Request 
{
    [Serializable]
    public class ZiYuanQuanXianRequest
    {
        [SerializeField]
        public string id;

        [SerializeField]
        public string type;

        [SerializeField]
        public List<string> tags = new List<string>();
    }

}
