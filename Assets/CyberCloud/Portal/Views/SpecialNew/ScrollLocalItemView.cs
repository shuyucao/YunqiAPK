using UnityEngine;
using System.Collections;
using ImageLoaderPlugin;
using BestHTTP;
using DG.Tweening;

public class ScrollLocalItemView : BaseScrollItem
{

    [SerializeField]
    UILabel nameLabel;
    [SerializeField]
    UITexture texture;
    bool NeedAnimation = true;
    [SerializeField]
    GameObject mBG;
    LocalPhotoModel LData;
    Vector3 initPos;
    Vector3 targetPos;

    void Start()
    {
        UIEventListener.Get(this.gameObject).onClick = onItemClick;
        UIEventListener.Get(gameObject).onHover = OnHoverImageItem;
        initPos = this.gameObject.transform.localPosition;
        targetPos = new Vector3(initPos.x, initPos.y, initPos.z - 80);
    }

    private void OnHoverImageItem(GameObject go, bool onHover)
    {
        if (onHover)
            this.transform.DOLocalMove(targetPos, 0.5f);
        else
            this.transform.DOLocalMove(initPos, 0.5f);
        mBG.SetActive(onHover);
        InfoAlpha(onHover);
    }
    private void InfoAlpha(bool isShow) //显示名字栏
    {
        if (isShow)
            DOTween.To(() => nameLabel.alpha, x => nameLabel.alpha = x, 1, 1f);
        else
            DOTween.To(() => nameLabel.alpha, x => nameLabel.alpha = x, 0, 1f);
    }
    void onItemClick(GameObject go)
    {

        if (LocalPageScreen.Layer == LocalPageScreen.LocalLayer.Folder)
        {
            Bundle bundle = new Bundle();
            bundle.SetValue<string>("TitleName", LData.Title);
            //Debug.Log("LData.Title  :  " + LData.Title);
            MsgManager.Instance.SendMsg(MsgID.FolderIn, bundle);
            LocalPageScreen.Layer = LocalPageScreen.LocalLayer.Picture;
        }
        else if (LocalPageScreen.Layer == LocalPageScreen.LocalLayer.Picture)
        {
            Debug.Log("^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^");
            Debug.Log(LData.Title);
            Debug.Log(LData.ThumbnailLink);
            Debug.Log(LData.PhotoLink);
            if (GalleryActivity.Instance.FileExistOrNot(LData.Title, LData.ThumbnailLink))
            {
                GPlayerManager.Instance.Play(LData);
                CachePhotoData.Instance.localIndex = FindData();
            }
            else
                CommonAlert.Show("Player_Photo_Deleted");
        }
        OnHoverImageItem(this.gameObject, false);

    }
    LocalImageLoader localImageLoader;
    public override void UpdateData(int index)
    {
        base.UpdateData(index);

        LData = itemData as LocalPhotoModel;
        if (index >= 6) NeedAnimation = false;
        if (LData == null)
        {
            Debug.Log("LData is null  " + index);
            return;
        }
        //if(LocalPageScreen.Layer == LocalPageScreen.LocalLayer.Picture)
        nameLabel.text = SubTitle(LData.Title);
        nameLabel.alpha = 0;
        //Debug.Log(LData.Title + "    |||    " + nameLabel.text + "  index=="+ index);
        //else
        //nameLabel.text = LData.Title;
        if(CachePhotoData.Instance.IsIconTextureExist(LData.ThumbnailLink))
        {
            texture.mainTexture = CachePhotoData.Instance.GetIconTexture(LData.ThumbnailLink);
            return;
        }
        try
        {
            if (string.IsNullOrEmpty(LData.ThumbnailLink) || LData.ThumbnailLink.Contains("JsonData object")) return;
            System.Uri uri = new System.Uri(LData.ThumbnailLink);

            localImageLoader = new LocalImageLoader(LData.ThumbnailLink, true, getThumbnail);
            localImageLoader.StartLoad();
        }
        catch (System.Exception)
        {
            Debug.LogError("link:" + LData.ThumbnailLink);
            throw;
        }
    }

    void getThumbnail(LocalImageLoader localImageLoader, LocalImageLoaderResponse response)
    {
        if (localImageLoader == null )
        {
            Debug.Log("localImageLoader  is null !!!");
            //localImageLoader.StartLoad();
            return;
        }
        else if (response == null)
        {
            Debug.Log("Response is null !!!");
            //localImageLoader.StartLoad();
            return;
        }
        //Debug.Log("localImageLoader url = " + localImageLoader.Url);
        if (response.DataAsTexture2D && this != null)
        {
            //SetTexture(response.DataAsTexture2D);
            texture.mainTexture = response.DataAsTexture2D;
            if (NeedAnimation) TextureShowAnim();
            CachePhotoData.Instance.AddIconTexture(LData.ThumbnailLink, response.DataAsTexture2D);
        }

    }
    string SubTitle(string str)
    {
        string s = str.Substring(str.LastIndexOf("/") + 1);
        //Debug.Log(LocalPageScreen.Layer + " LocalPageScreen.Layer " + str);
        if (LocalPageScreen.Layer == LocalPageScreen.LocalLayer.Folder)
            return s;
        else
        {
            return s.Split('.')[0];
        }
    }

    void ImageDownloaded(HTTPRequest req, HTTPResponse resp)
    {
        switch (req.State)
        {
            case HTTPRequestStates.Finished:
                if (resp.IsSuccess)
                {
                    //Debug.Log("################################");
                    //SetTexture(resp.DataAsTexture2D);

                    texture.mainTexture = resp.DataAsTexture2D;
                    if (NeedAnimation) TextureShowAnim();
                    //CachePhotoData.Instance.AddIconTexture(SData.ThemeID, resp.DataAsTexture2D);
                    //释放上一个Texture
                    //Resources.UnloadAsset(tex);
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
    private void TextureShowAnim()
    {
        texture.transform.DOScale(0f, 0f);
        DOTween.To(() => texture.transform.localScale, x => texture.transform.localScale = x, Vector3.one, 0.5f).SetEase(Ease.Linear);
        NeedAnimation = false;
    }

    private int FindData()
    {
        int i = 0;
        while (!CachePhotoData.Instance.PhotosList[i].Equals(LData))
        {
            i++;
        }
        return i;

    }
}
