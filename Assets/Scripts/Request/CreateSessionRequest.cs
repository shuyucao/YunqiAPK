using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
namespace Assets.Scripts.Request
{
    [Serializable]
    public class CreateSessionRequest
    {
        [SerializeField]
        public string device_id;
    }
}
