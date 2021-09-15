using System.Collections.Generic;
using UnityEngine;

public class LocalPageVScrView : VerticalScrolleView
{
    private List<LocalPhotoModel> mDataList = null;

    public void InitData(List<LocalPhotoModel> list)
    {
        //if (list == null)
        //{
        //    //TODO
        //    Debug.LogError("the list is null!");
        //    SetActiveCtrBar(false);
        //    return;
        //}
        //mDataList = list;
        //base.Init(list.Count, PageManager.PageType.LocalFilePage);
    }

    public override void FillData(int begin, int end)
    {
        //mDataList = GetPhotoData(Constant.ImgCountPerPage, begin, end);
        //for (int i = 0; i < ImgItemList.Count; i++)
        //{
        //    if (mDataList != null && i < mDataList.Count)
        //    {
        //        ImageItemBase item = ImgItemList[i].GetComponent<ImageItemBase>();
        //        item.gameObject.SetActive(true);
        //        item.Init(mDataList[i]);
        //    }
        //    else
        //    {
        //        ImgItemList[i].gameObject.SetActive(false);
        //    }
        //}
        //if (mDataList == null || mDataList.Count == 0)
        //{
        //    SetActiveCtrBar(false);
        //}
        //else
        //{
        //    SetActiveCtrBar(true);
        //}
    }

    //private List<LocalPhotoModel> GetPhotoData(int perpage, int head, int end)
    //{
        //List<LocalPhotoModel> list = new List<LocalPhotoModel>();
        //if (perpage > 0 && head > 0 && end > 0 && end >= head && mDataList.Count > (perpage * (head - 1)))
        //{
        //    int i = perpage * (head - 1);
        //    while (i < (perpage * end) && i < mDataList.Count)
        //    {
        //        list.Add(mDataList[i++]);
        //    }
        //}
        //else
        //{
        //    Debug.Log("the perpage or page num is not legal!");
        //}

        //return list;
    //}
}