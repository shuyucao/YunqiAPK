using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Assets.Scripts.Data;

public class PointerSeletOverArea : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        ShiYanXuanZeWindowData.Instance.TempObject = gameObject.transform.parent.parent.gameObject;
        gameObject.transform.parent.GetChild(2).gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ShiYanXuanZeWindowData.Instance.TempObject = null;
        gameObject.transform.parent.GetChild(2).gameObject.SetActive(false);
    }
}
