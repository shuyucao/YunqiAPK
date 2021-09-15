using UnityEngine;
using System.Collections;

public class QuitGame : MonoBehaviour {
    private GameAppControl gameAppControl;
    // Use this for initialization
    void Start () {
        GameObject gamePlane = GameObject.Find("GamePlane");
        if (gamePlane != null)
        {

            gameAppControl = gamePlane.GetComponent<GameAppControl>();
        }
    }
   
    // Update is called once per frame
    void Update()
    {
        //portal中按横向的标需要返回到launcher

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameAppControl.getGameRuning() == false)
            {
                Debug.Log("ucvr : KeyCode.Escape exit portal");
       
                DataLoader.Instance.portalAPI.stopSessionReport();
                
                if (!GameAppControl.enableSafePanel)
                {
                    MyTools.setSystemProperties("persist.pvrcon.seethrough.enable", "1");
                }
                #if picoSdk
                Pvr_UnitySDKAPI.Controller.UPvr_IsEnbleHomeKey(true);
#endif
                Application.Quit();
                System.Diagnostics.Process.GetCurrentProcess().Kill();//解決home鍵有時沒退乾淨的問題
            }
            else
            {//游戏里按返回返回portal如果是直接启动的游戏返回到launcher
                print("ucvr exitCyberCloud KeyCode.Escape");
                StartCloudAppTest.startTestAutoStartapp = false;
                gameAppControl.exitCyberGame();//
                if (CyberCloudConfig.ExportOnlyPlayer)
                {
                    DataLoader.Instance.portalAPI.stopSessionReport();
                    if (!GameAppControl.enableSafePanel)
                    {
                        MyTools.setSystemProperties("persist.pvrcon.seethrough.enable", "1");
                    }
#if picoSdk
                    Pvr_UnitySDKAPI.Controller.UPvr_IsEnbleHomeKey(true);
#endif
                    Application.Quit();
                    System.Diagnostics.Process.GetCurrentProcess().Kill();//解決home鍵有時沒退乾淨的問題
                }
            }
        }
       
    }
}
