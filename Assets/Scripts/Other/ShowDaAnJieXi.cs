using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShowDaAnJieXi : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log(1212);
        gameObject.transform.GetChild(2).gameObject.SetActive(true);
        float height = LayoutUtility.GetPreferredHeight(gameObject.transform.GetChild(2).transform.GetChild(0).GetComponent<RectTransform>());
        Debug.Log("height:" + height);
        gameObject.transform.GetChild(2).GetComponent<RectTransform>().sizeDelta = new Vector2(700,height+ 60);
        gameObject.transform.GetChild(3).gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.transform.GetChild(2).gameObject.SetActive(false);
        gameObject.transform.GetChild(3).gameObject.SetActive(false);
    }
}
