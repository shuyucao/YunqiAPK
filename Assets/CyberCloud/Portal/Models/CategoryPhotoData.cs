using System.Collections.Generic;
using UnityEngine;

public class CategoryPhotoData
{
    public string CategoryID { get; set; }
    public int TotalPage { get; set; }
    public int TotalNum { get; set; }
    public int CurrentPage { get; set; }
    public int CurrentCachePage { get; set; }
    private Dictionary<string, PhotoModel> mCagoryPhotoDict;
    private List<PhotoModel> mCagoryPhotoList;
    public List<PhotoModel> PhotoList
    {
        get
        {
            return mCagoryPhotoList;
        }
    }

    public void AddPhotoList(List<PhotoModel> list)
    {
        if (list == null)
        {
            UnityEngine.Debug.LogError("the photo list is null!");
            return;
        }
        if (mCagoryPhotoDict == null)
        {
            mCagoryPhotoDict = new Dictionary<string, PhotoModel>();
        }
        if (mCagoryPhotoList == null)
        {
            mCagoryPhotoList = new List<PhotoModel>();
        }
        for (int i = 0; i < list.Count; i++)
        {
            if (mCagoryPhotoDict.ContainsKey(list[i].MID))
            {
                mCagoryPhotoDict.Remove(list[i].MID);
                mCagoryPhotoDict.Add(list[i].MID, list[i]);
            }
            else
            {
                mCagoryPhotoDict.Add(list[i].MID, list[i]);
                mCagoryPhotoList.Add(list[i]);
            }
        }

        if (CurrentCachePage == TotalPage)
        {
            TotalNum = mCagoryPhotoList.Count;
        }
        else
        {
            TotalNum = TotalPage * Constant.MaxLimitEachPage;
        }
    }

    public List<PhotoModel> GetNextPageData()
    {
        return GetOnePageData(Constant.ImgCountPerPage, ++CurrentPage);
    }

    //是否足够填充一页pageitem
    public bool CanFullOnePage()
    {
        int need = (CurrentPage + Constant.PageNumCreatOneTime) * Constant.ImgCountPerPage;
        if (TotalPage == CurrentCachePage)
        {
            return true;
        }
        else
        {
            return mCagoryPhotoList.Count >= need;
        }
    }

    public List<PhotoModel> GetOnePageData(int perpage, int page)
    {
        List<PhotoModel> list = null;
        if (perpage > 0 && page > 0)
        {
            if (mCagoryPhotoList.Count > (perpage * (page - 1)))
            {
                list = new List<PhotoModel>();
                int i = perpage * (page - 1);
                while (i < (perpage * page) && i < mCagoryPhotoList.Count)
                {
                    list.Add(mCagoryPhotoList[i++]);
                }
            }
            else
            {
                UnityEngine.Debug.Log("the mCagoryPhotoList is not full!");
            }
        }
        else
        {
            UnityEngine.Debug.Log("the perpage or page num is not legal!");
        }

        return list;
    }

    public List<PhotoModel> GetPhotoData(int perpage, int head, int end)
    {
        List<PhotoModel> list = null;
        if (perpage > 0 && head > 0 && end > 0 && end >= head && mCagoryPhotoList.Count > (perpage * (head - 1)))
        {
            list = new List<PhotoModel>();
            int i = perpage * (head - 1);
            while (i < (perpage * end) && i < mCagoryPhotoList.Count)
            {
                list.Add(mCagoryPhotoList[i++]);
            }
        }
        else
        {
            UnityEngine.Debug.Log("the perpage or page num is not legal!");
        }

        return list;
    }

    public bool IsLastPage(int perpage, int pageid)
    {
        return (PhotoList.Count <= perpage * pageid) && (PhotoList.Count > perpage * (pageid - 1));
    }

    public PhotoModel GetPhotoModelByMID(string mid)
    {
        PhotoModel data = null;
        mCagoryPhotoDict.TryGetValue(mid, out data);
        return data;
    }

    public int GetIndexByID(string mid)
    {
        for (int i = 0; i < PhotoList.Count; i++)
        {
            if (PhotoList[i].MID.Equals(mid))
            {
                return i;
            }
        }
        return -1;
    }

    public PhotoModel MoveNextPhoto(bool isnext)
    {
        PhotoModel data = null;
        int index = -1;
        for (int i = 0; i < PhotoList.Count; i++)
        {
            if (PhotoList[i].MID.Equals(CachePhotoData.Instance.CurrentPhotoIndex))
            {
                index = i;
                if (isnext && PhotoList.Count > (i + 1))
                {
                    data = PhotoList[i + 1];
                }
                else if (!isnext && i > 0)
                {
                    data = PhotoList[i - 1];
                }
            }
        }
        if (data != null)
        {
            CachePhotoData.Instance.CurrentPhotoIndex = data.MID;
        }
        else if (index == 0)
        {
            CommonAlert.Show("Bar_First", false, null, false);
        }
        else if (index == PhotoList.Count - 1)
        {
            CommonAlert.Show("Bar_Last", false, null, false);
        }
        return data;
    }

    public int GetTotalPageNum(int perpage)
    {
        return Mathf.CeilToInt((float)PhotoList.Count / perpage);
    }
}