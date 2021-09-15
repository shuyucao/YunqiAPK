using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Assets.Scripts.Result
{
    [Serializable]
    class GetGradeListResult
    {
        [SerializeField]
        public string code;

        [SerializeField]
        public string msg;

        [SerializeField]
        public List<GetGradeListItemResult> data = new List<GetGradeListItemResult>();
    }
}
