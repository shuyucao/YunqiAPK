using BestHTTP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GalleryPlayerScreen : ScreenBase, IMsgHandle
{
    #region data
    public GameObject mScreen;
    public GameObject mBtnList;
    public GameObject mBtnToggle;
    public GameObject mListBack;
    public GameObject mLoading;
    public CircleScrolleView mScrollePage;
    public GameObject m2DTexture;
    public GameObject mReFreshPage;
    public GameObject mRefreshBtn;
    public CircleScrolleBar mRefBar;
    Texture2D tempTexture;
    public GameObject mTitle;
    string title;

    private string mLocalMainPhotoPath;
    private PanoramaBall mPanoramaBall;
    private PhotoModelBase mCurentPhoto = null;
    private bool IsLoadSuccess = false;
    private PlayerType mPlayerType = PlayerType.OnLineList;
    List<string> formatSupport = new List<string>() { "png", "bmp", "jpg", "jpeg" };
    #endregion 

    #region init 
    void Awake()
    {
        MsgManager.Instance.RegistMsg(MsgID.PlayNextImage, this);
        MsgManager.Instance.RegistMsg(MsgID.PlayLastImage, this);
        MsgManager.Instance.RegistMsg(MsgID.NetWorkNotAvaillable, this);
    }

    public override void OprateChangeScreen(Bundle bundle)
    {
        if (mPanoramaBall == null) InitScreen();

        m2DTexture.SetActive(false);
        mReFreshPage.SetActive(false);


        InitButtons();

       // InputManager.OnBack += OnBack;
        mScrollePage.itemClick += ReLoadPhoto;
        GalleryTools.ShowLeftBar(false);
        if (bundle == null)
        {
            Debug.LogError("lack some needed elements!");
            //CommonAlert.Show("Player_DownloadPhoto_Failed", true);
            ScreenManager.Instance.CloseScreen(this);
        }
        else
        {
            PhotoModelBase data = bundle.GetValue<PhotoModelBase>("data");
            PlayerType ty = bundle.GetValue<PlayerType>("playertype");
            StartPlayerByType(data, ty);
        }
       // InputManager.OnClickLeft += OnLeft;
       // InputManager.OnClickRight += OnRight;

        SliderController.OnLeft_Whole += OnLeft;
        SliderController.OnRight_Whole += OnRight;
    }

    public override void OprateCloseScreen()
    {
        //SetThreadStop();
        //InputManager.OnBack -= OnBack;
        //InputManager.OnClickLeft -= OnLeft;
       // InputManager.OnClickRight -= OnRight;

        SliderController.OnLeft_Whole -= OnLeft;
        SliderController.OnRight_Whole -= OnRight;
        if (this != null)
        {
            Destroy(gameObject);
            Destroy(mPanoramaBall.gameObject);
        }
    }

    public void HandleMessage(MsgID id, Bundle bundle)
    {
        if (id == MsgID.PlayNextImage)
        {
            PlayNextPhoto(true);
        }
        else if (id == MsgID.PlayLastImage)
        {
            PlayNextPhoto(false);
        }
        else if (id == MsgID.NetWorkNotAvaillable)
        {
            CheckNetState();
        }
    }

    private void StartPlayerByType(PhotoModelBase data, PlayerType type)
    {
        mPlayerType = type;
        switch (type)
        {
            case PlayerType.OnLineList:
                mBtnList.SetActive(true);
                mBtnToggle.SetActive(false);
                mScrollePage.gameObject.SetActive(true);
                InitPhotoData(data, false);
                break;
            case PlayerType.Local:
                mBtnList.SetActive(false);
                mBtnToggle.SetActive(true);
                mScrollePage.gameObject.SetActive(false);
                InitPhotoData(data, true);
                break;
            case PlayerType.OnLineOneImg:
            case PlayerType.Recommend:
                mBtnList.SetActive(false);
                mBtnToggle.SetActive(false);
                mBtnToggle.SetActive(false);
                mScrollePage.gameObject.SetActive(false);
                InitPhotoData(data, false);
                break;
            default:
                break;
        }
    }

    private void InitPhotoData(PhotoModelBase data, bool islocal)
    {
        InitScrollePage();
        StartLoadPhoto(data, islocal);
    }

    private void InitScrollePage()
    {
        if (mPlayerType == PlayerType.OnLineList)
        {
            mScrollePage.Init();
        }
    }

    private void PlayNextPhoto(bool isnext)
    {
        if (mPlayerType == PlayerType.OnLineList)
        {
            PhotoModel data = CachePhotoData.Instance.GetNextPhoto(isnext);
            if (data != null)
            {
                ReLoadPhoto(data);
            }
            else
            {
                if (isnext)
                {
                    CommonAlert.Show("Bar_Last", false, null, false);
                }
                else
                {
                    CommonAlert.Show("Bar_First", false, null, false);
                }
            }
        }
        else if(mPlayerType == PlayerType.Local)
        {
            if (isnext)
            {
                if(CachePhotoData.Instance.localIndex +1 == CachePhotoData.Instance.PhotosList.Count)
                {
                    //toast
                    CommonAlert.Show("Bar_Last", false, null, false);
                }
                else
                {
                    CachePhotoData.Instance.localIndex++;
                    ReLoadPhoto((PhotoModelBase) CachePhotoData.Instance.PhotosList[CachePhotoData.Instance.localIndex]);
                }
            }
            else
            {
                if (CachePhotoData.Instance.localIndex == 0)
                {
                    //toast
                    CommonAlert.Show("Bar_First", false, null, false);
                }
                else
                {
                    CachePhotoData.Instance.localIndex--;
                    ReLoadPhoto((PhotoModelBase)CachePhotoData.Instance.PhotosList[CachePhotoData.Instance.localIndex]);
                }

            }
        }
    }

    private void StartLoadPhoto(PhotoModelBase data, bool islocal)
    {
        ClearLoadState();
        if (!islocal)
        {
            string[] links = (data as PhotoModel).PhotoLink.Split('.');
            string format = links[links.Length - 1];
            if (!formatSupport.Contains(format.ToLower()))
            {
                Debug.LogError("not support !!!");
                mLoading.SetActive(false);
                ShowUI(false);
                mPanoramaBall.SetTextureAndDestroyOld(null, PhotoTextureType.Photo);
                CommonAlert.Show("Player_LocalPhoto_Failed", true);
                return;
            }
        }
        SetLoadingClock();
        StartCoroutine(StartLoading(islocal));
        StartCoroutine(ChangeMainPhotoCover(data, islocal));
    }

    private void ClearLoadState()
    {
        IsLoadSuccess = false;
        mReFreshPage.SetActive(false);
        StopAllCoroutines();
        SetThreadStop();
    }

    private void ReLoadPhoto(PhotoModelBase data)
    {
        Debug.Log("reload photo!");
        switch (mPlayerType)
        {
            case PlayerType.OnLineList:
                StartLoadPhoto(data, false);
                break;
            case PlayerType.Local:
                StartLoadPhoto(data, true);
                break;
            case PlayerType.OnLineOneImg:
            case PlayerType.Recommend:
                StartLoadPhoto(data, false);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 20s后弹出相应提示
    /// </summary>
    private void SetLoadingClock()
    {
        if (mPlayerType == PlayerType.Local)
        {
            return;
        }
        if (Ftimer.HasTimer("loadclock"))
        {
            Ftimer.Reset("loadclock");
        }
        else
        {
            Ftimer.AddEvent("loadclock", Constant.PlayerLoadTimeOut, () =>
            {
                if (this != null && !IsLoadSuccess)
                {
                    OprateNetError();
                }
            });
        }
    }

    private void CheckNetState()
    {
        if (!MachineState.IsWifiAvailable)
        {
            CommonAlert.Clear();
            mReFreshPage.SetActive(true);
            mLoading.SetActive(false);
            mRefBar.SetData(displayProgress);
        }
    }

    private void OprateNetError()
    {
        Debug.Log("OprateLoadError");
        if (MachineState.IsWifiAvailable)
        {
            CommonAlert.Show("Net_Slow");
        }
        else
        {
            CommonAlert.Clear();
            mReFreshPage.SetActive(true);
            mLoading.SetActive(false);
            mRefBar.SetData(displayProgress);
        }
    }
    #endregion

    #region android msg
    string PhotoType_Activity = "";

    public void GetPhotoTextureType_Activity(string type)
    {
        Debug.Log("Jar : GetPhotoTextureType_Activity");
        PhotoType_Activity = type;
    }

    public void GetMainPhotoPath_Activity(string photoPath)
    {
        Debug.Log("Jar : GetMainPhotoPath_Activity");
        PhotoTextureType type = PhotoTextureType.Photo;
        if (PhotoType_Activity == "1")
        {
            type = PhotoTextureType.Photo;
        }
        else
        {
            type = PhotoTextureType.Cover;
        }

        LoadLocalMainPhoto(photoPath, type);
    }

    public void GetErrorMessage()
    {
        mLoading.SetActive(false);
        CommonAlert.Show("LoadFail_Tip");
    }
    #endregion

    #region buttons
    private void InitButtons()
    {
        UIEventListener.Get(mBtnList).onClick = OnButtonClick;
        UIEventListener.Get(mBtnList).onHover = OnButtonHover;
        UIEventListener.Get(mBtnToggle).onClick = OnButtonClick;
        UIEventListener.Get(mBtnToggle).onHover = OnButtonHover;
        UIEventListener.Get(mListBack).onClick = OnButtonClick;
        UIEventListener.Get(mListBack).onHover = OnButtonHover;
        UIEventListener.Get(mRefreshBtn).onClick = OnButtonClick;
    }

    private void InitScreen()
    {
        mPanoramaBall = UnityTools.CreateComptent<PanoramaBall>(mScreen, transform.parent.parent, Vector3.zero, Quaternion.Euler(-90, 0, 0), 100 * Vector3.one, "PanoramaBall");
        UnityTools.SetMirrorUV(m2DTexture);
    }

    private bool isopen = false;
    private void OnButtonClick(GameObject obj)
    {
        //Debug.Log("OnButtonClick : " + obj.name);
        if (obj == mListBack)
        {
            PicoUnityActivity.CallObjectMethod("setThreadStop");
            OnBack();
        }
        else if (obj == mBtnList)
        {
            //Debug.Log("obj show:" + isopen);
            ShowUI(!isopen);
        }
        else if (obj == mBtnToggle)
        {
            if (!m2DTexture.activeSelf)
            {
                mBtnToggle.GetComponent<UISprite>().spriteName = "bt_360_selected";
                mBtnToggle.GetComponent<UIButton>().normalSprite = "bt_360_selected";
                m2DTexture.SetActive(true);
                mPanoramaBall.gameObject.SetActive(false);
                m2DTexture.GetComponent<Renderer>().material.mainTexture = tempTexture;

                UnityTools.SetCameraBlack(true);
            }
            else
            {
                mBtnToggle.GetComponent<UISprite>().spriteName = "bt_2d_selected";
                mBtnToggle.GetComponent<UIButton>().normalSprite = "bt_2d_selected";
                mPanoramaBall.gameObject.SetActive(true);
                m2DTexture.SetActive(false);
            }
        }
        else if (obj == mRefreshBtn)
        {
            if (MachineState.IsWifiAvailable)
            {
                ReLoadPhoto(mCurentPhoto);
            }
            else
            {
                CommonAlert.Show("Home_NoNet");
            }
        }
    }

    private void OnButtonHover(GameObject obj, bool isHover)
    {
        if (obj == mBtnToggle)
        {
            if (isHover)
            {
                if (m2DTexture.activeSelf)
                {
                    mBtnToggle.GetComponent<UISprite>().spriteName = "bt_360_selected";
                    mBtnToggle.GetComponent<UIButton>().normalSprite = "bt_360_selected";
                }
                else
                {
                    mBtnToggle.GetComponent<UISprite>().spriteName = "bt_2d_selected";
                    mBtnToggle.GetComponent<UIButton>().normalSprite = "bt_2d_selected";
                }
            }
            else
            {
                if (m2DTexture.activeSelf)
                {
                    mBtnToggle.GetComponent<UISprite>().spriteName = "bt_360_normal";
                    mBtnToggle.GetComponent<UIButton>().normalSprite = "bt_360_normal";
                }
                else
                {
                    mBtnToggle.GetComponent<UISprite>().spriteName = "bt_2d_normal";
                    mBtnToggle.GetComponent<UIButton>().normalSprite = "bt_2d_normal";
                }
            }
        }
        //TweenScale ta = TweenScale.Begin(obj, 0.6f, isHover ? 1.1f * Vector3.one : Vector3.one);
        //ta.PlayForward();
    }

    //private void OnArrowButtonHover(GameObject obj, bool status)
    //{
    //    TweenScale ta = TweenScale.Begin(obj, 0.6f, status ? 1.1f * Vector3.one : Vector3.one);
    //    ta.PlayForward();
    //}

    private void ShowUI(bool show)
    {
        isopen = show;
        mScrollePage.gameObject.SetActive(show);
    }
    #endregion

    #region load image
    private void StartDownloadMainPhoto(string mid)
    {
        PhotoModel data = CachePhotoData.Instance.GetPhotoModelByMID(mid);
        if (Application.platform == RuntimePlatform.Android)
        {
            Debug.Log("*********** DownLoadPhoto_Activity ********");
            GalleryActivity.Instance.DownLoadPhoto_Activity(mid);
        }
        else
        {
            if (data == null)
            {
                Debug.Log("the data is not exist:" + mid);
                return;
            }
            string link = data.PhotoLink;
            GetTextureByBestHttp(link, PhotoTextureType.Photo);
        }
        //开启进度条
        StartCoroutine(StartLoading(false));
    }

    // 编辑器里的进度值其实是假的，因为BestHttp返回不了下载进度
    private int displayProgress = 0;
    private IEnumerator StartLoading(bool isLocal)
    {
        ShowUI(false);
        mLoading.SetActive(true);
#if UNITY_EDITOR
#elif UNITY_ANDROID
            if (isLocal || GalleryActivity.Instance.checkImageIfExitsByMid(CachePhotoData.Instance.CurrentPhotoIndex))
            {//本地 或者 已经下载的图片（按mid判断）
                mLoading.transform.Find("circle").gameObject.SetActive(false);
                mLoading.transform.Find("Label").GetComponent<UILabel>().text = Localization.Get("Loading2");
            }
            else
#endif
        {
            mLoading.transform.Find("circle").gameObject.SetActive(true);
            mLoading.transform.Find("Label").GetComponent<UILabel>().text = Localization.Get("Loading");
            UILabel num = mLoading.transform.Find("circle/Num").GetComponent<UILabel>();
            UISprite round = mLoading.transform.Find("circle/Texture_Round").GetComponent<UISprite>();
            displayProgress = 0;
            int toProgress = 100;
            while (displayProgress < toProgress)
            {

#if UNITY_EDITOR
                displayProgress += UnityEngine.Random.Range(5, 15);
                if (displayProgress > 100)
                {
                    displayProgress = 100;
                }
#elif UNITY_ANDROID

                    //从jar读进度值
                    displayProgress = GalleryActivity.Instance.GetProgress_Activity();
#endif
                //下载失败
                if (displayProgress < 0)
                {
                    break;
                }
                if (num != null)
                {
                    num.text = displayProgress.ToString() + "%";
                }

                if (round != null)
                {
                    round.fillAmount = displayProgress / 100.0f;
                }

                //yield return new WaitForEndOfFrame();
                yield return new WaitForSeconds(0.5f);
            }
            mLoading.SetActive(false);
#if UNITY_EDITOR
#elif UNITY_ANDROID
                GalleryActivity.Instance.SetDone2Zero_Activity();
#endif
        }
    }
    private void LoadLocalMainPhoto(string photoPath, PhotoTextureType type)
    {
        mLocalMainPhotoPath = photoPath;
        DyChangeTextureNew(mLocalMainPhotoPath, type);
    }

    void showTitle()
    {
        if (mLoading.activeSelf)
            mLoading.SetActive(false);
        if (!mTitle.activeSelf)
        {
            mTitle.SetActive(true);
            mTitle.GetComponent<UILabel>().text = title;
            Invoke("hideTitle", 2.0f);

        }
    }

    void hideTitle()
    {
        mTitle.GetComponent<UILabel>().text = "";
        mTitle.SetActive(false);
    }
    private IEnumerator ChangeMainPhotoCover(PhotoModelBase data, bool islocal)
    {
        if (data == null)
        {
            Debug.LogError("the data is null!");
            CommonAlert.Show("Player_DownloadPhoto_Failed", true);
            yield return null;
        }
        else
        {
            CommonAlert.Clear();
            mCurentPhoto = data;
            if (islocal)
            {
                Debug.Log("*********** load local photo ********");
                LocalPhotoModel LData = data as LocalPhotoModel;
                title = LData.Title.Substring(LData.Title.LastIndexOf("/") + 1).Split('.')[0];
                DyChangeTextureNew(LData.PhotoLink, PhotoTextureType.Photo);
            }
            else
            {
                Debug.Log("*********** load net photo ********");
                PhotoModel pdata = data as PhotoModel;
                title = pdata.Title;
                CachePhotoData.Instance.CurrentPhotoIndex = pdata.MID;

                // 此处加载缩略图，最多等待时间为1s
                float curtime = Time.time;
                while (!CachePhotoData.Instance.IsIconTextureExist(pdata.MID) && Time.time - curtime < 1f)
                {
                    Debug.Log("*********** wait ********");
                    DataLoader.Instance.LoadTexture(pdata);
                    yield return 0;
                }


                if (mPanoramaBall != null)
                {
                    Debug.Log("*********** set texture ********  pdata.MID = " + pdata.MID);
                    Texture icon = CachePhotoData.Instance.GetIconTexture(pdata.MID);
                    SetTexture(icon);
                    if (GalleryActivity.Instance.checkImageIfExitsByMid(CachePhotoData.Instance.CurrentPhotoIndex))
                    {
                        Debug.Log("Exist  "+(data as PhotoModel).PhotoLink);
                        GalleryActivity.Instance.openByMid(pdata.MID);
                    }
                    else
                    {
                        Debug.Log("!!!!  " + (data as PhotoModel).PhotoLink);
                        // 先检查网络状态，有网的情况下开始下载大图
                        if (!MachineState.IsWifiAvailable)
                        {
                            CheckNetState();
                        }
                        else
                        {
                            StartDownloadMainPhoto(pdata.MID);
                        }
                    }
                }
            }
        }
    }

    private void DownLoadMainPhoto_MID(string mid)
    {
        GalleryActivity.Instance.DownLoadPhoto_Activity(mid);
    }

    private void SetTexture(Texture icon)
    {
        mPanoramaBall.SetTextureAndDestroyOld(icon, PhotoTextureType.Cover);
    }

    // 通过BestHttp下载贴图的函数 Editor里用
    private void GetTextureByBestHttp(string thumbPath, PhotoTextureType type)
    {
        if (string.IsNullOrEmpty(thumbPath)) return;
        var request = new HTTPRequest(new System.Uri(thumbPath), ImageDownloaded);
        // Send out the request
        request.Tag = type;
        request.Send();
    }

    // Callback function of the image download http requests
    private void ImageDownloaded(HTTPRequest req, HTTPResponse resp)
    {
        // Increase the finished count regardless of the state of our request
        switch (req.State)
        {
            // The request finished without any problem.
            case HTTPRequestStates.Finished:
                if (resp.IsSuccess)
                {
                    PhotoTextureType type = (PhotoTextureType)req.Tag;
                    if (mPanoramaBall != null)
                    {
                        Texture2D tex = resp.DataAsTexture2D;
                        int w = tex.width;
                        int h = tex.height;
                        Debug.Log("ImageDownloaded texture size : width * height =  " + tex.width + " * " + tex.height);
                        mPanoramaBall.SetTextureAndDestroyOld(tex, type);
                        IsLoadSuccess = true;
                    }
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
    }

    // 安卓下加载本地图片用的函数
    private IEnumerator GetLocalImage(string url, PhotoTextureType type)
    {
        //string filePath = "file:///" + CACHE_FILE_PATH + url.GetHashCode();
#if UNITY_EDITOR

        string filePath = "file:///" + url;
#elif UNITY_ANDROID
        string filePath = "file://" + url;
#endif
        Debug.Log("GetLocalImage filePath : " + filePath);
        WWW www = new WWW(filePath);
        yield return www;
        //if(www.error == null && www.progress >= 1.0f)
        if (www.error == null && www.texture != null)
        {
            Texture2D image = www.texture;
            image.wrapMode = TextureWrapMode.Clamp;
            image.filterMode = FilterMode.Bilinear;
            image.Apply();
            Debug.Log("GetLocalImage texture size : width * height =  " + image.width + " * " + image.height);
            //if( www.isDone )
            //{ }
            if (mPanoramaBall != null)    //换贴图
            {
                mPanoramaBall.SetTextureAndDestroyOld(image, type);
            }
            //SetLoadingLabelActive(false);
        }
        else
        {
            Debug.Log("GetLocalImage failed : " + www.error);
        }
        www.Dispose();
    }

    //private string tempath = string.Empty;
    //private Texture2D mtemTex = null;
    // 旧版方式，新版将缩略图的创建放到了安卓层
    private void DyChangeTexture(string path, PhotoTextureType type)
    {
        //float time = Time.time;
        Debug.Log("FileStream filePath : " + path);
        Loom.RunAsync(() =>
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            int fsLen = (int)fs.Length;
            Debug.Log("FileStream : File size : " + fsLen);
            byte[] fsByte = new byte[fsLen];
            try
            {
                if (fs != null)
                {
                    fs.Read(fsByte, 0, fsByte.Length);
                    fs.Close();
                    Debug.Log("FileStream : Finished");
                }
                else
                {
                    Debug.Log("FileStream : fs == null");
                    OprateLoadPhotoFailed();
                }
            }
            catch (OutOfMemoryException e)
            {
                Debug.Log("FileStream : OutOfMemoryException : " + e);
                OprateLoadPhotoFailed();
            }
            catch (Exception e)
            {
                Debug.Log("FileStream : IOException : " + e);
                OprateLoadPhotoFailed();
            }
            //Debug.Log("$$$$$$$$$$$read byte cost:" + (Time.time - time));
            //time = Time.time;
            //Run some code on the main thread  必须在主线程内执行的
            Loom.QueueOnMainThread(() =>
            {
                Texture2D mtemTex = new Texture2D(2, 2, TextureFormat.ETC_RGB4, true);
                mtemTex.LoadImage(fsByte);
                mtemTex.filterMode = FilterMode.Bilinear;
                mtemTex.wrapMode = TextureWrapMode.Clamp;
                mtemTex.Apply();

                fsByte = null;
                GC.Collect();

                if (mtemTex.width < 10 && mtemTex.height < 10)
                {
                    CommonAlert.Show("Player_DownloadPhoto_Failed", true);
                    Debug.Log("open commonAlert , create a default texture :  ?");
                }
                else
                {
                    if (mPanoramaBall != null)    //换贴图
                    {
                        showTitle();
                        tempTexture = mtemTex;
                        mPanoramaBall.SetTextureAndDestroyOld(tempTexture, type);
                        IsLoadSuccess = true;
                        //Debug.Log("$$$$$$$$$$$SetTexture cost:" + (Time.time - time));
                    }
                    else
                    {
                        Debug.Log("mPanoramaBall is null !!!");
                    }
                }
            });
        });
    }

    // 新版方式，降低大图加载卡顿
    private string CurPhotoPath = string.Empty;
    private void DyChangeTextureNew(string path, PhotoTextureType type)
    {
        Debug.Log("CreateBitMap ");
        CurPhotoPath = path;
        PicoUnityActivity.CallObjectMethod("CreateBitMap", path);
    }

    private void TexturePtrCreatedOK(string path)
    {
        int texPtr = 0;
        if(!CurPhotoPath.Equals(path))
        {
            // 与当前图片不相同，直接返回
            return;
        }
        else
        {
            PicoUnityActivity.CallObjectMethod<int>(ref texPtr, "CreateTexturePtr", path);
        }
       

        if (texPtr == 0)
        {
            OprateLoadPhotoFailed();
            return;
        }
        

        Texture2D nativeTexture = Texture2D.CreateExternalTexture(4096, 2048, TextureFormat.ETC2_RGBA8, false, false, (IntPtr)texPtr);

        if (mPanoramaBall != null)    //换贴图
        {
            showTitle();
            tempTexture = nativeTexture;

            mPanoramaBall.SetTextureAndDestroyOld(tempTexture, PhotoTextureType.Photo);
            IsLoadSuccess = true;
        }
        else
        {
            Debug.Log("mPanoramaBall is null !!!");
        }
    }

    private void OprateLoadPhotoFailed()
    {
        Debug.LogError("***LoadPhotoFailed***");
        CommonAlert.Show("Player_DownloadPhoto_Failed", true);
    }
    #endregion

    #region back
    void OnBack()
    {
        if (mPlayerType == PlayerType.Recommend)
        {
            GalleryTools.QuitApp();
        }
        else
        {
            UnityTools.SetCameraBlack(false);
            ScreenManager.Instance.CloseScreen(this);
            CommonAlert.Clear();
            if (mLoading.activeSelf)
                SetThreadStop();
        }
    }

    private void SetThreadStop()
    {
        Debug.Log("Stop the Thread!");
        PicoUnityActivity.CallObjectMethod("setThreadStop");
    }
    public void OnLeft()
    {
        if (mScrollePage.gameObject.activeInHierarchy)
        {
            mScrollePage.MoveToLeft();
        }
        else
        {
            PlayNextPhoto(false);
        }
    }

    public void OnRight()
    {
        if (mScrollePage.gameObject.activeInHierarchy)
        {
            mScrollePage.MoveToRight();
        }
        else
        {
            PlayNextPhoto(true);
        }
    }
    #endregion
}