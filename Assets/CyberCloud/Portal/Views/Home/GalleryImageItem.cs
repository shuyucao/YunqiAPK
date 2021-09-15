using BestHTTP;
using UnityEngine;
using Com.PicoVR.Gallery;
using DG.Tweening;

public class GalleryImageItem : ImageItemBase
{
    public ItemClickCallback itemClick = null;
    [SerializeField]
    UILabel mTitle;
    [SerializeField]
    GameObject mBG;
    [SerializeField]
    GameObject mLogo;
    [SerializeField]
    UILabel mLogoText;
    [SerializeField]
    GameObject mProvider;
    [SerializeField]
    UILabel mProviderText;
    Vector3 initPos, targetPos;

    private PhotoModel GData
    {
        get
        {
            return Data as PhotoModel;
        }
    }

    void Start()
    {
        UIEventListener.Get(gameObject).onClick = OnClickImageItem;
        UIEventListener.Get(gameObject).onHover = OnHoverImageItem;

        mTitle.alpha = 0;
        ResetHoverPos();
    }

    public override void Init(PhotoModelBase data)
    {
        if (GData != null && data != null && ((PhotoModel)data) != null
            && GData.MID.Equals(((PhotoModel)data).MID))
        {
            return;
        }
        else
        {
            Data = data;
            mTitle.text = GData.Title;
            ClearMarks();
            SetImageLogo(GData.LogoImg);
            //SetProvider(GData.Provider);
            ResetReadyState();
            DataLoader.Instance.AddImgItem(this);
        }
    }

    private void ClearMarks()
    {
        mLogo.SetActive(false);
        mProvider.SetActive(false);
    }

    private void SetImageLogo(string logo)
    {
        if (logo.Equals(Constant.Tag_New))
        {
            mLogo.SetActive(true);
            mLogoText.text = Localization.Get("Image_Logo_New");
        }
        else if (logo.Equals(Constant.Tag_Cehua))
        {

        }
        else if (logo.Equals(Constant.Tag_Fufei))
        {

        }
        else if (logo.Equals(Constant.Tag_Hot))
        {
            mLogo.SetActive(true);
            mLogoText.text = Localization.Get("Image_Logo_Hot");
        }
        else if (logo.Equals(Constant.Tag_Huiyuan))
        {

        }
        else if (logo.Equals(Constant.Tag_Huodong))
        {

        }
        else if (logo.Equals(Constant.Tag_Tuiguang))
        {

        }
        else if (logo.Equals(Constant.Tag_Zhuanti))
        {

        }
        else if (logo.Equals("0"))
        {
            mLogo.SetActive(false);
            mLogoText.text = "";
        }
    }

    private void SetProvider(string pro)
    {
        if (pro.Equals(Constant.Tag_720Yun))
        {
            mProvider.SetActive(true);
            mProviderText.text = Localization.Get("Image_Provider_720yun");
        }
        else if (pro.Equals(Constant.Tag_CNTraveler))
        {
            mProvider.SetActive(true);
            mProviderText.text = Localization.Get("Image_Provider_CNTraveler");
        }
        else
        {

        }
    }

    public override void LoadTexture()
    {
        if (CachePhotoData.Instance.IsIconTextureExist(GData.MID))
        {
            //Debug.LogError("already exist!");
            SetTexture(CachePhotoData.Instance.GetIconTexture(GData.MID));
            return;
        }
        try
        {
            if (string.IsNullOrEmpty(GData.CoverLink) || GData.CoverLink.Contains("JsonData object")) return;
            System.Uri uri = new System.Uri(GData.CoverLink);
            if (uri != null)
            {
                var request = new HTTPRequest(uri, ImageDownloaded);
                request.Tag = GData.MID;
                request.Send();
            }
        }
        catch (System.Exception)
        {
            Debug.LogError("link:" + GData.CoverLink);
            throw;
        }
    }

    void ImageDownloaded(HTTPRequest req, HTTPResponse resp)
    {
        switch (req.State)
        {
            case HTTPRequestStates.Finished:
                if (resp.IsSuccess)
                {
                    if (req.Tag.Equals(GData.MID))
                    {
                        SetTexture(resp.DataAsTexture2D);
                        CachePhotoData.Instance.AddIconTexture(GData.MID, resp.DataAsTexture2D);
                    }
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

    private void OnClickImageItem(GameObject go)
    {
        if (itemClick != null)
        {
            itemClick(GData);
        }
        else
        {
            GPlayerManager.Instance.Play(GData.MID);
            DataLoader.Instance.ReportHistory(GData.MID, HomePageScreen.CurrentID);
        }
        OnHoverImageItem(this.gameObject, false);
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
            DOTween.To(() => mTitle.alpha, x => mTitle.alpha = x, 1, 1f);
        else
            DOTween.To(() => mTitle.alpha, x => mTitle.alpha = x, 0, 1f);
    }

    public void ResetHoverPos() //hover前移的位置
    {
        initPos = this.gameObject.transform.localPosition;
        targetPos = initPos - (initPos - Vector3.zero) / 10; //朝圆心走
    }
}