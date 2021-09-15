using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using System;

public class LocalPageScreen : ScreenBase, IMsgHandle
{
    [SerializeField]
    GameObject mFolderPage;
    [SerializeField]
    GameObject mPicInFolderPage;
    [SerializeField]
    GameObject bottom;
    [SerializeField]
    GameObject leftButton;
    [SerializeField]
    GameObject mNoFileTips;

    [SerializeField]
    ScrollController scrollControllerFolders;
    List<BaseData> dataListFolders;


    [SerializeField]
    ScrollController scrollControllerPhotos;
    List<BaseData> dataListPhotos;

    string paths;
    string thumbnails;
    public static LocalLayer Layer = LocalLayer.Folder;
    void Start()
    {
        //mPicInFolderPage.SetActive(false);
        mFolderPage.SetActive(true);
        UIEventListener.Get(leftButton).onClick = OnLeftButtonClick;
        MsgManager.Instance.RegistMsg(MsgID.FolderIn, this);
        MsgManager.Instance.RegistMsg(MsgID.FolderOut, this);
        //MsgManager.Instance.RegistMsg(MsgID.LocalFolderDataReady, this);
        //MsgManager.Instance.RegistMsg(MsgID.LocalPhotosInFolderDataReady, this);
        //bottom.SetActive(false);
    }

    void OnLeftButtonClick(GameObject obj)
    {
        CloseInFolderPage();
    }

    public void HandleMessage(MsgID id, Bundle bundle)
    {
        if (id == MsgID.FolderIn)
        {
            //Debug.Log("&&&&&&&&Fold In&&&&&&&&&&");
            Layer = LocalLayer.Picture;
            CachePhotoData.Instance.localIndex = -1;
            if (bundle == null || !bundle.Contains<string>("TitleName"))
            {
                Debug.LogError("lose TitleName!");
            }
            else if (id.Equals(bundle.GetValue<string>("TitleName")))
            {
            }
            Debug.Log(" title name is  " + bundle.GetValue<string>("TitleName"));
            if (mFolderPage.activeInHierarchy)
            {
                mFolderPage.SetActive(false);
              }
            mPicInFolderPage.SetActive(true);


            thumbnails = GalleryActivity.Instance.GetLocalImages(bundle.GetValue<string>("TitleName"));
            Debug.Log("************start init************" + thumbnails);
            List<LocalPhotoModel> picList = new List<LocalPhotoModel>();
            if (!string.IsNullOrEmpty(thumbnails))
                picList = jsonParse(thumbnails);
            else
                Debug.Log("thumbnails data is null!!!");
            CachePhotoData.Instance.PhotosList = picList;

            if (dataListPhotos == null)
                dataListPhotos = new List<BaseData>();
            else
                dataListPhotos.Clear();
            if (Application.platform == RuntimePlatform.Android)
            {
            }
            else
            {
                picList = jsonParseForTest();
            }
            for (int i = 0; i < picList.Count; i++)
            {
                LocalPhotoModel mdata = new LocalPhotoModel();
                mdata = picList[i];
                dataListPhotos.Add(mdata);
                mdata = null;
            }
            Debug.Log("dataListPhotos.Count  === " + dataListPhotos.Count);
            scrollControllerPhotos.InitDataList(dataListPhotos, true);
            bottom.SetActive(true);
        }
        else if (id == MsgID.BackToSL)
        {
            OnBack();
        }
        else if (id == MsgID.FolderOut)
        {
            CloseInFolderPage();
        }
        //else if (id == MsgID.LocalFolderDataReady)
        //{

        //}
        //else if (id == MsgID.LocalPhotosInFolderDataReady)
        //{

        //}
    }

    //when this screen move to the top
    public override void OprateChangeScreen(Bundle bundle)
    {
        MsgManager.Instance.RegistMsg(MsgID.BackToSL, this);
        //InputManager.OnBack += OnBack;
        //GalleryTools.ShowLeftBar(true);
        InitFoldPageData();
    }

    private void InitFoldPageData()
    {
        if (Layer != LocalLayer.Folder)
        {
            return;
        }
#if UNITY_EDITOR
        paths = "[]";
#elif UNITY_ANDROID
        paths = GalleryActivity.Instance.GetImageDirs(null);
#endif
        Debug.Log("pahts : " + paths);

        if (!string.IsNullOrEmpty(paths))
            Debug.Log(JsonMapper.ToObject(paths).Count);

        List<LocalPhotoModel> folderList = new List<LocalPhotoModel>();
        string ERROR = "error";
        if (string.IsNullOrEmpty(paths) || ERROR.Equals(paths) || JsonMapper.ToObject(paths).Count == 0)
        {
            Debug.Log("data error " + paths);
#if UNITY_EDITOR
#elif UNITY_ANDROID
            if (!mNoFileTips.activeSelf)
            {
                mNoFileTips.SetActive(true);
                mFolderPage.SetActive(false);
            }
            return;
#endif
        }
        folderList = jsonParse(paths);
        mFolderPage.SetActive(true);
        mNoFileTips.SetActive(false);
        if (Application.platform == RuntimePlatform.Android)
        {
        }
        else
        {
            folderList = jsonParseForTest();
        }
        if (dataListFolders == null)
            dataListFolders = new List<BaseData>();
        else
            dataListFolders.Clear();
        for (int i = 0; i < folderList.Count; i++)
        {
            LocalPhotoModel mdata = new LocalPhotoModel();
            mdata = folderList[i];
            dataListFolders.Add(mdata);
        }
        scrollControllerFolders.InitDataList(dataListFolders, true);
    }

    //when this screen closed
    public override void OprateCloseScreen()
    {
       // InputManager.OnBack -= OnBack;
        MsgManager.Instance.RemoveMsg(MsgID.BackToSL, this);
    }

    void OnBack()
    {
        Debug.Log("localpagescreen back layer = " + LocalPageScreen.Layer);
        if (Layer == LocalLayer.Folder)
        {
            GalleryTools.QuitApp();
        }
        else if (Layer == LocalLayer.Picture)
        {
            CloseInFolderPage();
        }
    }

    private void CloseInFolderPage()
    {
        Layer = LocalLayer.Folder;
        bottom.SetActive(false);
        mFolderPage.SetActive(true);
        if (mPicInFolderPage.activeInHierarchy)
        {
            mPicInFolderPage.SetActive(false);
        }
    }

    public enum LocalLayer
    {
        Folder,
        Picture
    }

    List<LocalPhotoModel> jsonParse(string str)
    {
        List<LocalPhotoModel> list = new List<LocalPhotoModel>();
        JsonData jd = JsonMapper.ToObject(str);
        IDictionary tempDic = jd as IDictionary;
        try
        {
            if (jd.IsArray)
            {
                for (int i = 0; i < jd.Count; i++)
                {
                    LocalPhotoModel tmp = new LocalPhotoModel();
                    tmp.Title = jd[i]["filePath"].ToString();
                    tmp.ThumbnailLink = jd[i]["thumbStorePath"].ToString();
                    tmp.PhotoLink = jd[i]["thumbPath"].ToString();
                    list.Add(tmp);
                }
            }
            else
            {
                LocalPhotoModel tmp = new LocalPhotoModel();
                tmp.Title = jd["filePath"].ToString();
                tmp.ThumbnailLink = jd["thumbStorePath"].ToString();
                tmp.PhotoLink = jd["thumbPath"].ToString();
                list.Add(tmp);
            }
        }
        catch (Exception e)
        {
            Debug.Log("jsonParse data error");
        }
        return list;
    }

    List<LocalPhotoModel> jsonParseForTest()
    {
        List<LocalPhotoModel> datalist = new List<LocalPhotoModel>();
        for (int i = 0; i < 20; i++)
        {
            LocalPhotoModel tmp = new LocalPhotoModel();
            tmp.Title = "test" + i;
            tmp.ThumbnailLink = "http://photo.picovr.com/images/thumbs/b3/25/649a73f3e973ec3d972b58736c8be40ee2ea34c0.jpg";
            tmp.PhotoLink = "http://photo.picovr.com/images/photos/5f/46/1d147062f3aca4f4ac82ab3bea7debbdd5b34477.jpg";
            datalist.Add(tmp);
        }
        return datalist;
    }

    void OnDestroy()
    {
    }
}