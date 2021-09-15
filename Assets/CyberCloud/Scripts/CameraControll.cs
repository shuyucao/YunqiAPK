using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
/**
 * 处理相机送native层显示
 * */
public class CameraControll : MonoBehaviour
{

    [DllImport("CyberVRPlayerSDK")]
    private static extern IntPtr GetShowEvent();


    [DllImport("CyberVRPlayerSDK")]
    private static extern IntPtr SetShowTextureFromUnity(System.IntPtr texture, int w, int h, int eyeId);
    // Use this for initialization
    private InterfaceControl interfaceC;
    private Camera camera;
    int eyeid;
    void Start()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
            interfaceC = this.gameObject.GetComponent<InterfaceControl>();
            eyeid = (int)interfaceC.eye;
            camera = this.gameObject.GetComponent<Camera>();
            float[] fovs = MyTools.getFov(eyeid);
            double fovDegrees0 = Math.Atan(fovs[0]) * (180 / Math.PI);
            double fovDegrees1 = Math.Atan(fovs[1]) * (180 / Math.PI);
            camera.fieldOfView = (float)(fovDegrees0 + fovDegrees1);//unity的fov是纵轴的度数，横轴会根据纵轴和屏幕宽度自动计算
        
            initTexture(Screen.width, Screen.height);
        #endif
    }
    //private RenderTexture renderTexture;
    void initTexture(int width, int height)
    {
   
        if (CyberCloudConfig.NativeShowScreen)
        {
            var newRenderTexture = new RenderTexture(width, height, 16);
            //newRenderTexture.antiAliasing = multisampleCounts[(int)MultisampleCount];

            camera.targetTexture = newRenderTexture;

            GL.IssuePluginEvent(SetShowTextureFromUnity(newRenderTexture.GetNativeTexturePtr(), width, height, eyeid), eyeid);
        }
   
    }
    // Update is called once per frame
    void Update()
    {

    }
    /**
     * 相机渲染完成后需要将渲染纹理传给native层进行显示
     * */
    void OnPostRender()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
                  if(CyberCloudConfig.NativeShowScreen)
                    GL.IssuePluginEvent(GetShowEvent(), eyeid);
#endif
    }
}
