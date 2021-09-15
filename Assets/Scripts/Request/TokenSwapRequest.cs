using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Request
{
    [Serializable]
    public class TokenSwapRequest
    {
        [SerializeField]
        public string account_id;

        [SerializeField]
        public string session_id;

        [SerializeField]
        public MacInfoRequest macInfo = new MacInfoRequest();
    }
}
