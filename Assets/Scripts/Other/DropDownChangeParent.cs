using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Assets.Scripts.Other
{
    class DropDownChangeParent:MonoBehaviour
    {
        public Transform parent;
        private void Start()
        {
            transform.SetParent(parent);
        }
    }
}
