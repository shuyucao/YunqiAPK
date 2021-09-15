using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomePageSVItem : ScrolleItemBase, IMsgHandle
{
    public string ID { get; set; }
    private int mCurrentPage = 0;     // note:here a page is Constant.MaxLimitEachPag

    public void Init(string id)
    {
        base.InitBase();
        ID = id;
        Data.Style = PageStyle.PS_2R3C;
        Data.NumPerPage = 6;
        MsgManager.Instance.RegistMsg(MsgID.PhotoDataRefresh, this);
    }

    public void HandleMessage(MsgID id, Bundle bundle)
    {
        if (id == MsgID.PhotoDataRefresh)
        {
            if (bundle != null && bundle.Contains<string>("categoryID"))
            {
                string caid = bundle.GetValue<string>("categoryID");
                //Debug.LogError("PhotoDataRefresh:" + caid + "cruid:" + ID);
                if (caid.Equals(ID))
                {
                    CreateItemList();
                }
            }
        }
    }

    private bool islock = false;
    public override void CreateItemList()
    {
        if (islock)
        {
            return;
        }
        StartCoroutine(CreatePageItems());
    }

    public IEnumerator CreatePageItems()
    {
        MyTools.PrintDebugLogError("ucvr what this");
        islock = true;
        if (mCurrentPage != 0)
        {
            yield return new WaitForSeconds(1f);
        }
        for (int i = 0; i < Constant.PageNumCreatOneTime; i++)
        {
            PageItemBase item = PageManager.Instance.CreateOnePageItem(grid.transform, ID, Data);
            if (item != null)
            {
                grid.AddChild(item.transform);
                mItemList.Add(item.gameObject);
            }
            else
            {
                break;
            }
            mCurrentPage++;
            if (i % 2 == 0)
            {
                yield return new WaitForEndOfFrame();
            }
        }
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        MsgManager.Instance.SendMsg(MsgID.RefreshCtrBar, null);
        islock = false;
    }
}