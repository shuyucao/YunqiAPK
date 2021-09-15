using UnityEngine;
using System.Collections;
using BestHTTP;
using DG.Tweening;

public class ScrollThemesPhotosView : BaseScrollItem
{
    [SerializeField]
    UITexture texture;
    bool NeedAnimation = true;
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
    PhotoModel data;
    Vector3 initPos;
    Vector3 targetPos;

    void Start()
    {
        UIEventListener.Get(this.gameObject).onClick = onItemClick;
        UIEventListener.Get(this.gameObject).onHover = onItemHover;
        initPos = this.gameObject.transform.localPosition;
        targetPos = new Vector3(initPos.x, initPos.y, initPos.z-80);
    }
    private void onItemHover(GameObject go, bool onHover)
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
    void onItemClick(GameObject go)
    {
        Debug.Log("Data.Title  :  " + data.Title);
        Debug.Log("Show Photo list !!!!!");
        GPlayerManager.Instance.Play(data.MID);
        if(MachineState.IsWifiAvailable)
            DataLoader.Instance.ReportHistory(data.MID, HomePageScreen.CurrentID);
        onItemHover(this.gameObject, false);
    }
    public override void UpdateData(int index)
    {
        base.UpdateData(index);


        data = itemData as PhotoModel;
        if (index >= 6) NeedAnimation = false;
        if (data == null)
        {
            Debug.Log("data is null  " + index);
            return;
        }
        //Debug.Log("data.Title   =====  "+ data.Title);
        mTitle.text = data.Title;
        mTitle.alpha = 0;
        SetImageLogo(data.LogoImg);
        mProvider.SetActive(false);
        SetProvider(data.Provider);

        if (CachePhotoData.Instance.IsIconTextureExist(data.MID))
        {
            texture.mainTexture = CachePhotoData.Instance.GetIconTexture(data.MID);
            if (NeedAnimation) TextureShowAnim();
            return;
        }

        try
        {
            if (string.IsNullOrEmpty(data.ThumbnailLink) || data.ThumbnailLink.Contains("JsonData object")) return;
            System.Uri uri = new System.Uri(data.ThumbnailLink);
            if (uri != null)
            {
                var request = new HTTPRequest(uri, ImageDownloaded);
                request.Tag = data.MID;
                request.Send();
            }
        }
        catch (System.Exception)
        {
            Debug.LogError("link:" + data.ThumbnailLink);
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
                    //Debug.Log(req.Tag +" "+data.MID);
                    if (req.Tag.Equals(data.MID))
                    {
                        texture.mainTexture = resp.DataAsTexture2D;
                        if (NeedAnimation) TextureShowAnim();
                    }
                    CachePhotoData.Instance.AddIconTexture(data.MID, resp.DataAsTexture2D);
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
        else if (pro.Equals(Constant.Tag_AP))
        {
            mProvider.SetActive(true);
            mProviderText.text = Localization.Get("Image_Logo_AP");
        }
        else if (pro.Equals(Constant.Tag_Pico))
        {
            mProvider.SetActive(true);
            mProviderText.text = "Pico";
        }
        else
        {
        }
    }
}
