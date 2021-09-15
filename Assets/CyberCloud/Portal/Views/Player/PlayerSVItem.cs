using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSVItem : ScrolleItemBase
{
    private string ID = string.Empty;
    List<PageItemBase> mPageItemList = new List<PageItemBase>();

    private bool islock = false;
    private int CachePage = 0;          //缓冲页数

    public void Init(string id)
    {
        //Debug.LogError("PlayerSVItem init:" + id);
        base.InitBase();
        Data.Style = PageStyle.PS_1R5C;
        Data.NumPerPage = 5;
        if (!ID.Equals(id))
        {
            ID = id;
            Clear();
            CreateItemList();
        }
        //MsgManager.Instance.RegistMsg(MsgID.CategoryPhotoDataReady, this);
    }

    //public void HandleMessage(MsgID id, Bundle bundle)
    //{
    //    if (id == MsgID.CategoryPhotoDataReady)
    //    {
    //        CreateItemList();
    //    }
    //}

    public override void CreateItemList()
    {
        StartCoroutine(CreateNextPageItems());
    }

    private void Clear()
    {
        StartCoroutine(ClearPageItems());
        //MsgManager.Instance.RemoveMsg(MsgID.CategoryPhotoDataReady, this);
    }

    private IEnumerator ClearPageItems()
    {
        //Debug.LogError("clear list");
        islock = true;
        for (int i = 0; i < mPageItemList.Count; i++)
        {
            Destroy(mPageItemList[i].gameObject);
            yield return new WaitForEndOfFrame();
        }
        mPageItemList.Clear();
        CachePage = 0;
        islock = false;
    }

    public IEnumerator CreateNextPageItems()
    {
        while (islock)
        {
            //Debug.LogError("wait!");
            yield return 0;
        }
        //Debug.LogError("start to creat page items");
        for (int i = 0; i < Constant.PageNumCreatOneTime_ForPlayerScreen; i++)
        {
            PageItemBase item = PageManager.Instance.CreateOnePageItem(grid.transform, ID, ++CachePage, Data);
            if (item != null)
            {
                grid.AddChild(item.transform);
                mPageItemList.Add(item);
                yield return new WaitForEndOfFrame();
            }
            else
            {
                DataLoader.Instance.RequestNextPage();
                break;
            }
        }
        yield return null;
    }

    public void MoveNextPage()
    {
        MoveScrolleView(true);
    }

    public void MoveLastPage()
    {
        MoveScrolleView(false);
    }

    private void MoveScrolleView(bool forward)
    {
        scrolleView.Scroll(forward ? -5f : 5f);
    }
}