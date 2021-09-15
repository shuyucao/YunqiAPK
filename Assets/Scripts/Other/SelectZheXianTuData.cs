using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Assets.Scripts.Data;
using Assets.Scripts.Windows;

public class SelectZheXianTuData : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler,IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        ShiYanCeShiXiangQingJiLuWindowData.Instance.TempSelectNodeObj = gameObject.transform.GetChild(0).transform.GetChild(0).gameObject;
        ShiYanCeShiXiangQingJiLuWindowData.Instance.SelectLien = gameObject.transform.GetChild(2).gameObject;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        foreach (var item in ShiYanCeShiXiangQingJiLuWindowData.Instance.ZheXianItemsDic.Keys)
        {
            item.transform.GetChild(2).gameObject.SetActive(false);
            item.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
        }
        gameObject.transform.GetChild(2).gameObject.SetActive(true);
        gameObject.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
        if (ShiYanCeShiXiangQingJiLuWindowData.Instance.IsClickCount)
        {
            if (ShiYanCeShiXiangQingJiLuWindowData.Instance.GetShiYanCeShiXiangQingJiLuWindow == null)
            {
                return;
            }
            if (ShiYanCeShiXiangQingJiLuWindowData.Instance.GetShiYanCeShiXaingQingJiLuResult() == null)
            {
                return;
            }
            ShiYanCeShiXiangQingJiLuWindowData.Instance.GetShiYanCeShiXiangQingJiLuWindow.ShowSerisData(ShiYanCeShiXiangQingJiLuWindowData.Instance.GetShiYanCeShiXaingQingJiLuResult(), ShiYanCeShiXiangQingJiLuWindowData.Instance.ZheXianItemsDic[gameObject]);
        }
        else
        {
            if (ShiYanCeShiXiangQingJiLuWindowData.Instance.GetShiYanCeShiXiangQingJiLuWindow == null)
            {
                return;
            }
            if (ShiYanCeShiXiangQingJiLuWindowData.Instance.GetShiYanCeShiXiangQingJiLuYearResult() == null)
            {
                return;
            }
            ShiYanCeShiXiangQingJiLuWindowData.Instance.GetShiYanCeShiXiangQingJiLuWindow.ShowSerisDateDataInfo(ShiYanCeShiXiangQingJiLuWindowData.Instance.GetShiYanCeShiXiangQingJiLuYearResult(), ShiYanCeShiXiangQingJiLuWindowData.Instance.ZheXianItemsDic[gameObject]);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.transform.GetChild(2).gameObject.SetActive(false);
        gameObject.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
        if (ShiYanCeShiXiangQingJiLuWindowData.Instance.IsClickCount)
        {
            if (ShiYanCeShiXiangQingJiLuWindowData.Instance.GetShiYanCeShiXiangQingJiLuWindow == null)
            {
                return;
            }
            ShiYanCeShiXiangQingJiLuWindowData.Instance.TempSelectNodeObj.SetActive(true);
            ShiYanCeShiXiangQingJiLuWindowData.Instance.SelectLien.SetActive(true);
            ShiYanCeShiXiangQingJiLuWindowData.Instance.GetShiYanCeShiXiangQingJiLuWindow.GetSelectNodeData();
        }
    }
}
