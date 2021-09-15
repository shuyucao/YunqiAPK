using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Result
{
    [Serializable]
    public class ShiYanCeShiXaingQingJiLuParamResult
    {
        [SerializeField]
        public int page;

        [SerializeField]
        public int size;

        [SerializeField]
        public int search_mode;
    }
}
