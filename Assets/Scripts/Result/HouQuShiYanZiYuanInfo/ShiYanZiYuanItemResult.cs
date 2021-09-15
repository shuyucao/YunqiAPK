using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Result 
{
    [Serializable]
    public class ShiYanZiYuanItemResult
    {
        [SerializeField]
        public string duration;

        [SerializeField]
        public string teaching_material_id;

        [SerializeField]
        public string thumbnail;

        [SerializeField]
        public ResourceOperationResult resource_operation;

        [SerializeField]
        public string thumbnail_original;

        [SerializeField]
        public string src;

        [SerializeField]
        public string name;

        [SerializeField]
        public string description;

        [SerializeField]
        public string type;

        [SerializeField]
        public string objectId;

        [SerializeField]
        public List<string> tags = new List<string>();
    }
}

