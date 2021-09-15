using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Result
{
    [Serializable]
    public class ShiYanCeShiXiangQingJiLuYearItemResult
    {
        [SerializeField]
        public int times;

        [SerializeField]
        public string recordTime;

        [SerializeField]
        public float avgScore;

        [SerializeField]
        public float avgDuration;
    }
}
