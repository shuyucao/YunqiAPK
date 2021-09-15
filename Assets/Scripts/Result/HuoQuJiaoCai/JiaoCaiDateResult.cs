using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Result 
{
    [Serializable]
    public class JiaoCaiDateResult
    {
        [SerializeField]
        public string id;

        [SerializeField]
        public string name;

        [SerializeField]
        public List<JiaoCaiDataInfoResult> teaching_material_vo_list = new List<JiaoCaiDataInfoResult>();
    }
}

