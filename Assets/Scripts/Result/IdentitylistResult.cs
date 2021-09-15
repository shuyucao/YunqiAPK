using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Result 
{
    [Serializable]
    public class IdentitylistResult
    {
        [SerializeField]
        public string identity_code;

        [SerializeField]
        public string identity_name;

        [SerializeField]
        public string tenant_id;
    }
}
