using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Assets.Scripts.Data;

public class SelectUIPointerObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //射线移入
    public void OnPointerEnter(PointerEventData eventData)
    {
        ShiYanXuanZeWindowData.Instance.TempObject = gameObject.transform.GetChild(0).gameObject;
    }

    //射线移出
    public void OnPointerExit(PointerEventData eventData)
    {
        ShiYanXuanZeWindowData.Instance.TempObject = null;
    }
}
