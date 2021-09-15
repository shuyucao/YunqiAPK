using UnityEngine;
using System.Collections.Generic;

public class IconTexture
{
    public int index = 0;
    public Texture texture = null;
}



public class GlobalPhotoData : Singleton<GlobalPhotoData> 
{

    public int mColumnsPerPage = 4;

    public int mCurrentTotalPage = 0;
    public string mCurrentCategoryID = "";
    public int mCurrentPhotoIndex = 0;

    List<CategoryModel> mCategoryList = new List<CategoryModel>();
    List<PhotoModel> mPhotoModelList = new List<PhotoModel>();
    List<IconTexture> mIconTextureList = new List<IconTexture>();


    
    public List<CategoryModel> GetCategoryList()
    {
        return mCategoryList;
    }

    public List<PhotoModel> GetPhotoList()
    {
        return mPhotoModelList;
    }

    public List<IconTexture> GetIconTextureList()
    {
        return mIconTextureList;
    }
}
