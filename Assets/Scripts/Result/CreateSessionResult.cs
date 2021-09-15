using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Assets.Scripts.Result
{
    [Serializable]
    public class CreateSessionResult
    {
        [SerializeField]
        public string session_id;

        [SerializeField]
        public string session_key;

        [SerializeField]
        public string is_normal;

    }
}
