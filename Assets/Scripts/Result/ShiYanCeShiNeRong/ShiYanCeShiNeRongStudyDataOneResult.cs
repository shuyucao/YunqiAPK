using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Result
{
    [Serializable]
    public class ShiYanCeShiNeRongStudyDataOneResult
    {
        [SerializeField]
        public float duration;

        [SerializeField]
        public float knowledge_ability;

        [SerializeField]
        public float master_level;

        [SerializeField]
        public int this_error_num;

        [SerializeField]
        public float last_master_level;

        [SerializeField]
        public int error_num;

        [SerializeField]
        public int last_error_num;

        [SerializeField]
        public float skill_ability;

        [SerializeField]
        public float generalize_ability;

        [SerializeField]
        public int change_type;

        [SerializeField]
        public float operation_ability;

        [SerializeField]
        public float diligent_ability;
    }
}
