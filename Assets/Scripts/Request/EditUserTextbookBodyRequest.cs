using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Assets.Scripts.Request
{
    [Serializable]
    public class EditUserTextbookBodyRequest
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
