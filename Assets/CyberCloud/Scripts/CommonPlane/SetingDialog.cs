using System.Collections;
using System.Collections.Generic;
using Assets.CyberCloud.Scripts.OpenApi;
using UnityEngine;

public class SetingDialog : MonoBehaviour
{
    public delegate void ClickDelegate(ButtonIndex buttonIndex);
    public ClickDelegate ClickCallBackDelegate;
    public static string leftRightMode = "LeftRightMode";//手柄是左手模式还是右手模式 即左撇子还是右撇子
    public UISprite uisprite;
    /// <summary>
    /// 是否显示投屏按钮,lgu+试运营期间不需要投屏
    /// </summary>
    public static bool showCastScreen = true;
    [SerializeField]
    private CommonPlane commonPlane;
    public enum ButtonIndex
    {
        bt_ClickSameScreen,
        bt_Exit,
        bt_Close,
        bt_SwitchControl
    }
    /// <summary>
    ///0 同屏
    ///1 退出
    ///2 关闭
    /// </summary>
    [SerializeField]
    private List<UIEventListener> btList;
    [SerializeField]
    private UILabel castScreenLable;
    [SerializeField]
    private UILabel switchControlLable;
    private int btIndex = -1;
    private DialogBase db;
    private static string ystext = "切换到右手习惯";
    private static string zstext = "切换到左手习惯";

    private float[] btListTop=new float[4];


    // Use this for initialization
    void Start () {
        //bt_ok.onClick = OnButtonClickOK;
 
        btList[0].onClick = OnButtonClickSameScreen;
        btList[1].onClick = OnButtonClickExit;
        btList[2].onClick = OnButtonClickSwitchControl;
        btList[3].onClick = OnButtonClickClose;
        if (CyberCloudConfig.cvrScreen == CyberCloudConfig.CVRScreen.LGUJa) {
            MyTools.PrintDebugLogError("ucvr showCastScreen:"+ CyberCloudConfig.showCastScreen);
            if (CyberCloudConfig.showCastScreen == 1)
                showCastScreen = true;
            else
                showCastScreen = false;//不显示投屏按钮处理
        }

        //不显示投屏按钮处理
        for (int i = 0; i < 4; i++) {
  
            btListTop[i] = btList[i].gameObject.transform.localPosition.y;
            if (!showCastScreen)
            {
                if (i == 0)
                {
                    if (uisprite != null)
                        uisprite.height = 350;
                    btList[i].gameObject.SetActive(false);
              
                }
                else {
                    Vector3 pos = btList[i].gameObject.transform.localPosition;
                    btList[i].gameObject.transform.localPosition = new Vector3(pos.x, btListTop[i - 1], pos.z);
            
                }
               
            }
        }
      
        int mode = PlayerPrefs.GetInt(leftRightMode, 1);
        MyTools.PrintDebugLog("ucvr （0left,1right） current hand index：" + mode);
       
        if (mode == 0)
        {
           
            ystext = Localization.Get("right_hand_habit");
            switchControlLable.text = ystext;
        }
        else
        {
            zstext = Localization.Get("left_hand_habit");
            switchControlLable.text = zstext;
        }
    }
    private void intFocus() {
        if (btIndex == -1)
        {
            if (!showCastScreen)
            {
                setbtFocus(1);
            }
            else
            {
                setbtFocus(0);
            }
        }
    }
    // Update is called once per frame
    void Update () {

        if (CommonPlane.handlerList != null)
        {
            //组合键时前几针不处理防止出现连击
            /**  **/
            if (TipsControl.ZUHEJIAN)
            {
                if (frameNum < 2)
                {
                    bool triggerUp = ControllerTool.getControllerBtUp(CyberCloud_UnitySDKAPI.ControllerKeyCode.TRIGGER);
                    bool touchpadUp = ControllerTool.getControllerBtUp(CyberCloud_UnitySDKAPI.ControllerKeyCode.APP);
                    if (triggerUp)
                    {
                        frameNum += 1;
                    }
                    if (touchpadUp)
                    {
                        frameNum += 1;
                    }
                    if (frameNum == 2)
                    {
                        intFocus();
                    }
                    return;
                }

            }
            intFocus();
            int startIndex = showCastScreen ? 0 : 1;
            if (ControllerTool.getControllerBtUpDirection(CyberCloud_UnitySDKAPI.ControllerKeyCodePrivate.TOUCHPADUP))
            {//
                MyTools.PrintDebugLog("ucvr up");
                if (btIndex > 0)
                    setbtFocus(btIndex - 1);
            }
            else if (ControllerTool.getControllerBtUpDirection(CyberCloud_UnitySDKAPI.ControllerKeyCodePrivate.TOUCHPADDOWN))
            {//
                MyTools.PrintDebugLog("ucvr down");
                if (btIndex < btList.Count - 1)
                    setbtFocus(btIndex + 1);
            }
            else if (DialogBase.enterkeyup())
            {//一定要先判断方向将再判断确认键
                MyTools.PrintDebugLog("ucvr TOUCHPAD btIndex：" + btIndex);
                if (btIndex == 0)
                {
                    OnButtonClick1();
                }
                else if (btIndex == 1)
                {
                    OnButtonClick2();
                }
                else if (btIndex == 2)
                {
                    OnButtonClick3();
                }
                else
                {
                    OnButtonClick4();

                }
            }
        }
        else {
            //處理由手柄切換到頭盔時需要將，手柄遺留下來的凝視點清除掉
            if (btIndex != -1)
                setbtFocus(-1);
        }
    }
    //扳机键和确认键组合键弹出菜单保证是响应按下后再弹起而不只是弹起，否则刚显示就会关闭
    private int frameNum = 0;
  

    void OnGUI()
    {
        if (XMPPTool.castScreen == XMPPTool.CastScreen.noCastScreen)
        {
            string TV_for_screen = Localization.Get("TV_for_screen");
            castScreenLable.text = TV_for_screen;
        }
        else if (XMPPTool.castScreen == XMPPTool.CastScreen.castScreenTryConnect)
        {

            string Projective_connection = Localization.Get("Projective_connection");
            castScreenLable.text = Projective_connection;
        }
        else
        {
            string Close_screen = Localization.Get("Close_screen");
            castScreenLable.text = Close_screen;
        }
    }



    void OnButtonClickSameScreen(GameObject obj)
    {
        if (CommonPlane.handlerList != null)//防止練劍
            return;
        OnButtonClick1();

    }
    void OnButtonClick1() {
        MyTools.PrintDebugLog("ucvr OnButtonClickSameScreen");
        btIndex = 0;
        this.gameObject.SetActive(false);
        if (ClickCallBackDelegate != null)
            ClickCallBackDelegate(ButtonIndex.bt_ClickSameScreen);
        ClickCallBackDelegate = null;
    }
    void OnButtonClickExit(GameObject obj)
    {
        if (CommonPlane.handlerList != null)//防止練劍
            return;
        OnButtonClick2();

    }
    void OnButtonClick2() {
        MyTools.PrintDebugLog("ucvr OnButtonClickExit");
        btIndex = 1;
        this.gameObject.SetActive(false);

        if (ClickCallBackDelegate != null)
            ClickCallBackDelegate(ButtonIndex.bt_Exit);
        ClickCallBackDelegate = null;
        commonPlane.showExitDialog("want_quit",null);
    }
    void OnApplicationFocus(bool isFocus)
    {

        if (isFocus)
        {

        }
        else
        {
            this.gameObject.SetActive(false);
            ClickCallBackDelegate = null;
            MyTools.PrintDebugLog("ucvr  dialoag OnApplicationFocus false");  // 
        }
    }
    public void OnButtonClickSwitchControl(GameObject obj)
    {
        if (CommonPlane.handlerList != null)//
            return;
        OnButtonClick3();
    }
    void OnButtonClick3()
    {
        if (CommonPlane.handlerList == null || CommonPlane.handlerList.Count ==1) {
            //  MyTools.PrintDebugLogError("ucvr 单手不支持左右手切换");
          //  return;
        }
        MyTools.PrintDebugLogError("ucvr OnButtonClickSwitchControl");
        btIndex = 2;
        int mode = PlayerPrefs.GetInt(leftRightMode, 1);

        if (mode == 0)
        {
            Debug.Log("ucvr hand index change to right");
            zstext = Localization.Get("left_hand_habit");
            switchControlLable.text = zstext;

            PlayerPrefs.SetInt(leftRightMode, 1);

        }
        else
        {
            MyTools.PrintDebugLog("ucvr hand index change to left");
            ystext = Localization.Get("right_hand_habit");
            switchControlLable.text = ystext;

            PlayerPrefs.SetInt(leftRightMode, 0);

        }
        this.gameObject.SetActive(false);
        if (ClickCallBackDelegate != null)
            ClickCallBackDelegate(ButtonIndex.bt_SwitchControl);

        ClickCallBackDelegate = null;
    }
        public void OnButtonClickClose(GameObject obj)
    {
        if (CommonPlane.handlerList != null)//防止練劍
            return;
        OnButtonClick4();
    }
    public void OnButtonClick4() {
        MyTools.PrintDebugLog("ucvr OnButtonClickClose");
        btIndex = 3;
        this.gameObject.SetActive(false);
        if (ClickCallBackDelegate != null)
            ClickCallBackDelegate(ButtonIndex.bt_Close);
        ClickCallBackDelegate = null;
    }
    /// <summary>
    /// 被弹框外的其他组件关闭
    /// </summary>
    public void closeByOther()
    {
        MyTools.PrintDebugLog("ucvr closeByOther");
        btIndex = 3;
        this.gameObject.SetActive(false);
        if (ClickCallBackDelegate != null)
            ClickCallBackDelegate(ButtonIndex.bt_Close);
        ClickCallBackDelegate = null;
    }
    private void setbtFocus(int i)
    {
        btIndex = i;
        for (int index = 0; index < btList.Count; index++)
        {
            if (index == i)
            {

                btList[index].gameObject.SendMessage("OnHover", true, SendMessageOptions.DontRequireReceiver);
            }
            else {
                btList[index].gameObject.SendMessage("OnHover", false, SendMessageOptions.DontRequireReceiver);
            }
        }
      
    }
    public static bool isShowSetingDialog = false;
   
    void OnEnable()
    {
         //如果不是以手柄组合键进入的话不用设置frameNum=0，否则会出现设置界面有手柄连接时手柄不能用的情况
        if (CommonPlane.handlerList != null)
            frameNum = 0;
        else
            frameNum = 2;//
        isShowSetingDialog = true;
        btIndex = -1;
        if (OpenApiImp.CyberCloudOpenApiEnable==false)
            commonPlane.getExitBt().SetActive(false);

    }
    void OnDisable()
    {
        frameNum = 0;
        isShowSetingDialog = false;
        if (CommonPlane.handlerList == null)//沒有手柄時顯示設置按鈕
            if (OpenApiImp.CyberCloudOpenApiEnable == false)
                commonPlane.getExitBt().SetActive(true);
    }
 }
