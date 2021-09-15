using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Assets.Scripts.Request
{
    [Serializable]
    public class SaveActivityRequest
    {
        [SerializeField]
        public MacInfoRequest macInfo;

        [SerializeField]
        public string objectId;

        [SerializeField]
        public string objectName;

        [SerializeField]
        public string type;

        [SerializeField]
        public string timestampStart;

        [SerializeField]
        public string timestampEnd;

        [SerializeField]
        public object object_extensions;
    }
}
