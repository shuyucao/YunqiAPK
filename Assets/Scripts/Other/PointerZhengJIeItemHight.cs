using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PointerZhengJIeItemHight : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.GetChild(1).gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (gameObject.GetComponent<Toggle>().isOn)
        {
            return;
        }
        else
        {
            gameObject.transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (gameObject.GetComponent<Toggle>().isOn)
        {
            return;
        }
        else
        {
            gameObject.transform.GetChild(1).gameObject.SetActive(false);
        }
    }

}
