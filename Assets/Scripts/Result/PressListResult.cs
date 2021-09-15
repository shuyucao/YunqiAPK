using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Result
{
    [Serializable]
    class PressListResult
    {
        [SerializeField]
        public string code;

        [SerializeField]
        public string msg;

        [SerializeField]
        public List<PressItemResult> data = new List<PressItemResult>();
    }
}
