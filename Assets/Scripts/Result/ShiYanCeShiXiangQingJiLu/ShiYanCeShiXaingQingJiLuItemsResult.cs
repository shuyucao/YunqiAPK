using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Result
{
    [Serializable]
    public class ShiYanCeShiXaingQingJiLuItemsResult
    {
        [SerializeField]
        public int duration;

        [SerializeField]
        public float score;

        [SerializeField]
        public int experimentTime;

        [SerializeField]
        public int serverTime;

        [SerializeField]
        public float totalScore;

        [SerializeField]
        public int trainingMode;

        [SerializeField]
        public string entryId;

        [SerializeField]
        public string testTime;

        [SerializeField]
        public string testDate;
    }
}
