using System.Collections.Generic;
using UnityEngine;

public class ScrolleItemBase : MonoBase
{
    public UIScrollView scrolleView;
    public UIGrid grid;

    [HideInInspector]
    public List<GameObject> mItemList = new List<GameObject>();

    public int CurPage { get; set; }

    public PageData Data = new PageData();


    protected void InitBase()
    {
        Data.Width = scrolleView.panel.GetViewSize().x - 2 * scrolleView.panel.clipSoftness.x;
        Data.Height = scrolleView.panel.GetViewSize().y;
        CurPage = 1;
        //Debug.LogError("width:" + Data.Width + " height:" + Data.Height);
    }

    public virtual void CreateItemList() { }

    private int CurFocusIndex = 0;
    public void MoveNext(bool isnext)
    {
        int tem = isnext ? (CurFocusIndex + 1) : (CurFocusIndex - 1);
        if (tem < 0)
        {
            scrolleView.Scroll(1f);
        }
        else if (tem >= mItemList.Count)
        {
            scrolleView.Scroll(-1f);
        }
        else
        {
            UICenterOnChild uc = grid.GetComponent<UICenterOnChild>();
            if (uc != null)
            {
                uc.CenterOn(mItemList[tem].transform);
                CurFocusIndex = tem;
            }
            else
            {
                Debug.LogError("UICenterOnChild is null!!!");
            }
        }
    }
}

public class PageData
{
    public int NumPerPage { get; set; }

    public float Width { get; set; }

    public float Height { get; set; }

    public PageStyle Style { get; set; }

    public float radii = 20;
    public float Radii
    {
        get
        {
            return radii;
        }
        set
        {
            radii = value;
        }
    }

    public PageData() { }

    public PageData(float w, float h, int num, PageStyle sty)
    {
        Width = w;
        Height = h;
        Style = sty;
        NumPerPage = num;
    }
}