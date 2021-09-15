using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Result
{
    [Serializable]
    public class ShiYanCeShiXiangQingDaTiDataInfoResult
    {
        [SerializeField]
        public float score;

        [SerializeField]
        public string step_number;

        [SerializeField]
        public string step_name;

        [SerializeField]
        public float raw_score;

        [SerializeField]
        public List<ShiYanCeshiXiangQingDaTiListDataInfoResult> list = new List<ShiYanCeshiXiangQingDaTiListDataInfoResult>();
    }
}
