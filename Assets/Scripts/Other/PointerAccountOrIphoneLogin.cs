using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PointerAccountOrIphoneLogin : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (gameObject.GetComponent<Toggle>().isOn)
        {
            return;
        }
        gameObject.transform.GetChild(1).gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (gameObject.GetComponent<Toggle>().isOn)
        {
            return;
        }
        gameObject.transform.GetChild(1).gameObject.SetActive(false);
    }
}
