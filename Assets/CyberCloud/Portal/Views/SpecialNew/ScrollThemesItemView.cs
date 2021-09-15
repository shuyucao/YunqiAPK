using UnityEngine;
using System.Collections;
using BestHTTP;
using DG.Tweening;

public class ScrollThemesItemView : BaseScrollItem
{
    [SerializeField]
    UILabel Title;
    [SerializeField]
    GameObject mBG;
    [SerializeField]
    UILabel RT;
    [SerializeField]
    UITexture texture;
    bool NeedAnimation = true;
    ThemesModel data;
    Vector3 initPos;
    Vector3 targetPos;

    void Start()
    {
        UIEventListener.Get(this.gameObject).onClick = onItemClick;
        UIEventListener.Get(this.gameObject).onHover = onItemHover;
        initPos = this.gameObject.transform.localPosition;
        targetPos = new Vector3(initPos.x, initPos.y, initPos.z - 80);
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
            DOTween.To(() => Title.alpha, x => Title.alpha = x, 1, 1f);
        else
            DOTween.To(() => Title.alpha, x => Title.alpha = x, 0, 1f);
    }
    void onItemClick(GameObject go)
    {
        Debug.Log("Themes.Title  :  " + data.Title);
        Debug.Log("Themes.ThemeID  :  " + data.ThemeID);

        CategoryPhotoData cdata = CachePhotoData.Instance.GetThemesPhotosByID(data.ThemeID);
        if (cdata == null || cdata.PhotoList.Count == 0)
        {
            DataLoader.Instance.RequestThemesPhoto(data.ThemeID);
        }
        else
        {
            Bundle bundle = new Bundle();
            bundle.SetValue<string>("themeID", data.ThemeID);
            MsgManager.Instance.SendMsg(MsgID.ThemePhotoDataRefresh, bundle);
        }
        onItemHover(this.gameObject, false);
    }
    public override void UpdateData(int index)
    {
        base.UpdateData(index);

        data = itemData as ThemesModel;
        if (index >= 6) NeedAnimation = false;
        if (data == null)
        {
            Debug.Log("data is null  " + index);
            return;
        }
        //Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ " + " pp == "+ data.Provider);
        Title.text = data.Title;
        Title.alpha = 0;
        SetProvider(data.Provider);
        try
        {
            if (string.IsNullOrEmpty(data.Cover) || data.Cover.Contains("JsonData object")) return;
            System.Uri uri = new System.Uri(data.Cover);
            if (uri != null)
            {
                var request = new HTTPRequest(uri, ImageDownloaded);
                //request.Tag = item;
                request.Send();
            }
        }
        catch (System.Exception)
        {
            Debug.LogError("link:" + data.Cover);
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
                    Debug.Log(req.Tag + " " + data.ThemeID);
                    //if (req.Tag == this.gameObject.name)
                    {
                        texture.mainTexture = resp.DataAsTexture2D;
                        if (NeedAnimation) TextureShowAnim();
                    }
                    //CachePhotoData.Instance.AddIconTexture(data.MID, resp.DataAsTexture2D);
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
    private void SetProvider(string pro)
    {
        if (pro.Equals(Constant.Tag_720Yun))
        {
            RT.text = Localization.Get("Image_Provider_720yun");
        }
        else if (pro.Equals(Constant.Tag_CNTraveler))
        {
            RT.text = Localization.Get("Image_Provider_CNTraveler");
        }
        else if (pro.Equals(Constant.Tag_AP))
        {
            RT.text = Localization.Get("Image_Logo_AP");
        }
        else if (pro.Equals(Constant.Tag_Pico))
        {
            RT.text = "Pico";
        }
        else
        {
            RT.text = "";
        }
    }
}
