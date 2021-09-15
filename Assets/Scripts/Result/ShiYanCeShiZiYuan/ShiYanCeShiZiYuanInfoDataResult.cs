using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Result
{
    [Serializable]
    public class ShiYanCeShiZiYuanInfoDataResult
    {
        [SerializeField]
        public string id;

        [SerializeField]
        public string name;

        [SerializeField]
        public string type;

        [SerializeField]
        public string src;

        [SerializeField]
        public string thumbnail;

        [SerializeField]
        public string thumbnailOriginal;

        [SerializeField]
        public ShiYanCeShiZiYuanInfoDataQuanXianResult resource_operations = new ShiYanCeShiZiYuanInfoDataQuanXianResult();

        [SerializeField]
        public List<string> tags = new List<string>();

        [SerializeField]
        public ShiYanCeShiProcessDataResult test = new ShiYanCeShiProcessDataResult();
    }
}
