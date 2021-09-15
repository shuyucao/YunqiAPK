using Assets.Scripts.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PiointerHiight : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
{
    private void Start()
    {
        if (GetComponent<Toggle>().isOn) 
        {
            gameObject.transform.GetChild(2).gameObject.SetActive(true);
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GetComponent<Toggle>().isOn)
        {
            return;
        }
        else
        {
            gameObject.transform.GetChild(2).gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (GetComponent<Toggle>().isOn)
        {
            return;
        }
        else
        {
            gameObject.transform.GetChild(2).gameObject.SetActive(false);
        }
    }
}
