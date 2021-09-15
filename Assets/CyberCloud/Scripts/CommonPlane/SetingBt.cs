using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SetingBt : MonoBehaviour
{
    [SerializeField]
    private CommonPlane commonPlane;

    [SerializeField]
    private exitEffect exitBtAnimal;
    private Transform centerTran;
    private GameAppControl gameAppControl;
    private MyDialog mydialog;
    public GameObject camera;
    /**
     * 退出按钮距离相机的位置
     * */
    // public int distance = 3;
    //退出按钮跟随速度
    public int smothingSpeed = 60;
    private GameObject gamePlane;
    //public GameObject settingParent;
    // Use this for initialization
    private float inity = 0;
    private BoxCollider boxcollider;
    void Awake()
    {
        boxcollider = this.gameObject.GetComponent<BoxCollider>();
        gamePlane = GameObject.Find("GamePlane");
        inity = this.transform.position.y;
        Debug.LogError("ucvr ========================:" + inity);
    }
    void Start()
    {
        mydialog = commonPlane.getDialog();

        if (gamePlane != null)
        {
            centerTran = gamePlane.transform;
            gameAppControl = gamePlane.GetComponent<GameAppControl>();
        }
        else
        {
            MyTools.PrintDebugLogError("ucvr GamePlane pre can not find");
        }
        if (mydialog == null || exitBtAnimal == null)
            MyTools.PrintDebugLogError("ucvr mydialog、exitBtAnimal can not null");
        UIEventListener.Get(this.gameObject).onClick = OnButtonClick;

        exitBtAnimal.gameObject.SetActive(true);

        // UIEventListener.Get(this.gameObject).on = OnButtonHover;
    }
    private bool needFollow = false;
    // Update is called once per frame
    void Update()
    {
        if (DialogBase.isShow == false)
        {
            if (boxcollider.enabled == false)
            {
                this.transform.localScale = new Vector3(1f, 1f, 1f);
                this.gameObject.GetComponentInChildren<UISprite>().color = Color.white;
                boxcollider.enabled = true;
            }
        }
        else
        {
            if (boxcollider.enabled)
            {
                this.gameObject.GetComponentInChildren<UISprite>().color = Color.white;
                this.gameObject.GetComponentInChildren<UISprite>().spriteName = "seting";
                this.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
                boxcollider.enabled = false;
            }
        }
        if (needFollow)
        {
            flollow(false);
        }

    }
    void OnGUI()
    {

    }
    private Vector3 initVector3;
    private GameObject objStartTips;
    public void startTips(GameObject obj)
    {

        objStartTips = obj;
    }

    private void flollow(bool init)
    {


        Transform center = gamePlane.transform;
        if (objStartTips && objStartTips.activeSelf == true)
        {
            center = objStartTips.transform;//和启动提示窗口旋转同步否则会不居中

        }
        //Debug.Log("Update Update========================");
        Quaternion q = center.rotation;// Quaternion.Euler(y, x, 0);
        Vector3 direction = q * Vector3.forward;//相对场景中心点，相机前方（内部z轴）的单位向量
        Vector3 newPos = (direction) * (CyberCloudConfig.DialogDistance);//相对场景中心点
                                                                         // Debug.DrawRay(new Vector3(0, 0, 0), direction, Color.red,100);
        Vector3 temp;
        if (init)
        {

            if (objStartTips && objStartTips.activeSelf == true)
            {
                temp = new Vector3(center.position.x, inity, center.position.z);// + new Vector3(0,inity,0);
            }
            else
                temp = new Vector3(newPos.x, inity, newPos.z);// + new Vector3(0,inity,0);

            initVector3 = newPos;
            // this.transform.rotation = q;// Quaternion.Euler(q.eulerAngles.x, q.eulerAngles.y, 0); //Quaternion.Euler(30, 20, 20)* Quaternion.Euler(1, 1, 0);
            this.transform.position = temp;


            this.transform.rotation = center.rotation;

        }



    }

    void OnEnable()
    {
        Debug.LogError("ucvr OnEnable setingbt" + transform.position.z);

        needFollow = true;
        // 

        flollow(true);
        startAnimal();

    }
    void OnDisable()
    {
        needFollow = false;

    }

    private void startAnimal()
    {

        if (exitBtAnimal.isPlaying == false)
        {
            exitBtAnimal.play();
        }
    }
    void OnButtonClick(GameObject obj)
    {
        if (!this.gameObject.activeSelf)
            return;
        MyTools.PrintDebugLog("ucvr click seting");

        //StartCoroutine(showSetingDialoglater());
        commonPlane.showSetingDialog();

    }

}
