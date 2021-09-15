using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 没有手柄时焦点碰撞boxcollider后需要显示出凝视点
/// 有手柄时需要隐藏凝视点 和凝视线
/// </summary>
public class BoxColliderOnNoController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        UIEventListener.Get(this.gameObject).onHover = OnButtonHover;
    }
    void OnButtonHover(GameObject go, bool focus)
    {
        if (!GameAppControl.getGameRuning())
            return;

        if (focus)
        {

            // Debug.Log("===closeOrOpenCursor====" + focus);
            //游戏中只有头盔没有手柄时才显示凝视点
            if (CommonPlane.handlerList == null)
            {

                ControllerTool.CloseOrOpenAnyEnableContrlorCursor(CyberCloud_UnitySDKAPI.CursorStatus.openCursor);
            }
            else {//有手柄需要隱藏凝視點
                ControllerTool.CloseOrOpenAnyEnableContrlorCursor(CyberCloud_UnitySDKAPI.CursorStatus.closeCursor);
            }
        }
        else
        {
            ControllerTool.CloseOrOpenAnyEnableContrlorCursor(CyberCloud_UnitySDKAPI.CursorStatus.closeCursor);
        }

    }
    // Update is called once per frame
    void Update () {
		
	}
}
