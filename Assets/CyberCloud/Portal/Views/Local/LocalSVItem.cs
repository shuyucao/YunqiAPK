using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalSVItem : ScrolleItemBase
{
    private List<PageItemBase> mPageList = new List<PageItemBase>();
    public void Init(List<LocalPhotoModel> localpmlist, bool needclear = true)
    {
        base.InitBase();
        Data.Style = PageStyle.PS_2R4C;
        Data.NumPerPage = 8;

        StopCoroutine(ClearData());
        gameObject.SetActive(true);
        if (needclear) StartCoroutine(Clear());
        StartCoroutine(CreatePageItems(localpmlist));
    }

    public void CleanOut()
    {
        StartCoroutine(ClearData());
    }

    private IEnumerator ClearData()
    {
        yield return StartCoroutine(Clear());
        if (gameObject != null)
        {
            gameObject.SetActive(false);
        }
    }

    public IEnumerator CreatePageItems(List<LocalPhotoModel> localpmlist)
    {
        while (islock)
        {
            yield return 0;
        }
        if (localpmlist == null)
        {
            yield return null;
        }
        int page_num = localpmlist.Count / Data.NumPerPage;
        page_num = (localpmlist.Count % Data.NumPerPage == 0) ? page_num : (page_num + 1);
        for (int i = 0; i < page_num; i++)
        {
            List<LocalPhotoModel> one_page_list = new List<LocalPhotoModel>();
            for (int j = i * Data.NumPerPage; j < (i + 1) * Data.NumPerPage && j < localpmlist.Count; j++)
            {
                one_page_list.Add(localpmlist[j]);
            }
            PageItemBase item = PageManager.Instance.CreateOneLocalPageItem(grid.transform, Data, one_page_list);
            if (item != null)
            {
                grid.AddChild(item.transform);
                mPageList.Add(item);
                yield return new WaitForEndOfFrame();
            }
            else
            {
                Debug.LogError("CreateOneLocalPageItem fail");
            }
        }
        Ftimer.AddEvent("PageCreateOver", 0.6f, () =>
        {
            MsgManager.Instance.SendMsg(MsgID.RefreshCtrBar, null);
            MsgManager.Instance.SendMsg(MsgID.FolderPageCreated, null);
        });
        yield return null;
    }

    private bool islock = false;
    private IEnumerator Clear()
    {
        islock = true;
        for (int i = 0; i < mPageList.Count; i++)
        {
            Destroy(mPageList[i].gameObject);
        }
        mPageList.Clear();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        islock = false;
        yield return null;
    }
}