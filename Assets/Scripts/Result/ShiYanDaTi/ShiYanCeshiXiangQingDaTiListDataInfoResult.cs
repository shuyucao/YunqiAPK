using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Result
{
    [Serializable]
    public class ShiYanCeshiXiangQingDaTiListDataInfoResult
    {
        [SerializeField]
        public string action_name;

        [SerializeField]
        public int conclusion;

        [SerializeField]
        public string reason;

        [SerializeField]
        public string analysis;

        [SerializeField]
        public float score;

        [SerializeField]
        public float rowScore;

        [SerializeField]
        public float actionScore;
    }
}
