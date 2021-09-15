using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Result
{
    [Serializable]
    public class ShiYanCeShiYiCuoXiangDateResult
    {
        [SerializeField]
        public string experiment_id;

        [SerializeField]
        public string error_code;

        [SerializeField]
        public int error_type;

        [SerializeField]
        public string error_name;

        [SerializeField]
        public string analysis;

        [SerializeField]
        public int error_count;

        [SerializeField]
        public float error_rate;

        [SerializeField]
        public int is_new;
    }
}
