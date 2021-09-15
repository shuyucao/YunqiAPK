using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Com.PicoVR.Gallery;
using DG.Tweening;
using CyberCloud.PortalSDK;
using CyberCloud.PortalSDK.vo;
using System.Threading;

public class LeftMenu : MonoBehaviour, IMsgHandle
{
    /// <summary>
    /// 返回按钮
    /// </summary>
    [SerializeField]
    GameObject Back;
    /// <summary>
    /// 左侧背景
    /// </summary>
    [SerializeField]
    GameObject LeftBG;
    [SerializeField]
    GameObject L0;
    [SerializeField]
    GameObject L1;
    [SerializeField]
    GameObject L2;
    [SerializeField]
    GameObject L3;
    [SerializeField]
    GameObject secondPanel;
    [SerializeField]
    GameObject appRoot;
    /// <summary>
    /// 分类名
    /// </summary>
    [SerializeField]
    UILabel appName;
 
    [SerializeField]
    UIGrid mGrid;
    /// <summary>
    /// 分类按钮容器
    /// </summary>
    [SerializeField]
	UIGrid leftGrid;
	[SerializeField]
    GameObject mCategoryOri;
    GameObject highlight = null;
    GameObject previousH = null;
    Vector3 target = new Vector3(0, 0, -30);
    private Dictionary<string, CategoryItem> mItemDict = new Dictionary<string, CategoryItem>();
    //委托定义当菜单切换时触发，HomePageScreen中监听到后切换内容
    public System.Action<string> onMenuChange;
	//public CategoryBtnClick CategoryClick = null;
	List<LevelClass> mLevelList;
    List<Transform> mMenus;

	void Awake()
    {
        MsgManager.Instance.RegistMsg(MsgID.LeftMenu, this);
        MsgManager.Instance.RegistMsg(MsgID.LeftMenuChange, this);
        MsgManager.Instance.RegistMsg(MsgID.CategoryDataReady, this);
        MsgManager.Instance.RegistMsg(MsgID.BackBtn, this);
        highlight = L0;
        previousH = L0;

	}

	// Use this for initialization
	void Start()
    {
        ///监听返回按钮的按下状态
        UIEventListener.Get(Back).onClick = OnBackClick;
        //获取左侧的分类列表
        mMenus = leftGrid.GetChildList();
        //监听左侧分类菜单的点击和获取焦点的事件
        for (int i = 0; i < mMenus.Count; i++)
        {
            UIEventListener.Get(mMenus[i].gameObject).onClick = OnButtonClick;
            UIEventListener.Get(mMenus[i].gameObject).onHover = OnButtonHover;
        }
        //UIEventListener.Get(secondPanel).onHover = OnButtonHideHover;
        UIEventListener.Get(LeftBG).onHover = OnButtonHover;
        DataLoader.Instance.onGetAllClassOver =onGetAllClassOver;
        //HideSecondPanel();

    }

    /// <summary>
    /// DataLoader，加载完数据通过该回调通知LeftMenu接收数据，并初始化菜单
    /// </summary>
    /// <param name="categoryData"></param>
    private void onGetAllClassOver(DataLoader.CategoryData categoryData) {

       

        DataLoader.CategoryData getData = categoryData;// DataLoader.Instance.getData();

        if (getData.childCategory != null)
        {
            mLevelList = getData.childCategory;
               int maxNum = mLevelList.Count > 6 ? 6 : mLevelList.Count;
            //轮询分类数据对菜单进行赋值
            for (int j = 0; j < maxNum; j++)
            {
                mMenus[j].gameObject.GetComponentInChildren<UILabel>().text = mLevelList[j].categoryName;
                mMenus[j].gameObject.SetActive(true);
            }
            //隐藏多余的菜单
            for (int j = mLevelList.Count; j < mMenus.Count; j++)
            {
                mMenus[j].gameObject.SetActive(false);
            }
            //赋值当前分类id
            HomePageScreen.CurrentID = mLevelList[0].categoryCode;
            previousH = highlight;
            hightLeft(previousH, true);
            appName.text = mLevelList[0].categoryName;
            //创建内容模板
            //ScreenManager.Instance.ChangeScreen(UIScreen.Home);

            leftGrid.gameObject.SetActive(true);
            LeftBG.gameObject.SetActive(true);
            mGrid.gameObject.SetActive(true);
            appName.gameObject.transform.parent.gameObject.SetActive(true);

            MyTools.PrintDebugLog("ucvr leftmenu classes init over start init applist by categerID:"+ HomePageScreen.CurrentID);
            onMenuChange(HomePageScreen.CurrentID);


        }
        else
        {
            if (!DataLoader.NotReachable) {
                string msg = Localization.Get("Home_NoNet");
                WingToastManager.Instance.Show(msg);
            }
            //    WingToastManager.Instance.Show("==暂无分类数据==");

            MyTools.PrintDebugLog("ucvr leftMenu no clasess");
   
            for (int j = 0; j < mMenus.Count; j++)
            {
                mMenus[j].gameObject.SetActive(false);
               
            }

        }
    }
    void OnBackClick(GameObject obj)
    {
//        if (highlight == L1 || highlight == L2)
//            MsgManager.Instance.SendMsg(MsgID.BackToSL, null);
//        else
            GalleryTools.QuitApp();
    }
    void OnButtonClick(GameObject obj)
    {
        //Debug.LogError("btclick");
        if (highlight != obj)
        {
            hightLeft(highlight, false);
        }
        else
            return;
        ScrollController.pageIndex = 0;
        UnityTools.SetActive(appRoot.transform, true);
        highlight = obj;
        previousH = highlight;
        hightLeft(previousH, true);
        if (onMenuChange != null)
        {
            for (int i = 0; i < mMenus.Count; i++)
            {
                if (mMenus[i].gameObject == obj)
                {
                    onMenuChange(mLevelList[i].categoryCode);
					appName.text = mLevelList[i].categoryName;
                    break;
                }
            }

        }
    }
    /// <summary>
    /// 凝视点与菜单发生碰撞时触发用于高亮显示菜单或关闭高亮
    /// </summary>
    /// <param name="go"></param>
    /// <param name="status"></param>
    void OnButtonHover(GameObject go, bool status)
    {
        if (highlight == go || secondPanel == go)
            return;
        bool isMenu = false;
        for (int i = 0; i < mMenus.Count; i++)
        {
            if (mMenus[i].gameObject == go)
            {
                isMenu = true;
                break;
            }
        }
        if (isMenu)
        {
            if (status)
            {
                go.GetComponentInChildren<UILabel>().transform.DOLocalMove(target, 0.3f);
                go.GetComponentInChildren<UILabel>().fontStyle = FontStyle.Bold;
                go.GetComponentInChildren<UILabel>().color = new Color(1, 1, 1);
                go.GetComponentInChildren<UILabel>().spacingX = 8;
            }
            else
            {
                go.GetComponentInChildren<UILabel>().fontStyle = FontStyle.Normal;
                go.GetComponentInChildren<UILabel>().color = new Color(191 / 255f, 191 / 255f, 191 / 255f);
                go.GetComponentInChildren<UILabel>().transform.DOLocalMove(Vector3.zero, 0.3f);
                go.GetComponentInChildren<UILabel>().spacingX = 0;
            }
        }
    }



    public void HandleMessage(MsgID id, Bundle bundle)
    {
        Debug.Log("Left menu handle message " + id.ToString());
        if (id == MsgID.LeftMenu)
        {
            bool b = bundle.GetValue<bool>("isShow");
            UnityTools.SetActive(this.transform, b);
        }
        else if (id == MsgID.LeftMenuChange)
        {
            if (!UnityTools.IsActive(this.transform))
                UnityTools.SetActive(this.transform, true);

            UIScreen sc = bundle.GetValue<UIScreen>("CurrentScreen");
           // Debug.LogError("*********************LeftMenuChange   sc = " + sc.ToString()+ "  previousH.name = " + previousH.name);
            if(sc == UIScreen.None)
            {
                hightLeft(previousH, true);
                hightLeft(L3, false);
                return;
            }
            previousH = highlight;
			hightLeft(previousH, true);
//            switch (sc)
//            {
//                case UIScreen.Home:
//                    highlight = L0;
//                    break;
//                case UIScreen.Special:
//                    highlight = L1;
//                    break;
//                case UIScreen.Local:
//                    highlight = L2;
//                    break;
//                case UIScreen.Setting:
//                    highlight = L3;
//                    break;
//                default:
//                    highlight = L0;
//                    break;
//            }
//            hightLeft(highlight, true);
        }
        else if (MsgID.CategoryDataReady == id)
        {
            Debug.Log("CategoryList.Count is " + CachePhotoData.Instance.CategoryList.Count);
            StartCoroutine(CeateCategoryBtn());
        }
        else if (id == MsgID.BackBtn)
        {
//            if (highlight == L3)
//            {
//                Back.SetActive(false);
//            }
//            else
//            {
//                Back.SetActive(true);
//            }
        }
    }

    private IEnumerator CeateCategoryBtn()
    {
        if (mCategoryOri == null)
        {
            Debug.LogError("the origin is null!");
            yield return null;
        }
        for (int i = 0; i < CachePhotoData.Instance.CategoryList.Count; i++)
        {
            CategoryItem btn = UnityTools.CreateComptent<CategoryItem>(mCategoryOri, mGrid.transform);
            mGrid.AddChild(btn.transform);
            mGrid.repositionNow = true;

            if (mItemDict.ContainsKey(CachePhotoData.Instance.CategoryList[i].CategoryID))
            {
                mItemDict.Remove(CachePhotoData.Instance.CategoryList[i].CategoryID);
            }
            mItemDict.Add(CachePhotoData.Instance.CategoryList[i].CategoryID, btn);
            btn.Init(CachePhotoData.Instance.CategoryList[i]);
            //btn.Init(CachePhotoData.Instance.CategoryList[i], CategoryClick);
            yield return new WaitForEndOfFrame();
        }
    }
    void hightLeft(GameObject go,bool b)
    {
        go.transform.Find("bg").gameObject.SetActive(b);
        if(go == L0 && !b)
            L0.transform.Find("bg2").gameObject.SetActive(false);
        if (b)
        {
            go.GetComponentInChildren<UILabel>().transform.DOLocalMove(target, 0.3f);
            go.GetComponentInChildren<UILabel>().fontStyle = FontStyle.Bold;
            go.GetComponentInChildren<UILabel>().color = new Color(1, 1, 1);
            go.GetComponentInChildren<UILabel>().spacingX = 8;
        }
        else
        {
            go.GetComponentInChildren<UILabel>().transform.DOLocalMove(Vector3.zero, 0.3f);
            go.GetComponentInChildren<UILabel>().fontStyle = FontStyle.Normal;
            go.GetComponentInChildren<UILabel>().color = new Color(191 / 255f, 191 / 255f, 191 / 255f);
            go.GetComponentInChildren<UILabel>().spacingX = 0;
        }
    }
}
