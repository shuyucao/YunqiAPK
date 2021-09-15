using UnityEngine;
using System.Collections;

public class NetConstant
{
    //    string baseURL = "";
    //public const string test_baseURL = "";
    public const string test_baseURL = "";

    public const string web_baseURL = "";

    public const string web_apikey = "";

    public const string test_apikey = "";

    public static string baseURL = test_baseURL;
    public static string apikey = web_apikey;
    //	public static string baseURL = test_baseURL;
    //	public static string apikey = test_apikey;

    public static void setWebServer()
    {
        int serverType = 0;
        return;
#if UNITY_ANDROID

#endif
        Debug.Log("NetConstant-->setWebServer : serverType= " + serverType);
        switch (serverType)
        {
            case 0:
                apikey = web_apikey;
                baseURL = web_baseURL;
                break;

            case 1:
                apikey = test_apikey;
                baseURL = test_baseURL;
                break;

            default:
                apikey = web_apikey;
                baseURL = web_baseURL;
                break;
        }
    }
}