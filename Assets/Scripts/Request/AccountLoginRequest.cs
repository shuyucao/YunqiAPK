using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Assets.Scripts.Request
{
    [Serializable]
    public class AccountLoginRequest
    {
        [SerializeField]
        public string login_name;

        [SerializeField]
        public string login_name_type;

        [SerializeField]
        public string org_code;

        [SerializeField]
        public string password;

        [SerializeField]
        public string session_id;

        [SerializeField]
        public string identify_code;
    }
}
