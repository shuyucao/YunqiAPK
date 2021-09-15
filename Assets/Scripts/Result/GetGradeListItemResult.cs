using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Assets.Scripts.Result
{
    [Serializable]
    class GetGradeListItemResult
    {
        [SerializeField]
        public string key;

        [SerializeField]
        public string value;

        [SerializeField]
        public List<GradeVosResult> grade_vos = new List<GradeVosResult>();
    }
}
