using UnityEngine;
using System.Collections;
using ImageLoaderPlugin;
using BestHTTP;
public class SpecialImageItem : ImageItemBase
{
    [SerializeField]
    UILabel mTitle;
    [SerializeField]
    GameObject mBG;
    [SerializeField]
    GameObject mRT;

    private ThemesModel SData
    {
        get
        {
            return Data as ThemesModel;
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
        DataLoader.Instance.AddImgItem(this);
    }

    string SubTitle(string str)
    {
        string s = str.Substring(str.LastIndexOf("/") + 1);
        Debug.Log(SpecialPageScreen.layer + " SpecialPageScreen.Layer " + str);
        if (SpecialPageScreen.layer == SpecialPageScreen.Layer.SpecialList)
            return s;
        else
        {
            return s.Split('.')[0];
        }
    }
    public override void LoadTexture()
    {
        ////SetTexture(CachePhotoData.Instance.GetIconTexture(Data.MID));
        Debug.Log("SpecialImageItem. ThemeID   ==  " + SData.ThemeID);
        Debug.Log("SpecialImageItem. Title   ==  " + SData.Title);
        Debug.Log("SpecialImageItem. LogoImg   ==  " + SData.LogoImg);
        Debug.Log("SpecialImageItem. Provider   ==  " + SData.Provider);
        Debug.Log("SpecialImageItem. Cover   ==  " + SData.Cover);
        mRT.GetComponent<UILabel>().text = SData.Provider;
        mTitle.text = SData.Title;
        try
        {
            if (string.IsNullOrEmpty(SData.Cover) || SData.Cover.Contains("JsonData object")) return;
            System.Uri uri = new System.Uri(SData.Cover);
            if (uri != null)
            {
                var request = new HTTPRequest(uri, ImageDownloaded);
                //request.Tag = item;
                request.Send();
            }
        }
        catch (System.Exception)
        {
            Debug.LogError("link:" + SData.Cover);
            throw;
        }
    }

    private void OnClickImageItem(GameObject go)
    {
        //GPlayerManager.Instance.Play(Data.MID);
        if (SpecialPageScreen.Layer.SpecialList == SpecialPageScreen.layer)
        {
            Debug.Log("SData.Title  :  " + SData.Title);
            Debug.Log("SData.ThemeID  :  " + SData.ThemeID);
            Debug.Log("Show Photo list !!!!!");
            CategoryPhotoData data = CachePhotoData.Instance.GetThemesPhotosByID(SData.ThemeID);
            if (data == null || data.PhotoList.Count == 0)
            {
                DataLoader.Instance.RequestThemesPhoto(SData.ThemeID);
            }
            else
            {
                Bundle bundle = new Bundle();
                bundle.SetValue<string>("themeID", SData.ThemeID);
                MsgManager.Instance.SendMsg(MsgID.ThemePhotoDataRefresh, bundle);
            }
        }
        else if (SpecialPageScreen.Layer.PhotoList == SpecialPageScreen.layer)
        {
            Debug.Log("^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^");
            Debug.Log(SData.Title);
            //Debug.Log(LData.ThumbnailLink);
            //Debug.Log(LData.PhotoLink);
            //GPlayerManager.Instance.Play(LData);
        }
    }

    private void OnHoverImageItem(GameObject go, bool onHover)
    {
        mBG.SetActive(onHover);
    }

    void ImageDownloaded(HTTPRequest req, HTTPResponse resp)
    {
        switch (req.State)
        {
            case HTTPRequestStates.Finished:
                if (resp.IsSuccess)
                {
                    Debug.Log("################################");
                    SetTexture(resp.DataAsTexture2D);
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
}