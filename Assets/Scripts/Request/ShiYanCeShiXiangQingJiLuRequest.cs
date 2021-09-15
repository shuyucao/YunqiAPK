using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Request
{
    [Serializable]
    public class ShiYanCeShiXiangQingJiLuRequest
    {
        [SerializeField]
        public MacInfoRequest macInfo = new MacInfoRequest();

        [SerializeField]
        public string instanceId;

        [SerializeField]
        public int page;

        [SerializeField]
        public int size;

        [SerializeField]
        public int search_mode;
    }
}
