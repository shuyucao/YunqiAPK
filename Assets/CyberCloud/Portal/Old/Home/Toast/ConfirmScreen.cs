using UnityEngine;
using System.Collections;

public class ConfirmScreen : MonoBehaviour {

    public UILabel mLabel;
    public GameObject mButton;

    void Start()
    {
        UIEventListener.Get(mButton).onClick = OnButtonClick;
    }
    public void Show(string value)
    {
        this.gameObject.SetActive(true);
        mLabel.text = value;
    }

    private void OnButtonClick(GameObject obj)
    {
        this.gameObject.SetActive(false);
    }
}
