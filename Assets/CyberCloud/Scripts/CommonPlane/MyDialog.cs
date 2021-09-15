using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyDialog : MonoBehaviour
{
    public delegate void ClickDelegate(ButtonIndex buttonIndex);
    public ClickDelegate ClickCallBackDelegate;

    public enum ButtonIndex {
        bt_ok,
        bt_cancel,
             bt_center
    }
    // Use this for initialization
    [SerializeField]
    private UIEventListener bt_cancel;
    [SerializeField]
    private UIEventListener bt_ok;
    [SerializeField]
    private UIEventListener bt_ok_center;
    [SerializeField]
    private UILabel lb_msg;
    void Start () {
        bt_ok.onClick = OnButtonClickOK;
        bt_cancel.onClick = OnButtonClickCancel;
        bt_ok_center.onClick = OnButtonClickCenter;

    }
    public enum BtType {
        oneBt=0,
        twoBt=1
    }
    public void changeDialogText(BtType type, string diaDesc) {
        lb_msg.text = diaDesc;
        if (type == BtType.oneBt)
        {
            maxBts = 1;
            bt_ok_center.gameObject.SetActive(true);
            bt_ok.gameObject.SetActive(false);
            bt_cancel.gameObject.SetActive(false);
        }
        else {
            maxBts = 2;
            bt_ok_center.gameObject.SetActive(false);
            bt_ok.gameObject.SetActive(true);
            bt_cancel.gameObject.SetActive(true);
        }
    }
    public string getDesc() {
        return lb_msg.text;
    }
    void OnButtonClickOK(GameObject obj)
    {
        if (CommonPlane.handlerList != null)//防止練劍
            return;
        OnButtonClick1();

    }
    void OnButtonClick1() {
        clickBtIndex = btIndex = 1;
        this.gameObject.SetActive(false);
        if (ClickCallBackDelegate != null)
            ClickCallBackDelegate(ButtonIndex.bt_ok);
        ClickCallBackDelegate = null;
    }
    void OnButtonClickCenter(GameObject obj)
    {
        if (CommonPlane.handlerList != null)//防止練劍
            return;
        OnButtonClick2();
    }
    void OnButtonClick2()
    {
        clickBtIndex = btIndex = 0;
        this.gameObject.SetActive(false);
        if (ClickCallBackDelegate != null)
            ClickCallBackDelegate(ButtonIndex.bt_center);
        ClickCallBackDelegate = null;
    }
    void OnButtonClickCancel(GameObject obj)
    {
        if (CommonPlane.handlerList != null)//防止練劍
            return;
        OnButtonClick3();
    }
    void OnButtonClick3()
    {
        clickBtIndex = btIndex = 0;
        this.gameObject.SetActive(false);
        if (ClickCallBackDelegate != null)
            ClickCallBackDelegate(ButtonIndex.bt_cancel);
        ClickCallBackDelegate = null;
    }
        /// <summary>
        /// 被弹框外的其他组件关闭
        /// </summary>
        public void closeByOther() {
        clickBtIndex = btIndex = 0;
        this.gameObject.SetActive(false);
        ClickCallBackDelegate = null;
    }
    // Update is called once per frame

    void OnEnable()
    {
        btIndex = -1;
        clickBtIndex = -1;
    }
    /// <summary>
    /// 点击的按钮索引
    /// </summary>
    private int clickBtIndex = -1;
    /// <summary>
    /// 选择的按钮索引
    /// </summary>
    private int btIndex = -1;
    private int maxBts = 2;

    void Update () {
        //shoubingkeyong = true;
        if (CommonPlane.handlerList != null) {
            if (btIndex == -1) {
                setbt1Focus();
            }

            if (maxBts == 1) {
                if (DialogBase.enterkeyup())
                {//一定要先判断方向将再判断确认键
                    MyTools.PrintDebugLog("ucvr enter");
                    btEnterKey();
                }
            }
            else if (maxBts == 2)
            {
                if (ControllerTool.getControllerBtUpDirection(CyberCloud_UnitySDKAPI.ControllerKeyCodePrivate.TOUCHPADLEFT))
                {//
                    MyTools.PrintDebugLog("ucvr left");
                    setbt1Focus();
                }
                else if (ControllerTool.getControllerBtUpDirection(CyberCloud_UnitySDKAPI.ControllerKeyCodePrivate.TOUCHPADRIGHT))
                {//
                    MyTools.PrintDebugLog("ucvr right");
                    btIndex = 1;
                    bt_cancel.gameObject.SendMessage("OnHover", false, SendMessageOptions.DontRequireReceiver);
                    bt_ok.gameObject.SendMessage("OnHover", true, SendMessageOptions.DontRequireReceiver);
                }
                else if (DialogBase.enterkeyup())
                {//一定要先判断方向将再判断确认键
                    MyTools.PrintDebugLog("ucvr enter");
                    btEnterKey();
                }
            }
        }
        else
        {
            //處理由手柄切換到頭盔時需要將，手柄遺留下來的凝視點清除掉
            if (btIndex != -1) {
                btIndex = -1;
                bt_cancel.gameObject.SendMessage("OnHover", false, SendMessageOptions.DontRequireReceiver);
                bt_ok.gameObject.SendMessage("OnHover", false, SendMessageOptions.DontRequireReceiver);
            }
               
        }
    }

    private void setbt1Focus() {

        if (maxBts == 1)
            bt_ok_center.gameObject.SendMessage("OnHover", true, SendMessageOptions.DontRequireReceiver);
        else
        {
            bt_ok.gameObject.SendMessage("OnHover", false, SendMessageOptions.DontRequireReceiver);
            bt_cancel.gameObject.SendMessage("OnHover", true, SendMessageOptions.DontRequireReceiver);
        }
        btIndex = 0;
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
    private void btEnterKey() {
        //和上次点击的按钮不同
        if (clickBtIndex != btIndex)
        {
            if (btIndex == 0)
            {
                if (maxBts == 1)
                    OnButtonClick2();
                else
                    OnButtonClick3();
            }
            else
            {
                OnButtonClick1();
            }
        }
        else {
            MyTools.PrintDebugLog("ucvr click same but no use btIndex:"+ btIndex);
        }
    }
}
