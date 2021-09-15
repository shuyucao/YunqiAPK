using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Result
{
    [Serializable]
    public class ShiYanCeShiSubjectAreaDataResult
    {
        [SerializeField]
        public string tag_id;

        [SerializeField]
        public string tag_name;

        [SerializeField]
        public string tag_first_letter;

        [SerializeField]
        public int sort;

        [SerializeField]
        public string parent_id;

        [SerializeField]
        public bool has_disable;

        [SerializeField]
        public bool has_default;

        [SerializeField]
        public List<ShiYanCeShiSubjectAndRegionChildDataResult> child = new List<ShiYanCeShiSubjectAndRegionChildDataResult>();
    } 
}
