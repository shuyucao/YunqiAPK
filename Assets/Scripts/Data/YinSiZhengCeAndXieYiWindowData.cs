using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Data 
{
    public class YinSiZhengCeAndXieYiWindowData : Singleton<YinSiZhengCeAndXieYiWindowData>
    {
        //判断是用户协议还是隐私政策
        private string IsYinSiOrXieYi = "";

        public string GetYinSiOrXieYi
        {
            get { return IsYinSiOrXieYi; }
            set { IsYinSiOrXieYi = value; }
        }
    }
}

