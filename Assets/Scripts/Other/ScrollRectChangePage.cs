using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Assets.Scripts.Data;

public class ScrollRectChangePage : MonoBehaviour,IDragHandler,IBeginDragHandler,IEndDragHandler
{
    private ScrollRect rect;
    // Start is called before the first frame update
    void Start()
    {
        rect = gameObject.transform.parent.GetComponent<ScrollRect>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        rect.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rect.OnDrag(eventData);
        ShiYanCeShiWindowData.Instance.ScrollRectPosValue = rect.verticalNormalizedPosition;
        ShiYanXuanZeWindowData.Instance.ScrollRectPosValue = rect.verticalNormalizedPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        rect.OnEndDrag(eventData);
    }

}
