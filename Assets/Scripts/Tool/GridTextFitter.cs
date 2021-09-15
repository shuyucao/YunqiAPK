using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
namespace Assets.Scripts.Tool
{
    class GridTextFitter:MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
    {
        public Text _Showtext;
        public RectTransform TextRT;
        public RectTransform BgImageRT;
        bool isHide = false;
        string realStr;
        string cutStr;
        public string ShowText
        {
            get
            {
                return _Showtext.text;
            }
            set
            {
                _Showtext.text = value;
                OnValueChange(value);
            }
        }
        private void OnValueChange(string inputValue)
        {
            realStr = inputValue;
            if (inputValue.Length>10)
            {
                isHide = true;
                cutStr = inputValue.Substring(0, 9) + "...";
                _Showtext.text = cutStr;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isHide)
            {
                _Showtext.text = realStr;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isHide)
            {
                _Showtext.text = cutStr;
            }
        }
        void Update()
        {
            BgImageRT.sizeDelta = new Vector2(BgImageRT.sizeDelta.x, TextRT.sizeDelta.y + 20);
        }
    }
}
