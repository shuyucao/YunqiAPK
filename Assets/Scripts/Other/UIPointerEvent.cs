using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Tool 
{
    public class UIPointerEvent : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            gameObject.transform.GetChild(1).gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            gameObject.transform.GetChild(1).gameObject.SetActive(false);
        }
    }
}

