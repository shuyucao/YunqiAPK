using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Request 
{
    [Serializable]
    public class UserInfoRequest
    {
        [SerializeField]
        public MacInfoRequest macInfo;

        [SerializeField]
        public string userId;
    }
}

