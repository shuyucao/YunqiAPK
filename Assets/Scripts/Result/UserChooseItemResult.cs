using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Result 
{
    [Serializable]
    public class UserChooseItemResult
    {
        [SerializeField]
        public string teaching_material_id;

        [SerializeField]
        public string teaching_material_name;

        [SerializeField]
        public string teaching_material_icon;

        [SerializeField]
        public string teaching_material_grade;

        [SerializeField]
        public string teaching_material_subject;

        [SerializeField]
        public string teaching_material_edition;

        [SerializeField]
        public string source;
    }
}

