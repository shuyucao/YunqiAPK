using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Request 
{
    [Serializable]
    public class UserChooseListRequest
    {
        [SerializeField]
        public MacInfoRequest macInfo;

        [SerializeField]
        public string subject;

        [SerializeField]
        public string edition;

        [SerializeField]
        public int page;

        [SerializeField]
        public int size;

    }
}

