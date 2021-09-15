using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Result
{
    [Serializable]
    public class Address_DetailResult
    {
        [SerializeField]
        public string city;

        [SerializeField]
        public int city_code;

        [SerializeField]
        public string district;

        [SerializeField]
        public string province;

        [SerializeField]
        public string street;

        [SerializeField]
        public string street_number;

        [SerializeField]
        public Address_DetailResult(string city, int city_code, string district, string province, string street, string street_number)
        {
            this.city = city;
            this.city_code = city_code;
            this.district = district;
            this.province = province;
            this.street = street;
            this.street_number = street_number;
        }
    }
}
