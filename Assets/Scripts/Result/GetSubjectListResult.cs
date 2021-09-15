using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


namespace Assets.Scripts.Result
{
    [Serializable]
    class GetSubjectListResult
    {
        [SerializeField]
        public string code;

        [SerializeField]
        public string msg;

        [SerializeField]
        public List<GetSubjectItemResult> data = new List<GetSubjectItemResult>();
    }
}
