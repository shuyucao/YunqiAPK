using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Result 
{
    [Serializable]
    public class ChildrenNodeResult
    {
        [SerializeField]
        public string id;

        [SerializeField]
        public string name;

        [SerializeField]
        public string intro;

        [SerializeField]
        public string node_path;

        [SerializeField]
        public List<ChildrenNodeChidResult> child_nodes = new List<ChildrenNodeChidResult>();
    }
}

