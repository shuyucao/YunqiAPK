using UnityEngine;
using System.Collections;

public class BtnGroup : MonoBehaviour {

    [SerializeField]
    private UISprite btnUp;
    [SerializeField]
    private UISprite btnDown;
    [SerializeField]
    private UISprite btnTop;

    public bool isTopMove = false;
    public delegate void OnClickNotify();
    public OnClickNotify onClickUpNotify;
    public OnClickNotify onClickDownNotify;
    public OnClickNotify onClickTopNotify;

    public delegate void NotifyHide();
    public NotifyHide notifyHide;
    [SerializeField]
    private GroupType gt;
    // Use this for initialization
    void Start()
    {
        UIEventListener.Get(btnUp.gameObject).onClick += OnClickUp;
        UIEventListener.Get(btnUp.gameObject).onHover += OnHoverUp;
        UIEventListener.Get(btnDown.gameObject).onClick += OnClickDown;
        UIEventListener.Get(btnDown.gameObject).onHover += OnHoverDown;
        UIEventListener.Get(btnTop.gameObject).onClick += OnClickTop;
        UIEventListener.Get(btnTop.gameObject).onHover += OnHoverTop;
    }

    private void OnClickUp(GameObject _obj)
    {
        onClickUpNotify();
    }
    private void OnHoverUp(GameObject _obj, bool _isHover)
    {
    }
    private void OnClickDown(GameObject _obj)
    {
        onClickDownNotify();
    }
    private void OnHoverDown(GameObject _obj, bool _isHover)
    {
        if (notifyHide != null)
        {
            if (gt == GroupType.GT_GAMELIST)
            {
                notifyHide();
            }
        }
    }
    private void OnClickTop(GameObject _obj)
    {
        isTopMove = true;
        onClickTopNotify();
    }
    private void OnHoverTop(GameObject _obj, bool _isHover)
    {
        if (notifyHide != null)
        {
            if (gt == GroupType.GT_GAMELIST)
            {
                notifyHide();
            }
        }
    }
    void OnDestory()
    {
        UIEventListener.Get(btnUp.gameObject).onClick -= OnClickUp;
        UIEventListener.Get(btnUp.gameObject).onHover -= OnHoverUp;
        UIEventListener.Get(btnDown.gameObject).onClick -= OnClickDown;
        UIEventListener.Get(btnDown.gameObject).onHover -= OnHoverDown;
        UIEventListener.Get(btnTop.gameObject).onClick -= OnClickTop;
        UIEventListener.Get(btnTop.gameObject).onHover -= OnHoverTop;
    }

    public void Init(bool _isOne, GroupType _gt)
    {
        gt = _gt;
        btnUp.gameObject.SetActive(false);
        btnTop.gameObject.SetActive(false);
        if(_isOne)
        {
            btnDown.gameObject.SetActive(false);
        }
        else
        {
            btnDown.gameObject.SetActive(true);
        }
    }
    
    public void RefreshActive(bool _isFirst, bool _isLast)
    {
        btnTop.gameObject.SetActive(false);
        if(!_isFirst && !_isLast)
        {
            btnUp.gameObject.SetActive(true);
            btnDown.gameObject.SetActive(true);
        }
        else if(_isFirst)
        {
            btnUp.gameObject.SetActive(false);
            btnDown.gameObject.SetActive(true);
        }
        else if(_isLast)
        {
            btnUp.gameObject.SetActive(true);
            btnDown.gameObject.SetActive(false);
            btnTop.gameObject.SetActive(true);
        }
    }

    public enum GroupType
    {
        GT_MAINBUTTON,
        GT_SECCATEGOREY,
        GT_GAMELIST,
        GT_MINELIST
    }
}
