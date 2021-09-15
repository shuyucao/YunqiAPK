using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Result 
{
    [Serializable]
    public class ShiYanCeShiNeRongDataResult
    {
        [SerializeField]
        public string summary;

        [SerializeField]
        public List<ShiYanCeShiNeRongDataNodeResult> node = new List<ShiYanCeShiNeRongDataNodeResult>();

        [SerializeField]
        public string update_time;

        [SerializeField]
        public int update_user;

        [SerializeField]
        public string create_time;

        [SerializeField]
        public string guide_type;

        [SerializeField]
        public string testRate;

        [SerializeField]
        public int create_user;

        [SerializeField]
        public string activity_set_name;

        [SerializeField]
        public int activity_total;

        [SerializeField]
        public string introduction;

        [SerializeField]
        public string objectId;
    }
}

