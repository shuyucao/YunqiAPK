using UnityEngine;
using System.Collections;
using BestHTTP;
using DG.Tweening;
using CyberCloud.PortalSDK.vo;
using System.Collections.Generic;
using System.IO;
using System.Threading;
/// <summary>
/// 磁贴类
/// </summary>
public class ScrollPhotosItemView : BaseScrollItem
{
    [SerializeField]
    UITexture texture;

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
    bool NeedAnimation = true;
    PhotoModel data;
    Vector3 initPos;
    Vector3 targetPos;
    /// <summary>
    /// 流化显示界面
    /// </summary>
    GameObject gamePanel;
    void Start()
    {
        gamePanel = GameObject.Find("GamePlane");
        if (gamePanel == null)
            MyTools.PrintDebugLogError("ucvr GamePlane pre mast in scene");
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
       // Debug.Log("==================over:"+ onHover);
    }
    //点击磁贴启动应用 启动游戏
    //点击磁贴启动应用 启动游戏
    void onItemClick(GameObject go)
    {
        if (GameAppControl.getGameRuning() == true)
        {
            MyTools.PrintDebugLog("ucvr is gameing can not change next ");
            return;
        }
       
        // Debug.LogError("data.Title   =====  " + data.Title);
           GameAppControl.AppName = data.Title;
        MyTools.PrintDebugLog("ucvr call startApp:"+ data.Title);
        if (DataLoader.isLoadTls)
            return;
        
       

            DataLoader.Instance.mLoadUI.SetActive(true);
            DataLoader.Instance.startAppById(data.MID);
            onItemHover(this.gameObject, false);
        
    }
 

    private void InfoAlpha(bool isShow) //显示名字栏
    {
        if (isShow)
            DOTween.To(() => mTitle.alpha, x => mTitle.alpha = x, 1, 1f);
        else
            DOTween.To(() => mTitle.alpha, x => mTitle.alpha = x, 0, 1f);
    }
    public override void refreshItem(int pageindex)
    {
        base.refreshItem(pageindex);
        if (data == null)
        {
            return;
        }
        int currentRow = SCDataCount(currnetIndex);
        int currentPageIndex = SCDataPageCount(currentRow);
        if (currentPageIndex == pageindex || (currentPageIndex == pageindex + 1))
            loadPic();

    }
    //通过当前行获取当前页
    public int SCDataPageCount(int rownums)
    {
        int count = 0;
        if (rownums > 0)
        {
            count = rownums / 2;
            //Debug.LogError("mDataList.Count:" + mDataList.Count + ";columnCount:" + columnCount);
            if (rownums % 2 > 0)
                count += 1;

        }

        return count;
    }
    //获取当前行
    public int SCDataCount(int index)
    {
        int count = 0;
        if (index > 0)
        {
            count = index / 3;
            //Debug.LogError("mDataList.Count:" + mDataList.Count + ";columnCount:" + columnCount);
            if (index % 3 > 0)
                count += 1;

        }
        return count;
    }
    public override void clear()
    {
        texture.mainTexture = null;
        base.clear();
    }
    //当前索引
    private int currnetIndex = 0;
    public override void UpdateData(int index)
    {
        base.UpdateData(index);
        currnetIndex = index;

        if (index >= 6) NeedAnimation = false;
        //else NeedAnimation = true;
        data = itemData as PhotoModel;
        if (data == null)
        {
            Debug.Log("data is null  " + index);
            return;
        }
        //Debug.Log("data.Title   =====  "+ data.Title);
        mTitle.text = data.Title;
        mTitle.alpha = 0;
        //SetImageLogo(data.LogoImg);
        mProvider.SetActive(false);
        //SetProvider(data.Provider);
        //只渲染首页 其他页面翻页时渲染
        if (index < 9)
            loadPic();


    }
    private void loadPic()
    {
        if (texture.mainTexture != null)
            return;
        if (CachePhotoData.Instance.IsIconTextureExist(data.MID))
        {
            texture.mainTexture = CachePhotoData.Instance.GetIconTexture(data.MID);
            if (NeedAnimation)
                TextureShowAnim();
            return;
        }
        else
            try
            {

                if (string.IsNullOrEmpty(data.ThumbnailLink) || data.ThumbnailLink.Contains("JsonData object")) return;
                System.Uri uri = new System.Uri(data.ThumbnailLink);
                if (uri != null)
                {
                    //Debug.Log("=======ucvr loadurl:"+uri);
                    //var request = new HTTPRequest(uri, ImageDownloaded);
                    //request.Tag = data.MID;
                    //request.Send();
                       int picversionindex = uri.ToString().LastIndexOf("v=");
                    string picoVersion = picversionindex>0? uri.ToString().Substring(picversionindex+2) : MyTools.getVersionCode();
                    //string mid = data.MID + "_" + MyTools.getVersionCode();
                    string mid = data.MID + "_" + picoVersion;

                    loadItemPic(uri, mid);
                }
            }
            catch (System.Exception)
            {
                Debug.LogError("link:" + data.ThumbnailLink);
                throw;
            }

    }
    private void loadItemPic(System.Uri url, string mid)
    {
        string dir = Application.persistentDataPath + "/MyItemImages";

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        if (!File.Exists(dir + "/" + mid))
        {
            StartCoroutine(LoadThumbnail(url, mid));
        }
        else
        {
            //StartCoroutine(LoadThumbnail(url, mid));
            StartCoroutine(LoadLocalImage(mid));
        }


    }
    //private Texture2D getMyPicTexture(Texture2D t2)
    private Texture2D getMyPicTexture(byte[] bytedata)
    {
        if (bytedata != null)
        {
            //Texture2D temp = t2;
            //byte[] bytedata = temp.EncodeToPNG();
            Texture2D mytexture = new Texture2D(40, 20, UnityEngine.TextureFormat.ARGB32, true);
            mytexture.LoadImage(bytedata);
            mytexture.wrapMode = TextureWrapMode.Clamp;
            mytexture.filterMode = FilterMode.Bilinear;
            mytexture.Apply();
            return mytexture;
        }

        return null;
    }
    /// <summary>
    /// 从本地缓存中读取
    /// </summary>
    /// <param name="url"></param>
    /// <param name="mid"></param>
    /// <returns></returns>
    private IEnumerator LoadLocalImage(string mid)
    {
        // 已在本地缓存  
        string filePath = "file:///" + Application.persistentDataPath + "/MyItemImages/" + mid;
        WWW www = new WWW(filePath);
        yield return www;



        texture.mainTexture = www!=null?getMyPicTexture(www.bytes):null;
        //t = www.texture;
        //texture.mainTexture.
        if (NeedAnimation)
            TextureShowAnim();
        //Thread thread1 = new Thread(loadTexture);
        //thread1.Start();
    }
    private Texture2D t;
    private void loadTexture()
    {
        texture.mainTexture = t;
    }
    /// <summary>
    /// 网络中加载图片并缓存
    /// </summary>
    /// <param name="url"></param>
    /// <param name="mid"></param>
    /// <returns></returns>
    private IEnumerator LoadThumbnail(System.Uri url, string mid)
    {
        WWW www = new WWW((url).AbsoluteUri);
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.Log(string.Format("Failed to load image: {0}, {1}", url, www.error));
            yield break;
        }
        //Texture2D image = www.texture;
        //    将图片保存至缓存路径  
        byte[] pngData = www.bytes;// image.EncodeToPNG();
        File.WriteAllBytes(Application.persistentDataPath + "/MyItemImages/" + mid, pngData);
        texture.mainTexture = getMyPicTexture(pngData);
        if (NeedAnimation)
            TextureShowAnim();

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
                        if (NeedAnimation)
                            TextureShowAnim();
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
                Debug.LogError("request error:" + req.Uri);
                // Debug.LogError("Request Finished with Error! " + (req.Exception != null ? (req.Exception.Message + "\n" + req.Exception.StackTrace) : "No Exception"));
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
            Debug.LogWarning("No flag found++" + pro);
        }
    }
}
