using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Request 
{
    [Serializable]
    public class ShiYanZiYuanInfoRequest
    {
        [SerializeField]
        public string teachingMaterialId;

        [SerializeField]
        public int offset;

        [SerializeField]
        public int limit;

        [SerializeField]
        public string chapterPath;

        [SerializeField]
        public MacInfoRequest macInfo = new MacInfoRequest();
    }
}

