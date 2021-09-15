using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 滚动列表 注册列表数据刷新接口 并通知列表刷新数据
/// </summary>
public class ScrollViewPage : MonoBehaviour, IMsgHandle
{
    public HomePageVScrView mScrolleView;
    //public UIScrollBar MainBar;
    //public UIScrollBar Corbar;

    private List<CategoryModel> mCategoryList = null;
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
            //mCategoryList = CachePhotoData.Instance.CategoryList;
            //if (mCategoryList == null || mCategoryList.Count < 1)
            //{
            //    return;
            //}
            //Ftimer.AddEvent("show_scrolle_view", 2f, () =>
            //{
            //    MainBar.gameObject.SetActive(true);
            //    Corbar.gameObject.SetActive(true);
            //});
            MsgManager.Instance.RegistMsg(MsgID.PhotoDataRefresh, this);
            isinit = true;
        }
    }

    public void HandleMessage(MsgID id, Bundle bundle)
    {

        if (id == MsgID.PhotoDataRefresh)
        {
            //Debug.LogError("MsgID.PhotoDataRefresh2:" + bundle.GetValue<string>("categoryID"));
            if (bundle != null && bundle.Contains<string>("categoryID") && bundle.GetValue<string>("categoryID").Equals(HomePageScreen.CurrentID))
            {
              
                CategoryPhotoData data = CachePhotoData.Instance.GetCatPhotoDataByID(HomePageScreen.CurrentID);
                //Debug.LogError("MsgID.PhotoDataRefresh3:" + id );
                mScrolleView.InitData(data);
            }
        }
    }

    public void Switch(CategoryModel model)
    {
        if (model == null)
        {
            Debug.LogError("the model is null!");
            return;
        }
        if (!HomePageScreen.CurrentID.Equals(model.CategoryID))
        {
            HomePageScreen.CurrentID = model.CategoryID;
            Debug.Log("set  HomePageScreen.CurrentID  == " + HomePageScreen.CurrentID);
            //首次点击当前章节需要请求一次数据
            CategoryPhotoData data = CachePhotoData.Instance.GetCatPhotoDataByID(HomePageScreen.CurrentID);
            if (data == null)
            {
                DataLoader.Instance.RequestPhotoDataByID(HomePageScreen.CurrentID);
                MsgManager.Instance.SendMsg(MsgID.CategoryPhotoLoad, null);
            }
            else
            {
                mScrolleView.InitData(data);
            }
        }
    }

    public void SetSVControler(bool add)
    {
        //mScrolleView.SetController(add);
    }
}