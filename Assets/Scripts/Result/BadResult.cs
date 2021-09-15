using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Assets.Scripts.Result
{
    [Serializable]
    class BadResult
    {
        [SerializeField]
        public string host_id;

        [SerializeField]
        public string request_id;

        [SerializeField]
        public string server_time;

        [SerializeField]
        public string code;

        [SerializeField]
        public string message;
    }
}
