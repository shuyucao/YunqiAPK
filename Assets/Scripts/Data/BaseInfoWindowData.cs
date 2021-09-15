using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data
{
    public class BaseInfoWindowData: Singleton<BaseInfoWindowData>
    {
        private string gradeName /*= "一年级"*/;
        public string GradeName
        {
            get
            {
                return gradeName;
            }
            set
            {
                gradeName = value;
            }
        }
        private string sectionName/* = "小学"*/;
        public string SectionName
        {
            get
            {
                return sectionName;
            }
            set
            {
                sectionName = value;
            }
        }

        public string FromWindow
        {
            get;set;
        }

        public string GradeID
        {
            get; set;
        }
        public string SectionID
        {
            get; set;
        }
    }
}
