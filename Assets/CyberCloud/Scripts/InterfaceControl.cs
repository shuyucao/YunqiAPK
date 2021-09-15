using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using System.Threading;
using Assets.CyberCloud.Scripts.OpenApi;
using static Assets.CyberCloud.Scripts.OpenApi.OpenApiImp;

public enum EyeIndex
{
    leftEye = 0,
    rightEye = 1
}
/// <summary>
/// 调用底层plugin处理纹理
/// </summary>
public class InterfaceControl : MonoBehaviour {
    private Texture2D _renderTex;
    public EyeIndex eye;
    private GameObject centerEyeTransform;
    private static int frameIndex = -1;
    //public TextAsset imageTextAsset ;
    //private 
    //private Transform camera; 
    /// <summary>
    /// 透传纹理
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="w"></param>
    /// <param name="h"></param>
#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
	    [DllImport ("__Internal")]
#else
    [DllImport("CyberVRPlayerSDK")]
    #endif
    private static extern IntPtr SetTextureFromUnity(System.IntPtr texture, int w, int h, float[] fov,int eyeId);

    //
#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
	    [DllImport ("__Internal")]
#else
    [DllImport("CyberVRPlayerSDK")]
#endif
    private static extern void SetLeftControllerIndex(int leftContollerIndex);

    [DllImport("CyberVRPlayerSDK")]
    private static extern void SetPrintFrameTimeDelayEnable(int enable);
#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
	    [DllImport ("__Internal")]
#else
    [DllImport("CyberVRPlayerSDK")]
#endif
    private static extern IntPtr GetRenderModelFunc();




#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
	    [DllImport ("__Internal")]
#else
    [DllImport("CyberVRPlayerSDK")]
    #endif
    private static extern void SetCameraRotation(int eyeID, float x, float y, float z, float w);

    #if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
	    [DllImport ("__Internal")]
    #else
        [DllImport("CyberVRPlayerSDK")]
    #endif
    private static extern IntPtr GetRenderEvent();



#if UNITY_WEBGL && !UNITY_EDITOR
	    [DllImport ("__Internal")]
	    private static extern void RegisterPlugin();
#endif
    private Camera cam;
    void Awake()
    {
        cam = this.GetComponent<Camera>();
        if (cam == null) {
            MyTools.PrintDebugLogError("ucvr please bind this scripe on you camera of left and right");
        }

    }

    public void setCenterEyeTransform(GameObject transform) {
        this.centerEyeTransform = transform;
    }
    /// <summary>
    /// 创建界面纹理并赋值给renderTex将纹理传递给so库使用
    /// </summary>
    public void CreateTextureAndPassToPlugin(GameObject cavans)
    {
        MyTools.PrintDebugLog("ucvr CreateTextureAndPassToPlugin:"+ eye);
        if (cavans == null)
        {
            MyTools.PrintDebugLogError("ucvr gameplane is null to interface");
            if (OpenApiImp.CyberCloudOpenApiEnable)
                OpenApiImp.getOpenApi().notify().systemStatusCallback(OpenApiImp.systemException, ErrorCodeOpenApi.canshucuowu.ToString());
            return;
        }
        int singleEyeW = (int)(Screen.width * 0.6);//0.7=1.4*0.5//0.5单目宽度，1.4 是需要在头盔中将 纹理放大到1.4倍
        int playerH = (int)(Screen.height* 1.2);
   
       // CyberCloudConfig.useAppResolution = 1;
        if (CyberCloudConfig.useAppResolution==1){
            singleEyeW = (int)(2880 * 0.6);//0.7=1.4*0.5//0.5单目宽度，1.4 是需要在头盔中将 纹理放大到1.4倍
            playerH = (int)(1600 * 1.2);
            MyTools.PrintDebugLog("ucvr useAppResolution:1");
        }
        int eyeid = eye == EyeIndex.leftEye ? 0 : 1;
        float[] fovs = MyTools.getFov(eyeid);
        float scx = 2 * (fovs[2] * 4);// + 0.05f;//4是距离 乘以0.1是因为plane单位是10米，加0.05是多留出来的
        float scy = 2 * (fovs[0] * 4);// + 0.05f;
        MyTools.PrintDebugLog("ucvr cam fov:" + cam.fieldOfView);

        if (_renderTex == null) { 
            _renderTex = new Texture2D(singleEyeW, playerH, TextureFormat.ARGB32, false); 
            cavans.transform.localScale = new Vector3(scx, scy ,1);
            //颜色用来着色前3个 mip levels
            var colors = new Color32[3];
            colors[0] = Color.black;
            colors[1] = Color.black;
            colors[2] = Color.black;
            var mipCount = Mathf.Min(3, _renderTex.mipmapCount);
            // tint each mip level
            //着色每个 mip levels
            
            for (var mip = 0; mip < mipCount; ++mip)
            {
                var cols = _renderTex.GetPixels32(mip);
                for (var i = 0; i < cols.Length; ++i)
                {
                    cols[i] = colors[mip];// Color32.Lerp(cols[i], colors[mip], 0.33f);
                }
                _renderTex.SetPixels32(cols, mip);

            }
            _renderTex.wrapMode = TextureWrapMode.Clamp;
            _renderTex.filterMode = FilterMode.Bilinear;
            //_renderTex.Compress=
            //_renderTex.textureFormat = TextureImporterFormat.AutomaticTruecolor;
            // Call Apply() so it's actually uploaded to the GPU
            _renderTex.Apply();


            MyTools.PrintDebugLog("ucvr CreateTextureAndPassToPlugin eyeid:" + eyeid + ";localScalex:" + scx + ";localScaley" + scy + ";screenW:" + singleEyeW + ";screenH:" + Screen.height + ";fov:" + fovs[0] + "," + fovs[2]);
           // if(isTest==false)
            cavans.GetComponent<Renderer>().material.mainTexture = _renderTex;

            if (isTest)
            {
                //SetLeftControllerIndex(0);
                Debug.Log("CreateTextureAndPassToPlugin start2");
   
                Debug.Log("CreateTextureAndPassToPlugin end");
            }
            else
            {


#if UNITY_ANDROID && !UNITY_EDITOR
                   try
                {
                    if (CyberCloudConfig.printFrameTimeDelay == 1)
                        SetPrintFrameTimeDelayEnable(CyberCloudConfig.printFrameTimeDelay);
                }
                catch (Exception e) {
                    MyTools.PrintDebugLogError("ucvr SetPrintFrameTimeDelayEnable error");
                }
               Debug.Log("CreateTextureAndPassToPlugin start eventid:"+eyeid);
                 GL.IssuePluginEvent(SetTextureFromUnity(_renderTex.GetNativeTexturePtr(), singleEyeW, playerH, fovs, eyeid), eyeid);
                    MyTools.PrintDebugLog("ucvr CreateTextureAndPassToPlugin eyeid over" );
#endif
            }
        }
    }



    public void toSetLeftControllerIndex() {
        int mainControllIndex = CyberCloud_UnitySDKAPI.ControllerManager.getMainControllerIndex();
        int mode = PlayerPrefs.GetInt(SetingDialog.leftRightMode, 1);
        MyTools.PrintDebugLog("ucvr change hand index：" + mode+ ";mainControllIndex:"+ mainControllIndex);
        int leftContollerIndex = 0;
        //如果左右拿主手柄，左手索引就是主手柄索引
        if (mode == 0)//
        {
            leftContollerIndex = mainControllIndex + 1;//native层手柄索引从1开始的，unity接口手柄索引从0开始这里需要统一加1
        }
        else
        {//右手模式
            int leftIndex = mainControllIndex == 0 ? 1 : 0;
            leftContollerIndex = leftIndex + 1;//右手是主手柄时左手索引应该和右手相反，右手1左手0
        }
        // if (eyeid == 0)
        setLeftControllerIndex(leftContollerIndex);
    }
    /// <summary>
    /// 设置左手手柄的索引
    /// </summary>
    /// <param name="leftContollerIndex"></param>
    private static void setLeftControllerIndex(int leftContollerIndex) {
        SetLeftControllerIndex(leftContollerIndex);
    }
    private void callPluginFunction() {
#if UNITY_ANDROID && !UNITY_EDITOR
        int index = eye == EyeIndex.leftEye ? 0 : 1;
        //将相机的旋转角度透传给so库使用
        //此返回为左手坐标系和unity相同 但是底层数据拿上来的是右手坐标系通常需要转换 转换方法如：    "Rotation" + x + y + -z + -w + "Position" + px + py + -pz 
        Quaternion rotate = centerEyeTransform.transform.rotation;
        //保证两个相机只有最先的那个调用setcamera,一帧只调用一次
        if (frameIndex != Time.frameCount)
        {
          
            SetCameraRotation(index, rotate[0], rotate[1], -rotate[2], -rotate[3]);
            
         }

        frameIndex = Time.frameCount;
        //为了从插件做任何渲染，应该从脚本调用GL.IssuePluginEvent，这将使您的本地插件可以被渲染线程调用。
        GL.IssuePluginEvent(GetRenderEvent(), index);
#endif
    }

    private int picnum=0;
    private bool isTest = false;
    /// <summary>
    /// 在摄像机开始渲染场景之前调用
    /// 只有脚本被附加到相机并被启用时才会调用这个函数。
    /// </summary>
    void OnPreRender() {
        if (isTest)
            return;
        
        if (GameAppControl.getGameRuning())
        {

            //if(eye==0)
              //Debug.Log("ucvr OnPreRender eye index:" + eye);

            //int index = eye == EyeIndex.leftEye ? 0 : 1;
            //Debug.Log("OnPreRender eye index:" + eye);
            //将相机的旋转角度透传给so库使用
            //此返回为左手坐标系和unity相同 但是底层数据拿上来的是右手坐标系通常需要转换 转换方法如：    "Rotation" + x + y + -z + -w + "Position" + px + py + -pz 
          //  if(_pause==false)
                callPluginFunction();//调用plugin不能在OnPreRender中直接调用否则多线程渲染是左眼有问题 不知道啥原因 反正再定义个方法如此处转调一下就可以了
            //Quaternion rotate = centerEyeTransform.transform.rotation;

            //SetCameraRotation(index, rotate[0], rotate[1], -rotate[2], -rotate[3]);

            //为了从插件做任何渲染，应该从脚本调用GL.IssuePluginEvent，这将使您的本地插件可以被渲染线程调用。
            //GL.IssuePluginEvent(GetRenderEventFunc(), index);    



        }

    }
  
 }
