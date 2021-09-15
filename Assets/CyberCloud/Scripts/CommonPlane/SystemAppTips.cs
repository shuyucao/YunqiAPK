using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// app键提示
/// 跟随相机旋转
/// </summary>
public class SystemAppTips : MonoBehaviour {

    private Transform center;
    public int distance = 3;
    public int smothingSpeed = 60;
    // Use this for initialization
    void Start () {
    
	}
    private float showTime = 0;
    void OnEnable()
    {
 
        print("script was enabled");
        showTime = 0;
    }
    public void setCenter(Transform center) {
        this.center = center;
    }
    // Update is called once per frame
    void Update () {
        showTime += Time.deltaTime;
        if (showTime < 3)
        {
            Quaternion q = center.rotation;// Quaternion.Euler(y, x, 0);
            Vector3 direction = q * Vector3.forward;//相对场景中心点，相机前方（内部z轴）的单位向量
            Vector3 newPos = direction * distance;//相对场景中心点
            Vector3 temp = center.position + newPos;//
                                                    //this.transform.position =  Vector3.Lerp(transform.position, new Vector3(temp.x, this.transform.position.y, temp.z), smothingSpeed * Time.deltaTime);
            this.transform.position = Vector3.Lerp(transform.position, new Vector3(temp.x, temp.y, temp.z), smothingSpeed * Time.deltaTime);
            this.transform.LookAt(center);//使退出按钮的z轴对准相机的位置
            this.transform.rotation = this.transform.rotation * Quaternion.Euler(0, 180, 0);//y轴旋转180度否则图标是反的
        }
        else {
            this.gameObject.SetActive(false);
        }
    }

}
