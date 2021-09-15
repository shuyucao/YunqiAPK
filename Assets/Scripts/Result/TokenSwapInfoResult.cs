using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Result
{
    [Serializable]
    public class TokenSwapInfoResult
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
    }
}
