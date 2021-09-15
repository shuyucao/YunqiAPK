using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Result 
{
    [Serializable]
    public class ShiYanCeShiAreaAndXueKeDataResult
    {
        [SerializeField]
        public Dictionary<string, string> tag_recommend_region = new Dictionary<string, string>();

        [SerializeField]
        public ShiYanCeShiSubjectAreaDataResult tag_subject = new ShiYanCeShiSubjectAreaDataResult();

        [SerializeField]
        public ShiYanCeShiSubjectAreaDataResult tag_region = new ShiYanCeShiSubjectAreaDataResult();
    }
}

