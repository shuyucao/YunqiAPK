using System.Collections;
using System.Collections.Generic;
using Assets.CyberCloud.Scripts.OpenApi;
using UnityEngine;
/// <summary>
/// 友好提示
/// </summary>
public class TipsControl : MonoBehaviour
{
    [SerializeField]
    private GameObject appControlTips;
    [SerializeField]
    private GameObject setingBt;
    [SerializeField]
    private CommonPlane commonPlane;
    private Transform center;


    private MyDialog mydialog;
    /**
   * 距离相机的位置
   * */

    public int distance = 3;


    public enum TipesType
    {
        /// <summary>
        /// 退出提示
        /// </summary>
        showExitTips
    }
    // Use this for initialization
    void Start()
    {
        if (OpenApiImp.CyberCloudOpenApiEnable) {
            this.gameObject.SetActive(false);
        }
            GameObject gamePlane = GameObject.Find("GamePlane");
        mydialog = commonPlane.getDialog();
        if (gamePlane != null)
        {
            center = gamePlane.transform;
            SystemAppTips appTips = null;//= gameObject.GetComponentInChildren<SystemAppTips>();//app键提示
            foreach (Transform child in transform)
            {
                if (child.gameObject.name.Equals("controllerApp"))
                {
                    GameObject controllerApp = child.gameObject;
                    appTips = child.gameObject.GetComponent<SystemAppTips>();
                    break;
                }

            }
            if (appTips == null)
                MyTools.PrintDebugLogError("ucvr SystemAppTips mast added on controllerApp");
            appTips.setCenter(center);
        }
        else
        {
            MyTools.PrintDebugLogError("ucvr GamePlane can not be finded");
        }

        if (appControlTips == null || setingBt == null || mydialog == null)
            Debug.LogError("param appControl,exitBt ,mydialog can not be null");
        //StartCoroutine(testwhile());
    }
    IEnumerator testwhile()
    {
        yield return new WaitForSeconds(10);

    }
    // Update is called once per frame
    void Update()
    {
        //Debug.Log("============Update==========================");
        return;

        //Debug.Log("GameAppControl.getGameRuning():" + GameAppControl.getGameRuning());
        if (GameAppControl.getGameRuning())
        {

            if (CommonPlane.handlerList != null)
            {
                //手柄连接上时并且有设置键 需要隐藏设置键
                if (setingBt.gameObject.activeSelf)
                {

                    setingBtVisble(false);
                    //隐藏凝视点
                    ControllerTool.CloseOrOpenAnyEnableContrlorCursor(CyberCloud_UnitySDKAPI.CursorStatus.closeCursor);
                }
                if (menuKeyDown())
                {
                    //游戏运行时点击应用键退出

                    MyTools.PrintDebugLog("ucvr click App key game runing isShowSetingDialog" + SetingDialog.isShowSetingDialog);

                    if (SetingDialog.isShowSetingDialog)
                    {

                        commonPlane.closeAllDialog();
                    }
                    else
                    {
                        commonPlane.closeAllDialog();
                        commonPlane.showSetingDialog();
                    }
                }
            }
            else
            {//没有手柄时需要显示设置按钮
                menuDown = new bool[2];
                _touchDown = new bool[2];
                _triggerDown = new bool[2];
                if (!setingBt.gameObject.activeSelf)
                {
                    MyTools.PrintDebugLog("ucvr mei you shou bing lianjie xianshi shezhi");

                    setingBtVisble(true);

                }
            }

        }
        else
            setingBtVisble(false);
    }
    public void setingBtVisble(bool b)
    {
     
        if (b)
        {
            if (b)
            {
                Debug.LogError("ucvr activeSelfstart============================inity---------------------------------------");
                setingBt.gameObject.GetComponentInChildren<SetingBt>().startTips(appControlTips);
                setingBt.gameObject.GetComponentInChildren<UISprite>().color = Color.white; ;
                //setingBt.
            }
        }
        setingBt.gameObject.SetActive(b);
    }
    private bool[] menuDown = new bool[2];//菜单键是否可用
    private bool[] _touchDown = new bool[2];//按键按下是瞬时的需要保存按下的状态
    private bool[] _triggerDown = new bool[2];
    public static bool ZUHEJIAN = true;
    private bool menuKeyDown()
    {
        if (ZUHEJIAN == false)
        {
            //非组合键
            return ControllerTool.getControllerBtUp(CyberCloud_UnitySDKAPI.ControllerKeyCode.APP);
        }
        else
        {
            if (menuKeyDown(0) || menuKeyDown(1))
            {
                return true;
            }
            else
                return false;
        }

    }
    private bool menuKeyDown(int index)
    {
        bool APPdown = ControllerTool.getControllerBtDown(CyberCloud_UnitySDKAPI.ControllerKeyCode.APP, index);
        if (APPdown)
        {
            _touchDown[index] = true;
        }
        else
        {
            bool APPup = ControllerTool.getControllerBtUp(CyberCloud_UnitySDKAPI.ControllerKeyCode.APP, index);
            if (APPup)
            {
                _touchDown[index] = false;
            }
        }
        bool triggrtdown = ControllerTool.getControllerBtDown(CyberCloud_UnitySDKAPI.ControllerKeyCode.TRIGGER, index);
        if (triggrtdown)
        {
            _triggerDown[index] = true;
        }
        else
        {
            bool triggrtUp = ControllerTool.getControllerBtUp(CyberCloud_UnitySDKAPI.ControllerKeyCode.TRIGGER, index);
            if (triggrtUp)
                _triggerDown[index] = false;
        }

        if (_triggerDown[index] && _touchDown[index])
        {
            if (menuDown[index])//如果按下中只返回一次下次等弹起后再返回
                return false;
            menuDown[index] = true;

        }
        else
            menuDown[index] = false;
        return menuDown[index];

    }


    /// <summary>
    /// 如果有手柄显示 点击app键弹出系统菜单提示
    /// 如果没有手柄显示退出按钮
    /// </summary>
    /// <param name="display">是显示还是隐藏</param>
    /// <param name="type"></param>
    public void dispalyCursorTips(bool display, TipesType type)
    {
        if (display == false)
        {
            appControlTips.SetActive(false);
            MyTools.PrintDebugLog("ucvr close tips");
            return;
        }
        if (type == TipesType.showExitTips && DialogBase.isShow == false)
        {
            /**
             *     * **/
            //弹出退出提示
            if (CommonPlane.handlerList != null)
            {
                MyTools.PrintDebugLog("ucvr you shou bing lianjie ");

            }
            else
            {
                MyTools.PrintDebugLog("ucvr mei you shou bing lianjie xianshi shezhi");
                //没有手柄连接时显示退出按钮

            }
            if (CyberCloudConfig.currentType != CyberCloudConfig.DeviceTypes.Pico2)//pico2不显示外设提示              
                    appControlTips.SetActive(true);

        }

    }



}
