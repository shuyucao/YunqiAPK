using BestHTTP;
using Com.PicoVR.Gallery;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CyberCloud.PortalSDK;
using CyberCloud.PortalSDK.vo;
using System.Threading;
using System;
using System.IO;
using Assets.CyberCloud.Scripts.OpenApi;

/*
 * DataLoader
 * manage all images load task
 */
public class DataLoader : MonoBase
{
    /// <summary>
    /// 定义分类加载结束的事件，此事件会有leftMenu处理
    /// </summary>
    /// <param name="categoryData"></param>
    public delegate void OnGetAllClassOver(CategoryData categoryData);
    public OnGetAllClassOver onGetAllClassOver;
    /// <summary>
    /// 左侧菜单加载结束
    /// </summary>
    public delegate void OnLeftMenuLoadOver();
    public OnLeftMenuLoadOver onLeftMenuLoadOver;
    public delegate void OnGetItemListByCategory(RetAppListInfo appListInfo);
    public OnGetItemListByCategory onGetItemListByCategory;
    public delegate void OnGetTenantList(RctTenantList tenantList);
    public OnGetTenantList onGetTenantList;
    private static DataLoader mInstance = null;
	public string mCategoryName = null;
	public PortalAPI portalAPI = null;
	private GalleryNetApi mNetAPI = new GalleryNetApi();
	private CyberCloud.PortalSDK.vo.LevelClass mLevelList;
    private List<LevelClass> childCategory;
    /// <summary>
    /// 是否已经初始化
    /// </summary>
    public InitStatus initStatus = InitStatus.idle;
    ///portal初始化结果 是否初始化成功
    public bool portalIintRet = false;
    /// <summary>
    /// 初始化状态 空闲，初始化中 结束
    /// </summary>
    public enum InitStatus
    {
        idle,
        initing,
        initOver
    }
    public class CategoryData {
        public List<LevelClass> childCategory;
        public bool is6dof = false;
    }
    private CategoryData getData() {
        CategoryData categoryData = new CategoryData();
        categoryData.childCategory = childCategory;
        categoryData.is6dof = is6dof();
        return categoryData;
    }
    public static DataLoader Instance
    {
        get
        {
            if (mInstance == null)
            {
                GameObject go = new GameObject("ImageLoader");
                mInstance = go.AddComponent<DataLoader>();
            }
            return mInstance;
        }
    }
    /// <summary>
    /// 通过appid获取流化启动串
    /// </summary>
    PortalAPI.StartCloudInfo startCloudInfo = null;

    private bool getTlsParamOver = false;
    public static bool isLoadTls = false;
    private string temp_appid = "";
    GameObject gamePanel;
    private void getTlsParam()
    {
        try
        {
            int loadTimes = 0;
            getTlsParamOver = false;
            startCloudInfo = null;
            while ((startCloudInfo == null || startCloudInfo.retCode!=0) && loadTimes < 3)
            {

                startCloudInfo = DataLoader.Instance.portalAPI.getStartCloudInfo(temp_appid);
                loadTimes += 1;
            }
            getTlsParamOver = true;
        }
        catch (System.Exception e)
        {
            getTlsParamOver = true;

            Debug.LogError("getTlsParam error:" + e.Message);

        }
        //Debug.Log("threaInit over:"+ bInitRet);
    }
    /// <summary>
    /// 此为无portal展示的场景应用启动调用
    /// apk启动后不展示portal直接启动应用，需要等初始化完成（ onInitDataOver）后进行应用启动
    /// </summary>
    /// <param name="appID"></param>
    public void skipPortalStartAppByIdOnAppStart(GetStartAppUrlFromCVRGatewayParam param)
    {
        getStartAppUrlFromCVRGatewayParam = param;
        startAppById(param.appID);
    }
    /// <summary>
    /// 通过appID启动应用，应用启动前需要判断apk的版本号 如果需要升级先进行升级操作
    /// </summary>
    /// <param name="appID"></param>
    public void startAppById(String appID) {
        temp_appid = appID;
        if (installApk != null && installApk.downloadNewVersion>-1) {
            installApk.showupdateapkdialog(installApk.downloadNewVersion);
            return;
        }
        if (CyberCloudConfig.ExportOnlyPlayer == true||OpenApiImp.CyberCloudOpenApiEnable)
        {//通过appid直接启动应用
            PortalAPI.getCybertlsPath();//获取系统路径不能在线程中获取所有此处在主线程中获取
            StartCoroutine(asyApiCall(ApiType.init));
        }
        else
            StartCoroutine(asyStartApp(appID));
    }
    public void getTenantList(String tenantId)
    {
        StartCoroutine(asyApiCall(ApiType.getTenantList, tenantId));
    }
        //异步启动
    public IEnumerator asyStartApp(string appid)
    {
        MyTools.PrintDebugLog("ucvr getTlsParam ");

        isLoadTls = true;
       
        getTlsParamOver = false;
        Thread workThread = new Thread(getTlsParam);

        //HttpThreadClose = false;
        if (workThread != null)
            workThread.Start();
        while (getTlsParamOver == false)
        {
            yield return new WaitForEndOfFrame();//等到该帧结束
        }
        //if (getTlsParamOver)//请求结束
        //{
        MyTools.PrintDebugLog("ucvr getTlsParam.apiAsynCallOver");

        getTlsParamOver = false;

        startAppByStartUrl();

    }
    private void startAppByStartUrl() {
        if (startCloudInfo != null && startCloudInfo.retCode == 0)
        {
            string tempStartCloudUrl = "";
            string startCloudUrl = startCloudInfo.startUrl;
            //将流化串的backappid置空这样 流化应用内主动退出就不会退回到流化portal了
            if (startCloudUrl != null && startCloudUrl.IndexOf("&BackAppID=") > 0)
                tempStartCloudUrl = startCloudUrl.Replace("&BackAppID=", "&BackAppID =-1&UcvrDelecte=");
            else
                tempStartCloudUrl = startCloudUrl;

            if (gamePanel == null)
                gamePanel = GameObject.Find("GamePlane");
            gamePanel.GetComponent<GameAppControl>().startApp(tempStartCloudUrl);
        }
        else
        {
            if (OpenApiImp.CyberCloudOpenApiEnable)
                gamePanel.GetComponent<GameAppControl>().startAppException(OpenApiImp.ErrorCodeOpenApi.gatewayFailed.ToString());
            else
            {
                commonPlaneCom = getCommonPlane();
                if (startCloudInfo != null)
                    commonPlaneCom.showDialogOneBtExitDialog("Home_NoNet", startCloudInfo.retCode.ToString());
                else
                    commonPlaneCom.showDialogOneBtExitDialog("Home_NoNet", "110001");
            }
        }
        isLoadTls = false;
        if (mLoadUI != null)
            mLoadUI.SetActive(false);
    }

    CommonPlane commonPlaneCom;
    private CommonPlane getCommonPlane()
    {
        GameObject commonPlane = GameObject.Find("CyberCloudCommonPlane");
        if (commonPlane == null)
            Debug.LogError("ucvr commonPanel mast add to screen");
        commonPlaneCom = commonPlane.GetComponent<CommonPlane>();
        if (commonPlaneCom == null)
            Debug.LogError("ucvr CyberCloudCommonPlane mast contain CommonPlane");
        return commonPlaneCom;
    }

    private bool is6dof() {
        if (isTest)
            return true;
        CyberCloud_UnitySDKAPI.HeadBoxDofType mode = ControllerTool.getDofType();
        if (mode == CyberCloud_UnitySDKAPI.HeadBoxDofType.Dof3)
        {
            return false;
        }
        else
        {
            return true;
        }

    }

    private List<LevelClass>  subStringCategoryName3dof(List<LevelClass> tempchildCategory)
    {
        if (tempchildCategory == null)
            return null;
        for (int j = 0; j < tempchildCategory.Count; j++)
        {
            LevelClass c = tempchildCategory[j];
            int nameL = c.categoryName.Length;

            //末尾以3结束，去掉末尾字符
            if (c.categoryName.LastIndexOf("3") == nameL - 1|| c.categoryName.LastIndexOf("6") == nameL - 1)
            {
                c.categoryName = c.categoryName.Substring(0, nameL - 1);
            }
                       
        }
        return tempchildCategory;
    }
  
    /// <summary>
    /// 游戏列表显示的是否是3dof游戏列表
    /// </summary>
    public static bool gameDofType3 = true;
 
    private bool needDof3Game() {
        if (!is6dof())//3dof头盔
        {

         
            //MyTools.PrintDebugLog("ucvr 3dof hmd  mast insert nolo before into game ");
            gameDofType3 = true;
          

        }
        else
            gameDofType3 = false;
        return gameDofType3;
    }

    private void filterData()
    {
        gameDofType3=needDof3Game();
        MyTools.PrintDebugLog("ucvr head type3:"+ gameDofType3);
        childCategory = null;
        List<LevelClass> tempchildCategory = mLevelList.childCategory;
        if (tempchildCategory != null)
        {

            for (int j = 0; j < tempchildCategory.Count; j++)
            {
                LevelClass c = tempchildCategory[j];
                int nameL = c.categoryName.Length;
                //3dof的头盔分类
                if (c.categoryName.IndexOf("3dof") > -1 || c.categoryName.LastIndexOf("3") == nameL - 1)
                {
                    if (!is6dof()&& gameDofType3 == true)
                    {
                        if (childCategory == null)
                            childCategory = new List<LevelClass>();

                        childCategory.Add(c);
                    }
                }
                else
                {
                    //6dof头盔或大鹏头盔展示6dof游戏
                    if (is6dof()|| gameDofType3==false)
                    {
                        if (childCategory == null)
                            childCategory = new List<LevelClass>();
                        childCategory.Add(c);
                    }
                }
            }
            childCategory = subStringCategoryName3dof(childCategory);
        }
        else
        {
            MyTools.PrintDebugLogError("ucvr child class is null");
            string msg = Localization.Get("Home_NoNet");
            WingToastManager.Instance.Show(msg);
        }
    }
    public static bool NotReachable = false;
    public bool isTest = false;
    public GameObject mLoadUI;
    public void showLoad(bool show) {
        if (show)
            mLoadUI.SetActive(true);
        else
            mLoadUI.SetActive(false);
    }
    /// <summary>
    /// 重新初始化
    /// </summary>
    public void reInit()
    {
        NotReachable = false;
        //
        portalIintRet = false;
        initStatus = InitStatus.idle;
        Init(mLoadUI);
    }
    public void Init(GameObject mLoadUI)
    {
        this.mLoadUI = mLoadUI;
        //Debug.LogError("ucvr init");
        if (initStatus==InitStatus.initing) return;
        NotReachable = false;
        initStatus = InitStatus.initing;
        GalleryNetApi.OnDataCallbackDelegate += OnDataCallback;
        //StartLoad();
        MyTools.PrintDebugLog("ucvr startInit CyberCloudConfig.ExportOnlyPlayer:" + CyberCloudConfig.ExportOnlyPlayer + ";cvrScreen:"
            + CyberCloudConfig.cvrScreen + ";tls:" + CyberCloudConfig.tls+";cvrLanguage:" + CyberCloudConfig.cvrLanguage);
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            NotReachable = true;
            MyTools.PrintDebugLogError("ucvr network is NotReachable can not get data form cvr backend");
            initStatus = InitStatus.idle;
            mLoadUI.SetActive(true);
            return;
        }
        else
        {
#if UNITY_ANDROID && !UNITY_EDITOR
                string wifi= MyTools.getWifiInfo();
                MyTools.PrintDebugLog("ucvr start init  wifi:" + wifi);
#endif
        }
       
        mLoadUI.SetActive(true);
        if (isTest)
            PortalAPI.startDebugMode();
        PortalAPI.getCybertlsPath();//获取系统路径不能在线程中获取所有此处在主线程中获取
        if(portalAPI==null)
        portalAPI = new PortalAPI();
        if (CyberCloudConfig.ExportOnlyPlayer==false&& OpenApiImp.CyberCloudOpenApiEnable==false)
            StartCoroutine(asyApiCall(ApiType.init));
        try
        {
            installApk = new InstallApk();
            installApk.startDownLoad();
       

        }
        catch (Exception e) {
            MyTools.PrintDebugLogError("ucvr loadfile error:"+e.Message);
        }
  
    }
    InstallApk installApk;
  
    
    
   
        ///
        int initNum = 0;
    /// <summary>
    /// 初始化结果，如果初始化失败再次尝试初始化，如果初始化成功非快速启动模式需要获取分类，如果是快速启动模式需要进行应用启动
    /// 应用启动需要判断当网关模式或OpenApi模式由于初始化成功时已经通过网关获取到了启动地址，因此直接进行启动应用，否则需要使用appid通过tls获取启动串
    /// </summary>
    /// <param name="bInitRet"></param>
    private void onInitDataOver(bool bInitRet) {
        MyTools.PrintDebugLog("ucvr PortalAPI init result:" + bInitRet);
        portalIintRet = bInitRet;
        initStatus = InitStatus.initOver;
        if (bInitRet)
        {
            MyTools.PrintDebugLog("ucvr start getClasses or startApp");

            /**每次版本升级需要清空图片缓存列表**/
            string dir = Application.persistentDataPath + "/MyItemImages";
            if (Directory.Exists(dir))
            {
                FileInfo file = new FileInfo(dir + "/" + "_" + MyTools.getVersionCode());
                if (!file.Exists)
                {//每个版本清空一次缓存目录
                    MyTools.DeleteFolder(dir);
                    file.Create();
                }
            }
            if (CyberCloudConfig.TestAutoStartAppID != null && CyberCloudConfig.TestAutoStartAppID != "") {
                //StartCloudAppTest startCloudAppTest = new StartCloudAppTest();
                StartCloudAppTest startCloudAppTest = GameObject.Find("testAutoStartApp").GetComponent<StartCloudAppTest>();
                startCloudAppTest.startAppById(CyberCloudConfig.TestAutoStartAppID);
            }

            if (CyberCloudConfig.ExportOnlyPlayer == true || OpenApiImp.CyberCloudOpenApiEnable)
            { //通过appid直接启动应用
                if ( CyberCloudConfig.tls.IndexOf(GameAppControl.gatewayPort) > -1||OpenApiImp.CyberCloudOpenApiEnable)//判断终端配置的是否是网关地址
                {
                  
                    startAppByStartUrl();
                }
                else
                    StartCoroutine(asyStartApp(temp_appid));
            }
            else
                StartCoroutine(asyApiCall(ApiType.getClasses));
        }
        else
        {
            initNum = initNum + 1;
            if (initNum > 3)
            {
            	if(mLoadUI!=null)
                mLoadUI.SetActive(false);
                if (OpenApiImp.CyberCloudOpenApiEnable) {
                    OpenApiImp.getOpenApi().notify().systemStatusCallback(OpenApiImp.systemException,OpenApiImp.ErrorCodeOpenApi.gatewayFailed.ToString());
                    return;
                }
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    NotReachable = true;
                    MyTools.PrintDebugLogError("ucvr ***NotReachable***");
                }
                else
                {
                    //MyTools.PrintDebugLogError("ucvr init failed ip:" + Network.player.ipAddress);
                    string initfailed = Localization.Get("Home_NoNet");
                    // WingToastManager.Instance.Show(initfailed);
                    commonPlaneCom = getCommonPlane();
                    commonPlaneCom.showDialogOneBtExitDialog("Home_NoNet", "110002");

                }
            }
            else {
                Debug.LogError("ucvr "+initNum+"times init failed,repeat init next");
                StartCoroutine(asyApiCall(ApiType.init));
            }
           
        }
    }
    private void onGetAllLevelsClassesOver(RetAllLevelsClasses levels)
    {
        if (levels.retCode == 0)
        {
            List<LevelClass> list = levels.data;
            if (list.Count > 0)
            {
                foreach (LevelClass i0 in list)
                {
                     if (CyberCloudConfig.classificationOfProfessions == i0.categoryType)
                    {
                        mLevelList = i0;
                        filterData();

                        break;
                    }
                }
            }
            else
            {
                string msg = Localization.Get("Home_NoNet");
                WingToastManager.Instance.Show(msg);
            }
        }
        else
        {
            portalIintRet = false;

            string msg = Localization.Get("Home_NoNet");
            WingToastManager.Instance.Show(msg + levels.retCode);
        }
        if (onGetAllClassOver != null)
        {
            mLoadUI.SetActive(false);
            onGetAllClassOver(getData());
            //Debug.Log("onLeftMenuLoadOver 1");
            //if (onLeftMenuLoadOver!=null) {
              //  Debug.Log("onLeftMenuLoadOver true" );
              //  onLeftMenuLoadOver();
            //}
        }
    }
#region data Request & dispatch
    //On the data callback, the callback when request finshed
    private void OnDataCallback(HTTPRequest req, HTTPResponse resp)
    {
        //Debug.LogError("OnDataCallback");
        bool dispatchSuccess = false;
        //string key = req.Tag as String;
        string key = req.Tag as string;
        switch (req.State)
        {
            // The request finished without any problem.
            case HTTPRequestStates.Finished:
                if (!resp.IsSuccess)
                {
                    Debug.LogWarning("OnDataCallback-> statusCode= " + resp.StatusCode + " Message= " + resp.Message + "result= " + resp.DataAsText);
                }
                dispatchSuccess = DispatchResult(key, resp.DataAsText);
                break;
            // The request finished with an unexpected error. The request's Exception property may contain more info about the error.
            case HTTPRequestStates.Error:
                Debug.LogWarning("OnDataCallback->Request Finished with Error! " + (req.Exception != null ? (req.Exception.Message + "\n" + req.Exception.StackTrace) : "No Exception"));
                break;
            // The request aborted, initiated by the user.
            case HTTPRequestStates.Aborted:
                Debug.LogWarning("OnDataCallback->Request Aborted!");
                break;
            // Ceonnecting to the server is timed out.
            case HTTPRequestStates.ConnectionTimedOut:
                Debug.LogWarning("OnDataCallback->Connection Timed Out!");
                break;
            // The request didn't finished in the given time.
            case HTTPRequestStates.TimedOut:
                Debug.LogWarning("OnDataCallback->Processing the request Timed Out!");
                break;
            default:
                Debug.LogError("OnDataCallback->Unknown Error!");
                break;
        }
        bool success = false;
        if (req.State == HTTPRequestStates.Finished && dispatchSuccess == true)
        {
            success = true;
            CommonAlert.Clear();
        }
        if (!success)
        {
            OprateRequestError(key);
        }
    }

    bool DispatchResult(string key, string result)
    {
        Debug.Log("DispatchRequest->this is callback of key : " + key);
        bool ret = false;
        int message = 0;
        int total_num = 0;
        int current_num = 0;
        TagType ty = DispatchTag(key);
        switch (ty)
        {
            case TagType.GetCategory:
                List<CategoryModel> newCategoryList = JsonUtils.ParseCategoryJson(result, ref message);
                if (newCategoryList == null)
                {
                    Debug.LogError("DispatchResult funGetCategory newCategoryList is null !");
                }
                else
                {
                    CachePhotoData.Instance.InitCategory(newCategoryList);
                    ret = true;
                }
                break;
            case TagType.GetCategoryPhoto:
                //List<PhotoModel> newListFirst = JsonUtils.ParsePhotoJson(result, ref message, ref total_num, ref current_num);
                ////Debug.LogError("totale page:" + total_num + "current page:" + current_num);
                //if (newListFirst == null)
                //{
                //    Debug.LogError("DispatchResult funGetCategory newCategoryList is null !");
                //}
                //else
                //{
                    //string[] param = key.Split('_');
                    //if (param.Length != 2)
                    //{
                    //    Debug.Log("the req key has some elemnt lose!");
                    //}
                    //else
                    //{
                        //Debug.LogError("SetCatPhoModleData:" + current_num);
                        CachePhotoData.Instance.SetCatPhoModleData(null, total_num, current_num, null);
                        ret = true;
                    //}
                //}
                break;
            case TagType.AccessDimensionDoor:
                List<PhotoModel> list = JsonUtils.ParaseDimensionDoor(result, ref message);
                //Debug.LogError("totale page:" + total_num + "current page:" + current_num);
                if (list == null)
                {
                    Debug.LogError("DispatchResult AccessDimensionDoor list is null !");
                }
                else
                {
                    CachePhotoData.Instance.SetDimensionDoorData(list);
                    ret = true;
                }
                break;
            case TagType.GetThemes:
                List<ThemesModel> tlist = JsonUtils.ParaseThemesJson(result, ref message);
                //Debug.LogError("totale page:" + total_num + "current page:" + current_num);
                if (tlist == null)
                {
                    Debug.LogError("DispatchResult ThemesModel list is null !");
                }
                else
                {
                    CachePhotoData.Instance.InitThemes(tlist);
                    ret = true;
                }
                break;
            case TagType.GetThemesPhoto:
                List<PhotoModel> plist = JsonUtils.ParseThemesPhotoJson(result, ref message);
                //Debug.LogError("totale page:" + total_num + "current page:" + current_num);
                if (plist == null)
                {
                    Debug.LogError("DispatchResult funGetCategory newCategoryList is null !");
                }
                else
                {
                    string[] param = key.Split('_');
                    if (param.Length != 2)
                    {
                        Debug.Log("the req key has some elemnt lose!");
                    }
                    else
                    {
                        //Debug.LogError("SetCatPhoModleData:" + current_num);
                        CachePhotoData.Instance.SetThemePhoModleData(param[1], plist);
                        ret = true;
                    }
                }
                break;
            case TagType.CheckUpdate:
                int upinfo = JsonUtils.ParseVersionUpdate(result);
                ret = (upinfo != 0);
                Bundle bundle = new Bundle();
                bundle.SetValue<int>("VersionInfo", upinfo);
                MsgManager.Instance.SendMsg(MsgID.CheckUpdateInfo, bundle);
                break;
            default:
                ret = false;
                break;
        }
        return ret;
    }

    private enum TagType
    {
        UnKnow = 0,
        GetCategory = 1,
        GetCategoryPhoto = 2,
        AccessDimensionDoor = 3,
        GetThemes = 4,
        GetThemesPhoto = 5,
        CheckUpdate = 6,
    }

    private TagType DispatchTag(string TAG)
    {
        TagType ty = TagType.UnKnow;
        if (TAG == GalleryNetApi.funGetCategory)
        {
            ty = TagType.GetCategory;
        }
        else if (TAG.Contains(GalleryNetApi.funGetCategoryPhoto))
        {
            ty = TagType.GetCategoryPhoto;
        }
        else if (TAG.Contains(GalleryNetApi.funAccessDimensionDoor))
        {
            ty = TagType.AccessDimensionDoor;
        }
        else if (TAG.Contains(GalleryNetApi.funGetThemes))
        {
            ty = TagType.GetThemes;
        }
        else if (TAG.Contains(GalleryNetApi.funGetThemesPhoto))
        {
            ty = TagType.GetThemesPhoto;
        }
        else if (TAG.Contains(GalleryNetApi.funCheckUpdate))
        {
            ty = TagType.CheckUpdate;
        }
        return ty;
    }

    private void OprateRequestError(string key)
    {
        TagType ty = DispatchTag(key);
        switch (ty)
        {
            case TagType.UnKnow:
                break;
            case TagType.GetCategory:
            case TagType.GetCategoryPhoto:
            case TagType.GetThemes:
            case TagType.GetThemesPhoto:
                MsgManager.Instance.SendMsg(MsgID.CheckRefreshPage, null);
                break;
            case TagType.AccessDimensionDoor:
                //DataLoader.Instance.RequestDimensionDoor();
                break;
            case TagType.CheckUpdate:
                //TODO
                MsgManager.Instance.SendMsg(MsgID.CheckUpdateInfo, null);
                break;
            default:
                break;
        }
    }
#endregion

#region category
    public void RequestCategoryData()
    {
		Debug.Log("11111111111122222222222222223333333333333333344444444444444444444: " + mCategoryName);
		//mNetAPI.GetCategory(GalleryNetApi.funGetCategory);
    }
#endregion

#region category photo data load manage
    public void RequestPhotoDataByID(string caid)
    {
        string key = GalleryNetApi.funGetCategoryPhoto + "_" + caid;
        //mNetAPI.GetCategoryPhoto(key, caid, Constant.MaxNumOfOnceRequest.ToString(), "1");
        Debug.Log("key:" + key + " caid:" + caid + " limit:" + Constant.MaxNumOfOnceRequest + " page:" + 1);
    }

    //abandon this way
    public void RequestNextPage()
    {
    }
#endregion

#region theme
    public void RequestThemes()
    {
        mNetAPI.GetThemes(GalleryNetApi.funGetThemes, Constant.pageMaxThemes);
    }

    public void RequestThemesPhoto(string tid)
    {
        mNetAPI.GetThemesPhoto(GalleryNetApi.funGetThemesPhoto + "_" + tid, tid, Constant.pageMaxThemesPhoto);
    }
#endregion

#region image load manage
    private Queue<ImageItemBase> mLoadQueue = null;
    // private Dictionary<string, ImageItemBase> mLoadedDict = new Dictionary<string, ImageItemBase>();
    private PhotoModel CurrentLoad = null;
    public void LoadTexture(PhotoModel data)
    {
        if (CachePhotoData.Instance.IsIconTextureExist(data.MID) || CurrentLoad != null || data == null)
        {
            return;
        }
        else
        {
            CurrentLoad = data;
        }
        try
        {
            if (string.IsNullOrEmpty(data.CoverLink) || data.CoverLink.Contains("JsonData object")) return;
            System.Uri uri = new System.Uri(data.CoverLink);
            if (uri != null)
            {
                var request = new HTTPRequest(uri, ImageDownloaded);
                request.Send();
            }
        }
        catch (System.Exception)
        {
            Debug.LogError("link:" + data.CoverLink);
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
                    CachePhotoData.Instance.AddIconTexture(CurrentLoad.MID, resp.DataAsTexture2D);
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
        CurrentLoad = null;
    }

    private IEnumerator LoadImage()
    {
        while (true)
        {
            while (mLoadQueue != null && mLoadQueue.Count != 0)
            {
                ImageItemBase temp = mLoadQueue.Dequeue();
                while (!AllowToLoad(temp) && mLoadQueue != null && mLoadQueue.Count != 0)
                {
                    temp = mLoadQueue.Dequeue();
                }
                if (temp != null)
                {
                    temp.LoadTexture();
                }
               
                yield return new WaitForEndOfFrame();
            }
    
            yield return 0;
        }
    }

    private bool AllowToLoad(ImageItemBase item)
    {
        if (item == null || item.Data == null || item.IsTexReady)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    private bool startLoadImage = false;
    public void StartLoad()
    {
        if (startLoadImage == false)
        {
            startLoadImage = true;
            StartCoroutine(LoadImage());
        }
    }

    public void Pause()
    {
        //StopCoroutine(LoadImage());
    }

    public void AddImgItem(ImageItemBase item)
    {
        if (item == null || item.Data == null)
        {
            Debug.LogError("the item is null or data is null!");
            return;
        }
        if (mLoadQueue == null)
        {
            mLoadQueue = new Queue<ImageItemBase>();
        }
        mLoadQueue.Enqueue(item);
    }

    private void Clear()
    {

    }
#endregion

#region dimensiondoor
    public void RequestDimensionDoor()
    {
        mNetAPI.AccessDimensionDoor(GalleryNetApi.funAccessDimensionDoor);
    }
#endregion

#region report history
    public void ReportHistory(string mid, string cid)
    {
        System.DateTime now = System.DateTime.Now;
        string time = now.Year + "-" + now.Month + "-" + now.Day + " " + now.Hour + ":" + now.Minute + ":" + now.Second;
        Debug.Log("report history:" + " mid:" + mid + " cid:" + cid + " time:" + time);
        mNetAPI.ReportHistory(GalleryNetApi.funReportHistory, mid, cid, time);
    }
#endregion
    public enum ApiType {
        init,
        getClasses,
        GetItemListByCategory,
        UpdateCheckUp,
        getTenantList

    }
    public class AsynResult {
        /// <summary>
        /// 异步调用是否结束
        /// </summary>
        public bool apiAsynCallOver = false;
        /// <summary>
        /// 初始化结果
        /// </summary>
        public bool Init = false;
        /// <summary>
        /// 获取分类结果
        /// </summary>
        public RetAllLevelsClasses levels;
        /// <summary>
        /// 通过分类获取的应用列表
        /// </summary>
        public RetAppListInfo retAppListInfo;
        /// <summary>
        /// 获取升级检测服务数据
        /// </summary>
        public RetUpdate retUpdate;
        /// <summary>
        /// 获取租户列表
        /// </summary>
        public RctTenantList rctTenantList;
    }
    private bool HttpThreadClose = false;
    AsynResult asynResult;
   
    public static string deviceSN;
    /// <summary>
    /// 由于sdk接口是同步的为了避免卡死的情况 此处封装异步调用
    /// 1、接口调用判断如果是初始化接口调用
    /// 1.1获取设备唯一标识如果有ctn使用ctn
    /// 1.2初始化有两种模式a、网关模式，b、tls模式
    /// 1.2a、网关初始化需要调用网关的接口获取日志收集地址和应用启动地址。
    /// 1.2b、tls模式需要获取业务系统地址
    /// 1.3初始化完成后，如果初始化失败再次尝试初始化，如果初始化成功非快速启动模式需要获取分类，如果是快速启动模式需要进行应用启动
    /// 1.3应用启动需要判断当网关模式或OpenApi模式由于初始化成功时已经通过网关获取到了启动地址，因此直接进行启动应用，否则需要使用appid通过tls获取启动串进行启动
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public IEnumerator asyApiCall(ApiType type,string param=null)
    {
        MyTools.PrintDebugLog("ucvr start asyApiCall:" + type);
        asynResult = new AsynResult() ;
        HttpThreadClose = false;

        if (type == ApiType.init)
        {
            deviceSN = CyberCloud_UnitySDKAPI.HeadBox.getDeviceSN();// "PA7210DGB7040082G";

            deviceSN = CyberCloudConfig.currentType.ToLower() + deviceSN;

            if (LoadConfig.useNullDeviceSN == "null")
            {
                deviceSN = null;
                MyTools.PrintDebugLog("ucvr test useNullDeviceSN 1");
            }
            else if (LoadConfig.useNullDeviceSN != null && LoadConfig.useNullDeviceSN != "")
            {
                deviceSN = LoadConfig.useNullDeviceSN;
                MyTools.PrintDebugLog("ucvr test useNullDeviceSN:" + LoadConfig.useNullDeviceSN);
            }

            if (portalAPI == null)
                portalAPI = new PortalAPI();
            portalAPI.setCardID(deviceSN);

            String CTN = MyTools.getIntentMapValueStringByKey("CTN");
            //ctn是u+定义的启动参数operateUserIDUserID是其他场景定义的唯一标识
            if (CTN != null && CTN != "")
            {
                deviceSN = CTN + deviceSN;
                MyTools.PrintDebugLog("ucvr cvn:" + CTN);
            }
            else if (getStartAppUrlFromCVRGatewayParam != null && getStartAppUrlFromCVRGatewayParam.operateUserIDUserID != null && getStartAppUrlFromCVRGatewayParam.operateUserIDUserID != "")
                deviceSN = getStartAppUrlFromCVRGatewayParam.operateUserIDUserID;
            //string stackInfo = new System.Diagnostics.StackTrace().ToString();
            MyTools.PrintDebugLog("ucvr android sdk deviceSN:" + deviceSN);

            Thread workThread = new Thread(threaInit);
            workThread.Start();
        }
        else if (type == ApiType.getClasses)
        {
            Thread workThread = new Thread(threadGetAllLevelsClasses);
            workThread.Start();
        }
        else if (type == ApiType.GetItemListByCategory)
        {
            CatergoryItemListParam obj = new CatergoryItemListParam();
            //Debug.LogError("param:"+ param);
            // if (param == "100001001004")
            //    param ="4";
            obj.catergoryID = param;
            obj.age = MyTools.getIntentMapValueIntByKey("age");
            Debug.Log("ucvr age:" + obj.age);
            Thread workThread = new Thread(new ParameterizedThreadStart(threadGetItemListByCategory));
            workThread.Start(obj);
        }
        else if (type == ApiType.getTenantList) {
            Thread workThread = new Thread(new ParameterizedThreadStart(getTenantListFromGateway));
            workThread.Start(param);
        }
        else
        {

            MyTools.PrintDebugLogError("ucvr unknown load data type");

        }
     
        while (!HttpThreadClose) {

            yield return new WaitForEndOfFrame();//等到该帧结束
            if (asynResult.apiAsynCallOver)//请求结束
            {
                MyTools.PrintDebugLog("ucvr asynResult.apiAsynCallOver:" + type);
                HttpThreadClose = true;
                if (type == ApiType.init)
                    onInitDataOver(asynResult.Init);
                else if (type == ApiType.getClasses)
                    onGetAllLevelsClassesOver(asynResult.levels);
                else if (type == ApiType.GetItemListByCategory)
                {
                    if (onGetItemListByCategory != null)
                        onGetItemListByCategory(asynResult.retAppListInfo);
                }
                else if (type == ApiType.UpdateCheckUp)
                {


                    RetUpdate retUpdate = asynResult.retUpdate;
                    if (retUpdate != null && retUpdate.retCode == 0 && retUpdate.data != null && retUpdate.data.Count > 0)
                    {
                        AppUpdateData update = retUpdate.data[0];

                    }

                }
                else if (type == ApiType.getTenantList)
                {
                    if (onGetTenantList != null) {
                        onGetTenantList(asynResult.rctTenantList);
                    }
                }
               asynResult.apiAsynCallOver = false;
            }

        }

    }
    //
    public class GetStartAppUrlFromCVRGatewayParam {
        //authType:HangYanYuan,ChengYanYuan或者空，用于调用不同的用户认证接口
        public string operateUserIDUserID, appID, tenantID, userToken, corpToken, tenantToken, authType;
    }
    GetStartAppUrlFromCVRGatewayParam getStartAppUrlFromCVRGatewayParam;
    /// <summary>
    /// 获取租户列表用于验证租户是否合法
    /// </summary>
    private void getTenantListFromGateway(object tenantID)
    {//江西移动分值获取获取接入参数
        if (portalAPI == null)
            portalAPI = new PortalAPI();
        RctTenantList ret = portalAPI.getTenantListFromCVRGateway(CyberCloudConfig.tls, tenantID.ToString());
        asynResult.apiAsynCallOver = true;
        asynResult.rctTenantList = ret;
    }
    private void gateWayInit()
    {//江西移动分值获取获取接入参数
        RetStartUrl ret = portalAPI.getStartAppUrlFromCVRGateway(CyberCloudConfig.tls,
            getStartAppUrlFromCVRGatewayParam.operateUserIDUserID, getStartAppUrlFromCVRGatewayParam.appID, 
            getStartAppUrlFromCVRGatewayParam.tenantID, getStartAppUrlFromCVRGatewayParam.userToken,
            getStartAppUrlFromCVRGatewayParam.corpToken, getStartAppUrlFromCVRGatewayParam.tenantToken, getStartAppUrlFromCVRGatewayParam.authType);
        asynResult.apiAsynCallOver = true;
        if (ret == null)
            asynResult.Init = false;
        else
        {
            if (ret.retCode == 0)
            {
                asynResult.Init = true;
                startCloudInfo = new PortalAPI.StartCloudInfo();
                startCloudInfo.startUrl = ret.data.startUrl;
                GameAppControl.AppName = ret.data.appName;
                CyberCloudConfig.statisticsUpLoadUrl = ret.data.statisticUploadUrl;
                if (CyberCloudConfig.statisticsUpLoadUrl != null && CyberCloudConfig.statisticsUpLoadUrl != "") {
                    if(!OpenApiImp.CyberCloudOpenApiEnable)//CyberCloudOpenApiEnable状态下受统计服务接口控制
                        CyberCloudConfig.statisticsUpLoad = 1;
                }
            
                Debug.Log("ucvr ret.startUrl:"+ ret.data.startUrl+ ";ret.appName:"+ ret.data.appName+ ";ret.statisticUploadUrl:"+ ret.data.statisticUploadUrl);
            }
            else {
                asynResult.Init = false;
            }
        }
    }
    /// <summary>
    /// 初始化有两种模式a、网关模式，b、tls模式
    /// a、网关初始化需要调用网关的接口获取日志收集地址和应用启动地址。
    /// b、tls模式需要获取业务系统地址
    /// </summary>
    private void threaInit()
    {
        try
        {
            if (portalAPI == null)
                portalAPI = new PortalAPI();
            bool bInitRet = false;
            //判断初始化时使用网关初始化还是tls初始化
            if (CyberCloudConfig.tls.IndexOf(GameAppControl.gatewayPort) >-1|| OpenApiImp.CyberCloudOpenApiEnable)
            {
                gateWayInit();
                return;
            }
            if (CyberCloudConfig.cvrScreen == CyberCloudConfig.CVRScreen.YanShi2)
            {
                //Debug.LogError("yanshichushihua");
                bInitRet= portalAPI.yanshichushihua(deviceSN);

            }
            else
                bInitRet= portalAPI.Init(deviceSN);
            asynResult.apiAsynCallOver = true;
            asynResult.Init = bInitRet;
        }
        catch (System.Exception e) {
            Debug.LogError("threaInit error:"+e.Message);
            asynResult.apiAsynCallOver = true;
            asynResult.Init = false;
        }
        //Debug.Log("threaInit over:"+ bInitRet);
    }
    //通过获取布局接口GetPagedata返回的布局数据中过滤出pic，video和app所有的分类
    private void threadGetAllLevelsClasses() {
        try {
           
            RetAllLevelsClasses levels = portalAPI.GetAllLevelsClasses();
            asynResult.apiAsynCallOver = true;
            asynResult.levels = levels;//onGetAllClassOverFillter(levels);
        }
        catch (System.Exception e)
        {
            MyTools.PrintDebugLogError("ucvr threadGetAllLevelsClasses error:" + e.Message);
            asynResult.apiAsynCallOver = true;
            asynResult.levels = null;
        }
       
    }
    
    private class CatergoryItemListParam
    {
        public string catergoryID;
        public int age;
    }
    private void threadGetItemListByCategory(object obj) {
        try {         
            CatergoryItemListParam item = (CatergoryItemListParam)obj;
            // RetAppListInfo retAppListInfo = DataLoader.Instance.portalAPI.GetItemListByCategory(catergoryID);
            RetAppListInfo retAppListInfo = DataLoader.Instance.portalAPI.GetItemListByCategory(item.catergoryID, item.age.ToString());
      
            asynResult.apiAsynCallOver = true;
            asynResult.retAppListInfo = retAppListInfo;
        }
        catch (System.Exception e)
        {
            MyTools.PrintDebugLogError("ucvr threadGetItemListByCategory error:" + e.Message);
            asynResult.apiAsynCallOver = true;
            asynResult.retAppListInfo = null;
        }
    }

  
}