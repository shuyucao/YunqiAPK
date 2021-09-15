using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Result
{
    [Serializable]
    public class ContentResult
    {
        [SerializeField]
        public string address;

        [SerializeField]
        public Address_DetailResult address_detail;

        [SerializeField]
        public PointResult point;
    }
}
