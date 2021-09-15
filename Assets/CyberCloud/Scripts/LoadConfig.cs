using CyberCloud.PortalSDK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using Assets.CyberCloud.Scripts.OpenApi;

public class LoadConfig : MonoBehaviour {
  
    // Use this for initialization
    void Awake()
    {
        loadConfig();   
    }

    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public static string action = "android.intent.action.startfromunity";
    public void loadConfig()
    {
        try
        {
            IniFile ini = new IniFile();
            bool fenable= ini.LoadFromResouceConfig();
            string publish = ini.GetValue("systemsettings", "publish", "");
            if (!fenable) {
                MyTools.PrintDebugLogError("====================ucvr ucvr no config file in assets================ " );
                return;
            }
            MyTools.PrintDebugLog("ucvr publishType: " + publish);
            foreach (var name in ini.GetAllSectionName())
            {
                if (string.Equals(name, "systemsettings"))
                {
                    configFromAssetsConfig(ini, name);
                   // break;
                }
                if (string.Equals(name, "playersettings"))
                {
                    string bundleVersion = ini.GetValue(name, "bundleVersion", "");
                    MyTools.PrintDebugLog("ucvr sdk version :" + bundleVersion);
                  //  break;
                }
            }
          
            //debug版本需要讀取sdcard目錄的配置文件
            if (publish != "release")
            {
                if(OpenApiImp.CyberCloudOpenApiEnable==false)
                    configFromScard();
            }
            
            try
            {                
                if (CyberCloudConfig.tls == null || CyberCloudConfig.tls == "")
                {
                    Debug.LogError("**********************ucvr load from PlayerPrefs tlsUrl************************");
                    return;
                }
                PortalAPI.setTlsURL(CyberCloudConfig.tls);
                if (CyberCloudConfig.autoStartCastScreenTestService == 1)
                {
                   
                    MyTools.PrintDebugLog("ucvr autoStartCastScreenTestService=1");
                    if (MyTools.isServiceRunning("com.cloud.HWRouteService") ==false)
                        MyTools.startService("com.cloud", action);
                }
                if (CyberCloudConfig.logOutLevel > 0 && CyberCloudConfig.logOutLevel < 7)
                {
                    MyTools.PrintDebugLog("ucvr =====test start log level VERBOSE==" + CyberCloudConfig.logOutLevel);
                    MyTools.setSdkLogLevel(CyberCloudConfig.logOutLevel);
                }
            }
            catch (Exception e)
            {
                MyTools.PrintDebugLogError("ucvr setTlsURL error" + e.Message);
            }
        }
        catch (Exception e) {

            MyTools.PrintDebugLogError("ucvr loadConfig:"+e.Message);
        }

    }
    private void configFromScard() {
        string beixuan = Application.persistentDataPath + "/cybertls.txt";
        string cybertlsPath = beixuan;
        #if UNITY_ANDROID && !UNITY_EDITOR
        cybertlsPath = "sdcard/cybertls.txt";
        #endif
        MyTools.PrintDebugLog("ucvr loadconfig " + cybertlsPath);
     
        bool localLoadSuceess = readLocalConfig(cybertlsPath);
        if (localLoadSuceess == false)
        {
        MyTools.PrintDebugLog("ucvr sdcard read failed start read backup " + beixuan);
            localLoadSuceess = readLocalConfig(beixuan);
        }
        if (localLoadSuceess)//如果配置文件存在读取配置文件
        {
                
            string sm = configValueByKey("startMode=");
            MyTools.PrintDebugLog("ucvr loadConfig from file startMode=" + sm);

            string tempStartMode = sm;
            if (tempStartMode != null)
                CyberCloudConfig.startMode = int.Parse(sm);
            string tempLoginInfo = configValueByKey("loginInfo=");
            if (tempLoginInfo != null && tempLoginInfo.Length > 0)
            {
                CyberCloudConfig.ExportOnlyPlayer = true;
                CyberCloudConfig.loginInfo = tempLoginInfo;
            }
            string ShowFPS = configValueByKey("ShowFPS=");
            useNullDeviceSN = configValueByKey("useNullDeviceSN=");
            CyberCloudConfig.mapapps = configValueByKey("mapapps=");
            CyberCloudConfig.castScreenTestConfigFileUrl = configValueByKey("castScreenTestConfigFileUrl=");
            CyberCloudConfig.TestAutoStartAppID = configValueByKey("TestAutoStartAppID=");
            CyberCloudConfig.DeepoonCastScreenDemonstration = configValueByKey("DeepoonCastScreenDemonstration=");
            CyberCloudConfig.printFrameTimeDelay = configValueByKey("printFrameTimeDelay=") != null ? int.Parse(configValueByKey("printFrameTimeDelay=")) : 0;
            CyberCloudConfig.autoStartCastScreenTestService = configValueByKey("AutoStartCastScreenTestService=") != null ? int.Parse(configValueByKey("AutoStartCastScreenTestService=")) : 0;
            CyberCloudConfig.autoStartCastScreen = configValueByKey("autoStartCastScreen=") != null ? int.Parse(configValueByKey("autoStartCastScreen=")) : 0;
            CyberCloudConfig.showCastScreen = configValueByKey("showCastScreen=") != null ? int.Parse(configValueByKey("showCastScreen=")) : 0;//
            CyberCloudConfig.statisticsUpLoad = configValueByKey("statisticsUpLoad=") != null ? int.Parse(configValueByKey("statisticsUpLoad=")) : 0;
            CyberCloudConfig.controllerType = configValueByKey("controllerType=") != null ? int.Parse(configValueByKey("controllerType=")) : 0;
            CyberCloudConfig.useTerminalFrmRtCtrl = configValueByKey("useTerminalFrmRtCtrl=") != null ? int.Parse(configValueByKey("useTerminalFrmRtCtrl=")) : 0;
            CyberCloudConfig.CyberZoneCode= configValueByKey("CyberZoneCode=");
            CyberCloudConfig.CyberZoneDesc = configValueByKey("CyberZoneDesc=");
            CyberCloudConfig.startParam = configValueByKey("startParam=");
            CyberCloudConfig.logOutLevel = configValueByKey("logOutLevel=") != null ? int.Parse(configValueByKey("logOutLevel=")) : 6;

            CyberCloudConfig.deviceTestNumJscj = configValueByKey("deviceTestNumJscj=") != null ? int.Parse(configValueByKey("deviceTestNumJscj=")) : 0;
            string terminalType = configValueByKey("terminalType=");
            CyberCloudConfig.terminalType = terminalType != null&& terminalType!="" ? int.Parse(terminalType) : 0;
            string usePingCmd = configValueByKey("usePingCmd=");
            CyberCloudConfig.usePingCmd = usePingCmd != null && usePingCmd != "" ? int.Parse(usePingCmd) : 1;//默认启用ping指令

            string tenantID = configValueByKey("tenantID=");
            CyberCloudConfig.tenantID = tenantID != null && tenantID != "" ? tenantID : "cybercloud";//默认租户是cybercloud
            //usePingCmd
            //是否使用應用分辨率
            CyberCloudConfig.useAppResolution = configValueByKey("useAppResolution=") != null ? int.Parse(configValueByKey("useAppResolution=")) : 0;
            CyberCloudConfig.deltaTimeTest = configValueByKey("deltaTimeTest=") != null ? int.Parse(configValueByKey("deltaTimeTest=")) : 0;
            string type = configValueByKey("classificationOfProfessions=");
            CyberCloudConfig.classificationOfProfessions = type != null && type != "" ? type : CyberCloudConfig.classificationOfProfessions;//默认app有配置时读取配置
            CyberCloudConfig.adapt_platform = configValueByKey("adapt_platform=");
            CyberCloudConfig.newScreenProtocol = configValueByKey("newScreenProtocol=") != null ? int.Parse(configValueByKey("newScreenProtocol=") ): 0;
            Debug.Log("ucvr adapt_platform:" + CyberCloudConfig.adapt_platform);
            string lan = configValueByKey("cvrLaguage=");
            if (lan != null && lan != "")
                CyberCloudConfig.cvrLanguage = lan;
            CyberCloudConfig.tls = configValueByKey("tls=");
            if (CyberCloudConfig.tls.IndexOf(GameAppControl.gatewayPort) < 0)//网关的统计服务地址通过网关接口获取
                CyberCloudConfig.statisticsUpLoadUrl = configValueByKey("statisticsUpLoadUrl=");
            MyTools.PrintDebugLog("ucvr loadConfig tls:"+ CyberCloudConfig.tls+";startMode:" + CyberCloudConfig.startMode + 
                ";loginInfo:" + CyberCloudConfig.loginInfo + ";useNullDeviceSN" + useNullDeviceSN +
                ";castScreenTestConfigFileUrl:" + CyberCloudConfig.castScreenTestConfigFileUrl + 
                ";TestAutoStartAppID:" + CyberCloudConfig.TestAutoStartAppID + 
                ";printFrameTimeDelay:" + CyberCloudConfig.printFrameTimeDelay +
                ",logOutLevel" + CyberCloudConfig.logOutLevel + 
                ";classificationOfProfessions:" + CyberCloudConfig.classificationOfProfessions + 
                ";statisticsUpLoad:" + CyberCloudConfig.statisticsUpLoad+
                ";terminalType:"+ CyberCloudConfig.terminalType+
                "deviceTestNumJscj:"+ CyberCloudConfig.deviceTestNumJscj+
                ";useAppResolution:" + CyberCloudConfig.useAppResolution+
                ";newScreenProtocol:" + CyberCloudConfig.newScreenProtocol+
                ";cvrLaguage:"+lan+ ";usePingCmd:"+ usePingCmd);
        }
        else
        {
            MyTools.PrintDebugLog("ucvr no need loadconfig ");
        }
    }
    private void configFromAssetsConfig(IniFile ini, string name) {
        CyberCloudConfig.mapapps = ini.GetValue(name, "mapapps", ""); 
        CyberCloudConfig.tls = ini.GetValue(name, "tls", ""); 
        CyberCloudConfig.currentType = ini.GetValue(name, "deviceType", "");
        CyberCloudConfig.adapt_platform = ini.GetValue(name, "adapt_platform", "");
        string newScreenProtocol = ini.GetValue(name, "newScreenProtocol", "");
        CyberCloudConfig.newScreenProtocol = newScreenProtocol != null&& newScreenProtocol!="" ? int.Parse(ini.GetValue(name, "newScreenProtocol", "")) : 0;
       // Debug.Log("ucvr adapt_platform:" + CyberCloudConfig.adapt_platform);
        string terminalType = ini.GetValue(name, "terminalType", "");
        CyberCloudConfig.terminalType = terminalType != null && terminalType != "" ? int.Parse(terminalType) : 0;
        string tenantID = ini.GetValue(name, "tenantID", ""); ;
        CyberCloudConfig.tenantID = tenantID != null && tenantID != "" ? tenantID : "cybercloud";//
        string usePingCmd = ini.GetValue(name, "usePingCmd", "");
        CyberCloudConfig.usePingCmd = usePingCmd != null && usePingCmd != "" ? int.Parse(usePingCmd) : 1;//默认启用ping指令
        CyberCloudConfig.cvrScreen = ini.GetValue(name, "cvrScreen", "");
        CyberCloudConfig.cvrLanguage = ini.GetValue(name, "cvrLanguage", "");        
        CyberCloudConfig.autoStartCastScreenTestService = int.Parse( ini.GetValue(name, "autoStartCastScreenTestService", ""));
        CyberCloudConfig.autoStartCastScreen = int.Parse(ini.GetValue(name, "autoStartCastScreen", ""));        
        string type = ini.GetValue(name, "classificationOfProfessions", "");
        CyberCloudConfig.classificationOfProfessions = type != null && type != "" ? type : CyberCloudConfig.classificationOfProfessions;//默认app有配置时读取配置
        MyTools.PrintDebugLog("ucvr configFromAssetsConfig tls:" + CyberCloudConfig.tls +
            ";currentType:" + CyberCloudConfig.currentType + ";cvrLanguage" + CyberCloudConfig.cvrLanguage +
            ";autoStartCastScreenTestService:" + CyberCloudConfig.autoStartCastScreenTestService +
            ";autoStartCastScreen:" + CyberCloudConfig.autoStartCastScreen +
            ";classificationOfProfessions:" + CyberCloudConfig.classificationOfProfessions+ ";terminalType:"+ CyberCloudConfig.terminalType+
            "; newScreenProtocol:" + CyberCloudConfig.newScreenProtocol+ ";usePingCmd:"+ usePingCmd);

    }
    /**测试模式 是否使用空的唯一标示 1:使用空的唯一标识 0：不使用空的唯一标识**/
    public static string useNullDeviceSN = null;
    /// <summary>
    /// 配置文件信息
    /// </summary>
    private string[] fileConfig = null;
    private bool readLocalConfig(string fileName)
    {
        try
        {
            if (File.Exists(fileName))
            {
                fileConfig = File.ReadAllLines(fileName);
                return true;
            }
        }
        catch (Exception e) {
            MyTools.PrintDebugLogError("ucvr readLocalConfig fileName:" + fileName + ";"+e.Message);
        }
        return false;
    }
    /// <summary>
    /// 根据关键词解析配置文件中的数据
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    private string configValueByKey(string key)
    {
        try
        {
            return MyTools.configValueByKey(key, fileConfig);
          
        }
        catch (Exception e) {
            MyTools.PrintDebugLogError("ucvr configValueByKey key:"+key+";"+e.Message);
        }
        return null;
    }
}
