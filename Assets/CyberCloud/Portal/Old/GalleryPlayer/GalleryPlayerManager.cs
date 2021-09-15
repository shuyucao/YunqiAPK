using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BestHTTP;
using System.IO;
using System;

public class GalleryPlayerManager : ScreenBase
{
    public PanoramaBall mPanoramaBall;
    public GameObject mListObj;
    public GameObject mBackObj;
    public GameObject mScrollView;
    public ScrollPanel_Player mScrollPanel;

    public GameObject mLoading;
    public PlayerToast mPlayerToast;
    public GameObject mLeftBtn;
    public GameObject mRightBtn;

    private bool mShowScrollView = false;

    List<PhotoModel> mPhotoModelList = null;
    List<IconTexture> mIconTextureList = null;

    string mLocalMainPhotoPath;
    private Texture mDefaultCover = null;

    // Use this for initialization
    void Start()
    {
        if (mDefaultCover == null)
        {
            mDefaultCover = Resources.Load("Textures/default poster") as Texture;
        }
        InitPhotoData();
        InitButtons();
      //  InputManager.OnBack += OnBack;
    }

    void OnDestroy()
    {
        if (mScrollPanel != null)
        {
            mScrollPanel.DestroyBase();
        }
      //  InputManager.OnBack -= OnBack;
    }

    void OnBack()
    {
        OnButtonClick(mBackObj);
    }

    void InitPhotoData()
    {
        if (ChangeMainPhotoCover(GlobalPhotoData.Instance.mCurrentPhotoIndex) == true)
        {
            if (mScrollPanel != null)
            {
                mScrollPanel.IconButtonAccept = false;
            }
            StartDownloadMainPhoto(GlobalPhotoData.Instance.mCurrentPhotoIndex);
        }
        else
        {
            return;
        }
        if (mScrollPanel != null)
        {
            mScrollPanel.InitBase();
        }
    }

    public void StartDownloadMainPhoto(int index)
    {
        mPhotoModelList = GlobalPhotoData.Instance.GetPhotoList();

        if (index + 1 > mPhotoModelList.Count)
        {
            Debug.LogError("StartDownloadMainPhoto : photoIndex + 1 > mPhotoModelList.Count !");
            return;
        }


#if UNITY_EDITOR

        string link = mPhotoModelList[index].PhotoLink;
        //string link = mPhotoModelList[photoIndex].ThumbnailLink;

        GetTextureByBestHttp(link, PhotoTextureType.Photo);  //BestHttp下载
        //StartCoroutine(loadThumnail(link)); //WWW下载
        //string localPath = "D:/testLocal.jpg";
        //string localPath = "D:/1.jpg";
        //StartCoroutine(GetLocalImage(localPath, PhotoTextureType.Photo)); //本地图片
#elif UNITY_ANDROID
        string mid = mPhotoModelList[index].MID;
        GalleryActivity.Instance.DownLoadPhoto_Activity( mid );
#endif
        //开启进度条
        StartCoroutine(StartLoading());
    }

    //编辑器里的进度值其实是假的，因为BestHttp返回不了下载进度
    IEnumerator StartLoading()
    {
        ShowUI(false);
        if (mLoading != null)
        {
            mLoading.SetActive(true);
            UILabel num = mLoading.transform.Find("Num").GetComponent<UILabel>();
            UISprite round = mLoading.transform.Find("Texture_Round").GetComponent<UISprite>();
            int displayProgress = 0;
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
                    //恢复图片列表的点击
                    if (mScrollPanel != null)
                    {
                        mScrollPanel.IconButtonAccept = true;
                    }

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
                yield return new WaitForSeconds(.1f);
            }
            mLoading.SetActive(false);
#if UNITY_EDITOR
#elif UNITY_ANDROID
            GalleryActivity.Instance.SetDone2Zero_Activity();
#endif
        }

    }

    #region jar里下载完图片UnitySendMessage用的函数

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
    #endregion

    public void LoadLocalMainPhoto(string photoPath, PhotoTextureType type)
    {
        mLocalMainPhotoPath = photoPath;
        //StartCoroutine(GetLocalImage(mLocalMainPhotoPath, type));
        StartCoroutine(GetLocalImageFromFile(mLocalMainPhotoPath, type));
    }

    #region 下载完图片后读取图片数据填充贴图

    IEnumerator GetLocalImageFromFile(string path, PhotoTextureType type)
    {
        Debug.Log("FileStream GetLocalImageFromFile Start");
        yield return StartCoroutine(DyChangeTexture(mLocalMainPhotoPath, type));

        Debug.Log("FileStream GetLocalImageFromFile Finished");
        yield return null;
        GC.Collect();
    }


    IEnumerator DyChangeTexture(string path, PhotoTextureType type)
    {
        Debug.Log("FileStream filePath : " + path);
        try
        {
            //string path__ = "/storage/emulated/0/git.png";
            //string path__ = "/storage/emulated/0/git.png";
            //string path__ = "/storage/emulated/0/picoGallery/download/photo/中文.jpg";

            //byte[] path_b = System.Text.Encoding.UTF8.GetBytes(path);
            //string path_utf8 = System.Text.Encoding.UTF8.GetString(path_b);
            //String urlPath = URLDecoder.decode(path, "UTF-8");


            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            if (fs != null)
            {
                int fsLen = (int)fs.Length;
                Debug.Log("FileStream : File size : " + fsLen);


                byte[] fsBytes = new byte[fsLen];
                int r = fs.Read(fsBytes, 0, fsBytes.Length);


                Texture2D _tex2 = new Texture2D(2, 2);
                _tex2.LoadImage(fsBytes);

                //GameObject.DestroyImmediate(_tex2);
                if(_tex2.width<10 && _tex2.height<10){
                    mPlayerToast.Show(Localization.Get("Player_LocalPhoto_Failed"));
                }
                else{
                    if (mPanoramaBall != null)    //换贴图
                    {
                        mPanoramaBall.SetTextureAndDestroyOld(_tex2, type);
                    }
                    else
                        Debug.Log("mPanoramaBall is null !!!");
                }
                _tex2 = null;
                fs.Close();

                Debug.Log("FileStream : Finished");
            }
            else
            {
                Debug.Log("FileStream : fs == null");
                mPlayerToast.Show(Localization.Get("Player_LocalPhoto_Failed"));
            }


        }
        catch (OutOfMemoryException e)
        {
            Debug.Log("FileStream : OutOfMemoryException : " + e);
            mPlayerToast.Show(Localization.Get("Player_LocalPhoto_Failed"));

            //恢复图片列表的点击
            if (mScrollPanel != null)
            {
                mScrollPanel.IconButtonAccept = true;
            }
        }
        catch (Exception e)
        {
            Debug.Log("FileStream : IOException : " + e);
            mPlayerToast.Show(Localization.Get("Player_LocalPhoto_Failed"));

            //恢复图片列表的点击
            if (mScrollPanel != null)
            {
                mScrollPanel.IconButtonAccept = true;
            }
        }


        //恢复图片列表的点击
        if (mScrollPanel != null)
        {
            mScrollPanel.IconButtonAccept = true;
        }

        yield return null;
    }

    #endregion

    public bool ChangeMainPhotoCover(int index)
    {
        mIconTextureList = GlobalPhotoData.Instance.GetIconTextureList();
        if (mIconTextureList.Count <= 0)
        {
            Debug.LogError("ChangeMainPhotoCover : mIconTextureList.Count <= 0 !");
            return false;
        }

        if (index + 1 > mIconTextureList.Count)
        {
            Debug.LogError("ChangeMainPhotoCover : photoIndex + 1 > mIconTextureList.Count !");
            return false;
        }

        if (mPanoramaBall != null)
        {

            IconTexture icon = mIconTextureList.Find(
                delegate (IconTexture it)
                {
                    return it.index == index;
                });

            if (icon == null)
            {
                Debug.LogError("ChangeMainPhotoCover : mIconTextureList does not have the texture !");
                mPanoramaBall.SetTextureAndDestroyOld(mDefaultCover, PhotoTextureType.Cover);
            }
            else
            {
                mPanoramaBall.SetTextureAndDestroyOld(icon.texture, PhotoTextureType.Cover);
            }

            return true;
        }
        return false;
    }

    public void DownLoadMainPhoto_MID(string mid)
    {
#if UNITY_ANDROID

        GalleryActivity.Instance.DownLoadPhoto_Activity(mid);

#endif

        //开启进度条
        StartCoroutine(StartLoading());
    }

    void InitButtons()
    {
        if (mListObj != null)
        {
            UIEventListener.Get(mListObj).onClick = OnButtonClick;
            UIEventListener.Get(mListObj).onHover = OnButtonHover;

        }

        if (mBackObj != null)
        {
            UIEventListener.Get(mBackObj).onClick = OnButtonClick;
            UIEventListener.Get(mBackObj).onHover = OnButtonHover;
        }
        UIEventListener.Get(mLeftBtn).onHover = OnArrowButtonHover;
        UIEventListener.Get(mRightBtn).onHover = OnArrowButtonHover;
    }

    void OnButtonClick(GameObject obj)
    {
        Debug.Log("OnButtonClick : " + obj.name);


        if (obj == mBackObj)
        {
            //返回键要做针对网络请求能不能点的处理
            if (mScrollPanel != null)
            {
                if (mScrollPanel.CheckIsRequesting() == false)
                {

                    //如果正在下载大图, 通知jar取消下载
#if UNITY_EDITOR
#elif UNITY_ANDROID
#endif
                    Debug.Log("Back to Home  Press Button : Second : " + System.DateTime.Now.Second + " Millisecond : " + System.DateTime.Now.Millisecond);
                    //StartCoroutine(BackToHome(null));
                    PicoUnityActivity.CallObjectMethod("setThreadStop");
                    Home.levelIndex = 1;
                    Application.LoadLevel(0);
                }
            }
        }
        else if (obj == mListObj)
        {
            if (mScrollPanel != null)
            {
                if (mScrollPanel.CheckIsInitialized() == true)
                {
                    mShowScrollView = !mShowScrollView;
                    ShowUI(mShowScrollView);
                }
            }
        }
    }

    void OnButtonHover(GameObject obj, bool status)
    {
        GameObject go = obj.GetComponent<UIButton>().tweenTarget;
        int zOrder = status ? 95 : 100;
        Vector3 pos = go.transform.localPosition;
        pos.z = zOrder;
        go.transform.localPosition = pos;
    }

    void OnArrowButtonHover(GameObject obj, bool status)
    {
        Vector3 pos = obj.transform.localPosition;
        obj.transform.localPosition = new Vector3(pos.x, pos.y, status ? 147 : 150);
    }

    IEnumerator BackToHome(string param)
    {
        Home.levelIndex = 1;
        Application.LoadLevel(0);
        yield return 0;
    }

    void ShowUI(bool show)
    {
        if (mScrollView != null)
        {
            mScrollView.SetActive(show);
        }
    }

    //通过BestHttp下载贴图的函数 Editor里用
    private void GetTextureByBestHttp(string thumbPath, PhotoTextureType type)
    {
        if (string.IsNullOrEmpty(thumbPath)) return;
        var request = new HTTPRequest(new System.Uri(thumbPath), ImageDownloaded);
        // Send out the request
        request.Tag = type;
        request.Send();
    }

    /// <summary>
    /// Callback function of the image download http requests
    /// </summary>
    void ImageDownloaded(HTTPRequest req, HTTPResponse resp)
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
                    }


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
        //恢复图片列表的点击
        if (mScrollPanel != null)
        {
            mScrollPanel.IconButtonAccept = true;
        }
    }

    //安卓下加载本地图片用的函数
    IEnumerator GetLocalImage(string url, PhotoTextureType type)
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
            //SetLoadingLabelText("加载失败");
            mPlayerToast.Show(Localization.Get("Player_LocalPhoto_Failed"));
        }
        www.Dispose();

        //恢复图片列表的点击
        if (mScrollPanel != null)
        {
            mScrollPanel.IconButtonAccept = true;
        }
    }
}