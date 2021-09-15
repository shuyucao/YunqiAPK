using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class YearMonthDatePointer : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    //没选中字体的颜色
    private Color DeColor = new Color32(49, 255, 255, 255);
    //选中字体的颜色
    private Color SeColor = Color.white;
  
    public void OnPointerEnter(PointerEventData eventData)
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        gameObject.transform.GetChild(1).GetComponent<Text>().color = SeColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        gameObject.transform.GetChild(1).GetComponent<Text>().color = DeColor;
    }
}
