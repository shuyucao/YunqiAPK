using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Result 
{
    [Serializable]
    public class ShiYanJiLuDataResult
    {

        [SerializeField]
        public List<ShiYanJiLuItemList> weekList = new List<ShiYanJiLuItemList>();

        [SerializeField]
        public int size;

        [SerializeField]
        public List<ShiYanJiLuItemList> todayList = new List<ShiYanJiLuItemList>();
       
        [SerializeField]
        public int page;

        [SerializeField]
        public List<ShiYanJiLuItemList> moreList = new List<ShiYanJiLuItemList>();
    }
}

