using UnityEngine;
using System.Collections;

public class BtnEffect : MonoBehaviour {

    public delegate void OnClickNotify();
    public OnClickNotify onClickUpNotify;
    public OnClickNotify onClickDownNotify;
    public OnClickNotify onClickTopNotify;
    [SerializeField]
    private btnType bt;
	// Use this for initialization
	void Start () {
        UIEventListener.Get(this.gameObject).onClick += OnClick;
        UIEventListener.Get(this.gameObject).onHover += OnHover;
	}
	
    private void OnClick(GameObject _obj)
    {
        Debug.Log("onclick btnEffect");
        switch(bt)
        {
            case btnType.BT_BTNUP:
                onClickUpNotify();
                break;
            case btnType.BT_BTNDOWN:
                onClickDownNotify();
                break;
            case btnType.BT_TOP:
                onClickTopNotify();
                break;
            default:
                Debug.LogError("btn is ivalid");
                break;
        }
    }
    private void OnHover(GameObject _obj, bool _isHover)
    {
        Debug.Log("OnHover btnEffect");
    }

    void OnDestory()
    {
        UIEventListener.Get(this.gameObject).onClick -= OnClick;
        UIEventListener.Get(this.gameObject).onHover -= OnHover;
    }

    enum btnType
    {
        BT_BTNUP,
        BT_BTNDOWN,
        BT_TOP
    }
}
