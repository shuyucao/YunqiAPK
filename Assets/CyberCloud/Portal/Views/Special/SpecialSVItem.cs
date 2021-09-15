using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialSVItem : ScrolleItemBase
{
    private List<PageItemBase> mPageList = new List<PageItemBase>();
    public void Init(List<ThemesModel> list, bool needclear = true)
    {
        base.InitBase();
        Data.Style = PageStyle.PS_2R4C;
        Data.NumPerPage = 8;

        StopCoroutine(ClearData());
        gameObject.SetActive(true);
        if (needclear) StartCoroutine(Clear());
        StartCoroutine(CreatePageItems(list));
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

    public IEnumerator CreatePageItems(List<ThemesModel> list)
    {
        while (islock)
        {
            yield return 0;
        }
        if (list == null)
        {
            yield return null;
        }
        int page_num = list.Count / Data.NumPerPage;
        page_num = (list.Count % Data.NumPerPage == 0) ? page_num : (page_num + 1);
        for (int i = 0; i < page_num; i++)
        {
            List<ThemesModel> one_page_list = new List<ThemesModel>();
            for (int j = i * Data.NumPerPage; j < (i + 1) * Data.NumPerPage && j < list.Count; j++)
            {
                one_page_list.Add(list[j]);
            }
            PageItemBase item = PageManager.Instance.CreateOneSpecialPageItem(grid.transform, Data, one_page_list);
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