using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCloudAppTest:MonoBehaviour  {
    /// <summary>
    /// 流化显示界面
    /// </summary>
    GameObject gamePanel;
    private GameAppControl gameAppControl;
    public static bool startTestAutoStartapp;
    public static int startTimes;
    // Use this for initialization
    void Start()
    {
        gamePanel = GameObject.Find("GamePlane");
        if (gamePanel == null)
            MyTools.PrintDebugLogError("ucvr GamePlane mast added in screne");
        else {
            gameAppControl = gamePanel.GetComponent<GameAppControl>();
        }
    }
    void Update()
    {
  

    }
    public void startAppById(string appid) {
        startTimes = 0;
    
        startTestAutoStartapp = true;
        StartCoroutine(loopSendMessage(appid));
    }
   
    private float lastStartTime = 0;
    public IEnumerator loopSendMessage(string appid)
    {
        //startTimes = 0;
        while (startTestAutoStartapp)
        {
            MyTools.PrintDebugLog("ucvr TestAutoStartApp startTimes: " + startTimes);
            if (GameAppControl.getGameRuning() == false)
            {
                startTimes = startTimes + 1;
                lastStartTime = Time.time;
                onItemClick(appid);
            }
            else
            {
                //启动成功后1分钟关闭
                if (Time.time - lastStartTime > 60) {
                    gameAppControl.exitCyberGame();
                }
            }
            yield return new WaitForSeconds(2f);
        }
        yield return true;
    }

     void onItemClick(string appid)
    {
     
        string startCloudUrl = DataLoader.Instance.portalAPI.GetStartCloudAppUrl(appid);

        Debug.Log("call startApp");
        string tempStartCloudUrl = "";
        //将流化串的backappid置空这样 流化应用内主动退出就不会退回到流化portal了
        if (startCloudUrl != null && startCloudUrl.IndexOf("&BackAppID=") > 0)
            tempStartCloudUrl = startCloudUrl.Replace("&BackAppID=", "&BackAppID =-1&UcvrDelecte=");
        else
            tempStartCloudUrl = startCloudUrl;
        gamePanel.GetComponent<GameAppControl>().startApp(tempStartCloudUrl);

    }
}
