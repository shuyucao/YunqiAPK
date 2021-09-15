using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SetDropDownLayer : MonoBehaviour
{
    private void Start()
    {
        Canvas canvas = gameObject.GetComponent<Canvas>();
        canvas.sortingOrder = 29999;
    }
}
