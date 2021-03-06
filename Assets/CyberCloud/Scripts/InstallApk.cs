using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CyberCloud.Scripts.OpenApi;
using UnityEngine;
/**
 * apk 安装以及读取服务器配置
 * */
public class InstallApk 
{
    
    private string loaclapkupdatefile = "";
    public void startDownLoad() {
  
        getCommonPlane();
  
        MyTools loadfile = new MyTools();
        loadfile.wwwDataLoad = loadFileResult;
        if (CyberCloudConfig.tls.IndexOf(GameAppControl.gatewayPort) > -1)
        {//网关的升级策略
            //江西移動升級策略
        }
        else
        {
            if(OpenApiImp.CyberCloudOpenApiEnable==false)
            DataLoader.Instance.StartCoroutine(loadfile.downloadfile(CyberCloudConfig.tls + "/config.txt"));
        }
    
    }
    private void loadFileResult(MyTools.ToolBaseResult result)
    {
        if (result != null && result.retCode == 0 && result.data != null && result.data != "")
        {

            MyTools.PrintDebugLog("ucvr loadFileResult config:" + result.data);
            String configPath = Application.persistentDataPath + "/serverConfig.ini";
            IniFile ini = new IniFile();
            bool fileEnable = ini.LoadFromFile(configPath);
            if (!fileEnable)
            {
                MyTools.PrintDebugLogError("ucvr serverConfig.ini not exit");
                return;
            }
            CyberCloudConfig.CyberZoneCode = ini.GetValue("CyberZoneCode", "CyberZoneCode", "");
            CyberCloudConfig.CyberZoneDesc = ini.GetValue("CyberZoneCode", "CyberZoneDesc", "");
          
            string statisticsUpLoad = ini.GetValue("statistics", "statisticsUpLoad", "0");
            float temstatiststate = ((statisticsUpLoad != null) ? float.Parse(statisticsUpLoad) : 0);
            MyTools.PrintDebugLog("ucvr statisticsUpLoad :" + statisticsUpLoad);
            if (temstatiststate > 0 && temstatiststate <=100)
            {//按百分比计算是否上传
                int num = UnityEngine.Random.Range(0, 100);
                if (LoadConfig.useNullDeviceSN != null && LoadConfig.useNullDeviceSN != ""&& LoadConfig.useNullDeviceSN.IndexOf("cyberCloudTest")>-1)
                    CyberCloudConfig.statisticsUpLoad = 1;
                else if (num <=temstatiststate)
                    CyberCloudConfig.statisticsUpLoad = 1;
                else
                {
                    MyTools.PrintDebugLog("ucvr statisticsUpLoad Random failed no need upload");
                }
            }
            else {
         
                    CyberCloudConfig.statisticsUpLoad =  (temstatiststate==0?0:110);//0或110
            }
              
        
            if (CyberCloudConfig.statisticsUpLoad == 1 || CyberCloudConfig.statisticsUpLoad ==110)
            {
                string statisticsUpLoadUrl = ini.GetValue("statistics", "statisticsUpLoadUrl", "");
                
                CyberCloudConfig.statisticsUpLoadUrl = statisticsUpLoadUrl;
                MyTools.PrintDebugLog("ucvr statisticsUpLoadUrl:" + statisticsUpLoadUrl);
            }
            string apktype = CyberCloudConfig.currentType.ToLower() + "apkversion";
            foreach (var name in ini.GetAllSectionName())
            {
                if (string.Equals(name, apktype))
                {
                    //升級
                    string cvrScreen = ini.GetValue(apktype, "cvrScreen", "");
                    string apkversioncode = ini.GetValue(apktype, "apkversioncode", "-1");
                    int serverapkcode = int.Parse(apkversioncode);
                    string apkdownurl = ini.GetValue(apktype, "apkdownurl", "");
                    string versioncodestr = MyTools.getVersionCode();
                    int versioncode = int.Parse(versioncodestr);
                  string mid = ini.GetValue(apktype, "mid", "");
                    Debug.Log("ucvr config type :" + cvrScreen+";device type:"+ CyberCloudConfig.cvrScreen+ ";mid:"+ mid);
             
                    if (cvrScreen == CyberCloudConfig.cvrScreen)
                    {
               
                        MyTools.PrintDebugLog("apk serverapkcode:"+ serverapkcode+ ";deviceapkcode :"+ versioncode);
                        if (serverapkcode > versioncode)
                        {
                          
                            if (mid != "" && mid != null && (CyberCloudConfig.currentType == CyberCloudConfig.DeviceTypes.Pico2 || CyberCloudConfig.currentType == CyberCloudConfig.DeviceTypes.Pico|| CyberCloudConfig.currentType == CyberCloudConfig.DeviceTypes.PicoNeo2))
                            {//pico 头盔配置了资源id,跳转到商城
                          			 Debug.Log("ucvr Update_Force _mid:"+ _mid);
                                _mid = mid;
                                downloadNewVersion = 0;
                                commonPlaneCom.showupdateapkdialog("Update_Force", "", dialogClickCallBack);     
                            }
                            else
                            {
                             
                                if (apkdownurl != null && apkdownurl.Length > 10)
	                            {
                               
                                    MyTools.PrintDebugLog("ucvr need download apkdownurl :" + apkdownurl);
	                                loaclapkupdatefile = Application.persistentDataPath + "/download/update_" + serverapkcode + ".apk";
	                                StartDownLoad(apkdownurl, loaclapkupdatefile);
	                            }
	                            else
	                            {
	                                MyTools.PrintDebugLogError("ucvr apk need update apkdownurl error :" + apkdownurl + ";tls server need put a config.txt and config apkdownurl by http://x.x.x.x..x.apk");
	                            }
                            }
                        }
                        else {
                            Debug.Log("apk is latest");
                        }
                    }
                    break;
                }
            }


        }
        else
        {
            MyTools.PrintDebugLogError("ucvr server config is null");
        }
    }
    public int downloadNewVersion = -1;
    /// <summary>
    /// 开始下载文件apk
    /// </summary>
    /// <param name="url">下载服务器地址</param>
    /// <param name="file_SaveUrl">本地存储地址</param>
    /// <returns></returns>
    private void StartDownLoad(string url, string file_SaveUrl)
    {
        MyTools loadApk = new MyTools();
        loadApk.wwwDataLoad = loadApkResult;
        DataLoader.Instance.StartCoroutine(loadApk.DownApk(url, file_SaveUrl));
      
    }
    CommonPlane commonPlaneCom;
    private void getCommonPlane()
    {
        GameObject commonPlane = GameObject.Find("CyberCloudCommonPlane");
        if (commonPlane == null)
            MyTools.PrintDebugLogError("ucvr commonPanel mast add to screen");
        commonPlaneCom = commonPlane.GetComponent<CommonPlane>();
        if (commonPlaneCom == null)
            MyTools.PrintDebugLogError("ucvr CyberCloudCommonPlane mast contain CommonPlane");
    }
    //apk下载结束
    private void loadApkResult(MyTools.ToolBaseResult result)
    {

        if (result != null && result.retCode == 0)
        {
            showupdateapkdialog(0);
        }
        else
        {
            MyTools.PrintDebugLogError("ucvr server loadapk is error");
            if(result==null)
                showupdateapkdialog(-1);
            else
                showupdateapkdialog(result.retCode);
        }
    }
    public void showupdateapkdialog(int code) {
        downloadNewVersion = code;
        Debug.Log("ucvr Update_Force");
        if(code==0)
            commonPlaneCom.showupdateapkdialog("Update_Force", "", dialogClickCallBack);
        else
            commonPlaneCom.showupdateapkdialog("Home_Category_Failed", "", dialogClickCallBack2);
        
    }
    private string _mid = "";
    private void dialogClickCallBack(MyDialog.ButtonIndex buttonIndex)
    {
        Debug.Log("ucvr InstallAPK ");
        if(_mid!=null&& _mid!="")
            MyTools.startPicoStoredetail(_mid);
        else
            MyTools.InstallAPK(loaclapkupdatefile);
#if picoSdk
        Pvr_UnitySDKAPI.Controller.UPvr_IsEnbleHomeKey(true);
#endif
        Application.Quit();// 不升级无法再使用
    }
    private void dialogClickCallBack2(MyDialog.ButtonIndex buttonIndex)
    {
        Debug.LogError("ucvr download failed ");
#if picoSdk
        Pvr_UnitySDKAPI.Controller.UPvr_IsEnbleHomeKey(true);
#endif
        Application.Quit();// 不升级无法再使用
    }
}

