using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Result 
{
    [Serializable]
    public class GatewayConfigResult
    {
        //需要加载的MEC
        [SerializeField]
        public string MECUrl = "";

        //需要加载的SBY
        [SerializeField]
        public string SBYUrl = "";

        //默认的MEC
        [SerializeField]
        public string MECUrlDefult = "";

        //默认的SBY
        [SerializeField]
        public string SBYUrlDefult = "";
    }
}
