using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BestHTTP;
using System;

public class GalleryNetApi
{
    public delegate void HttpCallback(HTTPRequest req, HTTPResponse resp);

    public static HttpCallback OnDataCallbackDelegate;

    //测试
    public const string baseTestURL = "";

    private string apiTestKey = "";

    //现网
    public const string baseFormalURL = "";

    public string baseURL
    {
        get
        {
            string url = "";
#if UNITY_EDITOR
#elif UNITY_ANDROID
            url = GalleryActivity.Instance.GetBaseUrl();
            Debug.Log("from goodman , url is " + url);
#endif
            if (string.IsNullOrEmpty(url))
                url = baseFormalURL;
            else if (url.Equals(baseTestURL))
                apiKey = apiTestKey;
            Debug.Log("after   url is " + url + "  and apikey is " + apiKey);
            return url;
        }
    }

    private string apiKey = "";
    public const string funGetCategory = "";
    public const string funGetCategoryPhoto = "1";
    public const string funGetPhoto = "";
    public const string funGetThemes = "";
    public const string funGetThemesPhoto = "";
    public const string funReportHistory = "";
    public const string funReportHistoryPack = "";
    public const string funAccessDimensionDoor = "";
    public const string funCheckUpdate = "";

    //The req dic.save the request instance for user called request.abort() to abort current request
    private Dictionary<string, HTTPRequest> reqDic;

    private const double favoriteTimeOut = 3.0f;
    private const double favoriteConnectTimeOut = 3.0f;
    private double timeOut = 12;
    private double connectTimeOut = 6;

    //seconds
    public double TimeOut
    {
        get
        {
            return timeOut;
        }
        set
        {
            if (value <= 0)
            {
                timeOut = 20;
            }
            timeOut = value;
        }
    }

    //seconds
    public double ConnectTimeOut
    {
        get
        {
            return connectTimeOut;
        }
        set
        {
            if (value <= 0)
            {
                connectTimeOut = 10;
            }
            connectTimeOut = value;
        }
    }

    public GalleryNetApi()
    {
        reqDic = new Dictionary<string, HTTPRequest>();
    }

    public void GetCategory(string key)
    {
    }

    public void GetCategoryPhoto(string key, string cid, string limit, string page)
    {
    }

    public void GetPhoto(string key, string mid)
    {
    }

    public void GetThemes(string key, int limit, int page = 1)
    {
    }

    public void GetThemesPhoto(string key, string tid, int limit, int page = 1)
    {
    }

    //watched_at格式 :“Y-m-d H:i:s”
    public void ReportHistory(string key, string mid, string cid, string watched_at)
    {
    }

    //pack 格式 : 图片id_分类id_观看时间，多条记录以西文逗号分隔。时间格式：“Y-m-d H:i:s”
    public void ReportHistoryPack(string key, string pack)
    {
    }

    public void AccessDimensionDoor(string key)
    {
    }

    public void CheckUpdate(string key, string versionCode, string deviceType)
    {
    }

    public bool IsValidRequest(string key)
    {
        bool valid = false;
        if (key != null)
        {
            if (reqDic.ContainsKey(key))
            {
                valid = true;
            }
        }

        return valid;
    }

    public void StopNetRequest(string key)
    {
        if (key != null)
        {
            if (reqDic.ContainsKey(key))
            {
                HTTPRequest request = reqDic[key];
                request.Abort();
                reqDic.Remove(key);
                Debug.Log("GalleryNetApi-> StopNetRequest: key is " + key);
            }
        }
    }

    private void OnRequestFinished(HTTPRequest req, HTTPResponse resp)
    {
        string dicKey = req.Tag as String;
        if (dicKey != null)
        {
            if (!reqDic.ContainsKey(dicKey))
            {
                return;
            }
        }

        if (resp == null)
        {
            Debug.LogWarning("GalleryNetApi-> OnRequestFinished: resp is null");
        }
        else
        {
            Debug.Log("GalleryNetApi->Request Finished! key= " + dicKey + " Text received: " + resp.DataAsText);
        }

        //remove先执行
        reqDic.Remove(dicKey);
        OnDataCallbackDelegate(req, resp);
    }

    public bool HavePendingRequest()
    {
        return reqDic.Count > 0 ? true : false;
    }
}