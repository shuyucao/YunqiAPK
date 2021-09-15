
using UnityEngine;
using System.Collections;
using System.IO;

public class GalleryActivity : Singleton<GalleryActivity>
{
    //public GalleryPlayerManager mPlayerManager = null;
    private AndroidJavaObject galleryActivity;
    //string PhotoTexturetype = "";

    public GalleryActivity()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            string packageName = "com.picovr.gallery.UnityActivity";
            if (galleryActivity == null)
            {
                galleryActivity = new AndroidJavaClass(packageName).GetStatic<AndroidJavaObject>("unityActivity");
            }
        }
    }

    public string GetImageDirs(string path)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            return galleryActivity.Call<string>("GetDirs", path);
        }
        else
        {
            return null;
        }
    }

    public string GetLocalImages(string path)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            return galleryActivity.Call<string>("GetLocalImages", path);
        }
        else
        {
            return null;
        }
    }

    //判断本地图片是否存在
    public bool FileExistOrNot(string photolink,string thumbnaillink)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            return galleryActivity.Call<bool>("FileExistOrNot", photolink, thumbnaillink);
        }
        else
        {
            return false;
        }
    }

    //判断在线图片是否已下载
    public bool checkImageIfExitsByMid(string mid)
    {
        if (mid == null)
            return false;

        if (Application.platform == RuntimePlatform.Android)
        {
            return galleryActivity.Call<bool>("checkImageIfExitsByMid", mid);
        }
        else
        {
            return false;
        }
    }

    public void openByMid(string mid)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            galleryActivity.Call("openByMid", mid);
        }
    }
    //private void GetPlayerManager()
    //{
    //    GameObject Root = GameObject.Find("UI Root");
    //    if (null == Root)
    //    {
    //        Debug.LogError("UI Root not exist");

    //        mPlayerManager = null;
    //        return;
    //    }

    //    mPlayerManager = Root.GetComponent<GalleryPlayerManager>();
    //}

    public void SetDone2Zero_Activity()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            galleryActivity.Call("setDone2Zero");
        }
    }

    public bool IsNetworkAvailable_Activity()
    {
        int state = galleryActivity.Call<int>("isNetworkAvailable");
        if (state == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public int GetProgress_Activity()
    {

        int progress = galleryActivity.Call<int>("getProgress");
        Debug.Log("PlayerManager mJar : GetProgress_Activity == " + progress);
        return progress;
    }

    public void DownLoadPhoto_Activity(string mid)
    {
        Debug.Log("PlayerManager mJar : getThumbnailPhoto");
        if (Application.platform == RuntimePlatform.Android)
        {
            //GetPlayerManager();
            galleryActivity.Call("getThumbnailPhoto", mid);
        }
    }

    //public void GetPhotoTextureType(string type)
    //{
    //    Debug.Log("PlayerManager mJar : GetPhotoTextureType");
    //    PhotoTexturetype = type;
    //}

    //public void GetMainPhotoPath_Activity(string photoPath)
    //{
    //    Debug.Log("PlayerManager mJar : GetMainPhotoPath_Activity");

    //    PhotoTextureType type = PhotoTextureType.Photo;
    //    if (PhotoTexturetype == "1")
    //    {
    //        type = PhotoTextureType.Photo;
    //    }
    //    else
    //    {
    //        type = PhotoTextureType.Thumbnail;
    //    }

    //    if (mPlayerManager != null)
    //    {
    //        mPlayerManager.LoadLocalMainPhoto(photoPath, type);
    //    }
    //    else
    //    {
    //        GetPlayerManager();
    //        if (mPlayerManager != null)
    //        {
    //            mPlayerManager.LoadLocalMainPhoto(photoPath, type);
    //        }
    //    }
    //}

    public string GetCacheSize_Activity()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            string ret = galleryActivity.Call<string>("showChaceSize");
            Debug.Log("GetCacheSize_Activity : ret = " + ret);
            if (ret == null)
                ret = "0.0Byte";
            return ret;
        }
        else return "0.0Byte";
    }

    public void ClearCache_Activity()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            galleryActivity.Call("cleanChace");
        }
    }

    public void StartUpdate()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            string url = SettingScreen.url;
            //string url = "http://static.appstore.picovr.com/upload/d/8/9/1/d8910d00ebbf8979269759daa8450b77.apk";//store自更新地址
            galleryActivity.Call("startUpdate", url);
        }
    }

    public void InstallSilent()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            galleryActivity.Call<string>("installSilent");
        }
    }

    public string GetBaseUrl()
    {
        string baseUrl = null;
        baseUrl = galleryActivity.Call<string>("GetUrlWithSuffix", "Gallery", "Photo");
        return baseUrl;
    }

    public string GetProperty(string deviceType)
    {
        string dt = null;
        dt = galleryActivity.Call<string>("getProperty", deviceType);
        Debug.Log("Current device type is " +dt);
        return dt;
    }
}