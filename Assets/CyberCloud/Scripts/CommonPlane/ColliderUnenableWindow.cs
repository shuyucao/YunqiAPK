using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//pico或deepoon系统弹框 弹出时防止portal响应按键 添加此拦截
public class ColliderUnenableWindow : MonoBehaviour
{
    public static int systemWindowNum = 0;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnEnable()
    {
        systemWindowNum = systemWindowNum + 1;
        MyTools.PrintDebugLog("ucvr systemWindowNum+=:"+ systemWindowNum);
    }
    void OnDisable()
    {
        systemWindowNum = systemWindowNum - 1;
        MyTools.PrintDebugLog("ucvr systemWindowNum-=:" + systemWindowNum);
        if (systemWindowNum < 0)
            systemWindowNum = 0;
    }
}
