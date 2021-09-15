using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;


public class TestCastScreen : MonoBehaviour {


    [SerializeField]
    private List<UIEventListener> btList;
    [SerializeField]
    private UILabel playUrl;
    [SerializeField]
    private UILabel titles;
    [SerializeField]
    private UILabel eventName;
    [SerializeField]
    private UILabel resultTest;
    private GameAppControl gameAppControl;
    private MyTools mytools_open;
    private MyTools mytools_close;
    private MyTools mytools_video;
    private string savefile = "";
    private CommonPlane commonPlaneCom;
    //public static string startCastScreenTest;
    XMPPTool xmpp;
    void Start()
    {
        GameObject commonPlane = GameObject.Find("CyberCloudCommonPlane");
        if (commonPlane == null)
            MyTools.PrintDebugLogError("ucvr commonPanel mast added in screne");
        commonPlaneCom = commonPlane.GetComponent<CommonPlane>();

        GameObject gamePlane = GameObject.Find("GamePlane");
        savefile = Application.persistentDataPath ;
        if (gamePlane != null)
        {
            gameAppControl = gamePlane.GetComponent<GameAppControl>();
        }
        strPlayUrl = gameAppControl.Cyber_GetCastScreenPlayUrl();
        MyTools.PrintDebugLog("ucvr playurl:"+ strPlayUrl);

        for (int i = 0; i < btList.Count; i++) {
            btList[i].onClick = OnButtonClick ;
        }

        xmpp = GameObject.Find("cvrMsgNotify").GetComponent<XMPPTool>() ;
      
        btList[1].gameObject.SetActive(false);

        titles.text="配置文件服务器ip和端口："+ CyberCloudConfig.castScreenTestConfigFileUrl;
    }

   void OnEnable()
    {
        strPlayUrl = gameAppControl.Cyber_GetCastScreenPlayUrl();
    }
        //       string _jsonParas;
        XMPPData data_open = new XMPPData();
    XMPPData data_close = new XMPPData();
    XMPPData data_video = new XMPPData();
    private void mytoolsVoidDelegateWWWDatatest(MyTools.ToolBaseResult result,string state)
    {
        try
        {
            //resultTest.text
            MyTools.PrintDebugLog("ucvr mytoolsVoidDelegateWWWDatatest result:" + result.retCode);
            if (result.retCode != 0)
            {
                MyTools.PrintDebugLogError("ucvr load" + CyberCloudConfig.castScreenTestConfigFileUrl + "/" + state + ".ini failed");
                resultTest.text = state + ".ini load failed";
                btList[1].gameObject.SetActive(false);
                btList[0].gameObject.SetActive(true);
                return;
            }
            else
            {
                if (state == "close")
                    btList[1].gameObject.SetActive(true);
            }
            bool localLoadSuceess = readLocalConfig(savefile + "/" + state + ".ini");
            if (localLoadSuceess)//如果配置文件存在读取配置文件
            {
                string eventNamestr = configValueByKey("functionType=");
                //eventName.text = "事件名："+eventNamestr +"   加载时间："+ DateTime.Now;

                XMPPData data = new XMPPData();
             
                data.functionType = eventNamestr;

                data.platform = configValueByKey("platform=");
                data.action = configValueByKey("action=");
                data.playUrl = configValueByKey("playUrl=");

                data.mediaCode = configValueByKey("mediaCode=");
                data.mediaType = configValueByKey("mediaType=");
                data.playByBookMark = configValueByKey("playByBookMark=");
                data.playByTime = configValueByKey("playByTime=");

                data.actionSource = configValueByKey("actionSource=");
                data.delay = configValueByKey("delay=");
                data.trickPlayControl = configValueByKey("trickPlayControl=");
               

                data.rot = configValueByKey("rot=");
                data.userID = configValueByKey("userID=");
                data.videoType = configValueByKey("videoType=");
                // 
                try
                {
                    string trickplayMode = configValueByKey("trickplayMode=");
                    data.trickplayMode = trickplayMode!=null&& trickplayMode!=""?int.Parse(trickplayMode):0;
                    string isvrStr = configValueByKey("isVR=");
                    data.isVR = isvrStr != null && isvrStr != "" ? int.Parse(isvrStr) : 0;
                    string fs = configValueByKey("fastSpeed=");
                    data.fastSpeed = fs != null && fs != "" ? int.Parse(fs) : 0;
                    string seekPostionstr = configValueByKey("seekPostion=");
                    data.seekPostion = seekPostionstr != null && seekPostionstr != "" ? int.Parse(seekPostionstr) : 0;

                }
                catch (Exception e)
                {
                    MyTools.PrintDebugLogError("ucvr config readError :" + e.StackTrace);
                }

                if (state == "open")
                    data_open = data;
                else if (state == "video")
                {
                    data_video = data;
                    if (data_video.playUrl == null || data_video.playUrl == "")
                    {
                        sartUrlIsNull = true;
                        data_video.playUrl = strPlayUrl;
           
                    }
                    else {

               
                    }
                }
                else
                    data_close = data;
                //string str = JsonConvert.SerializeObject(data_open);

            }
            else
            {
                MyTools.PrintDebugLogError("ucvr config downloadfile no find:" + savefile);
                resultTest.text = "配置文件 " + state + ".ini加载失败";
                btList[1].gameObject.SetActive(false);
                btList[0].gameObject.SetActive(true);
            }
        }
        catch (Exception e)
        {
            MyTools.PrintDebugLogError("ucvr assetload :" + e.StackTrace);

            resultTest.text = "配置文件 " + state + ".ini加载失败";
            btList[1].gameObject.SetActive(false);
            btList[0].gameObject.SetActive(true);
        }

    }
    private bool sartUrlIsNull;
    private string configValueByKey(string key)
    {
        try
        {
            if (fileConfig == null)
                return null;
            foreach (string st in fileConfig)
            {
                if (st.StartsWith(key))
                {
                    //string[] kv = st.Split(key.ToCharArray());
                    string[] kv = Regex.Split(st, key, RegexOptions.IgnoreCase);
                    if (kv.Length > 1)
                    {
                        return kv[1];
                    }
                    else
                        MyTools.PrintDebugLogError("ucvr configValueByKey key:" + key);
                }

            }

        }
        catch (Exception e)
        {
            MyTools.PrintDebugLogError("ucvr configValueByKey key:" + key + ";" + e.Message);
        }
        return null;
    }
    string[] fileConfig;
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
        catch (Exception e)
        {
            MyTools.PrintDebugLogError("ucvr readLocalConfig fileName:" + fileName + ";" + e.Message);
        }
        return false;
    }
    private int startTimes = 0;
    //投屏每次循环需要20秒，10秒超时时间 投屏5秒 关闭等5秒再启动
    public IEnumerator loopSendMessage() {
        startTimes = 0;
        while (startTest) {
            startTimes = startTimes + 1;
            //if (startTest)
            castScreen =XMPPTool.CastScreen.castScreenTryConnect;
            MyTools.PrintDebugLog("ucvr TestCastScreen startTimes: "+ startTimes);
            string jsonParas = JsonConvert.SerializeObject(data_open);
         
            xmpp.sendBroadcast(jsonParas);
            
            yield return new WaitForSeconds(20f);
        }
       

        yield return true;
    }
    /// <summary>
    /// 投屏成功后延迟5秒关闭投屏
    /// </summary>
    /// <returns></returns>
    public IEnumerator closeCastScreen()
    {
    
        yield return new WaitForSeconds(10f);
        string jsonParas = JsonConvert.SerializeObject(data_close);
    
        castScreen = XMPPTool.CastScreen.noCastScreen;
        xmpp.sendBroadcast(jsonParas);
    }
    public void StartDownLoad(string url, string file_SaveUrl)
    {
        btList[1].gameObject.SetActive(false);
        mytools_open = new MyTools();
        mytools_open.wwwDataLoad = openResult;///
        StartCoroutine(mytools_open.DownFile(url+"/open.ini", file_SaveUrl + "/open.ini"));
    
        mytools_video = new MyTools();
        mytools_video.wwwDataLoad = videoResult;///
        StartCoroutine(mytools_video.DownFile(url + "/video.ini", file_SaveUrl + "/video.ini"));

        mytools_close = new MyTools();
        mytools_close.wwwDataLoad = closeResult;///
        StartCoroutine(mytools_close.DownFile(url + "/close.ini", file_SaveUrl + "/close.ini"));
    }
    private void openResult(MyTools.ToolBaseResult result) {
        mytoolsVoidDelegateWWWDatatest(result,"open");
    }
    private void closeResult(MyTools.ToolBaseResult result)
    {
        mytoolsVoidDelegateWWWDatatest(result, "close");
    }
    private void videoResult(MyTools.ToolBaseResult result)
    {
        mytoolsVoidDelegateWWWDatatest(result, "video");
    }
    private string strPlayUrl;
    void Update()
    {
        playUrl.text = "playUrl:"+strPlayUrl;
     
    }
    private bool startLoad = false;
    void OnGUI()
    {
        
    }
    private List<string> jsonStrResult;
    public static XMPPTool.CastScreen castScreen = XMPPTool.CastScreen.noCastScreen;
    int linenum = 0;
    private void messagePro(string jsonStr)
    {
        MyTools.PrintDebugLog("ucvr messagePro Msg:" + jsonStr);
        if (linenum > 11)
        {
            resultTest.text = "";
            linenum = 0;
        }
           resultTest.text += jsonStr + "\n";
        

        linenum = linenum + 1;




        if (jsonStr != null && !jsonStr.Equals(""))
        {
            //XMPPData r= JsonConvert.DeserializeObject<XMPPData>(jsonStr);
            //if (r.functionType == VRSTBReady) {
            //兰亭有个bug在close的时候发送了VRSTBReady，牛博说我程序健壮性的问题 好吧我忍了 此处加个判断吧 castScreen ==CastScreen.CastScreenTryConnect;
            if ((jsonStr.IndexOf(XMPPTool.VRSTBReady) > -1 || jsonStr.IndexOf(XMPPTool.lantingsx) > -1) && castScreen == XMPPTool.CastScreen.castScreenTryConnect)
            {
                if(sartUrlIsNull == true)
                    data_video.playUrl = strPlayUrl;
                string jsonParas = JsonConvert.SerializeObject(data_video);
      
                xmpp.sendBroadcast(jsonParas);
                castScreen = XMPPTool.CastScreen.castScreenTryConnect;
            }
            else if (jsonStr.IndexOf(XMPPTool.VRSTBError) > -1)
            //else if (r.functionType == VRSTBError)
            {
                castScreen = XMPPTool.CastScreen.noCastScreen;
                //处理异常
            
                commonPlaneCom.showHintMstByDesckey("Screen_casting_failed", 3, XMPPTool.VRSTBError);
                MyTools.PrintDebugLog("ucvr castScreen failed please try later");

            }
            else if (jsonStr.IndexOf(XMPPTool.VRVideoReady) > -1)
            //else if (r.functionType == VRVideoReady)
            {
                MyTools.PrintDebugLog("ucvr TestCastScreen currentTimes: " + startTimes+";result:success");
                castScreen = XMPPTool.CastScreen.castScreening;
                StartCoroutine(closeCastScreen());
            }
           
        }

    }
    private bool startTest = false;
    void OnButtonClick(GameObject obj)
    {
  
        if (obj.name == "btTest_close")
        {
            MyTools.PrintDebugLog("ucvr btTest_close ");
            xmpp.testCastScreenMessageCallBack = null;
            startTest = false;
            resultTest.text = "";
            linenum = 0;
            StartCoroutine(closeCastScreen());
            this.gameObject.SetActive(false);
        }
        else if (obj.name == "btTest_send")
        {
            MyTools.PrintDebugLog("ucvr btTest_send ");
            btList[2].gameObject.SetActive(true);
            if (data_open != null&&data_video!=null&&data_close!=null)
            {
                startTest = true;
                xmpp.testCastScreenMessageCallBack = messagePro;
                StartCoroutine(loopSendMessage());
                titles.text = "广播已发送";
            }
            else {
                MyTools.PrintDebugLogError("ucvr config error");
            }
        }
        else {
            startLoad = true;
            MyTools.PrintDebugLog("ucvr btTest_load ");
            StartDownLoad(CyberCloudConfig.castScreenTestConfigFileUrl, savefile);
            obj.SetActive(false);
        }



    }


}
