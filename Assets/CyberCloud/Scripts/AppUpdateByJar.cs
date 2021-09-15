using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 使用Jar包更新apk
/// </summary>
public class AppUpdateByJar 
{
    public const string CvrUpdateTool = "com.cybercloud.vr.CvrUpdateTool";
    public static int getAndroidSDKVersion()
    {
        int androidSDKVersion = 0;
        try
        {

            //AndroidJavaClass只能调用静态方法，获取静态属性 AndroidJavaObject能调用公开方法和公开属性
            //AndroidJavaClass handler = new AndroidJavaClass(cname);

            AndroidJavaObject handler = new AndroidJavaObject(CvrUpdateTool);
            //调用jar包方法获取当前apk的versioncode
            androidSDKVersion = handler.Call<int>("getAndroidSDKVersion");
            MyTools.PrintDebugLog("ucvr getAndroidSDKVersion:" + androidSDKVersion);
        }
        catch (Exception e)
        {
            MyTools.PrintDebugLogError("ucvr androidSDKVersion:" + e.Message);
        }
        return androidSDKVersion;
    }
}
