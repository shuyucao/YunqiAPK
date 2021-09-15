using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

public class TextScrollView : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    private  float width = 0f;
    private RectTransform rectTransform;
    private void Start()
    {
        gameObject.transform.parent.GetChild(0).gameObject.SetActive(false);
        width = GetComponent<RectTransform>().rect.width;
        rectTransform = transform.GetChild(0).GetComponent<RectTransform>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        gameObject.transform.parent.GetChild(0).gameObject.SetActive(true);
        float rectWidth = LayoutUtility.GetPreferredWidth(rectTransform);
        if (rectWidth <= width)
        {
            return;
        }
        else
        {
            float offset = rectWidth - width;
            rectTransform.DOAnchorPos3D(new Vector3(-offset, 0, 0), 2f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.transform.parent.GetChild(0).gameObject.SetActive(false);
        rectTransform.DOAnchorPos3D(Vector3.zero, 0.5f);
    }
}
