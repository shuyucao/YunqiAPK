using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Assets.Scripts.Result
{
    [Serializable]
    public class SMSLoginResult
    {
        [SerializeField]
        public string account_type;

        [SerializeField]
        public string account_id;

        [SerializeField]
        public string access_token;

        [SerializeField]
        public string refresh_token;

        [SerializeField]
        public string mac_algorithm;

        [SerializeField]
        public string mac_key;

        [SerializeField]
        public string expires_at;

        [SerializeField]
        public string server_time;

        [SerializeField]
        public string region;

        [SerializeField]
        public Boolean auto_register;

        [SerializeField]
        public string source_token_account_type;

    }
}
