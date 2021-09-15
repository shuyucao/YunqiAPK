using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Result 
{
    [Serializable]
    public class UserInfoDataResult
    {
        [SerializeField]
        public string imgUrl;

        [SerializeField]
        public int graduate_year;

        [SerializeField]
        public string section_name;

        [SerializeField]
        public string name;

        [SerializeField]
        public string section_tag;

        [SerializeField]
        public string grade_tag;

        [SerializeField]
        public string section;

        [SerializeField]
        public List<IdentitylistResult> identity_list = new List<IdentitylistResult>();

        [SerializeField]
        public string grade_name;
    }
}

