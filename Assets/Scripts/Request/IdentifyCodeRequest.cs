using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
namespace Assets.Scripts.Result
{
    [Serializable]
    class IdentifyCodeRequest
    {
        [SerializeField]
        public string identify_code;

        [SerializeField]
        public bool is_delete_when_right;
    }
}
