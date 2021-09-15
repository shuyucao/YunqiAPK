using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Assets.Scripts.Windows;
using Assets.Scripts.Data;

public class PointerHoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //光标移入
    public void OnPointerEnter(PointerEventData eventData)
    {
        WindowManager.root.GetComponent<ShiYanXuanZeWindow>().GetJiaoCaiContentSmallObj.SetActive(false);
        WindowManager.root.GetComponent<ShiYanXuanZeWindow>().GetSmllTriggerImage.SetActive(false);
        WindowManager.root.GetComponent<ShiYanXuanZeWindow>().GetBigeTriggerImage.SetActive(true);
        WindowManager.root.GetComponent<ShiYanXuanZeWindow>().GetJiaoCaiContentBigObj.transform.localScale = Vector3.one;
        ShiYanXuanZeWindowData.Instance.TempObject = WindowManager.root.GetComponent<ShiYanXuanZeWindow>().GetJiaoCaiContentBigObj.transform.GetChild(0).gameObject;

    }

    //光标移出
    public void OnPointerExit(PointerEventData eventData)
    {
        WindowManager.root.GetComponent<ShiYanXuanZeWindow>().GetSmllTriggerImage.SetActive(true);
        WindowManager.root.GetComponent<ShiYanXuanZeWindow>().GetBigeTriggerImage.SetActive(false);
        WindowManager.root.GetComponent<ShiYanXuanZeWindow>().GetJiaoCaiContentSmallObj.SetActive(true);
        WindowManager.root.GetComponent<ShiYanXuanZeWindow>().GetJiaoCaiContentBigObj.transform.localScale = Vector3.zero;
        ShiYanXuanZeWindowData.Instance.TempObject = null;
    }
}
