using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Result
{
    [Serializable]
    public class ShiYanCeShiNeRongDataOneResult
    {
        [SerializeField]
        public string summary;

        [SerializeField]
        public string create_time;

        [SerializeField]
        public string last_study_activity;

        [SerializeField]
        public int activity_total;

        [SerializeField]
        public List<ShiYanCeShiNeRongDataNodeOneResult> node = new List<ShiYanCeShiNeRongDataNodeOneResult>();

        [SerializeField]
        public string update_time;

        [SerializeField]
        public int update_user;

        [SerializeField]
        public string guide_type;

        [SerializeField]
        public string testRate;

        [SerializeField]
        public int create_user;

        [SerializeField]
        public string activity_set_name;

        [SerializeField]
        public string introduction;

        [SerializeField]
        public string objectId;
    }
}
