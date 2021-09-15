using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppStartOrExitCallBack : MonoBehaviour {
    private GameAppControl gameAppCtr;
    private CommonPlane commonPlaneCom;
    // Use this for initialization
    void Start () {
        GameObject commonPlane = GameObject.Find("CyberCloudCommonPlane");
        if (commonPlane == null)
            MyTools.PrintDebugLogError("ucvr commonPanel mast in screne");
        commonPlaneCom = commonPlane.GetComponent<CommonPlane>();
        gameAppCtr = this.gameObject.GetComponent<GameAppControl>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
  
    public void statusCallback(string statusType, RetData param)
    {
        MyTools.PrintDebugLog("ucvr statusCallback:"+ statusType);
        if (param == null)
        {
            param = new RetData();
            param.appRetCode = -1;
            param.msg = VRPortalCommunication.defaultError + "(" + statusType + ")";
        }
        if (statusType.Equals("appStarting"))
        {//应用启动状态中
            if (param.appRetCode != 0)
            {
                gameAppCtr.startResult(param.appRetCode);
            }
            else {
                if (GameAppControl.getGameRuning()) {
                    //commonPlaneCom.showHintMstByDesckey("应用启动中..", -1, 332);
                }
                else
                    MyTools.PrintDebugLog("ucvr game is not runing no needshow appstarting");
            }              
        }
        else if (statusType.Equals("appStartDone"))
        {//启动结束          
            gameAppCtr.startResult(param.appRetCode);           
        }
        else if (statusType.Equals("appExiting"))
        {//退出中

            if (param.appRetCode != 0)
            {

                gameAppCtr.exitResult(param.appRetCode);
            }
            else {
                commonPlaneCom.showHintMstByDesckey("Application_exit", -1);
            }
        }
        else if (statusType.Equals("appExitDone"))
        {//退出结束
        
            gameAppCtr.exitResult(param.appRetCode);
        }
        else
        {
            MyTools.PrintDebugLogError("ucvr unknown statusType：" + statusType);
        }
    }
}
