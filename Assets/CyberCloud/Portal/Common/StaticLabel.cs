using UnityEngine;
using System.Collections;

/*
 * bind a key from Localization
 */
[RequireComponent(typeof(UILabel))]
public class StaticLabel : MonoBehaviour
{
    [SerializeField]
    string mKey;
    void Start()
    {
        UILabel label = gameObject.GetComponent<UILabel>();
        label.text = Localization.Get(mKey);
    }
}
