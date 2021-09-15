using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// jar通知unity 通知消息体
/// </summary>
public class ComunicationResult  {
  
    public string appStatus;
    public RetData retData;

}
public class RetData {
    public int appRetCode;
    public string msg;
}
public class SystemStatusResult
{

    public string systemStatus;
    public string statusDescription;
    public systemRetData systemRetData;

}
public class AdaptPlatformCommand
{

    public string action;//”: “none/start/stop”,;
    public string cyberCloudEnterUrl;//”: “cybercloud://10.10.10.1:10531”,
    public string queueServiceBaseUrl;//”: “http://ip:port”
    public string appID;//
    public string appName;
    public string resolution;//”: “1920x1080”,
    public string frameRate;//”: 60,
    public string startDstResCode;//”: “192.168.1.1-rta-0”
    public string startExtParam;
}
public class systemRetData
{
    public string errCode;
}
