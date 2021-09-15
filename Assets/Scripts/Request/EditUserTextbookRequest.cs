using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


namespace Assets.Scripts.Request
{
    [Serializable]
    public class EditUserTextbookRequest
    {
        [SerializeField]
        public MacInfoRequest macInfo;

        [SerializeField]
        public List<EditUserTextbookBodyRequest> body = new List<EditUserTextbookBodyRequest>();
    }
}
