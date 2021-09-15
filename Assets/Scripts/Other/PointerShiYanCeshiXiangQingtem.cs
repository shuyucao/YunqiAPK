using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Assets.Scripts.Data;

public class PointerShiYanCeshiXiangQingtem : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (gameObject.transform.GetChild(2).GetChild(0).gameObject.activeSelf)
        {
            return;
        }
        else
        {
            gameObject.transform.GetChild(2).gameObject.SetActive(true);
            gameObject.transform.GetChild(2).GetChild(0).gameObject.SetActive(false);
        }
        ShiYanXuanZeWindowData.Instance.TempObject = gameObject.transform.parent.gameObject;
    }

    public void OnPointerExit(PointerEventData eventData)
    {

        if (gameObject.transform.GetChild(2).GetChild(0).gameObject.activeSelf)
        {
            return;
        }
        else
        {
            gameObject.transform.GetChild(2).gameObject.SetActive(false);
        }
        ShiYanXuanZeWindowData.Instance.TempObject = null;
    }
}
