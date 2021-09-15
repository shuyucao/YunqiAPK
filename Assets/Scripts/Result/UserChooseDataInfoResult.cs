using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Result 
{
    [Serializable]
    public class UserChooseDataInfoResult
    {
        [SerializeField]
        public int total_count;

        [SerializeField]
        public List<UserChooseItemResult> items = new List<UserChooseItemResult>();
    }
}

