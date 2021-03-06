using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 控制弹框的跟随
/// </summary>
public class DialogBase : MonoBehaviour {
    public static bool isShow = false;
    /// <summary>
    /// 用手柄操作弹出菜单
    /// </summary>
    //public static bool shoubingkeyong = false;

    /// <summary>
    /// 相机位置
    /// </summary>
    private Transform cameraCenter;
    // Use this for initialization
    /// <summary>
    /// 弹框的初始位置
    /// </summary>
    private Vector3 initPosition;
    bool isTest = false;
    GameObject gamePlane;
    private GameAppControl gameAppControl;
    [SerializeField]
    private GameObject setingBt;
    void Awake()
    {
        initPosition = this.gameObject.transform.position;
        gamePlane = GameObject.Find("GamePlane");
        if (gamePlane != null)
        {
            cameraCenter = gamePlane.transform;
        }
        if (gamePlane != null)
        {

            gameAppControl = gamePlane.GetComponent<GameAppControl>();
        }
    }

    void Start () {

    }

    // Update is called once per frame
    void Update () {
		
	}
    void OnGUI()
    {
        if (isShow)
        {
            updataPosition(false);
        }


    }
    private Vector3 initVector3;
    /// <summary>
    /// z轴距离保持不变
    /// </summary>
   private float initCenterZ = 0;
    /// <summary>
    /// 使用手柄时面板跟随手柄进行位置移动，面板自身的旋转焦点和手柄的自身的旋转角度保持同步
    /// </summary>
    /// 
    private void updataPosition(bool init)
    {

        Transform  center = gamePlane.transform;
        Quaternion q = center.rotation;// Quaternion.Euler(y, x, 0);
        Vector3 direction = q * Vector3.forward;//相对场景中心点，相机前方（内部z轴）的单位向量
        Vector3 newPos = direction * CyberCloudConfig.DialogDistance;//相对场景中心点
        //Transform troot = this.transform.parent;

        //Vector3 temp = new Vector3(center.position.x, center.position.y, 0) + newPos;
        Vector3 temp;
        if (init)
        {
          
          temp = new Vector3(center.position.x, center.position.y, center.position.z) + newPos;
            initVector3 = temp;
            initCenterZ = center.position.z;
            this.transform.rotation = Quaternion.Euler(q.eulerAngles.x, q.eulerAngles.y, 0); //Quaternion.Euler(30, 20, 20)* Quaternion.Euler(1, 1, 0);
            //distanceDialog = temp.z - center.position.z;
            this.transform.position = temp;
          
        }
        else
        {
            //return;
            Vector3 ys= this.transform.position;
            float mz = initVector3.z+(center.position.z - initCenterZ);// center.position.z+ distanceDialog;
            //this.transform.rotation = Quaternion.Euler(q.eulerAngles.x, q.eulerAngles.y, 0);
            temp = new Vector3(ys.x,ys.y, mz);
            this.transform.position = temp;
        
        }
      
     
    
    }
    private static float onEnableTime = 0;
    public static bool enterkeyup()
    {
        bool touchpadUp = ControllerTool.getControllerBtUp(CyberCloud_UnitySDKAPI.ControllerKeyCode.TOUCHPAD);
        //return touchpadUp;
        bool triggerUp = ControllerTool.getControllerBtUp(CyberCloud_UnitySDKAPI.ControllerKeyCode.TRIGGER);
        if (Time.time - onEnableTime < 0.7)
        {//小于1.5秒的操作屏蔽掉否则 会有误操作
            triggerUp = false;
        }
   
        bool enter = touchpadUp || triggerUp;

        if (enter)
        {
            Debug.Log("ucvr enterkeyup touchpadUp:" + touchpadUp + ";triggerUp:" + triggerUp);
        }
        return enter;

    }

    void OnEnable()
    {
        onEnableTime = Time.time;
        isShow = true;
        setingBt.gameObject.SetActive(false);
        if (CommonPlane.handlerList != null)//有手柄時彈框需要關閉手柄
            ControllerTool.CloseOrOpenAnyEnableContrlorCursor(CyberCloud_UnitySDKAPI.CursorStatus.closeCursor);
        
        updataPosition(true);
        
        gameAppControl.Cyber_ControlerEnable(0);
        //MyTools.PrintDebugLog("ucvr seting show shoubingkeyong:" + CommonPlane.handlerList != null);
    }


    void OnDisable()
    {
        //if (GameAppControl.getGameRuning()) {

        //setingBt.gameObject.SetActive(true);
        //}
         if (!GameAppControl.getGameRuning())        
            ControllerTool.CloseOrOpenAnyEnableContrlorCursor(CyberCloud_UnitySDKAPI.CursorStatus.openCursor);
        isShow = false;
        gameAppControl.Cyber_ControlerEnable(1);
        MyTools.PrintDebugLog("ucvr " + this.name + " was OnDisable");

    }
   

}
