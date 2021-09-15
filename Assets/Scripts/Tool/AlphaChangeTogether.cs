using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace Assets.Scripts.Tool
{
    class AlphaChangeTogether:MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
    {
        public List<Image> gameObjectImgs = new List<Image>();

        private void AlphaValueChange(float alphaValue)
        {
            for (int i = 0; i < gameObjectImgs.Count; i++)
            {
                gameObjectImgs[i].color = new Color(255, 255, 255, alphaValue);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            AlphaValueChange(0.7f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            AlphaValueChange(1f);
        }
    }
}
