using Assets.Scripts.Windows;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PointerJiaoCaiItem : MonoBehaviour
{

    private void OnMouseEnter()
    {
        Debug.Log(132132);
        gameObject.transform.GetChild(2).gameObject.SetActive(true);
    }

    private void OnMouseExit()
    {
        Debug.Log(12);
        gameObject.transform.GetChild(2).gameObject.SetActive(false);
    }
    private void Start()
    {
        gameObject.transform.GetChild(2).gameObject.SetActive(false);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        gameObject.transform.GetChild(2).gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.transform.GetChild(2).gameObject.SetActive(false);
    }
}
