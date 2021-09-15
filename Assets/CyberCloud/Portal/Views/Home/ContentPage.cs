using Com.PicoVR.Gallery;
using System.Collections.Generic;
using UnityEngine;

public class ContentPage : MonoBase
{
    [SerializeField]
    HomePageSVItem mScrolleOrigin;
    public UIScrollBar MainBar;
    public UIScrollBar Corbar;

    private Dictionary<string, HomePageSVItem> mItemDict = new Dictionary<string, HomePageSVItem>();
    private List<CategoryModel> mCategoryList = null;
    public static string CurrentID = string.Empty;
    private bool isinit = false;

    public void Init()
    {
        if (isinit)
        {
            Debug.LogError("already inited!");
            return;
        }
        else
        {
            mCategoryList = CachePhotoData.Instance.CategoryList;
            if (mCategoryList == null || mCategoryList.Count < 1)
            {
                //Debug.LogError("category is null!");
                return;
            }
            CreateViewPages();
            Ftimer.AddEvent("show_scrolle_view", 2f, () =>
            {
                MainBar.gameObject.SetActive(true);
                Corbar.gameObject.SetActive(true);
            });
            HideAllScrolleView();
            //Switch(mCategoryList[0]);
            isinit = true;
        }
    }

    private void CreateViewPages()
    {
        for (int i = 0; i < mCategoryList.Count; i++)
        {
            CreateOnePage(mCategoryList[i]);
        }
    }

    public void CreateNextPage()
    {
        HomePageSVItem hs;
        if (mItemDict.TryGetValue(CurrentID, out hs))
        {
            hs.CreateItemList();
        }
    }

    private void CreateOnePage(CategoryModel model)
    {
        if (model == null)
        {
            Debug.LogError("the model is null!");
        }
        else
        {
            HomePageSVItem item = UnityTools.CreateComptent<HomePageSVItem>(mScrolleOrigin.gameObject, transform);
            if (item != null)
            {
                item.Init(model.CategoryID);
                if (mItemDict.ContainsKey(model.CategoryID))
                {
                    mItemDict.Remove(model.CategoryID);
                }
                mItemDict.Add(model.CategoryID, item);
            }
            else
            {
                Debug.LogError("cannot create the ScroleViewItem!");
            }
        }
    }
    private void testNoIdea() {
         while(true) {
            Debug.Log("testNoIdea");
            Switch(null);
            if (mItemDict == null) {
                HomePageSVItem item;
                mItemDict.TryGetValue("key",out item);
                CurrentID = "key";
                UnityTools.SetActive(null, false);
            }
            break;
        }
    }
    public void Switch(CategoryModel model)
    {
        if (model == null)
        {
            Debug.LogError("the model is null!");
            return;
        }
        if (mItemDict == null || mItemDict.Count < 1)
        {
            Debug.LogError("category is null!");
            return;
        }
        if (mItemDict.ContainsKey(model.CategoryID))
        {
            HomePageSVItem item;
            mItemDict.TryGetValue(model.CategoryID, out item);
            if (!CurrentID.Equals(model.CategoryID))
            {
                if (!string.IsNullOrEmpty(CurrentID))
                {
                    HomePageSVItem cur;
                    mItemDict.TryGetValue(CurrentID, out cur);
                    cur.scrolleView.verticalScrollBar = null;
                    UnityTools.SetActive(cur.transform, false);
                }
                CurrentID = model.CategoryID;
                UnityTools.SetActive(item.transform, true);
                item.scrolleView.verticalScrollBar = MainBar;
                item.scrolleView.CheckScrollbars();

                if (gameObject.GetComponent<ScvControler>() != null)
                {
                    gameObject.GetComponent<ScvControler>().SetScrollView(item.scrolleView);
                }

                //首次点击当前章节需要请求一次数据
                CategoryPhotoData data = CachePhotoData.Instance.GetCatPhotoDataByID(CurrentID);
                if (data == null)
                {
                    DataLoader.Instance.RequestPhotoDataByID(CurrentID);
                    //DataLoader.Instance.RequestNextPage();
                    MsgManager.Instance.SendMsg(MsgID.CategoryPhotoLoad, null);
                }
                Debug.Log("Switch the category which name is:" + model.Name);
            }
        }
        else
        {
            Debug.LogError("there are not this item which name is :" + model.Name);
        }
    }

    private void HideAllScrolleView()
    {
        foreach (var item in mItemDict)
        {
            UnityTools.SetActive(item.Value.transform, false);
        }
    }

    public ScrolleItemBase GetCurrentView()
    {
        ScrolleItemBase sv = null;
        HomePageSVItem item;
        if (mItemDict.TryGetValue(CurrentID, out item))
        {
            sv = item;
        }
        return sv;
    }
}
