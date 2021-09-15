using Assets.Scripts.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class PointerCeShiJiLuItemHlight : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        ShiYanXuanZeWindowData.Instance.TempObject = gameObject.transform.parent.gameObject;
        gameObject.transform.GetChild(1).gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.transform.GetChild(1).gameObject.SetActive(false);
        ShiYanXuanZeWindowData.Instance.TempObject = null;
    }
}
