using UnityEngine;
using System.Collections;
using ImageLoaderPlugin;
using System.Collections.Generic;

public class LocalImageItem : ImageItemBase
{
    [SerializeField]
    UILabel mTitle;
    [SerializeField]
    GameObject mBG;
    [SerializeField]
    GameObject mRT;

    private LocalPhotoModel LData
    {
        get
        {
            return Data as LocalPhotoModel;
        }
    }

    void Start()
    {
        UIEventListener.Get(gameObject).onClick = OnClickImageItem;
        UIEventListener.Get(gameObject).onHover = OnHoverImageItem;
    }

    public override void Init(PhotoModelBase data)
    {
        Data = data;
        ResetReadyState();
        DataLoader.Instance.AddImgItem(this);
    }

    string SubTitle(string str)
    {
        string s = str.Substring(str.LastIndexOf("/") + 1);
        Debug.Log(LocalPageScreen.Layer + " LocalPageScreen.Laye " + str);
        if (LocalPageScreen.Layer == LocalPageScreen.LocalLayer.Folder)
            return s;
        else
        {
            return s.Split('.')[0];
        }
    }
    public override void LoadTexture()
    {
        //SetTexture(CachePhotoData.Instance.GetIconTexture(Data.MID));
        Debug.Log("LData.ThumbnailLink  ==  " + LData.ThumbnailLink);
        //        StartCoroutine(getThunbnail());
        LocalImageLoader localImageLoader = new LocalImageLoader(LData.ThumbnailLink, true, getThunbnail);
        localImageLoader.StartLoad();

        if (LocalPageScreen.Layer == LocalPageScreen.LocalLayer.Picture)
            mRT.SetActive(false);
        else if (LocalPageScreen.Layer == LocalPageScreen.LocalLayer.Folder)
            mRT.SetActive(true);

        mTitle.text = SubTitle(LData.Title);
    }

    //    IEnumerator getThunbnail()
    public void getThunbnail(LocalImageLoader localImageLoader, LocalImageLoaderResponse response)
    {
        //        WWW www = new WWW( "file:///"+LData.ThumbnailLink);
        //        yield return www;
        //        if (www.isDone && string.IsNullOrEmpty(www.error)) {
        //            SetTexture(www.texture);
        //        }
        //        else {
        //            Debug.LogError("something error:"+www.error);
        //        }
        if (localImageLoader == null || response == null)
        {
            Debug.Log("localImageLoader or response is null !!!");
            return;
        }
        Debug.Log("localImageLoader url = " + localImageLoader.Url);
        //        SetTexture(response.DataAsTexture2D);
        //        if(LocalPageScreen.Layer == LocalPageScreen.LocalLayer.Picture)
        if (response.DataAsTexture2D && this != null)
        {
            SetTexture(response.DataAsTexture2D);
        }

    }

    private void OnClickImageItem(GameObject go)
    {
        //GPlayerManager.Instance.Play(Data.MID);
        if (LocalPageScreen.Layer == LocalPageScreen.LocalLayer.Folder)
        {
            Bundle bundle = new Bundle();
            bundle.SetValue<string>("TitleName", LData.Title);
            Debug.Log("LData.Title  :  " + LData.Title);
            MsgManager.Instance.SendMsg(MsgID.FolderIn, bundle);
            LocalPageScreen.Layer = LocalPageScreen.LocalLayer.Picture;
        }
        else if (LocalPageScreen.Layer == LocalPageScreen.LocalLayer.Picture)
        {
            Debug.Log("^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^");
            Debug.Log(LData.Title);
            Debug.Log(LData.ThumbnailLink);
            Debug.Log(LData.PhotoLink);
            if (GalleryActivity.Instance.FileExistOrNot(LData.Title, LData.ThumbnailLink)) {
                GPlayerManager.Instance.Play(LData);
                CachePhotoData.Instance.localIndex = FindData();
            }
            else
                CommonAlert.Show("Player_Photo_Deleted");
        }
    }

    private void OnHoverImageItem(GameObject go, bool onHover)
    {
        mBG.SetActive(onHover);
    }

    private int FindData()
    {
        int i = 0;
        while(!CachePhotoData.Instance.PhotosList[i].Equals(LData))
        {
            i++;
        }
        return i;

    }
}