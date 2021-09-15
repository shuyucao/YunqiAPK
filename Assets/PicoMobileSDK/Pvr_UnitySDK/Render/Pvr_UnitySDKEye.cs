#if !UNITY_EDITOR && UNITY_ANDROID 
#define ANDROID_DEVICE
#endif

using System.Collections;
using System.Collections.Generic;
using Pvr_UnitySDKAPI;
using UnityEngine;
using UnityEngine.Rendering;
[RequireComponent(typeof(Camera))]
public class Pvr_UnitySDKEye : MonoBehaviour
{
    public static List<Pvr_UnitySDKEye> Instances = new List<Pvr_UnitySDKEye>();

    /************************************    Properties  *************************************/
    #region Properties
    public Eye eyeSide;

    public Camera eyecamera { get; private set; }

    #region BoundarySystem
    private int eyeCameraOriginCullingMask;
    private CameraClearFlags eyeCameraOriginClearFlag;
    private Color eyeCameraOriginBackgroundColor;
    private int applicationOriginFrameRate;
    private bool boundaryFrameRate;
    #endregion

    private Pvr_UnitySDKEyeManager controller;
    Matrix4x4 realProj = Matrix4x4.identity;
    private const int bufferSize = 3;
    private int IDIndex;

    private RenderEventType eventType = 0;
    private RenderEventType boundaryEventType = 0;

    public bool isFadeUSing;
    private float elapsedTime;
    public float fadeTime = 5.0f;
    public Color fadeColor = new Color(0f, 0f, 0f, 1.0f);
    private Material fadeMaterial;
    private bool isFading;
    private static Vector2 FoveationGainValue = Vector2.zero;
    private static float FoveationAreaValue;
    private static float FoveationMinimumValue;
    private int previousId = 0;

    public Pvr_UnitySDKEyeManager Controller
    {
        get
        {
            if (transform.parent == null)
            {
                return null;
            }
            if ((Application.isEditor && !Application.isPlaying) || controller == null)
            {
                return transform.parent.GetComponentInParent<Pvr_UnitySDKEyeManager>();
            }
            return controller;
        }
    }


    #endregion

    /*************************************  Unity API ****************************************/
    #region Unity API
    void Awake()
    {
        Instances.Add(this);

        eyecamera = GetComponent<Camera>();
        if (!Pvr_UnitySDKManager.SDK.HmdOnlyrot)
        {
            fadeMaterial = new Material(Shader.Find("Pvr_UnitySDK/Fade"));
        }
        Pvr_UnitySDKEyeManager.FFRLevelChanged += SetFFRParamByLevel;
        SetFFRParamByLevel();
    }

    void Start()
    {
        Setup(eyeSide);
        if (eyecamera != null)
        {
            eyecamera.enabled = !Pvr_UnitySDKManager.SDK.Monoscopic;

            #region BoundarySystem
            // record
            eyeCameraOriginCullingMask = eyecamera.cullingMask;
            eyeCameraOriginClearFlag = eyecamera.clearFlags;
            eyeCameraOriginBackgroundColor = eyecamera.backgroundColor;
            applicationOriginFrameRate = Application.targetFrameRate;
            boundaryFrameRate = BoundarySystem.UPvr_GetFrameRateLimit();
            #endregion
        }

    }

    void Update()
    {
        if (Pvr_UnitySDKManager.SDK.trackingmode == 2 || Pvr_UnitySDKManager.SDK.trackingmode == 3)
        {
#if ANDROID_DEVICE
            if (!Pvr_UnitySDKManager.SDK.HmdOnlyrot)
            {
                if (Pvr_UnitySDKManager.SDK.DefaultRange)
                {
                    if (Mathf.Sqrt(Mathf.Pow(Pvr_UnitySDKManager.SDK.HeadPose.Position.x, 2.0f) + Mathf.Pow(Pvr_UnitySDKManager.SDK.HeadPose.Position.z, 2.0f)) >= 0.8f)
                    {
                        isFading = true;
                        fadeMaterial.color = new Color(0f, 0f, 0f,
                            Mathf.Clamp((Mathf.Max(Mathf.Abs(Pvr_UnitySDKManager.SDK.HeadPose.Position.x),
                                             Mathf.Abs(Pvr_UnitySDKManager.SDK.HeadPose.Position.z)) - 0.8f) /
                                        0.16f, 0f, 0.3f));
                    }
                    else
                    {
                        if (isFadeUSing)
                        {
                            if (elapsedTime >= fadeTime)
                            {
                                fadeMaterial.color = new Color(0f, 0f, 0f, 0f);
                                isFading = false;
                            }
                        }
                        else
                        {
                            fadeMaterial.color = new Color(0f, 0f, 0f, 0f);
                            isFading = false;
                        }
                    }
                }
                else
                {
                    if (Mathf.Sqrt(Mathf.Pow(Pvr_UnitySDKManager.SDK.HeadPose.Position.x, 2.0f) + Mathf.Pow(Pvr_UnitySDKManager.SDK.HeadPose.Position.z, 2.0f)) >= Pvr_UnitySDKManager.SDK.CustomRange)
                    {
                        isFading = true;
                        fadeMaterial.color = new Color(0f, 0f, 0f,
                            Mathf.Clamp((Mathf.Max(Mathf.Abs(Pvr_UnitySDKManager.SDK.HeadPose.Position.x),
                                             Mathf.Abs(Pvr_UnitySDKManager.SDK.HeadPose.Position.z)) - Pvr_UnitySDKManager.SDK.CustomRange) /
                                        (Pvr_UnitySDKManager.SDK.CustomRange / 5f), 0f, 0.3f));
                    }
                    else
                    {
                        if (isFadeUSing)
                        {
                            if (elapsedTime >= fadeTime)
                            {
                                fadeMaterial.color = new Color(0f, 0f, 0f, 0f);
                                isFading = false;
                            }
                        }
                        else
                        {
                            fadeMaterial.color = new Color(0f, 0f, 0f, 0f);
                            isFading = false;
                        }
                    }
                }
            }
#endif
        }

        // boundary
        if (eyecamera != null && eyecamera.enabled)
        {
            int boundaryState = BoundarySystem.UPvr_GetSeeThroughState();
            if (boundaryState == 2)
            {
                // close camera render(close camera render)
                if (eyecamera.cullingMask != 0)
                {
                    eyecamera.cullingMask = 0;
                }

                if (eyecamera.clearFlags != CameraClearFlags.SolidColor)
                {
                    eyecamera.clearFlags = CameraClearFlags.SolidColor;
                    eyecamera.backgroundColor = Color.black;
                }

                if (boundaryFrameRate)
                {
                    if (Application.targetFrameRate != 30)
                    {
                        Application.targetFrameRate = 30;
                    }
                }

            }
            else
            {
                // open camera render(recover)
                if (eyecamera.cullingMask != eyeCameraOriginCullingMask)
                {
                    eyecamera.cullingMask = eyeCameraOriginCullingMask;
                }

                if (eyecamera.clearFlags != eyeCameraOriginClearFlag)
                {
                    eyecamera.clearFlags = eyeCameraOriginClearFlag;
                    eyecamera.backgroundColor = eyeCameraOriginBackgroundColor;
                }

                if (Application.targetFrameRate != applicationOriginFrameRate)
                {
                    Application.targetFrameRate = applicationOriginFrameRate;
                }
            }
        }
    }

    void OnEnable()
    {
        isFadeUSing = Pvr_UnitySDKManager.SDK.ScreenFade;
        if (isFadeUSing)
        {
            fadeMaterial = new Material(Shader.Find("Pvr_UnitySDK/Fade"));
            if (fadeMaterial != null)
            {
                PLOG.I("Get fade material success");
            }
            else
            {
                PLOG.I("Get fade material Error");
                isFadeUSing = false;
            }
            StartCoroutine(ScreenFade());
        }
#if UNITY_2018_1_OR_NEWER
        if (UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset != null)
            RenderPipelineManager.beginCameraRendering += MyPreRender;
#endif
    }

    private void OnDisable()
    {
#if UNITY_2018_1_OR_NEWER
        if (UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset != null)
            RenderPipelineManager.beginCameraRendering -= MyPreRender;
#endif
    }

    private void OnDestroy()
    {
        Instances.Remove(this);
    }

    public void MyPreRender(ScriptableRenderContext context, Camera camera)
    {
        OnPreRender();
    }

    public static bool setLevel = false;

    void OnPreRender()
    {
#if ANDROID_DEVICE
        SetFFRParameter();
        Pvr_UnitySDKPluginEvent.Issue(RenderEventType.BeginEye);
#endif
    }

    void OnPostRender()
    {
        //DrawVignetteLine();
        screenFade();

        // eyebuffer
        int eyeTextureId = Pvr_UnitySDKManager.SDK.eyeTextureIds[IDIndex];
        Pvr_UnitySDKAPI.System.UPvr_UnityEventData(eyeTextureId);
        Pvr_UnitySDKPluginEvent.Issue(eventType);
        // boundary
        Pvr_UnitySDKAPI.System.UPvr_UnityEventData(Pvr_UnitySDKManager.SDK.RenderviewNumber);
        Pvr_UnitySDKPluginEvent.Issue(boundaryEventType);

#if ANDROID_DEVICE
        Pvr_UnitySDKPluginEvent.Issue(RenderEventType.EndEye);
#endif
    }

#if UNITY_EDITOR
    void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        ModifyShadePara();
        Graphics.Blit(source, dest, Pvr_UnitySDKManager.SDK.Eyematerial);
    }

    void ModifyShadePara()
    {
        Matrix4x4 proj = Matrix4x4.identity;
        float near = GetComponent<Camera>().nearClipPlane;
        float far = GetComponent<Camera>().farClipPlane;
        float aspectFix = GetComponent<Camera>().rect.height / GetComponent<Camera>().rect.width / 2;

        proj[0, 0] *= aspectFix;
        Vector2 dir = transform.localPosition; // ignore Z
        dir = dir.normalized * 1.0f;
        proj[0, 2] *= Mathf.Abs(dir.x);
        proj[1, 2] *= Mathf.Abs(dir.y); proj[2, 2] = (near + far) / (near - far);
        proj[2, 3] = 2 * near * far / (near - far);

        Vector4 projvec = new Vector4(proj[0, 0], proj[1, 1],
                                    proj[0, 2] - 1, proj[1, 2] - 1) / 2;

        Vector4 unprojvec = new Vector4(realProj[0, 0], realProj[1, 1],
                                        realProj[0, 2] - 1, realProj[1, 2] - 1) / 2;

        float distortionFactor = 0.0241425f;
        Shader.SetGlobalVector("_Projection", projvec);
        Shader.SetGlobalVector("_Unprojection", unprojvec);
        Shader.SetGlobalVector("_Distortion1",
                                new Vector4(Pvr_UnitySDKManager.SDK.pvr_UnitySDKConfig.device.devDistortion.k1, Pvr_UnitySDKManager.SDK.pvr_UnitySDKConfig.device.devDistortion.k2, Pvr_UnitySDKManager.SDK.pvr_UnitySDKConfig.device.devDistortion.k3, distortionFactor));
        Shader.SetGlobalVector("_Distortion2",
                               new Vector4(Pvr_UnitySDKManager.SDK.pvr_UnitySDKConfig.device.devDistortion.k4, Pvr_UnitySDKManager.SDK.pvr_UnitySDKConfig.device.devDistortion.k5, Pvr_UnitySDKManager.SDK.pvr_UnitySDKConfig.device.devDistortion.k6));

    }

#endif
    #endregion

    /************************************ Public Interfaces  *********************************/
    #region Public Interfaces

    public void EyeRender()
    {
        SetupUpdate();
        if (Pvr_UnitySDKManager.SDK.eyeTextures[IDIndex] != null)
        {
            Pvr_UnitySDKManager.SDK.eyeTextures[IDIndex].DiscardContents();
            eyecamera.targetTexture = Pvr_UnitySDKManager.SDK.eyeTextures[IDIndex];
        }
    }

    #endregion

    /************************************ Private Interfaces  *********************************/
    #region Private Interfaces
    private void Setup(Eye eyeSide)
    {
        eyecamera = GetComponent<Camera>();
        transform.localPosition = Pvr_UnitySDKManager.SDK.EyeOffset(eyeSide);
        eyecamera.aspect = Pvr_UnitySDKManager.SDK.EyesAspect;
        eyecamera.rect = new Rect(0, 0, 1, 1);
#if UNITY_EDITOR
        eyecamera.rect = Pvr_UnitySDKManager.SDK.EyeRect(eyeSide);
#endif
        //  AW
        eventType = (eyeSide == Eye.LeftEye) ?
                        RenderEventType.LeftEyeEndFrame :
                        RenderEventType.RightEyeEndFrame;

        boundaryEventType = (eyeSide == Eye.LeftEye) ?
                        RenderEventType.BoundaryRenderLeft :
                        RenderEventType.BoundaryRenderRight;
    }

    private void SetupUpdate()
    {
        eyecamera.fieldOfView = Pvr_UnitySDKManager.SDK.EyeVFoV;
        eyecamera.aspect = Pvr_UnitySDKManager.SDK.EyesAspect;
        IDIndex = Pvr_UnitySDKManager.SDK.currEyeTextureIdx + (int)eyeSide * bufferSize;
        eyecamera.enabled = true;
    }

    #region  DrawVignetteLine

    private Material mat_Vignette;

    void DrawVignetteLine()
    {
        if (null == mat_Vignette)
        {
            mat_Vignette = new Material(Shader.Find("Diffuse"));//Mobile/
            if (null == mat_Vignette)
            {
                return;
            }
        }
        GL.PushMatrix();
        mat_Vignette.SetPass(0);
        GL.LoadOrtho();
        vignette();
        GL.PopMatrix();
        screenFade();
    }

    void screenFade()
    {
        if (isFading)
        {
            fadeMaterial.SetPass(0);
            GL.PushMatrix();
            GL.LoadOrtho();
            GL.Color(fadeMaterial.color);
            GL.Begin(GL.QUADS);
            GL.Vertex3(0f, 0f, -12f);
            GL.Vertex3(0f, 1f, -12f);
            GL.Vertex3(1f, 1f, -12f);
            GL.Vertex3(1f, 0f, -12f);
            GL.End();
            GL.PopMatrix();
        }
    }

    IEnumerator ScreenFade()
    {
        elapsedTime = 0.0f;
        fadeMaterial.color = fadeColor;
        Color color = fadeColor;
        isFading = true;
        while (elapsedTime < fadeTime)
        {
            yield return new WaitForEndOfFrame();
            elapsedTime += Time.deltaTime;
            color.a = 1.0f - Mathf.Clamp01(elapsedTime / fadeTime);
            fadeMaterial.color = color;
        }
        isFading = false;
    }
    void vignette()
    {
        GL.Begin(GL.QUADS);
        GL.Color(Color.black);
        //top
        GL.Vertex3(0.0f, 1.0f, 0.0f);
        GL.Vertex3(1.0f, 1.0f, 0.0f);
        GL.Vertex3(1.0f, 0.995f, 0.0f);
        GL.Vertex3(0.0f, 0.995f, 0.0f);
        //bottom
        GL.Vertex3(0.0f, 0.0f, 0.0f);
        GL.Vertex3(0.0f, 0.005f, 0.0f);
        GL.Vertex3(1.0f, 0.005f, 0.0f);
        GL.Vertex3(1.0f, 0.0f, 0.0f);
        //left
        GL.Vertex(new Vector3(0.0f, 1.0f, 0.0f));
        GL.Vertex(new Vector3(0.005f, 1.0f, 0.0f));
        GL.Vertex(new Vector3(0.005f, 0.0f, 0.0f));
        GL.Vertex(new Vector3(0.0f, 0.0f, 0.0f));
        //right
        GL.Vertex(new Vector3(0.995f, 1.0f, 0.0f));
        GL.Vertex(new Vector3(1.0f, 1.0f, 0.0f));
        GL.Vertex(new Vector3(1.0f, 0.0f, 0.0f));
        GL.Vertex(new Vector3(0.995f, 0.0f, 0.0f));
        GL.End();
    }

    #endregion

    #endregion

    

    private void SetFFRParamByLevel()
    {
        switch (Pvr_UnitySDKEyeManager.Instance.FoveationLevel)
        {
            case EFoveationLevel.None:
                FoveationGainValue = Vector2.zero;
                FoveationAreaValue = 0.0f;
                FoveationMinimumValue = 0.0f;
                break;
            case EFoveationLevel.Low:
                FoveationGainValue = new Vector2(2.0f, 2.0f);
                FoveationAreaValue = 0.0f;
                FoveationMinimumValue = 0.125f;
                break;
            case EFoveationLevel.Med:
                FoveationGainValue = new Vector2(3.0f, 3.0f);
                FoveationAreaValue = 1.0f;
                FoveationMinimumValue = 0.125f;
                break;
            case EFoveationLevel.High:
                FoveationGainValue = new Vector2(4.0f, 4.0f);
                FoveationAreaValue = 2.0f;
                FoveationMinimumValue = 0.125f;
                break;
        }
    }

    private void SetFFRParameter()
    {
        Vector3 eyePoint = Vector3.zero;
        if (Pvr_UnitySDKManager.SDK.isEnterVRMode)
        {
            eyePoint = Pvr_UnitySDKAPI.System.UPvr_getEyeTrackingPos();
        }
        int eyeTextureId = Pvr_UnitySDKManager.SDK.eyeTextureIds[IDIndex];

        Pvr_UnitySDKAPI.Render.UPvr_SetFoveationParameters(eyeTextureId, previousId, eyePoint.x, eyePoint.y, FoveationGainValue.x, FoveationGainValue.y, FoveationAreaValue, FoveationMinimumValue);
        previousId = eyeTextureId;
    }

    public static void GetFoveatedRenderingParameters(ref Vector2 ffrGainValue, ref float ffrAreaValue, ref float ffrMinimumValue)
    {
        ffrGainValue = FoveationGainValue;
        ffrAreaValue = FoveationAreaValue;
        ffrMinimumValue = FoveationMinimumValue;
    }

    public static void SetFoveatedRenderingParameters(Vector2 ffrGainValue, float ffrAreaValue, float ffrMinimumValue)
    {
        FoveationGainValue = ffrGainValue;
        FoveationAreaValue = ffrAreaValue;
        FoveationMinimumValue = ffrMinimumValue;
    }

}