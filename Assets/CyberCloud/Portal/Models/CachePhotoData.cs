using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CyberCloud.PortalSDK.vo;
public class CachePhotoData : Singleton<CachePhotoData>
{
    //public CachePhotoData()
    //{
    //    Texture2D tex = Resources.Load("Textures/default_posters_horizonta_VR") as Texture2D;
    //    AddIconTexture("default", tex);
    //}

    #region category data manage
    List<CategoryModel> mCategoryList = new List<CategoryModel>();
    public List<CategoryModel> CategoryList
    {
        get
        {
            return mCategoryList;
        }
    }

    public void InitCategory(List<CategoryModel> list)
    {
        if (mCategoryList.Count != 0)
        {
            Debug.Log("list has element need to clear!");
            mCategoryList.Clear();
        }
        //ugly
        CategoryModel allmodel = new CategoryModel();
        allmodel.CategoryID = Constant.CategoryOfAllCid;
        allmodel.Name = Localization.Get("Home_Category_All");
        mCategoryList.Add(allmodel);
        mCategoryList.AddRange(list);

        MsgManager.Instance.SendMsg(MsgID.CategoryDataReady, null);
    }

    public CategoryModel GetCategoryModelByID(string cid)
    {
        CategoryModel data = null;
        if (mCategoryList != null)
        {
            for (int i = 0; i < CategoryList.Count; i++)
            {
                if (CategoryList[i].CategoryID.Equals(cid))
                {
                    data = CategoryList[i];
                }
            }
        }
        return data;
    }

    public bool IsCategoryDataReady()
    {
        return mCategoryList.Count != 0;
    }
    #endregion

    #region category photo data manage
    public CategoryPhotoData CurCatPhoData
    {
        get
        {
            return GetCatPhotoDataByID(HomePageScreen.CurrentID);
        }
    }
    private Dictionary<string, CategoryPhotoData> mCagoryPhotoDict = new Dictionary<string, CategoryPhotoData>();
    /// <summary>
    /// 将内容列表数据整理成 分页数据并缓存
    /// </summary>
    /// <param name="category_id">分类id</param>
    /// <param name="total_num">分页个数</param>
    /// <param name="current_num"></param>
    /// <param name="list">内容数据</param>
    public void SetCatPhoModleData(string category_id, int total_num, int current_num, List<AppInfo> list)
    {
        if (list == null)
        {
            Debug.LogError("the photo list is null!");
            return;
        }
        //
        CategoryPhotoData data = null;
        if (!mCagoryPhotoDict.TryGetValue(category_id, out data))
        {
            data = new CategoryPhotoData();
            data.CategoryID = category_id;
            data.TotalPage = total_num;
            if (current_num > data.CurrentCachePage)
            {
                data.CurrentCachePage = current_num;
            }
            List<PhotoModel> models = new List<PhotoModel>();
		    for (int i = 0; i < list.Count; i++)
		    {
			    PhotoModel model = new PhotoModel();
			    AppInfo app = list[i];
			    model.MID = app.appID;
			    model.LogoImg = app.appImgUrl;
				model.ThumbnailLink = app.appImgUrl;
                model.PhotoLink = app.startCloudAppUrl;
                model.Title = app.appName;
			    models.Add(model);
               //Debug.Log("appid:" + app.appImgUrl);
		    }
            data.AddPhotoList(models);
            mCagoryPhotoDict.Add(category_id, data);
        }
        else
        {
            data.CurrentCachePage = current_num;
            //data.AddPhotoList(list);
        }
        
        //Debug.LogError("data.CanFullOnePage:" + data.CanFullOnePage());
        //need send CategoryDataReady msg when init one category in first time 
        Bundle bundle = new Bundle();
        bundle.SetValue<string>("categoryID", category_id);

        MsgManager.Instance.SendMsg(MsgID.PhotoDataRefresh, bundle);
    }

    public CategoryPhotoData GetCatPhotoDataByID(string id)
    {
        //Debug.LogError("=========GetPhotoModelByMID2===============");
        CategoryPhotoData data = null;
        if (!mCagoryPhotoDict.TryGetValue(id, out data))
        {
            Debug.Log("can not find the cat photo data which id is:" + id);
        }
        return data;
    }

    public PhotoModel GetPhotoModelByMID(string mid)
    {
        //Debug.LogError("=========GetPhotoModelByMID1===============");
        foreach (var item in mCagoryPhotoDict)
        {
            PhotoModel data = item.Value.GetPhotoModelByMID(mid);
            if (data != null)
            {
                return data;
            }
        }
        if (CurDimDoorPhoList != null)
        {
            for (int i = 0; i < CurDimDoorPhoList.Count; i++)
            {
                if (mid.Equals(CurDimDoorPhoList[i].MID))
                {
                    return CurDimDoorPhoList[i];
                }
            }
        }
        return null;
    }

    public bool IsFirstPageInit(string id)
    {
        CategoryPhotoData data = GetCatPhotoDataByID(id);
        if (data != null)
        {
            return data.PhotoList.Count != 0;
        }
        else
        {
            return false;
        }

    }

    public PhotoModel GetNextPhoto(bool isnext)
    {
        PhotoModel data = null;
        if (CurCatPhoData != null)
        {
            return CurCatPhoData.MoveNextPhoto(isnext);
        }

        return data;
    }
    #endregion

    #region photo data manage
    public string CurrentPhotoIndex;
    private Dictionary<string, Texture2D> mIconTextureDict = new Dictionary<string, Texture2D>();

    public void AddIconTexture(string mid, Texture2D tex)
    {
        if (mIconTextureDict.ContainsKey(mid))
        {
            Debug.Log("it has already contains the tex:" + mid);
        }
        else
        {
            mIconTextureDict.Add(mid, tex);
        }
    }

    public bool IsIconTextureExist(string mid)
    {
        return mIconTextureDict.ContainsKey(mid);
    }

    public Texture2D GetIconTexture(string mid)
    {
        Texture2D tex = null;
        mIconTextureDict.TryGetValue(mid, out tex);
        return tex;
    }

    public PhotoModel GetCurrentPhotoModel()
    {
        PhotoModel data = GetPhotoModelByMID(CurrentPhotoIndex);
        return data;
    }
    #endregion

    #region Themes
    private List<ThemesModel> mThemesList = new List<ThemesModel>();
    public List<ThemesModel> ThemesList
    {
        get
        {
            return mThemesList;
        }
    }

    private Dictionary<string, CategoryPhotoData> mThemeDict = new Dictionary<string, CategoryPhotoData>();

    public void InitThemes(List<ThemesModel> list)
    {
        if (mThemesList.Count != 0)
        {
            Debug.Log("list has element need to clear!");
            mThemesList.Clear();
        }
        mThemesList.AddRange(list);

        MsgManager.Instance.SendMsg(MsgID.ThemesDataReady, null);
    }

    public void SetThemePhoModleData(string themeid, List<PhotoModel> list)
    {
        if (list == null)
        {
            Debug.LogError("the photo list is null!");
            return;
        }
        CategoryPhotoData data = null;
        if (!mThemeDict.TryGetValue(themeid, out data))
        {
            data = new CategoryPhotoData();
            data.CategoryID = themeid;
            data.TotalPage = 1;
            data.CurrentCachePage = 1;
            data.AddPhotoList(list);
            mThemeDict.Add(themeid, data);
        }
        else
        {
            data.CurrentCachePage = 1;
            data.AddPhotoList(list);
        }

        Bundle bundle = new Bundle();
        bundle.SetValue<string>("themeID", themeid);
        MsgManager.Instance.SendMsg(MsgID.ThemePhotoDataRefresh, bundle);
    }

    public CategoryPhotoData GetThemesPhotosByID(string themeid)
    {
        CategoryPhotoData cadata = null;
        mThemeDict.TryGetValue(themeid, out cadata);
        return cadata;
    }
    #endregion

    #region DimensionDoor
    private List<PhotoModel> CurDimDoorPhoList { get; set; }

    private int CurrentDimensionDoorIndex = 0;

    public void SetDimensionDoorData(List<PhotoModel> list)
    {
        if (list != null && list.Count > 0)
        {
            CurDimDoorPhoList = list;
        }

        //open player screen
        //ScreenManager.Instance.CloseScreen(UIScreen.DimensionDoor);
        GPlayerManager.Instance.OpenDimonsionDoor();
    }

    public PhotoModel GetADimensionDoorPhoto()
    {
        PhotoModel data = null;
        if (CurDimDoorPhoList != null && CurrentDimensionDoorIndex < CurDimDoorPhoList.Count)
        {
            data = CurDimDoorPhoList[CurrentDimensionDoorIndex++];
        }
        else
        {
            CurrentDimensionDoorIndex = 0;
        }
        return data;
    }
    #endregion

    #region localphotodata
    public List<LocalPhotoModel> FolderList = null;
    public List<LocalPhotoModel> PhotosList = null;
    public int localIndex = -1;
    private Dictionary<string, List<LocalPhotoModel>> PhotosInFolderDict = new Dictionary<string, List<LocalPhotoModel>>();
    public void InitFolderData(List<LocalPhotoModel> ls)
    {
        FolderList = ls;
        PhotosInFolderDict.Clear();
        MsgManager.Instance.SendMsg(MsgID.LocalFolderDataReady, null);
    }

    public void InitPhotosInFolder(string folderpath, List<LocalPhotoModel> ls)
    {
        if (string.IsNullOrEmpty(folderpath) || ls == null)
        {
            Debug.LogError("the data is null!");
            return;
        }
        List<LocalPhotoModel> temp;
        if (PhotosInFolderDict.TryGetValue(folderpath, out temp))
        {
            temp = ls;
        }
        else
        {
            PhotosInFolderDict.Add(folderpath, ls);
        }
        Bundle bundle = new Bundle();
        bundle.SetValue<string>("TitleName", folderpath);
        MsgManager.Instance.SendMsg(MsgID.LocalPhotosInFolderDataReady, bundle);
    }

    public List<LocalPhotoModel> GetPhotosListByID(string id)
    {
        List<LocalPhotoModel> temp = null;
        if (PhotosInFolderDict.TryGetValue(id, out temp)) ;
        return temp;
    }
    #endregion
}