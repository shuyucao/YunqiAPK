using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace Assets.Scripts.Result
{
    [Serializable]
    public class ResponseBodyResult
    {
        [SerializeField]
        public string address;
        [SerializeField]
        public ContentResult content;
        [SerializeField]
        public int status;
    }
}
