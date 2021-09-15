using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Request
{
    [Serializable]
    public class ShiYanCeShiXiangQingMainRequest
    {
        [SerializeField]
        public string testId;

        [SerializeField]
        public MacInfoRequest macInfo = new MacInfoRequest();
    }
}
