using Assets.Scripts.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PiointerXueKeHiight : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GetComponent<Toggle>().isOn)
        {
            return;
        }
        else
        {
            gameObject.transform.GetChild(1).gameObject.SetActive(true);
        }
        ShiYanXuanZeWindowData.Instance.TempObject = gameObject.transform.parent.gameObject;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (GetComponent<Toggle>().isOn)
        {
            return;
        }
        else
        {
            gameObject.transform.GetChild(1).gameObject.SetActive(false);
        }
        ShiYanXuanZeWindowData.Instance.TempObject = null;
    }
}
