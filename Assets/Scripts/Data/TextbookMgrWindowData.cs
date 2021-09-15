using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Result;
using Assets.Scripts.Request;
namespace Assets.Scripts.Data
{
    class TextbookMgrWindowData:Singleton<TextbookMgrWindowData>
    {

        public string FormWindow;
        public List<EditUserTextbookBodyRequest> userChooseItemResults = new List<EditUserTextbookBodyRequest>();
    }
}
