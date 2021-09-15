using Com.PicoVR.Gallery;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using CyberCloud.PortalSDK;
using CyberCloud.PortalSDK.vo;
public class HomePageScreen : ScreenBase, IMsgHandle
{
    [SerializeField]
    CategoryPage mCategoriePage;
    [SerializeField]
    ScrollViewPage mContentPage;
    //任意门功能关闭
    //[SerializeField]
    //GameObject mBtnRenyi;
    //[SerializeField]
    //GameObject mBtnRenyiEffect;
    [SerializeField]
    GameObject mLoadUI;
    [SerializeField]
    GameObject mRefreshPage;
    [SerializeField]
    GameObject mBtnRefresh;
    public static string CurrentID = string.Empty;
    public UILabel uilable19Age;
    void Start()
    {
        mCategoriePage.CategoryClick = CategoryBtnClick;
        //UIEventListener.Get(mBtnRenyi).onClick = OnClickRenyi;
        //UIEventListener.Get(mBtnRenyi).onHover = OnHoverRenyi;
        UIEventListener.Get(mBtnRefresh).onClick = OnclickRefresh;
        //UIEventListener.Get(mBtnRefresh).onHover = OnHoverRefresh;
        MsgManager.Instance.RegistMsg(MsgID.CategoryDataReady, this);
        MsgManager.Instance.RegistMsg(MsgID.CategoryPhotoLoad, this);
        MsgManager.Instance.RegistMsg(MsgID.CheckRefreshPage, this);
        //监听整理完成分页数据事件
        MsgManager.Instance.RegistMsg(MsgID.PhotoDataRefresh, this);

        MsgManager.Instance.RegistMsg(MsgID.LeftSecondMenuClick, this);

        mContentPage.Init();
        GameObject.Find("LeftMenu").GetComponent<LeftMenu>().onMenuChange = onCatChange;
        //DataLoader.Instance.onLeftMenuLoadOver = onLeftMenuLoadOver;
        //onLeftMenuLoadOver();
        mLoadUI.SetActive(false);
    }
    public void onLeftMenuLoadOver() {
        onCatChange(HomePageScreen.CurrentID);
    }
    //bool changedPosition = false;
    void OnGUI() {

    }
    private int sessionID;
    private Coroutine routine;

    public void onCatChange(string currentCatergoryId)
	{
        HomePageScreen.CurrentID = currentCatergoryId;
        if (DataLoader.Instance.portalAPI != null) {
            mLoadUI.SetActive(true);
            DataLoader.Instance.onGetItemListByCategory = onGetItemListByCategory;
            sessionID =  (int)System.DateTime.Now.Ticks;
            routine=StartCoroutine(LoadOutTime(sessionID));
            StartCoroutine(DataLoader.Instance.asyApiCall(DataLoader.ApiType.GetItemListByCategory, currentCatergoryId));

		}
	}
    public System.Collections.IEnumerator LoadOutTime(int session) {
        yield return new WaitForSeconds(3);
        if (sessionID == session)
        {
            if (mLoadUI.activeSelf==true)
            {
                mLoadUI.SetActive(false);
                string msg = Localization.Get("Home_NoNet");
                WingToastManager.Instance.Show(msg);
            }
        }
    }
    private bool isTest = false;
    private void onGetItemListByCategory(RetAppListInfo appListInf) {
      
        List<AppInfo> list = null;
        //Debug.LogError("ucvr appListInf.retCode:"+ appListInf.retCode+ ";appListInf.data:"+ appListInf.data);
        if (appListInf != null && appListInf.retCode == 0)
        {
            list = appListInf.data;
            //list = null; ;
            if (list!=null&&list.Count > 0)
            {
                mContentPage.gameObject.SetActive(true);
                uilable19Age.gameObject.SetActive(false);
            }
            else {
                mContentPage.gameObject.SetActive(false);
                uilable19Age.gameObject.SetActive(true);
            }
            mLoadUI.SetActive(false);
            StopCoroutine(routine);
        }
        else
        {
            string msg = Localization.Get("Home_NoNet");
            WingToastManager.Instance.Show(msg);
        }
        if (isTest) {
            for (int i = 0; i < 12; i++) {
                AppInfo t = appListInf.data[1];
                AppInfo temp = new AppInfo();
                temp.appID = t.appID+"temp"+i;
                temp.appImgUrl = t.appImgUrl;
                temp.appName = t.appName+i;
                temp.appUrl = t.appUrl;
                list.Add(temp);
            }
        }
        //Debug.LogError("ucvr list.Count "+ list.Count);
        int total = 0;
        if (list != null)
        {
            //计算总页数，每页显示6个应用
            total = Mathf.CeilToInt(list.Count * 1.0f / 6);
            
        }
        CachePhotoData.Instance.SetCatPhoModleData(HomePageScreen.CurrentID, total, 1, list);
    }

    public void HandleMessage(MsgID id, Bundle bundle)
    {
        if (id == MsgID.CategoryDataReady)
        {
            //mCategoriePage.FillData();
            //if (CachePhotoData.Instance.CategoryList.Count > 0)
            //{
            //    if (CachePhotoData.Instance.GetCategoryModelByID(Main.StartValue) != null)
            //    {
            //        CategoryBtnClick(CachePhotoData.Instance.GetCategoryModelByID(Main.StartValue));
            //    }
            //    else
            //    {
            //        CategoryBtnClick(CachePhotoData.Instance.CategoryList[0]);
            //    }
            //    //mLoadUI.SetActive(false);
            //}
        }
        else if (id == MsgID.CategoryPhotoLoad)
        {
            mContentPage.gameObject.SetActive(false);
            mLoadUI.SetActive(true);
        }
        else if (id == MsgID.PhotoDataRefresh)
        {
            mLoadUI.SetActive(false);
            RefreshPageState();
        }
        else if (id == MsgID.CheckRefreshPage)
        {
            RefreshPageState();
        }
        else if (id == MsgID.LeftSecondMenuClick)
        {
            CategoryModel model = bundle.GetValue<CategoryModel>("ModelInfo");
            CategoryBtnClick(model);
        }
    }

    private void CategoryBtnClick(CategoryModel model)
    {
        if (model.CategoryID != HomePageScreen.CurrentID)
        {
            mLoadUI.SetActive(false);
            mRefreshPage.SetActive(false);
            mCategoriePage.Switch(model);
            if(!mContentPage.gameObject.activeSelf)
                mContentPage.gameObject.SetActive(true);
            mContentPage.Switch(model);
        }
    }

    //when this screen move to the top
    public override void OprateChangeScreen(Bundle bundle)
    {
       // InputManager.OnBack += Back;
        //GalleryTools.ShowLeftBar(true);
    }

    //when this screen closed
    public override void OprateCloseScreen()
    {
        //InputManager.OnBack -= Back;
    }
    //显示内容窗口
    private void RefreshPageState()
    {
        //mRefreshPage.SetActive(CheckNeededData() != 0);
        //if(CheckNeededData() != 0)
        //    MsgManager.Instance.SendMsg(MsgID.BackBtn, null);
        //if (mRefreshPage.activeInHierarchy)
        //{
        //    mLoadUI.SetActive(false);
        //    mContentPage.gameObject.SetActive(false);
        //}
        //else
        //{
            mContentPage.gameObject.SetActive(true);
        //}
    }

    /// <summary>
    /// 0: dont need show refresh page
    /// 1: category not ready
    /// 2: first page not ready
    /// </summary>
    /// <returns></returns>
    private int CheckNeededData()
    {
        if (!CachePhotoData.Instance.IsCategoryDataReady())
        {
            return 1;
        }
        else if (!CachePhotoData.Instance.IsFirstPageInit(HomePageScreen.CurrentID))
        {
            return 2;
        }
        return 0;
    }

    private void OnClickSetting(GameObject go)
    {
        //ScreenManager.Instance.ChangeScreen(UIScreen.Setting);
    }

    private void OnclickRefresh(GameObject go)
    {
        if (MachineState.IsWifiAvailable)
        {
            int state = CheckNeededData();

            Debug.Log("state  is  " + state);

            if (state == 0)
            {
                mRefreshPage.SetActive(false);
            }
            else if (state == 1)
            {
                mRefreshPage.SetActive(false);
                mLoadUI.SetActive(true);
                DataLoader.Instance.RequestCategoryData();
            }
            else if (state == 2)
            {
                mRefreshPage.SetActive(false);
                mLoadUI.SetActive(true);
                DataLoader.Instance.RequestPhotoDataByID(HomePageScreen.CurrentID);
            }
        }
        else
        {
            CommonAlert.Show("Home_NoNet");
        }
    }
    private void OnHoverRefresh(GameObject go, bool ishover)
    {
        TweenScale ts = TweenScale.Begin(mRefreshPage, 0.6f, ishover ? 1.1f * Vector3.one : Vector3.one);
    }
    private void Back()
    {
        GalleryTools.QuitApp();
    }
}