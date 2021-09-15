using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Assets.Scripts.Result
{
    [Serializable]
    public class AccountLoginResult
    {
        [SerializeField]
        public string account_type;

        [SerializeField]
        public string account_id;

        [SerializeField]
        public string user_id;

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
        public string source_token_account_type;

        [SerializeField]
        public string login_name_type;

        [SerializeField]
        public int password_change_regularly_claim;

        [SerializeField]
        public Boolean weak_pwd;
    }
}
