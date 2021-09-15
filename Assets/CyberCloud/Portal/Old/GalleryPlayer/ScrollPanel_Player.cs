using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BestHTTP;
using DG.Tweening;

public class ScrollPanel_Player : MonoBehaviour {

    public GalleryPlayerManager mPlayerManager;
    public GameObject mBarThumb;
    public GameObject mLeftSprite;
    public GameObject mRightSprite;

    public PlayerToast mPlayerToast;
    public GameObject mIconTemplate;  //列表Icon的Prefab
    public GameObject mListLoading;


    public delegate void RequestCallback( bool success = false );
    public static RequestCallback OnPageRequestFinished;
    public static RequestCallback OnAllIconDownloaded;

    GalleryNetApi mNetAPI = null;
    private string mMaxLimitEachPage = "5";
    private int mColumnsPerPage = 5;
    private int mTotalPage;
    private int mHavePage;
    private int mCurrentPage;
    private int mIndexInPage;

    List<PhotoModel> mPhotoModelList;
    List<IconTexture> mIconTextureList;

    private bool AcceptKeyEvent = false;
    private float PageMoveDuration = 1.0f;
    private float FadeAnimationDuration = 1.0f;
    private UISprite mBarThumbSprite = null;

    private int mDownloadingIcon = 0;
    private int mDownloadIconFailed = 0;
    private bool mPageTransiting = false;  //目前只用在OnAllIconDownloaded_First
    private bool mInitialized = false;


    private List<GameObject> mItemList = null;
    private float mAngleInterval = 14.0f;
    private Vector3 mAxisCenter = new Vector3(0.0f, 0.0f, -7.0f);
    private float mRadius = 153.0f;
    private Vector3 TargetEulerAngle = new Vector3();
    private Vector3 TargetPos = new Vector3();



    //只用于mainphoto加载的过程中不允许再点击IconButton
    private bool mIconButtonAccept = false;

    public bool IconButtonAccept
    {
        get {
            return mIconButtonAccept;
        }
        set {
            mIconButtonAccept = value;
        }
    }

    
    public enum PageDirection
    {
        Left,
        Right
    }


	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    void OnDestroy()
    {
        ClearIconData();
    }


    void OnEnable()
    {
       // InputManager.OnLeft += OnLeft;
        //InputManager.OnRight += OnRight;
    }

    void OnDisable()
    {
        //InputManager.OnLeft -= OnLeft;
        //InputManager.OnRight -= OnRight;
    }


    private void OnLeft()
    {
        TransitPage(PageDirection.Left);
    }

    private void OnRight()
    {
        TransitPage(PageDirection.Right);
    }




    public bool CheckIsInitialized()
    {
        if (mInitialized == false)
        {
            WingToastManager.Instance.Show(Localization.Get("Home_NoNet"));
            //mPlayerToast.Show(Localization.Get("Home_NoNet"));
        }
        return mInitialized;
    }


    public bool CheckIsRequesting()
    {
        bool ret = false;
        if (mNetAPI != null)
        {
            if (mNetAPI.HavePendingRequest() == true)
            {
                Debug.Log("CheckIsRequesting : HavePendingRequest");
                ret = true;
            }
        }


        if (mDownloadingIcon > 0)
        {
            Debug.Log("CheckIsRequesting : mDownloadingIcon > 0");
            ret = true;
        }
        return ret;
    }


    public void ResetDownloadCounting()
    {
        mDownloadingIcon = 0;
        mDownloadIconFailed = 0;
    }





    public void InitBase()
    {
        mNetAPI = new GalleryNetApi();
        GalleryNetApi.OnDataCallbackDelegate += OnDataCallback_Player;

        mPhotoModelList = GlobalPhotoData.Instance.GetPhotoList();
        mIconTextureList = GlobalPhotoData.Instance.GetIconTextureList();




        int have = mPhotoModelList.Count;
        int index = GlobalPhotoData.Instance.mCurrentPhotoIndex;


        if (have % mColumnsPerPage == 0)
        {
            mHavePage = have / mColumnsPerPage;
        }
        else
        {
            mHavePage = have / mColumnsPerPage + 1;
        }

        mCurrentPage = (index / mColumnsPerPage) + 1;
        mIndexInPage = index % mColumnsPerPage;


        //一进场景为了获得mTotalPage等数据发请求
        RequestFirst();
    }


    public void DestroyBase()
    {
        GalleryNetApi.OnDataCallbackDelegate -= OnDataCallback_Player;
    }


    public void SendNextPageRequest(string page)
    {
        //mNetAPI.GetCategoryPhoto(GalleryNetApi.funGetCategoryPhoto,
        //    GlobalPhotoData.Instance.mCurrentCategoryID, mMaxLimitEachPage, page);
    }



    private void RequestFirst()
    {
        ScrollPanel_Player.OnPageRequestFinished += OnPageRequestFinished_First;
        ScrollPanel_Player.OnAllIconDownloaded += OnAllIconDownloaded_First;

        int page = mCurrentPage;
        SendNextPageRequest(page.ToString());
    }


    public void OnPageRequestFinished_First(bool success = false)
    {
        
        ScrollPanel_Player.OnPageRequestFinished -= OnPageRequestFinished_First;
        if (success == true)
        {
            InitIconData();
            mInitialized = true;
        }
        else
        {
            WingToastManager.Instance.Show(Localization.Get("Home_NoNet"));
            //mPlayerToast.Show(Localization.Get("Home_NoNet"));
        }

    }

    public void OnAllIconDownloaded_First(bool success = false)
    {
        
        ScrollPanel_Player.OnAllIconDownloaded -= OnAllIconDownloaded_First;
        if (mPageTransiting == true)
        {
            mPageTransiting = false;
            Invoke("ResetAcceptKeyEvent", Mathf.Max(PageMoveDuration, FadeAnimationDuration) + 0.1f);
            Invoke("UpdateEventResponseForAllItems", Mathf.Max(PageMoveDuration, FadeAnimationDuration));
        }
        else
        {
            Invoke("ResetAcceptKeyEvent", 0.1f);
            Invoke("UpdateEventResponseForAllItems", 0.1f);
        }

    }






    private void ClearIconData()
    {
        StopAllCoroutines();

        if (mItemList != null)
        {
            while (mItemList.Count > 0)
            {
                GameObject itemToDelete = mItemList[0];
                GameObject.DestroyImmediate(itemToDelete);
                itemToDelete = null;
                mItemList.RemoveAt(0);
            }
            mItemList.Clear();
            mItemList = null;
        }
    }




    private void InitIconData()
    {
        //重置图片下载计数
        ResetDownloadCounting();


        mItemList = new List<GameObject>();
        InitItems(mPhotoModelList.Count);
        
        
        string title = "";
        for (int i = 0; i < mPhotoModelList.Count; i++)
        {
            //设置文本
            title = mPhotoModelList[i].Title;
            mItemList[i].transform.Find("Label").gameObject.GetComponent<UILabel>().text = title;


            mItemList[i].GetComponent<PanelIconButton>().ListIndex = i;
            mItemList[i].GetComponent<PanelIconButton>().MID = mPhotoModelList[i].MID;
            //按钮事件响应
            UIEventListener.Get(mItemList[i]).onClick = OnIconButtonClick;
            UIEventListener.Get(mItemList[i]).onHover = OnIconButtonHover;


            //设置贴图
            IconTexture icon = mIconTextureList.Find(
                    delegate(IconTexture it)
                    {
                        return it.index == i;
                    });

            if (icon == null)
            {
                Debug.Log("InitIconData Index : " + i + "  mIconTextureList does not have the texture !");

                //发送下载图片请求
                string coverLink = mPhotoModelList[i].CoverLink;
                GetTextureByBestHttp(coverLink, mItemList[i]);
            }
            else
            {
                mItemList[i].GetComponent<UITexture>().mainTexture = icon.texture;
            }

        }


        UpdateScrollBarState();
        InitButtons();
        UpdateIconArrayPick();


        //下载图片完成前不准点击
        MakeAllItemsDisabled();
        if (mDownloadingIcon == 0)
        {
            if (OnAllIconDownloaded != null)
            {
                OnAllIconDownloaded(true);
            }
        }
        
    }



    private void AddIconData()
    {
        //重置图片下载计数
        ResetDownloadCounting();


        string title = "";
        for (int i = mItemList.Count; i < mPhotoModelList.Count; i++)
        {
            mItemList.Add(InstantiateItemByIndex(i));


            //设置文本
            title = mPhotoModelList[i].Title;
            mItemList[i].transform.Find("Label").gameObject.GetComponent<UILabel>().text = title;


            mItemList[i].GetComponent<PanelIconButton>().ListIndex = i;
            mItemList[i].GetComponent<PanelIconButton>().MID = mPhotoModelList[i].MID;
            //按钮事件响应
            UIEventListener.Get(mItemList[i]).onClick = OnIconButtonClick;
            UIEventListener.Get(mItemList[i]).onHover = OnIconButtonHover;


            //设置贴图
            IconTexture icon = mIconTextureList.Find(
                    delegate(IconTexture it)
                    {
                        return it.index == i;
                    });

            if (icon == null)
            {
                //理论上这时候的图片都需要下载
                Debug.Log("AddIconData Index : " + i + "  mIconTextureList does not have the texture !");

                //发送下载图片请求
                string coverLink = mPhotoModelList[i].CoverLink;
                GetTextureByBestHttp(coverLink, mItemList[i]);
            }
            else
            {
                mItemList[i].GetComponent<UITexture>().mainTexture = icon.texture;
            }

        }
    }


    #region 生成Icon用的函数

    private void InitItems(int count)
    {
        for (int i = 0; i < count; i++)
        {
            mItemList.Add(InstantiateItemByIndex(i));
        }
    }

    private GameObject InstantiateItemByIndex(int index)
    {
        GameObject go = GameObject.Instantiate((Object)mIconTemplate, this.gameObject.transform.position, this.gameObject.transform.rotation) as GameObject;


        go.name = "Item_" + index;
        go.transform.parent = this.gameObject.transform;
        go.transform.localScale = this.gameObject.transform.localScale;


        go.transform.localRotation = CalculateItemRotationByIndex(index);
        go.transform.localPosition = CalculateItemPositionByAngleY(go.transform.localRotation.eulerAngles.y);
        go.GetComponent<PanelIconButton>().OriginPos = go.transform.localPosition;


        int indexInPage = index % mColumnsPerPage;
        int nowPage = (index / mColumnsPerPage) + 1;
        if (nowPage == mCurrentPage)
        {
        }
        else if (nowPage < mCurrentPage)
        {
            go.GetComponent<UITexture>().alpha = 0.0f;
            go.SetActive(false);
        }
        else
        {
            go.GetComponent<UITexture>().alpha = 0.0f;
            go.SetActive(false);
        }


        return go;
    }


    private Quaternion CalculateItemRotationByIndex(int index)
    {
        int indexInPage = index % mColumnsPerPage;
        float startAngle = -28.0f;
        float angleY = 0;
        Quaternion quaternion = new Quaternion();


        int nowPage = (index / mColumnsPerPage) + 1;
        if (nowPage == mCurrentPage)
        {
        }
        else if (nowPage < mCurrentPage)
        {
            startAngle -= mAngleInterval * 5.0f;
        }
        else
        {
            startAngle += mAngleInterval * 5.0f;
        }

        angleY = startAngle + mAngleInterval * indexInPage;
        quaternion.eulerAngles = new Vector3(0, angleY, 0);
        return quaternion;
    }



    private Vector3 CalculateItemPositionByAngleY(float angleY)
    {
        float x = Mathf.Sin(angleY * Mathf.Deg2Rad) * mRadius;
        float z = Mathf.Cos(angleY * Mathf.Deg2Rad) * mRadius;

        Vector3 position = new Vector3(0.0f, 0.0f, 0.0f);
        position.x = mAxisCenter.x + x;
        position.z = mAxisCenter.z + z;

        return position;
    }


    #endregion




    private void UpdateIconArrayPick()
    {
        GameObject bg = null;
        int index = 0;
        for (int i = 0; i < mItemList.Count; i++)
        {
            PanelIconButton btn = mItemList[i].GetComponent<PanelIconButton>();
            bg = btn.mBg;
            //bg = mItemList[i].transform.FindChild("Bg").gameObject;
            if (bg != null)
            {
                index = btn.ListIndex;
                if (index == GlobalPhotoData.Instance.mCurrentPhotoIndex)
                {
                    bg.SetActive(false);
                }
                else
                {
                    bg.SetActive(true);
                }
            }
        }

    }





    private void UpdateScrollBarState()
    {
        if (mBarThumb != null)
        {
            mBarThumbSprite = mBarThumb.GetComponent<UISprite>();
            mBarThumbSprite.width = System.Convert.ToInt32(40 / mTotalPage);

            Vector3 pos = mBarThumbSprite.transform.localPosition;
            pos.x = pos.x + (mBarThumbSprite.width * (mCurrentPage - 1));
            mBarThumbSprite.transform.localPosition = pos;
        }
    }


    private void AdjustScrollBarPosition(PageDirection direction)
    {
        if (mBarThumbSprite != null)
        {
            if (direction == PageDirection.Left)
            {
                mBarThumbSprite.transform.DOLocalMoveX(mBarThumbSprite.transform.localPosition.x - mBarThumbSprite.width, PageMoveDuration, false);
            }
            else
            {
                mBarThumbSprite.transform.DOLocalMoveX(mBarThumbSprite.transform.localPosition.x + mBarThumbSprite.width, PageMoveDuration, false);
            }
        }
    }





    private void InitButtons()
    {
        if (mLeftSprite != null)
        {
            UIEventListener.Get(mLeftSprite).onClick = OnButtonClick;
        }

        if (mRightSprite != null)
        {
            UIEventListener.Get(mRightSprite).onClick = OnButtonClick;
        }
    }


    private void OnButtonClick(GameObject obj)
    {
        Debug.Log("OnButtonClick : " + obj.name);


        if (obj == mLeftSprite)
        {
            TransitPage(PageDirection.Left);
        }
        else if (obj == mRightSprite)
        {
            TransitPage(PageDirection.Right);
        }
    }


    private void OnIconButtonClick(GameObject obj)
    {
        if (IconButtonAccept)
        {
            Debug.Log("OnIconButtonClick : " + obj.name);

            int index = obj.GetComponent<PanelIconButton>().ListIndex;
            Debug.Log(obj.name + " ListIndex : " + index.ToString());
            if (mPlayerManager != null)
            {
                if (mPlayerManager.ChangeMainPhotoCover(index) == true)
                {
                    GlobalPhotoData.Instance.mCurrentPhotoIndex = index;
                    UpdateIconArrayPick();



                    IconButtonAccept = false;
#if UNITY_EDITOR
                    mPlayerManager.StartDownloadMainPhoto(index);
#elif UNITY_ANDROID

                string mid = obj.GetComponent<PanelIconButton>().MID;
                mPlayerManager.DownLoadMainPhoto_MID(mid);
#endif
                }
                else
                {
                    Debug.LogError(obj.name + " ChangeMainPhotoCover Failed !");
                }
            }


        }
    }

    private void OnIconButtonHover(GameObject obj,bool status)
    {
        Debug.Log("OnIconButtonHover : " + obj.name);

        PanelIconButton btn = obj.GetComponent<PanelIconButton>();
        int index = btn.ListIndex;
        Debug.Log(obj.name + " ListIndex : " + index.ToString());
        btn.SetSelectActive(status);
        Vector3 pos = btn.OriginPos;
        float x = Mathf.Sin(obj.transform.localRotation.eulerAngles.y * Mathf.Deg2Rad) * 3;
        obj.transform.localPosition = new Vector3(status ? pos.x - x : pos.x, pos.y, status ? pos.z - 3 : pos.z);
    }


    private void TransitPage(PageDirection direction, bool isCallback = false)
    {
        if (AcceptKeyEvent)
        {
            if (direction == PageDirection.Left)
            {
                if (mCurrentPage == 1)
                {
                    Debug.Log("Reached the left-most page!");
                    mPlayerToast.Show(Localization.Get("Bar_First"));
                    return;
                }
            }
            else
            {
                if (mCurrentPage >= mTotalPage)
                {
                    Debug.Log("Reached right-most page!");
                    mPlayerToast.Show(Localization.Get("Bar_Last"));
                    return;
                }
            }

            AcceptKeyEvent = false;
            if (direction == PageDirection.Left)
            {
                mCurrentPage--;
            }
            else
            {
                mCurrentPage++;

                bool needRequest = false;
                if (mCurrentPage > mHavePage)
                {
                    needRequest = true;
                }
                else if (mCurrentPage == mHavePage)
                {
                    if (mCurrentPage < mTotalPage)
                    {
                        int shouldHave = mCurrentPage * mColumnsPerPage;
                        if (mPhotoModelList.Count < shouldHave)
                        {
                            needRequest = true;
                        }
                    }

                }


                if (needRequest)
                {
                    //请求新数据
                    mCurrentPage--;

                    //只有当请求完成才能继续操作
                    //AcceptKeyEvent = true;
                    MakeAllItemsDisabled();
                    RequestNextPage();
                    return;
                }
            }


            MakeAllItemsDisabled();

            if (isCallback == false)
            {
                mPageTransiting = true;
                ScrollPanel_Player.OnAllIconDownloaded += OnAllIconDownloaded_First;

                //检查贴图
                ResetDownloadCounting();
                IconTextureCheck();
            }

            foreach (GameObject obj in mItemList)
            {
                ItemMove(obj, direction);
            }
           

            AdjustScrollBarPosition(direction);
            //UpdateIconArrayPick();    //这个时候应该不需要了


            if (mDownloadingIcon == 0)
            {
                if (OnAllIconDownloaded != null)
                {
                    OnAllIconDownloaded(true);
                }
            }


        }
    }






    private void IconTextureCheck()
    {
        int indexInPage = 0;
        int nowPage = 0;
        //顺序应该就是索引
        for (int i = 0; i < mItemList.Count; i++)
        {
            indexInPage = i % mColumnsPerPage;
            nowPage = (i / mColumnsPerPage) + 1;
            if (nowPage == mCurrentPage)
            {
                IconTexture icon = mIconTextureList.Find(
                        delegate(IconTexture it)
                        {
                            return it.index == i;
                        });

                if (icon == null)
                {
                    Debug.LogError("startIndex : " + i + "  mIconTextureList does not have the texture !");

                    //发送下载图片请求
                    string coverLink = mPhotoModelList[i].CoverLink;
                    GetTextureByBestHttp(coverLink, mItemList[i]);
                }
                else
                {
                    mItemList[i].GetComponent<UITexture>().mainTexture = icon.texture;
                }

            }
        }

 
    }




    private void ItemMove(GameObject obj, PageDirection direction)
    {
        if (obj != null)
        {
            int index = int.Parse(obj.name.Split('_')[1]);
            int nowPage = (index / mColumnsPerPage) + 1;


            if (direction == PageDirection.Left)
            {
                if (nowPage == mCurrentPage)
                {
                    MoveAroundCircle(obj, mAngleInterval * 5.0f);
                    TweenAlpha.Begin(obj, PageMoveDuration, 1.0f);

                    obj.GetComponent<BoxCollider>().enabled = true;
                    obj.SetActive(true);
                }
                else if (nowPage == mCurrentPage + 1)
                {
                    MoveAroundCircle(obj, mAngleInterval * 5.0f);
                    TweenAlpha.Begin(obj, PageMoveDuration, 0.0f);
                    StartCoroutine(DelayToSetActionFalse(obj));
                }
                else
                {
                    obj.SetActive(false);
                }


            }
            else
            {
                if (nowPage == mCurrentPage)
                {
                    MoveAroundCircle(obj, -mAngleInterval * 5.0f);
                    TweenAlpha.Begin(obj, PageMoveDuration, 1.0f);

                    obj.GetComponent<BoxCollider>().enabled = true;
                    obj.SetActive(true);
                }
                else if (nowPage == mCurrentPage - 1)
                {
                    MoveAroundCircle(obj, -mAngleInterval * 5.0f);
                    TweenAlpha.Begin(obj, PageMoveDuration, 0.0f);
                    StartCoroutine(DelayToSetActionFalse(obj));
                }
                else
                {
                    obj.SetActive(false);
                }

            }

        }
    }


    private void MoveAroundCircle(GameObject obj, float angleChange)
    {
        float oldAngleY = obj.transform.localRotation.eulerAngles.y;
        float targetAngleY = oldAngleY + angleChange;
        PanelIconButton btn = obj.GetComponent<PanelIconButton>();
        //float x = Mathf.Sin(targetAngleY * Mathf.Deg2Rad) * mRadius;
        //float z = Mathf.Cos(targetAngleY * Mathf.Deg2Rad) * mRadius;

        //TargetPos = obj.transform.localPosition;
        //TargetPos.x = mAxisCenter.x + x;
        //TargetPos.z = mAxisCenter.z + z;
        //obj.transform.DOLocalMove(TargetPos, PageMoveDuration, false);
        List<Vector3> pathVector = new List<Vector3> { };
        for (int i = 0; i < 6; i++)
        {
            Vector3 middlePos = obj.transform.localPosition;
            float MiddleAngleY = oldAngleY + angleChange / 5 * i;
            middlePos.x = mAxisCenter.x + Mathf.Sin(MiddleAngleY * Mathf.Deg2Rad) * mRadius;
            middlePos.z = mAxisCenter.z + Mathf.Cos(MiddleAngleY * Mathf.Deg2Rad) * mRadius;
            pathVector.Add(middlePos);
            if (i == 5) btn.OriginPos = middlePos;
        }
        obj.transform.DOLocalPath(pathVector.ToArray(), PageMoveDuration, PathType.CatmullRom, PathMode.Full3D);

        TargetEulerAngle = obj.transform.rotation.eulerAngles;
        TargetEulerAngle.y = targetAngleY;
        obj.transform.DOLocalRotate(TargetEulerAngle, PageMoveDuration);
    }

    IEnumerator MoveZ(GameObject obj,float angleChange)
    {
        float duration = PageMoveDuration;
        while (true)
        {



        }
        yield return 0;
    }





    private void RequestNextPage()
    {
        ScrollPanel_Player.OnPageRequestFinished += OnPageRequestFinished_Panel;
        mPageTransiting = true;
        ScrollPanel_Player.OnAllIconDownloaded += OnAllIconDownloaded_First;

        int page = mCurrentPage + 1;
        SendNextPageRequest(page.ToString());
    }


    public void OnPageRequestFinished_Panel(bool success = false)
    {
        ScrollPanel_Player.OnPageRequestFinished -= OnPageRequestFinished_Panel;

        if (success == true)
        {
            int have = mPhotoModelList.Count;
            if (have % mColumnsPerPage == 0)
            {
                mHavePage = have / mColumnsPerPage;
            }
            else
            {
                mHavePage = have / mColumnsPerPage + 1;
            }


            //创建新的Icon
            AddIconData();


            ResetAcceptKeyEvent();
            TransitPage(PageDirection.Right, true);
        }
        else
        {
            Invoke("ResetAcceptKeyEvent", Mathf.Max(PageMoveDuration, FadeAnimationDuration) + 0.1f);
            Invoke("UpdateEventResponseForAllItems", Mathf.Max(PageMoveDuration, FadeAnimationDuration));
        }

    }




    /// <summary>
    /// Make all items disabled.This method is called before items move.
    /// If we don't call this method before items move,when items move away
    /// out of visible area,it can also receive hover event and play sound.
    /// </summary>
    private void MakeAllItemsDisabled()
    {
        foreach (GameObject obj in mItemList)
        {
            obj.GetComponent<BoxCollider>().enabled = false;
        }
    }


    /// <summary>
    /// Even items are invisible in the panel,they can receive hover event and play sound.
    /// So we need to disable their collider when items are invisible.
    /// </summary>
    private void UpdateEventResponseForAllItems()
    {
        foreach (GameObject obj in mItemList)
        {
            obj.GetComponent<BoxCollider>().enabled = true;
        }
    }


    private void ResetAcceptKeyEvent()
    {
        AcceptKeyEvent = true;
    }


    public IEnumerator DelayToSetActionFalse(GameObject obj)
    {
        yield return new WaitForSeconds(PageMoveDuration);
        obj.SetActive(false);
    }




    /// <summary>
    /// On the data callback, the callback when request finshed
    /// </summary>
    /// <param name="req">Req.</param>
    /// <param name="resp">Resp.</param>
    public void OnDataCallback_Player(HTTPRequest req, HTTPResponse resp)
    {
        bool dispatchSuccess = false;
        string key = req.Tag as string;
        switch (req.State)
        {
            // The request finished without any problem.
            case HTTPRequestStates.Finished:
                if (!resp.IsSuccess)
                {
                    Debug.LogWarning("OnDataCallback_Player-> statusCode= " + resp.StatusCode + " Message= " + resp.Message + "result= " + resp.DataAsText);
                }

                dispatchSuccess = DispatchResult_Player(key, resp.DataAsText);
                break;

            // The request finished with an unexpected error. The request's Exception property may contain more info about the error.
            case HTTPRequestStates.Error:
                Debug.LogWarning("OnDataCallback_Player->Request Finished with Error! " + (req.Exception != null ? (req.Exception.Message + "\n" + req.Exception.StackTrace) : "No Exception"));

                break;
            // The request aborted, initiated by the user.
            case HTTPRequestStates.Aborted:
                Debug.LogWarning("OnDataCallback_Player->Request Aborted!");

                break;
            // Ceonnecting to the server is timed out.
            case HTTPRequestStates.ConnectionTimedOut:
                Debug.LogWarning("OnDataCallback_Player->Connection Timed Out!");

                break;
            // The request didn't finished in the given time.
            case HTTPRequestStates.TimedOut:
                Debug.LogWarning("OnDataCallback_Player->Processing the request Timed Out!");
                WingToastManager.Instance.Show(Localization.Get("Home_Time_Out"));
                break;
            default:
                Debug.LogError("OnDataCallback_Player->Unknown Error!");
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
    /// DispatchResult_Player     	
    /// </summary>
    /// <param name="key">Key.</param>
    /// <param name="result">Result.</param>
    /// <param name="msg">Message.</param>
    bool DispatchResult_Player(string key, string result)
    {
        Debug.Log("DispatchResult_Player->this is callback of key : " + key);
        bool ret = false;
        int message = 0;
        int pageRef = 0;

        switch (key)
        {

            case GalleryNetApi.funGetCategoryPhoto:

                List<PhotoModel> newList = JsonUtils.ParsePhotoJson(result, ref message, ref mTotalPage, ref pageRef);

                if (newList == null)
                {
                    Debug.LogError("DispatchResult funGetCategoryPhoto newList is null !");
                }
                else
                {
                    int oldCount = mPhotoModelList.Count;
                    //int getNum = mCurrentPage * mColumnsPerPage;
                    int getNum = (pageRef - 1) * mColumnsPerPage + newList.Count;

                    //添加新数据
                    if (getNum > oldCount)
                    {
                        int addNum = getNum - oldCount;

                        for (int i = newList.Count - addNum; i < newList.Count; i++)
                        {
                            mPhotoModelList.Add( newList[i] );
                        }
                    }

                    //销毁newList
                    for (int i = 0; i < newList.Count; i++)
                    {
                        newList.RemoveAt( i );
                    }
                    newList.Clear();

                    ret = true;
                }
                    

                break;

            default:
                Debug.LogError("DispatchResult_Player : not have a key case !");
                break;
        }

        return ret;
    }




    //下载列表图片用
    private void GetTextureByBestHttp(string thumbPath, GameObject item)
    {
        if (string.IsNullOrEmpty(thumbPath)) return;
        var request = new HTTPRequest(new System.Uri(thumbPath), ImageDownloaded);
        // Send out the request
        request.Tag = item;
        request.Send();


        if (mDownloadingIcon == 0)
        {
            mListLoading.SetActive(true);
        }
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

                    GameObject gameObject = req.Tag as GameObject;
                    if (gameObject != null)
                    {
                        UITexture icon = gameObject.GetComponent<UITexture>();
                        if (icon != null)
                        {
                            icon.mainTexture = resp.DataAsTexture2D;
                        }

                        //把下载下来的texture保存到GlobalPhotoData
                        IconTexture IconTexture = new IconTexture();
                        IconTexture.index = gameObject.GetComponent<PanelIconButton>().ListIndex;
                        IconTexture.texture = resp.DataAsTexture2D;
                        mIconTextureList.Add(IconTexture);

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
                UITexture icon = gameObject.GetComponent<UITexture>();
                if (icon != null)
                {
                    //设置成默认图
                    icon.mainTexture = gameObject.GetComponent<PanelIconButton>().mDefaultTexture;
                }

                int index = gameObject.GetComponent<PanelIconButton>().ListIndex;
            }

            mDownloadIconFailed++;
        }


        //先不管成功与否，只要返回了就执行
        mDownloadingIcon--;
        if (mDownloadingIcon == 0)
        {
            mListLoading.SetActive(false);
            if (OnAllIconDownloaded != null)
            {
                bool success = mDownloadIconFailed > 0 ? false : true;
                OnAllIconDownloaded(success);
            }
        }

    }

}
