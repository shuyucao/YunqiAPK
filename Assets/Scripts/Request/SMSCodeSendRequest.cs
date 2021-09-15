using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
namespace Assets.Scripts.Request
{
    [Serializable]
    public class SMSCodeSendRequest
    {
        [SerializeField]
        public string op_type;

        [SerializeField]
        public string country_code;

        [SerializeField]
        public string mobile;

        [SerializeField]
        public string identify_code;
    }
}
