using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Result
{
    [Serializable]
    class PressItemResult
    {
        [SerializeField]
        public string name;

        [SerializeField]
        public string pressId;
    }
}
