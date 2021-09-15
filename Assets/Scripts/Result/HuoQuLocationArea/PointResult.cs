using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Result
{
    [Serializable]
    public class PointResult
    {
        [SerializeField]
        public string x;

        [SerializeField]
        public string y;

        [SerializeField]
        public PointResult(string x, string y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
