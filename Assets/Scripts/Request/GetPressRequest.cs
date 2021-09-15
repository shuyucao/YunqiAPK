using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Request
{
    [Serializable]
    class GetPressRequest
    {
        [SerializeField]
        public string subjectTag;

        [SerializeField]
        public string gradeTag;
    }
}
