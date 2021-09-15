using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 启动前描述提示
/// </summary>
public class DescSwitch : MonoBehaviour {
    [SerializeField]
    private Texture2D dof3Texture;
    [SerializeField]
    private Texture2D dof6Texture;

    [SerializeField]
    private Texture2D dof3ControllerTexture;//手柄
    [SerializeField]
    private Texture2D dof6ControllerTexture;

    [SerializeField]
    private UITexture desc1;
    [SerializeField]
    private UITexture desc2;
    public bool tipsEnable = true;
    /// <summary>
    /// 相机位置
    /// </summary>
    private GameObject cameraCenter;
    // Use this for initialization
    void Start () {
        if (!tipsEnable)
            return;
        if (CyberCloud_UnitySDKAPI.HeadBox.getHmdDofType() == CyberCloud_UnitySDKAPI.HeadBoxDofType.Dof3)
        {

            //把资源加载到内存中

            desc1.mainTexture = getTextByNameAndLanguage("dof3tips");
        }
        else {
  
            desc1.mainTexture = getTextByNameAndLanguage("dof6tips"); ;
        }

    }
    private Texture2D getTextByNameAndLanguage(string name) {
        Texture2D texture=null;
        string picName = name;
        if (CyberCloudConfig.cvrLanguage != CyberCloudConfig.CVRLaguage.zh)
        {
            picName = name + "_" + CyberCloudConfig.cvrLanguage;
            
        }
     
        texture = (Texture2D)Resources.Load(picName);
        if(texture==null)
            texture = (Texture2D)Resources.Load(name);

        return texture;
    }
    void Awake()
    {
        if (!tipsEnable)
            return;
        cameraCenter = GameObject.Find("GamePlane");
     
    }
    // Update is called once per frame
    void Update () {
        if (!tipsEnable)
            return;
        if (startTime < 5)
        {
            startTime += Time.deltaTime;
            desc1.gameObject.SetActive(true);
            desc2.gameObject.SetActive(false);
        }
        else if (startTime < 10)
        {
            //piconeo2头盔的3dof和6dof头盔提示都在一张图上
	        if (CyberCloudConfig.cvrLanguage != CyberCloudConfig.CVRLaguage.ko&&
                CyberCloudConfig.currentType != CyberCloudConfig.DeviceTypes.PicoNeo2&& CyberCloudConfig.currentType != CyberCloudConfig.DeviceTypes.Pico2)
            {
	            startTime += Time.deltaTime;	        
	            //desc1.gameObject.SetActive(false);
	           // desc2.gameObject.SetActive(true);
            }
        }
        else
        {
            startTime = 11;
            //desc1.gameObject.SetActive(false);
           // desc2.gameObject.SetActive(false);
        }

    }
    private float startTime = 0;
    void OnEnable()
    {

        if (!tipsEnable)
            return;
        desc1.gameObject.SetActive(false);
        desc2.gameObject.SetActive(false);
   
        if (CommonPlane.handlerList != null) {
            desc2.mainTexture = getTextByNameAndLanguage("hadHandTips");
        }else
            desc2.mainTexture = getTextByNameAndLanguage("noHandTips");
        startTime = 0;
        print("script was enabled");
        updataPosition();
    }

    private void updataPosition()
    {
      
        Transform center = cameraCenter.transform;
 
        Quaternion q = center.rotation;// Quaternion.Euler(y, x, 0);
        Vector3 direction = q * Vector3.forward;//相对场景中心点，相机前方（内部z轴）的单位向量
        Vector3 newPos = direction * CyberCloudConfig.DialogDistance;//相对场景中心点
        Vector3 temp = center.position + newPos;//当使用手柄时面板的旋转角度和手柄的旋转角度相同，面板的位置相对于手柄位置相同，非手柄时面板的位置可以根据相机的角度和距离相机的相对距离根据z轴计算相对位置
                                            
        this.transform.rotation = Quaternion.Euler(q.eulerAngles.x, q.eulerAngles.y, 0);
        this.transform.position = new Vector3(temp.x, this.transform.position.y, temp.z);
    }

}
