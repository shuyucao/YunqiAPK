using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Assets.Scripts.Request
{
    [Serializable]
    public class MacInfoRequest
    {
        [SerializeField]
        public string mac_key;

        [SerializeField]
        public string token;

        [SerializeField]
        public string session_key;
    }
}
