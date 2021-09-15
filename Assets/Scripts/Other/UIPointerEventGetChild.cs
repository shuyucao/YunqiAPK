using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Tool 
{
    public class UIPointerEventGetChild : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            gameObject.transform.GetChild(2).gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            gameObject.transform.GetChild(2).gameObject.SetActive(false);
        }
    }
}

