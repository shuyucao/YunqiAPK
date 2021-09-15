using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
namespace Assets.Scripts.Request
{
    [Serializable]
    public class SMSLoginRequest
    {
        [SerializeField]
        public string country_code;

        [SerializeField]
        public string mobile;

        [SerializeField]
        public string sms_code;

        [SerializeField]
        public string session_id;
    }
}
