using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BestHTTP;

public class HomeScreen : WingScreenBase
{
    public GameObject mCategoryButtons;
    public GameObject mThumbnail;  //这个已经没用了
    public GameObject mScrollView;
    public GameObject mSettingSprite;
    public GameObject mLoading;
    public GameObject mBackPanel;
    public GameObject mYesExit;
    public GameObject mNoExit;
    public float time;
    public ImageScrollView mImageScrollView;
    public delegate void RequestCallback(bool success = false);
    public static RequestCallback OnPageRequestFinished;
    public static RequestCallback OnAllIconDownloaded;
    GalleryNetApi mNetAPI;
    List<CategoryModel> mCategoryList = null;
    List<PhotoModel> mPhotoModelList = null;
    string mMaxLimitEachPage = "4";
    private int mCurrentTotalPage = 0;
    private string mCurrentCategoryID = "";
    private int mDownloadingIcon = 0;
    private int mDownloadIconFailed = 0;
    private GameObject mCurrentCategoryButton = null;

    void Awake()
    {
        Debug.Log("Back to Home  Awake : Second : " + System.DateTime.Now.Second + " Millisecond : " + System.DateTime.Now.Millisecond);
        InvokeRepeating("TimeCount", 0, 1);
    }

    void Start()
    {
        Debug.Log("Back to Home  Start : Second : " + System.DateTime.Now.Second + " Millisecond : " + System.DateTime.Now.Millisecond);


        mCategoryList = GlobalPhotoData.Instance.GetCategoryList();
        mPhotoModelList = GlobalPhotoData.Instance.GetPhotoList();

        mNetAPI = new GalleryNetApi();
        GalleryNetApi.OnDataCallbackDelegate += OnDataCallback;

        UIEventListener.Get(mSettingSprite).onHover = OnButtonHover;
        UIEventListener.Get(mYesExit).onHover = OnButtonHover;
        UIEventListener.Get(mNoExit).onHover = OnButtonHover;
        UIEventListener.Get(mYesExit).onClick = OnButtonClick;
        UIEventListener.Get(mNoExit).onClick = OnButtonClick;
        //InputManager.OnBack += OnBack;
        InitBase();
        Debug.Log("Back to Home  Start Finished : Second : " + System.DateTime.Now.Second + " Millisecond : " + System.DateTime.Now.Millisecond);
    }

    void TimeCount()  //1.0 5345
    {
        if (mImageScrollView.transform.childCount == 4)
        {
            mLoading.SetActive(true);
        }
        if (mLoading.activeSelf)
        {
            time++;
            if (time >= 20f)
            {
                WingToastManager.Instance.Show(Localization.Get("Home_Category_Failed"));
                time = 0;
            }
        }
        else
            time = 0;
    }

    void OnEnable()
    {
        Debug.Log("OnEnable");
    }

    void OnDisable()
    {
        Debug.Log("OnDisable");
    }

    void OnDestroy()
    {
        Debug.Log("OnDestroy");

      //  InputManager.OnBack -= OnBack;
        GalleryNetApi.OnDataCallbackDelegate -= OnDataCallback;
    }

    void OnBack()
    {
        if (Visible)
        {
            mBackPanel.SetActive(!mBackPanel.activeSelf);
        }
    }

    public override void Init()
    {
        base.Init();
    }

    public override void Show(object param)
    {
        base.Show(param);

        if (mScrollView != null)
        {
            GameObject panel = mScrollView.transform.Find("Panel").gameObject;
            if (panel != null)
            {
                panel.GetComponent<ImageScrollView>().InitInput();
            }
        }
    }

    public override void Destroy()
    {
        base.Destroy();

        if (mScrollView != null)
        {
            GameObject panel = mScrollView.transform.Find("Panel").gameObject;
            if (panel != null)
            {
                panel.GetComponent<ImageScrollView>().DestroyInput();
            }
        }
    }

    #region 给ImageScrollView用的几个public函数

    public void SendNextPageRequest(string page)
    {
        //mNetAPI.GetCategoryPhoto(GalleryNetApi.funGetCategoryPhoto, mCurrentCategoryID, mMaxLimitEachPage, page);
    }

    public int GetDownloadingIconNum()
    {
        return mDownloadingIcon;
    }

    public void ResetDownloadCounting()
    {
        mDownloadingIcon = 0;
        mDownloadIconFailed = 0;
    }

    public void DownloadIcon_Index(int index, GameObject item)
    {
        if (index + 1 <= mPhotoModelList.Count)
        {
            string textureLink = mPhotoModelList.ToArray()[index].CoverLink;
            GetTextureByBestHttp(textureLink, item);
        }
        else
        {
            Debug.LogError("DownloadIcon_Index : index out of range !");
        }
    }

    #endregion

    private void InitBase()
    {
        bool accessNet = false;
#if UNITY_EDITOR
        accessNet = true;

#elif UNITY_ANDROID
        accessNet = GalleryActivity.Instance.IsNetworkAvailable_Activity();

#endif

        InitButtonEvent();

        if (!accessNet)
        {
            WingToastManager.Instance.Show(Localization.Get("Home_NoNet"));
            return;
        }

        if (mCategoryList != null && mCategoryList.Count >= 6)
        {
            InitCategoryButtons(ref mCategoryList);

            bool needRecover = false;
            if (mCategoryButtons != null)
            {
                GameObject tempButton;
                string tempID;
                for (int i = 0; i < mCategoryButtons.transform.childCount; i++)
                {
                    tempButton = mCategoryButtons.transform.GetChild(i).gameObject;
                    if (tempButton != null)
                    {
                        tempID = tempButton.GetComponent<CategoryButton>().CategoryID;
                        if (tempID == GlobalPhotoData.Instance.mCurrentCategoryID)
                        {
                            mCurrentCategoryButton = tempButton;
                            needRecover = true;
                            break;
                        }
                    }
                }

            }

            if (needRecover)
            {
                mLoading.SetActive(true);

                //根据现有数据恢复列表
                mCurrentCategoryID = GlobalPhotoData.Instance.mCurrentCategoryID;

                int columnsPerPage = GlobalPhotoData.Instance.mColumnsPerPage;
                int photoCount = mPhotoModelList.Count;
                int havePage;
                if (photoCount % columnsPerPage == 0)
                {
                    havePage = photoCount / columnsPerPage;
                }
                else
                {
                    havePage = photoCount / columnsPerPage + 1;
                }

                //获得数据, 总页数和当前最后一行的数据
                HomeScreen.OnPageRequestFinished += OnPageRequestFinished_Recover;
                //mNetAPI.GetCategoryPhoto(GalleryNetApi.funGetCategoryPhoto + "Recover", mCurrentCategoryID, mMaxLimitEachPage, havePage.ToString());
            }
            else
            {
                //清理全局数据
                ClearGlobalData();
            }

        }
        else
        {
            RequestCategory();
        }

    }

    public void OnPageRequestFinished_Recover(bool success = false)
    {
        HomeScreen.OnPageRequestFinished -= OnPageRequestFinished_Recover;
        mLoading.SetActive(false);
        if (true == success)
        {
            //当前分类按钮的底色
            mCurrentCategoryButton.transform.Find("Pick").gameObject.SetActive(true);
            mCurrentCategoryButton.GetComponent<UIButton>().SetState(UIButtonColor.State.Normal, false);
            if (mScrollView != null)
            {
                mScrollView.GetComponentInChildren<ImageScrollView>().ClearData();
                mScrollView.GetComponentInChildren<ImageScrollView>().RecoverList(mCurrentTotalPage, mPhotoModelList.Count);

            }
        }
        else
        {
            mCurrentCategoryID = "";
            ClearGlobalData();
            WingToastManager.Instance.Show(Localization.Get("Home_NoNet"));
        }
    }

    void InitButtonEvent()
    {

        if (mCategoryButtons != null)
        {
            string buttonName;
            for (int i = 0; i < mCategoryList.Count + 1; i++) //分类数量修改
            {
                buttonName = "Button" + i.ToString();
                Debug.Log("buttonname " + buttonName);
                GameObject tempButton = mCategoryButtons.transform.Find(buttonName).gameObject;
                if (tempButton != null)
                {
                    UIEventListener.Get(tempButton).onClick = OnButtonClick;
                    UIEventListener.Get(tempButton).onHover = OnButtonHover;
                }
                if (0 == i)
                {
                    mCurrentCategoryButton = tempButton;
                }
            }
        }


        if (mSettingSprite != null)
        {
            UIEventListener.Get(mSettingSprite).onClick = OnSettingButtonClick;
        }
    }

    void OnSettingButtonClick(GameObject obj)
    {
        //if (mNetAPI.HavePendingRequest() == true)
        //{
        //    Debug.Log("HomeScreen OnButtonClick : HavePendingRequest");
        //    return;
        //}

        //if (mDownloadingIcon > 0)
        //{
        //    Debug.Log("HomeScreen OnButtonClick : mDownloadingIcon > 0");
        //    return;
        //}


        Debug.Log("HomeScreen OnSettingButtonClick : " + obj.name);
        WingScreenManager.Instance.PushScreen(EWingScreen.setting);
    }

    void OnButtonClick(GameObject obj)
    {
        if (obj == mYesExit)
        {
#if UNITY_EDITOR
            Debug.Log("OnBack");
#elif UNITY_ANDROID
        Debug.Log( "OnBack HomeScreen" );
        Application.Quit();
#endif
        }
        else if (obj == mNoExit)
        {
            mBackPanel.SetActive(false);
        }
        else
        {
            if (mNetAPI.HavePendingRequest() == true)
            {
                Debug.Log("HomeScreen OnButtonClick : HavePendingRequest");
                return;
            }

            //if (mDownloadingIcon > 0)
            //{
            //    Debug.Log("HomeScreen OnButtonClick : mDownloadingIcon > 0");
            //    return;
            //}


            Debug.Log("HomeScreen OnButtonClick : " + obj.name);
            mCurrentCategoryButton = obj;

            string id = obj.GetComponent<CategoryButton>().CategoryID;
            if (id != null)
            {
                if (id == mCurrentCategoryID)
                {
                    return;
                }

                //更新选中底图和当前按钮
                GameObject tempGO;
                for (int i = 0; i < mCategoryButtons.transform.childCount; i++)
                {
                    tempGO = mCategoryButtons.transform.GetChild(i).gameObject;
                    tempGO.transform.Find("Button" + i.ToString()).Find("Pick").gameObject.SetActive(false);            //改
                }
                obj.transform.GetChild(0).Find("Pick").gameObject.SetActive(true);                                   //改
                obj.GetComponent<UIButton>().SetState(UIButtonColor.State.Normal, false);

                Debug.Log("HomeScreen OnButtonClick CategoryID : " + obj.GetComponent<CategoryButton>().CategoryID);

                mCurrentCategoryID = id;
                GlobalPhotoData.Instance.mCurrentCategoryID = mCurrentCategoryID;

                RequestFirstPage();
            }
            else
            {
                if (null == mCategoryList || mCategoryList.Count <= 0)
                {
                    RequestCategory();
                }
                else
                {
                    Debug.LogError("OnButtonClick :null == mCategoryList || mCategoryList.Count <= 0 !");
                }
            }
        }
    }

    void OnButtonHover(GameObject obj, bool status)
    {
        if (obj == mSettingSprite)
        {
            Vector3 pos = obj.transform.parent.localPosition;
            obj.transform.parent.localPosition = new Vector3(pos.x, pos.y, status ? -3 : 0);
        }
        else if (obj == mYesExit || obj == mNoExit)
        {
            Vector3 pos = obj.transform.localPosition;
            obj.transform.localPosition = new Vector3(pos.x, pos.y, status ? -10 : 0);
        }
        else
        {
            if (mCurrentCategoryButton != obj)
            {

                //Vector3 pos = obj.GetComponent<CategoryButton>().OriginPosition;
                float x = Mathf.Sin(obj.transform.localRotation.eulerAngles.y * Mathf.Deg2Rad) * 3;
                //obj.GetComponentInChildren<UILabel>().transform.localPosition = new Vector3(status ? pos.x - x : pos.x, pos.y, status ? pos.z - 3 : pos.z);
                obj.GetComponentInChildren<UILabel>().transform.localPosition = new Vector3(status ? 0 - x : 0, 0, status ? 0 - 3 : 0);
            }
            else
            {
                mCurrentCategoryButton.GetComponent<UIButton>().SetState(UIButtonColor.State.Normal, false);
                Vector3 pos = obj.GetComponent<CategoryButton>().OriginPosition;                    //改 452 453
                obj.transform.localPosition = pos;

            }
        }
    }

    void InitCategoryButtons(ref List<CategoryModel> categoryList)
    {
        if (categoryList == null)
        {
            Debug.LogError("InitCategoryButtons categoryList is null");
            return;
        }

        if (mCategoryButtons != null)
        {
            string buttonName;
            GameObject tempButton;

            for (int i = 1; i < categoryList.Count + 1; i++)  //分类数量修改
            {
                buttonName = "Button" + i.ToString();
                tempButton = mCategoryButtons.transform.Find(buttonName).gameObject;
                if (tempButton != null)
                {
                    tempButton.GetComponent<CategoryButton>().CategoryID = mCategoryList.ToArray()[i - 1].CategoryID;
                    tempButton.GetComponent<CategoryButton>().SetName(mCategoryList.ToArray()[i - 1].Name);
                }
            }
            //全部按钮
            buttonName = "Button0";
            tempButton = mCategoryButtons.transform.Find(buttonName).gameObject;
            if (tempButton != null)
            {
                tempButton.GetComponent<CategoryButton>().CategoryID = "0";
                tempButton.GetComponent<CategoryButton>().SetName(Localization.Get("Home_Category_All"));
            }

            //重新计算位置
            buttonName = "Button3";
            Transform _transform = mCategoryButtons.transform.Find(buttonName);
            UILabel label = _transform.GetComponentInChildren<UILabel>();           //改
            Vector2 labelSize1 = label.localSize;                                     //改 495 496
            _transform.GetComponent<BoxCollider>().size = new Vector3(labelSize1.x, labelSize1.y, _transform.GetComponent<BoxCollider>().size.z);

            CategoryButton btn = _transform.GetComponent<CategoryButton>();
            btn.OriginPosition = _transform.localPosition;
            float singleLength = label.fontSize * _transform.localScale.x;//单个字的长度
            float leftPosX = -label.text.Length * singleLength / 2;
            float rightPosX = label.text.Length * singleLength / 2;
            float space = 8.0f;
            int count = 3;
            for (int i = 1; i < 4; i++)
            {
                //后三个分类
                int index = count + i;
                buttonName = "Button" + index;
                _transform = mCategoryButtons.transform.Find(buttonName);
                label = _transform.GetComponentInChildren<UILabel>();     //改
                Vector2 labelSize = label.localSize;                      //改 510-513
                float x = labelSize.x;
                float y = labelSize.y;
                _transform.GetComponent<BoxCollider>().size = new Vector3(x, y, _transform.GetComponent<BoxCollider>().size.z);
                btn = _transform.GetComponent<CategoryButton>();
                btn.OriginPosition = _transform.localPosition = new Vector3(rightPosX + space + label.text.Length * singleLength / 2, _transform.localPosition.y, _transform.localPosition.z);
                rightPosX = _transform.localPosition.x + label.text.Length * singleLength / 2;

                //前三个
                index = count - i;
                buttonName = "Button" + index;
                _transform = mCategoryButtons.transform.Find(buttonName);
                label = _transform.GetComponentInChildren<UILabel>();                 //改
                Vector2 _labelSize = label.localSize;                                  //改 523-526
                float _x = _labelSize.x;
                float _y = _labelSize.y;
                _transform.GetComponent<BoxCollider>().size = new Vector3(_x, _y, _transform.GetComponent<BoxCollider>().size.z);
                btn = _transform.GetComponent<CategoryButton>();
                btn.OriginPosition = _transform.localPosition = new Vector3(leftPosX - space - label.text.Length * singleLength / 2, _transform.localPosition.y, _transform.localPosition.z);
                leftPosX = _transform.localPosition.x - label.text.Length * singleLength / 2;
            }
        }
    }

    void ClearGlobalData()
    {
        ClearIconTextureList();

        for (int i = 0; i < mPhotoModelList.Count; i++)
        {
            mPhotoModelList.RemoveAt(i);
        }
        mPhotoModelList.Clear();

        for (int i = 0; i < mCategoryList.Count; i++)
        {
            mCategoryList.RemoveAt(i);
        }
        mCategoryList.Clear();


        GlobalPhotoData.Instance.mCurrentTotalPage = 0;
        GlobalPhotoData.Instance.mCurrentCategoryID = "";
        GlobalPhotoData.Instance.mCurrentPhotoIndex = 0;
    }

    void ClearIconTextureList()
    {
        //清理icon贴图
        List<IconTexture> IconTextureList = GlobalPhotoData.Instance.GetIconTextureList();
        for (int i = 0; i < IconTextureList.Count; i++)
        {
            if (IconTextureList[i].texture != null)
            {
                GameObject.DestroyImmediate(IconTextureList[i].texture);
                IconTextureList[i].texture = null;
            }
            IconTextureList.RemoveAt(0);
        }
        IconTextureList.Clear();
    }

    void InitScrollView(int totalPage, int itemCount)
    {
        if (mScrollView != null)
        {
            mScrollView.GetComponentInChildren<ImageScrollView>().ClearData();

            ClearIconTextureList();


            mScrollView.GetComponentInChildren<ImageScrollView>().InitData(totalPage, itemCount);
        }
    }

    void ScrollViewAddData(int addNum)
    {
        if (mScrollView != null)
        {
            mScrollView.GetComponentInChildren<ImageScrollView>().AddItem(addNum);
        }
    }

    void SetScrollItemTitle(int index, string title)
    {
        //目前只有这个函数用了mImageScrollView
        if (mImageScrollView != null)
        {
            mImageScrollView.SetItemTitle(index, title);
        }
    }

    private void RequestCategory()
    {
        HomeScreen.OnPageRequestFinished += OnPageRequestFinished_Category;
        mLoading.SetActive(true);
        //WingToastManager.Instance.Show(Localization.Get("Home_Category"), -1.0f);
        //mNetAPI.GetCategory(GalleryNetApi.funGetCategory);      //下载分类按钮的数据
    }

    public void OnPageRequestFinished_Category(bool success = false)
    {
        HomeScreen.OnPageRequestFinished -= OnPageRequestFinished_Category;
        mLoading.SetActive(false);
        if (success == true)
        {
            if (mCurrentCategoryButton != null)
            {
                //主动调OnButtonClick请求主页的数据
                OnButtonClick(mCurrentCategoryButton);
            }

        }
        else
        {
            WingToastManager.Instance.Show(Localization.Get("Home_Category_Failed"));
        }

    }

    private void RequestFirstPage()
    {
        HomeScreen.OnPageRequestFinished += OnPageRequestFinished_FirstPage;

        mLoading.SetActive(true);
        string key = GalleryNetApi.funGetCategoryPhoto + "First";
        //mNetAPI.GetCategoryPhoto(key, mCurrentCategoryID, mMaxLimitEachPage, "1");
    }

    public void OnPageRequestFinished_FirstPage(bool success = false)
    {
        HomeScreen.OnPageRequestFinished -= OnPageRequestFinished_FirstPage;
        mLoading.SetActive(false);

        if (success == true)
        {
            if (mCurrentTotalPage > 1)
            {
                HomeScreen.OnPageRequestFinished += OnPageRequestFinished_SecondPage;
                //mNetAPI.GetCategoryPhoto(GalleryNetApi.funGetCategoryPhoto, mCurrentCategoryID, mMaxLimitEachPage, "2");
            }
        }
        else
        {
            WingToastManager.Instance.Show(Localization.Get("Home_NoNet"));
        }

    }

    public void OnPageRequestFinished_SecondPage(bool success = false)
    {
        HomeScreen.OnPageRequestFinished -= OnPageRequestFinished_SecondPage;

        if (!success)
        {
            WingToastManager.Instance.Show(Localization.Get("Home_NoNet"));
        }
    }

    /// <summary>
    /// On the data callback, the callback when request finshed
    /// </summary>
    /// <param name="req">Req.</param>
    /// <param name="resp">Resp.</param>
    public void OnDataCallback(HTTPRequest req, HTTPResponse resp)
    {
        bool dispatchSuccess = false;
        //string key = req.Tag as String;
        string key = req.Tag as string;
        switch (req.State)
        {
            // The request finished without any problem.
            case HTTPRequestStates.Finished:
                if (!resp.IsSuccess)
                {
                    Debug.LogWarning("OnDataCallback-> statusCode= " + resp.StatusCode + " Message= " + resp.Message + "result= " + resp.DataAsText);
                }

                dispatchSuccess = DispatchResult(key, resp.DataAsText);
                break;

            // The request finished with an unexpected error. The request's Exception property may contain more info about the error.
            case HTTPRequestStates.Error:
                Debug.LogWarning("OnDataCallback->Request Finished with Error! " + (req.Exception != null ? (req.Exception.Message + "\n" + req.Exception.StackTrace) : "No Exception"));

                break;
            // The request aborted, initiated by the user.
            case HTTPRequestStates.Aborted:
                Debug.LogWarning("OnDataCallback->Request Aborted!");

                break;
            // Ceonnecting to the server is timed out.
            case HTTPRequestStates.ConnectionTimedOut:
                Debug.LogWarning("OnDataCallback->Connection Timed Out!");

                break;
            // The request didn't finished in the given time.
            case HTTPRequestStates.TimedOut:
                Debug.LogWarning("OnDataCallback->Processing the request Timed Out!");

                break;
            default:
                Debug.LogError("OnDataCallback->Unknown Error!");
                break;
        }



        bool success = false;
        if (req.State == HTTPRequestStates.Finished && dispatchSuccess == true)
        {
            success = true;
        }

        if (OnPageRequestFinished != null)
        {
            OnPageRequestFinished(success);
        }

    }

    /// <summary>
    /// DispatchResult     	
    /// </summary>
    /// <param name="key">Key.</param>
    /// <param name="result">Result.</param>
    /// <param name="msg">Message.</param>
    bool DispatchResult(string key, string result)
    {
        Debug.Log("DispatchRequest->this is callback of key : " + key);
        bool ret = false;
        int message = 0;
        int pageRef = 0;

        switch (key)
        {
            case GalleryNetApi.funGetCategory:
                List<CategoryModel> newCategoryList = JsonUtils.ParseCategoryJson(result, ref message);
                if (newCategoryList == null)
                {
                    Debug.LogError("DispatchResult funGetCategory newCategoryList is null !");
                }
                else
                {
                    if (mCategoryList.Count > 0)
                    {
                        for (int i = 0; i < mCategoryList.Count; i++)
                        {
                            mCategoryList.RemoveAt(i);
                        }
                        mCategoryList.Clear();
                    }

                    mCategoryList.AddRange(newCategoryList);

                    for (int i = 0; i < newCategoryList.Count; i++)
                    {
                        newCategoryList.RemoveAt(i);
                    }
                    newCategoryList.Clear();



                    InitCategoryButtons(ref mCategoryList);
                    ret = true;
                    InitButtonEvent();
                }

                break;

            case GalleryNetApi.funGetCategoryPhoto + "First":

                List<PhotoModel> newListFirst = JsonUtils.ParsePhotoJson(result, ref message, ref mCurrentTotalPage, ref pageRef);
                GlobalPhotoData.Instance.mCurrentTotalPage = mCurrentTotalPage;

                if (newListFirst == null)
                {
                    Debug.LogError("DispatchResult funGetCategoryPhotoFirst newListFirst is null !");
                }
                else
                {
                    if (mPhotoModelList == null)
                    {
                        mPhotoModelList = new List<PhotoModel>();
                    }
                    else
                    {
                        //实际上mPhotoModelList不存在为null的情况
                        //当切换分类的时候清除数据
                        for (int i = 0; i < mPhotoModelList.Count; i++)
                        {
                            mPhotoModelList.RemoveAt(0);
                        }
                        mPhotoModelList.Clear();
                    }

                    mPhotoModelList.AddRange(newListFirst);
                    newListFirst.Clear();


                    InitScrollView(mCurrentTotalPage, mPhotoModelList.Count);
                    for (int i = 0; i < mPhotoModelList.Count; i++)
                    {
                        SetScrollItemTitle(i, mPhotoModelList[i].Title);
                    }

                    //如果mCurrentTotalPage = 1,  ImageScrollView.InitData里加了OnAllIconDownloaded的回调
                    DownLoadPageTexture(0, mPhotoModelList.Count - 1);
                    ret = true;
                }

                break;

            case GalleryNetApi.funGetCategoryPhoto:

                List<PhotoModel> newList = JsonUtils.ParsePhotoJson(result, ref message, ref mCurrentTotalPage, ref pageRef);
                GlobalPhotoData.Instance.mCurrentTotalPage = mCurrentTotalPage;

                if (newList == null)
                {
                    Debug.LogError("DispatchResult funGetCategoryPhoto newList is null !");
                }
                else
                {
                    int addNum = newList.Count;
                    int oldCount = mPhotoModelList.Count;

                    mPhotoModelList.AddRange(newList);
                    newList.Clear();

                    ScrollViewAddData(addNum);

                    //firstGet意思是 是否请求第二行数据,因为第二行的数据是得到第一行之后自动请求的
                    bool firstGet = false;
                    if (pageRef == 2)
                    {
                        if (mScrollView != null)
                        {
                            mScrollView.GetComponentInChildren<ImageScrollView>().PrepareForGetFirst();
                        }
                        firstGet = true;
                    }




                    for (int i = oldCount; i < mPhotoModelList.Count; i++)
                    {
                        SetScrollItemTitle(i, mPhotoModelList[i].Title);
                    }

                    //如果有第二页,两页的icon全下完才执行OnAllIconDownloaded回调
                    DownLoadPageTexture(oldCount, mPhotoModelList.Count - 1, firstGet);
                    ret = true;
                }

                break;

            case GalleryNetApi.funGetCategoryPhoto + "Recover":


                List<PhotoModel> newListRecover = JsonUtils.ParsePhotoJson(result, ref message, ref mCurrentTotalPage, ref pageRef);
                GlobalPhotoData.Instance.mCurrentTotalPage = mCurrentTotalPage;

                if (newListRecover == null)
                {
                    Debug.LogError("DispatchResult funGetCategoryPhotoRecover newListRecover is null !");
                }
                else
                {
                    int oldCount = mPhotoModelList.Count;
                    int getNum = (pageRef - 1) * GlobalPhotoData.Instance.mColumnsPerPage + newListRecover.Count;

                    //添加新数据
                    if (getNum > oldCount)
                    {
                        int addNum = getNum - oldCount;

                        for (int i = newListRecover.Count - addNum; i < newListRecover.Count; i++)
                        {
                            mPhotoModelList.Add(newListRecover[i]);
                        }
                    }


                    //销毁newList
                    for (int i = 0; i < newListRecover.Count; i++)
                    {
                        newListRecover.RemoveAt(i);
                    }
                    newListRecover.Clear();

                    ret = true;
                }

                break;
            //case Constant.TYPE_VIDEO_FAVOURITE:
            //    DispatchFavouriteData(key, result, msg);
            //    break;
            //case Constant.TYPE_VIDEO_HISTORY:
            //    DispatchHistoryData(key, result, msg);
            //    break;
            //case Constant.TYPE_VIDEO_DOWNLOAD:
            //    break;
            //case Constant.TYPE_ADD_FAVORITE_VIDEO:
            //    DispatchAddFavoriteData(key, result, msg);
            //    break;
            //case Constant.TYPE_DEL_FAVORITE_VIDEO:
            //    DispatchDelFacoriteData(key, result, msg);
            //    break;
            //case Constant.TYPE_ADD_HISTORY_VIDEO:
            //    Debug.Log("Add to history successful !");
            //    break;
            //case Constant.TYPE_GET_CATEGORYID:
            //    DispatchGetCategoryIdData(key, result, msg);
            //    break;
            default:
                break;
        }

        return ret;
    }

    void DownLoadPageTexture(int indexStart, int indexEnd, bool firstGet = false)
    {
        if (mScrollView != null)
        {
            List<GameObject> list = mScrollView.GetComponentInChildren<ImageScrollView>().GetItemList();
            if (list == null)
            {
                Debug.LogError("DownLoadPageTexture : list == null !");
                return;
            }

            if (list.Count <= indexEnd)
            {
                Debug.LogError("DownLoadPageTexture : list.Count <= index !");
                return;
            }

            if (mPhotoModelList.Count <= indexEnd)
            {
                Debug.LogError("DownLoadPageTexture : list.Count != mPhotoModelList.Count !");
                return;
            }

            if (firstGet == false)
            {
                mDownloadingIcon = 0;
                mDownloadIconFailed = 0;
            }

            string textureLink;
            for (int i = indexStart; i <= indexEnd; i++)
            {
                //textureLink = mPhotoModelList.ToArray()[i].ThumbnailLink;
                textureLink = mPhotoModelList.ToArray()[i].CoverLink;
                GetTextureByBestHttp(textureLink, list[i]);
            }
        }
    }

    //StartCoroutine( loadThumnail(textureLink) ); 用BestHttp 不用WWW下载了
    IEnumerator loadThumnail(string url)
    {
        WWW www = new WWW(url);
        yield return www;

        if (www.error != null)
        {
            //mThumbnail.GetComponent<UITexture> ().mainTexture = Resources.Load ("Common/VideoList/default_poster_H") as Texture;
        }
        else if (url.Equals(www.url))
        {
            mThumbnail.GetComponent<UITexture>().mainTexture = www.texture as Texture;
        }
    }

    private void GetTextureByBestHttp(string thumbPath, GameObject item)
    {
        if (string.IsNullOrEmpty(thumbPath)) return;
        var request = new HTTPRequest(new System.Uri(thumbPath), ImageDownloaded);
        // Send out the request
        request.Tag = item;
        request.Send();

        mDownloadingIcon++;
    }

    /// <summary>
    /// Callback function of the image download http requests
    /// </summary>
    void ImageDownloaded(HTTPRequest req, HTTPResponse resp)
    {
        // Increase the finished count regardless of the state of our request

        bool downloadTexture = false;
        switch (req.State)
        {
            // The request finished without any problem.
            case HTTPRequestStates.Finished:
                if (resp.IsSuccess)
                {
                    // Load the texture
                    //tex.LoadImage(resp.Data);

                    //PosterItem item = req.Tag as PosterItem;
                    //if (item.VideoModel.VideoModelThumbPath == req.Uri.AbsoluteUri)
                    //{
                    //    UITexture textureIcon = item.mIconFrame;
                    //    textureIcon.transform.localRotation = Quaternion.identity;
                    //    textureIcon.mainTexture = resp.DataAsTexture2D;
                    //}

                    GameObject gameObject = req.Tag as GameObject;

                    if (gameObject != null)
                    {
                        GameObject textureObject = gameObject.transform.Find("texture").gameObject;
                        if (textureObject != null)
                        {
                            UITexture icon = textureObject.GetComponent<UITexture>();
                            if (icon != null)
                            {
                                icon.mainTexture = resp.DataAsTexture2D;
                            }
                        }


                        //把下载下来的texture保存到GlobalPhotoData
                        List<IconTexture> IconTextureList = GlobalPhotoData.Instance.GetIconTextureList();
                        IconTexture IconTexture = new IconTexture();
                        IconTexture.index = int.Parse(gameObject.name.Split('_')[1]);
                        IconTexture.texture = resp.DataAsTexture2D;
                        IconTextureList.Add(IconTexture);

                        downloadTexture = true;
                    }



                    //释放上一个Texture
                    //Resources.UnloadAsset(tex);



                    // Update the cache-info variable
                    //allDownloadedFromLocalCache = allDownloadedFromLocalCache && resp.IsFromCache;
                }
                else
                {
                    Debug.LogWarning(string.Format("Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}",
                                                   resp.StatusCode,
                                                   resp.Message,
                                                   resp.DataAsText));
                }
                break;

            // The request finished with an unexpected error. The request's Exception property may contain more info about the error.
            case HTTPRequestStates.Error:
                Debug.LogError("Request Finished with Error! " + (req.Exception != null ? (req.Exception.Message + "\n" + req.Exception.StackTrace) : "No Exception"));
                break;

            // The request aborted, initiated by the user.
            case HTTPRequestStates.Aborted:
                Debug.LogWarning("Request Aborted!");
                break;

            // Ceonnecting to the server is timed out.
            case HTTPRequestStates.ConnectionTimedOut:
                Debug.LogError("Connection Timed Out!");
                break;

            // The request didn't finished in the given time.
            case HTTPRequestStates.TimedOut:
                Debug.LogError("Processing the request Timed Out!");
                break;
        }


        if (downloadTexture == false)
        {
            GameObject gameObject = req.Tag as GameObject;
            if (gameObject != null)
            {
                int index = int.Parse(gameObject.name.Split('_')[1]);
            }

            mDownloadIconFailed++;
        }


        //先不管成功与否，只要返回了就执行
        mDownloadingIcon--;
        if (mDownloadingIcon > 0)
            mLoading.SetActive(true);
        if (mDownloadingIcon == 0)
        {

            if (mLoading.activeSelf)
                mLoading.SetActive(false);
            if (OnAllIconDownloaded != null)
            {
                bool success = mDownloadIconFailed > 0 ? false : true;
                OnAllIconDownloaded(success);
            }
        }

    }
}