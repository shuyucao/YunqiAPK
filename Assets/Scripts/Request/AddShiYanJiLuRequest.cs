using Assets.Scripts.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Request
{
    [Serializable]
    public class AddShiYanJiLuRequest
    {
        [SerializeField]
        public MacInfoRequest macInfo = new MacInfoRequest();

        [SerializeField]
        public string objectId;

        [SerializeField]
        public string objectName;

        [SerializeField]
        public string type;
        
        [SerializeField]
        public string timestampStart;

        [SerializeField]
        public string timestampEnd;

        [SerializeField]
        public ShiYanZiYuanItemResult objectExtensions = new ShiYanZiYuanItemResult();
    }
}
