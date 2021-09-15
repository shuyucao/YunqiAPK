using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Request
{
    [Serializable]
    public class ShiYanCeShiXiangQingMainOneRequest
    {

        [SerializeField]
        public string testId;

        [SerializeField]
        public string nodeId;

        [SerializeField]
        public MacInfoRequest macInfo;
    }
}
