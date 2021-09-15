using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Assets.Scripts.Request
{
    [Serializable]
    public class JiaoCaiRequest
    {
        [SerializeField]
        public string subjectTag;

        [SerializeField]
        public string gradeTag;

        [SerializeField]
        public string pressId;
    }
}
