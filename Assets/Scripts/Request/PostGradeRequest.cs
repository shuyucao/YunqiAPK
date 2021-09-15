using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
namespace Assets.Scripts.Request
{
    [Serializable]
    public class PostGradeRequest
    {
        [SerializeField]
        public string section;

        [SerializeField]
        public string graduateYear;

        [SerializeField]
        public MacInfoRequest macInfo = new MacInfoRequest();

        [SerializeField]
        public string userId;

    }
}
