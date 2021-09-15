using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Result
{
    [Serializable]
    public class ToKenSwapResult
    {
        [SerializeField]
        public string code;

        [SerializeField]
        public string msg;

        [SerializeField]
        public TokenSwapInfoResult data = new TokenSwapInfoResult();
    }
}
