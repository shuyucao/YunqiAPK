using System.Collections.Generic;
using UnityEngine;

public class ScvControler : MonoBehaviour, IMsgHandle
{
    public delegate void ControllerCallBack();
    public ControllerCallBack PageReacheEnd = null;
    [SerializeField]
    UIScrollBar mMainbar;    //主进度条
    [SerializeField]
    UIScrollBar mCorbar;    //副进度条
    [SerializeField]
    UIScrollView scrollView;

    private ScrolleItemBase mScrItem;
    //private float delta_android = 0.15f;
    //private float delta_pc = 0.08f;
    private bool canLoadNextPage = false;
    private string Key = "Horizontal";

    private static ScvControler _instance = null;
    public static ScvControler instance
    {
        get { return _instance; }
    }

    public enum ControleType
    {
        Horizontal,
        Vertical,
    }
    public ControleType CType = ControleType.Horizontal;

    public static ScvControler RegistController(GameObject go, ScrolleItemBase sv, UIScrollBar main_bar, UIScrollBar cor_bar = null, ControleType type = ControleType.Vertical)
    {
        if (_instance != null)
        {
            DestroyImmediate(_instance);
        }
        ScvControler controler = go.GetComponent<ScvControler>();
        if (controler == null)
        {
            controler = go.AddComponent<ScvControler>();
            controler.SetScrollBar(sv.scrolleView, main_bar, cor_bar, type);
            controler.SetInputControle();
            controler.mScrItem = sv;
            _instance = controler;
        }
        MsgManager.Instance.RegistMsg(MsgID.RefreshCtrBar, controler);
        return controler;
    }

    public static void RemoveController(GameObject go)
    {
        ScvControler con = go.GetComponent<ScvControler>();
        if (con != null)
        {
            Destroy(con);
        }
    }

    public void SetScrollBar(UIScrollView sv, UIScrollBar main_bar, UIScrollBar cor_bar, ControleType type)
    {
        SetScrollView(sv);
        mMainbar = main_bar;
        mCorbar = cor_bar;
        CType = type;
        if (CType == ControleType.Horizontal)
        {
            Key = "Horizontal";
        }
        else if (CType == ControleType.Vertical)
        {
            Key = "Vertical";
        }
    }

    private void SetInputControle()
    {
        if (CType == ControleType.Vertical)
        {
            PicoInputManager.OnDown += MoveNext;
            PicoInputManager.OnUp += MoveLast;
        }
        else if (CType == ControleType.Horizontal)
        {
            PicoInputManager.OnLeft += MoveNext;
            PicoInputManager.OnRight += MoveLast;
        }
    }

    private void RemoveInputControle()
    {
        if (CType == ControleType.Vertical)
        {
            PicoInputManager.OnDown -= MoveNext;
            PicoInputManager.OnUp -= MoveLast;
        }
        else if (CType == ControleType.Horizontal)
        {
            PicoInputManager.OnDown -= MoveNext;
            PicoInputManager.OnUp -= MoveLast;
        }
    }

    void Update()
    {
        SynScrollebar();
    }

    public void HandleMessage(MsgID id, Bundle bundle)
    {
        if (id == MsgID.RefreshCtrBar)
        {
            RefreshBarOriginSize();
        }
    }

    private float originSize = 0;
    private void SynScrollebar()
    {
        if (mMainbar != null)
        {
            if (mCorbar != null)
            {
                mCorbar.value = mMainbar.value;
                mCorbar.barSize = mMainbar.barSize;
            }
            if (mMainbar.barSize <= originSize / 2 && canMove)
            {
                canMove = false;
                Ftimer.AddEvent("stopmove", 1f, () =>
                {
                    canMove = true;
                    //RefreshBarOriginSize();
                });
                if (canLoadNextPage)
                {
                    DoPageReacheEnd();
                }
            }
        }
    }

    private float timeLast = 0;
    private void DoPageReacheEnd()
    {
        if ((Time.time - timeLast) > 3f)
        {
            //Debug.LogError("DoPageReacheEnd");
            if (PageReacheEnd != null) PageReacheEnd();
            //DataLoader.Instance.RequestNextPage();
            timeLast = Time.time;
        }
    }

    private bool canMove = true;   // 是否允许拖拽
    private void DragScrollView(float delta)
    {
        if (scrollView != null && canMove)
        {
            scrollView.Scroll(delta);
        }
    }

    public void SetScrollView(UIScrollView view)
    {
        scrollView = view;
        Ftimer.AddEvent("refreshbarsize", 0.3f, () =>
        {
            RefreshBarOriginSize();
        });
    }

    private void RefreshBarOriginSize()
    {
        originSize = mMainbar == null ? 0 : mMainbar.barSize;
        //Debug.LogError("size changed!" + originSize);
    }


#if UNITY_EDITOR
    private float delta_step = 1f;
#elif UNITY_ANDROID
    private float delta_step = 2f;
#endif

    private void MoveNext()
    {
        canLoadNextPage = true;
        DragScrollView(-delta_step);
        //Debug.LogError("move next");
        //mScrItem.MoveNext(true);
    }

    private void MoveLast()
    {
        canLoadNextPage = false;
        DragScrollView(delta_step);
        //Debug.LogError("move last");
        //mScrItem.MoveNext(false);
    }

    void OnDestroy()
    {
        RemoveInputControle();
        MsgManager.Instance.RemoveMsg(MsgID.RefreshCtrBar, this);
    }
}