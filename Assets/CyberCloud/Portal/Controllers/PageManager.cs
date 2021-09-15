using Com.PicoVR.Gallery;
using System.Collections.Generic;
using UnityEngine;

/* 
 * manage all pages for home scrolle view
*/
public class PageManager : Singleton<PageManager>
{
    private int totalPageNum;
    private Dictionary<string, List<PageItemBase>> pageDict = new Dictionary<string, List<PageItemBase>>();
    public enum PageType
    {
        PhotoPage = 1,
        ThemePage = 2,
        LocalFilePage = 3,
    }

    // create a page
    public PageItemBase CreateOnePageItem(Transform root, string id, PageData pagedata)
    {
        PageItemBase item = null;
        CategoryPhotoData data = CachePhotoData.Instance.GetCatPhotoDataByID(id);
        if (data != null)
        {
            List<PhotoModel> modellist = data.GetNextPageData();
            if (modellist != null)
            {
                GameObject tempgo = null;
                tempgo = GameObject.Instantiate(Resources.Load("UI/PageItem") as GameObject);
                UnityTools.ResetTran(tempgo.transform, root);
                item = tempgo.GetComponent<PageItemBase>();
                if (item == null) item = tempgo.AddComponent<PageItemBase>();
                //item fill data
                item.SetData(data.CategoryID, data.CurrentPage, pagedata, modellist);
                Add(id, item);
            }
        }
        return item;
    }

    public PageItemBase CreateOnePageForSpecial(Transform root, string id, PageData pagedata)
    {
        PageItemBase item = null;
        CategoryPhotoData data = CachePhotoData.Instance.GetThemesPhotosByID(id);
        if (data != null)
        {
            List<PhotoModel> modellist = data.GetNextPageData();
            if (modellist != null)
            {
                GameObject tempgo = null;
                tempgo = GameObject.Instantiate(Resources.Load("UI/PageItem") as GameObject);
                UnityTools.ResetTran(tempgo.transform, root);
                item = tempgo.GetComponent<PageItemBase>();
                if (item == null) item = tempgo.AddComponent<PageItemBase>();
                //item fill data
                item.SetData(data.CategoryID, data.CurrentPage, pagedata, modellist);
                Add(id, item);
            }
        }
        return item;
    }

    //for player scrolle view 
    //dont need to insert to the dict
    public PageItemBase CreateOnePageItem(Transform root, string id, int pagenum, PageData pagedata)
    {
        PageItemBase item = null;
        CategoryPhotoData data = CachePhotoData.Instance.GetCatPhotoDataByID(id);
        if (data != null)
        {
            List<PhotoModel> modellist = data.GetOnePageData(pagedata.NumPerPage, pagenum);
            if (modellist != null)
            {
                GameObject tempgo = null;
                tempgo = GameObject.Instantiate(Resources.Load("UI/PageItem") as GameObject);
                UnityTools.ResetTran(tempgo.transform, root);
                item = tempgo.GetComponent<PageItemBase>();
                if (item == null) item = tempgo.AddComponent<PageItemBase>();
                //item fill data
                item.SetData(data.CategoryID, data.CurrentPage, pagedata, modellist);
            }
        }
        return item;
    }

    public PageItemBase CreateOnePageItem(Transform root, float width, float height, PageType type = PageType.PhotoPage)
    {
        PageItemBase item = null;
        GameObject tempgo = null;
        tempgo = GameObject.Instantiate(Resources.Load(GetPageItemPath(type)) as GameObject);
        UnityTools.ResetTran(tempgo.transform, root);
        item = tempgo.GetComponent<PageItemBase>();
        if (item == null) item = tempgo.AddComponent<PageItemBase>();
        item.SetData(width, height);
        return item;
    }

    public PageItemBase CreateOneLocalPageItem(Transform root, PageData pagedata, List<LocalPhotoModel> modellist)
    {
        PageItemBase item = null;
        if (modellist != null)
        {
            GameObject tempgo = null;
            tempgo = GameObject.Instantiate(Resources.Load("UI/LocalPageItem") as GameObject);
            UnityTools.ResetTran(tempgo.transform, root);
            item = tempgo.GetComponent<PageItemBase>();
            if (item == null) item = tempgo.AddComponent<PageItemBase>();
            //item fill data
            item.SetData(pagedata, modellist);
        }
        return item;
    }

    public PageItemBase CreateOneSpecialPageItem(Transform root, PageData pagedata, List<ThemesModel> modellist)
    {
        PageItemBase item = null;
        if (modellist != null)
        {
            GameObject tempgo = null;
            tempgo = GameObject.Instantiate(Resources.Load("UI/SpecialPageItem") as GameObject);
            UnityTools.ResetTran(tempgo.transform, root);
            item = tempgo.GetComponent<PageItemBase>();
            if (item == null) item = tempgo.AddComponent<PageItemBase>();
            //item fill data
            item.SetData(pagedata, modellist);
        }
        return item;
    }

    private string GetPageItemPath(PageType type)
    {
        string path = "";
        switch (type)
        {
            case PageType.PhotoPage:
                path = "UI/PageItem";
                break;
            case PageType.ThemePage:
                path = "UI/SpecialPageItem";
                break;
            case PageType.LocalFilePage:
                path = "UI/LocalPageItem";
                break;
            default:
                break;
        }
        return path;
    }

    private void Add(string id, PageItemBase item)
    {
        List<PageItemBase> list = null;
        if (pageDict.TryGetValue(id, out list))
        {
            Debug.Log("find the page list which category is :" + id);
            list.Add(item);
        }
        else
        {
            Debug.Log("can not find the page list which category is :" + id);
            list = new List<PageItemBase>();
            list.Add(item);
        }
    }

    // when TotalNum greater than Max start to Dispose some pages
    public void Dispose()
    {
        //TODO

    }

    private void Clear()
    {
        //TODO

    }
}