using UnityEngine;
using System.Collections;

public class loadAnimation : MonoBehaviour
{
    public Transform loadingIcon;
    public UILabel label;
    void Start()
    {
        if (label) label.text = Localization.Get("Loading2");
    }
    void Update()
    {
        if (loadingIcon) loadingIcon.Rotate(260 * Vector3.back * Time.deltaTime, Space.Self);
    }
}