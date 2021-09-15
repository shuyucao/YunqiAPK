using UnityEngine;

public class GalleryTools
{
    //显示退出提示“再按一次退出”
    public static void QuitApp()
    {
        
            QuitAlert.Show(Quit);
    }
   
    private static void Quit()
    {
        if (!GameAppControl.getGameRuning())
        {
#if picoSdk
            Pvr_UnitySDKAPI.Controller.UPvr_IsEnbleHomeKey(true);
#endif
			Application.Quit();
            if (DataLoader.Instance.portalAPI != null)
                DataLoader.Instance.portalAPI.stopSessionReport();
        }
        else
        {
            GameObject gamePlane = GameObject.Find("GamePlane");
            if (gamePlane != null)
            {
                GameAppControl gameAppControl = gamePlane.GetComponent<GameAppControl>();
                Debug.Log("ucvr Quit exitCyberCloud ");
                gameAppControl.exitCyberGame();
            }
        }

    }

    public static void ShowLeftBar(bool isshow)
    {
        Bundle b = new Bundle();
        b.SetValue("isShow", isshow);
        MsgManager.Instance.SendMsg(MsgID.LeftMenu, b);
    }
}