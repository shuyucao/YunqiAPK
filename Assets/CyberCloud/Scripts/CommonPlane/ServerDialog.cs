using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 选择服务器
/// </summary>
public class ServerDialog : MonoBehaviour {

    public delegate void ClickDelegate(string zoonCode);
    public ClickDelegate clickDelegate;
    public UISprite uisprite;

    [SerializeField]
    private CommonPlane commonPlane;

    /// <summary>
    ///0 同屏
    ///1 退出
    ///2 关闭
    /// </summary>
    [SerializeField]
    private List<UIEventListener> btList;
    [SerializeField]
    private UILabel castScreenLable;

    private int btIndex = -1;
    private DialogBase db;
    private float[] btListTop = new float[4];
    /// <summary>
    /// 判断是否存在多区域码
    /// </summary>
    /// <returns></returns>
    public static bool MultipleZoneCode() {
        string CyberZoneCode = CyberCloudConfig.CyberZoneCode;
        if (CyberZoneCode != null && CyberZoneCode.Length > 0)
        {
            string[] CyberZoneCodes = CyberZoneCode.Split(';');
            if(CyberZoneCodes!=null&& CyberZoneCodes.Length>1)//多余一个分区时才需要选择
                return true;
        }
        return false;
    }
    // Use this for initialization
    void Start()
    {

        string CyberZoneCode = CyberCloudConfig.CyberZoneCode;
        if (CyberZoneCode == null || CyberZoneCode == "")
        {
            Debug.LogError("ucvr CyberZoneCodes null");
            return;
        }

        string [] CyberZoneCodes = CyberZoneCode.Split(';');
        if (CyberZoneCodes == null) {
            Debug.LogError("ucvr CyberZoneCode.Split(';') null");
            return;
        }
        string CyberZoneDesc = CyberCloudConfig.CyberZoneDesc;
        string[] CyberZoneDescs = CyberZoneDesc.Split(';');
        //Debug.LogError("ucvr CyberZoneCodes CyberZoneCodes.Length=========================================" + CyberZoneCodes.Length);
        //不显示投屏按钮处理
        for (int i =0; i < btList.Count; i++)
        {
            btList[i].onClick = delegate (GameObject obj)
            {
                if (clickDelegate != null) {
                      this.gameObject.SetActive(false);
                    clickDelegate(obj.name);
                }
            };
        
            if (i < CyberZoneCodes.Length)
            {
                if (uisprite != null)
                    uisprite.height = 200+50*i;
                btList[i].gameObject.SetActive(true);
                btList[i].gameObject.GetComponentInChildren<UILabel>().text = CyberZoneDescs[i];
                btList[i].gameObject.name = CyberZoneCodes[i];
            }
            else
            {                 
                btList[i].gameObject.SetActive(false);
            }
            
        }
    }
    private void intFocus()
    {
        if (btIndex == -1)
        {
           setbtFocus(0);
            
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (CommonPlane.handlerList != null)
        {
       
            intFocus();
            int startIndex =0 ;
            if (ControllerTool.getControllerBtUpDirection(CyberCloud_UnitySDKAPI.ControllerKeyCodePrivate.TOUCHPADUP))
            {//
                Debug.Log("ucvr up");
                if (btIndex > startIndex)
                    setbtFocus(btIndex - 1);
            }
            else if (ControllerTool.getControllerBtUpDirection(CyberCloud_UnitySDKAPI.ControllerKeyCodePrivate.TOUCHPADDOWN))
            {//
                Debug.Log("ucvr down");
                if (btIndex < btList.Count - 1)
                    setbtFocus(btIndex + 1);
            }
            else if (DialogBase.enterkeyup())
            {//一定要先判断方向将再判断确认键
                Debug.Log("ucvr TOUCHPAD btIndex：" + btIndex);             
                OnButtonClick(btIndex);              
            }
        }
    }
    //扳机键和确认键组合键弹出菜单保证是响应按下后再弹起而不只是弹起，否则刚显示就会关闭
    private int frameNum = 0;

  
    void OnGUI()
    {

    } 
    void OnButtonClick(int index)
    {
        Debug.Log("ucvr OnButtonClick:"+ index);
        btIndex = index;
              this.gameObject.SetActive(false);
        clickDelegate(btList[index].gameObject.name);
        //this.gameObject.SetActive(false);
    }
   
    void OnApplicationFocus(bool isFocus)
    {

        if (isFocus)
        {
        }
        else
        {
            Debug.Log("ucvr  dialoag OnApplicationFocus false");  //  返回游戏的时候触发     执行顺序 1
        }
    }

    /// <summary>
    /// 被弹框外的其他组件关闭
    /// </summary>
    public void closeByOther()
    {
        Debug.Log("ucvr closeByOther");
        this.gameObject.SetActive(false);
 
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
            else
            {
                btList[index].gameObject.SendMessage("OnHover", false, SendMessageOptions.DontRequireReceiver);
            }
        }

    }

    void OnEnable()
    {
        frameNum = 0;
        SetingDialog.isShowSetingDialog = true;
        btIndex = -1;
        commonPlane.getExitBt().SetActive(false);
    }
    void OnDisable()
    {
        frameNum = 0;
        SetingDialog.isShowSetingDialog = false;
        if (CommonPlane.handlerList == null)//沒有手柄時顯示設置按鈕
            commonPlane.getExitBt().SetActive(true);
    }

}
