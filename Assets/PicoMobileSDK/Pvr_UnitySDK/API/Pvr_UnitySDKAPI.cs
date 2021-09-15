#if !UNITY_EDITOR && UNITY_ANDROID 
#define ANDROID_DEVICE
#endif

using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Pvr_UnitySDKAPI
{
    public enum GlobalIntConfigs
    {
        EYE_TEXTURE_RESOLUTION0,
        EYE_TEXTURE_RESOLUTION1,
        SEENSOR_COUNT,
        ABILITY6DOF,
        PLATFORM_TYPE,
        TRACKING_MODE, 
        LOG_LEVEL,
        ENBLE_HAND6DOF_BY_HEAD,
        ENBLE_6DOF_GLOBAL_TRACKING,
        TARGET_FRAME_RATE,
        iShowFPS,
        SensorMode,
        LOGICFLOW,// 0 ,1 Viewer	
        EYE_TEXTURE_RES_HIGH,
        EYE_TEXTURE_RES_NORMAL,
        iCtrlModelLoadingPri,
        iPhoneHMDModeEnabled,
    };

    public enum GlobalFloatConfigs
    {
        IPD,
        VFOV,
        HFOV,
        NECK_MODEL_X,
        NECK_MODEL_Y,
        NECK_MODEL_Z,
        DISPLAY_REFRESH_RATE
    };

    public enum RenderTextureAntiAliasing
    {
        X_1 = 1,
        X_2 = 2,
        X_4 = 4,
        X_8 = 8,
    }

    public enum PlatForm
    {
        Android = 1,
        IOS = 2,
        Win = 3,
        Notsupport = 4,
    }

    public enum RenderTextureDepth
    {
        BD_0 = 0,
        BD_16 = 16,
        BD_24 = 24,
    }

    public enum RenderTextureLevel
    {
        Normal,
        High
    }

    public enum Sensorindex
    {
        Default = 0,
        FirstSensor = 1,
        SecondSensor = 2,
    }

    public enum Eye
    {
        LeftEye = 0,
        RightEye
    }

    public enum HeadDofNum
    {
        ThreeDof,
        SixDof
    }

    public enum HandDofNum
    {
        ThreeDof,
        SixDof
    }

    public enum HandNum
    {
        One,
        Two
    }

    /// <summary>
    /// Device Tracking Origin
    ///     Represents how the SDK is reporting pose data
    /// 
    /// EyeLevel:
    ///     Represents the tracking origin whereby (0,0,0) is on the "eye"(virtual camera).
    ///     This means that pose data returned in this mode will not include the user height.
    /// 
    /// FloorLevel:
    ///     Represents the tracking origin whereby (0,0,0) is on the "floor" or other surface detected by Device.
    ///     This means that pose data returned in this mode will include the height that user defined.
    /// </summary>
    public enum TrackingOrigin
    {
        EyeLevel,
        FloorLevel
    }

    public enum EFoveationLevel
    {
        None = -1,
        Low = 0,
        Med = 1,
        High = 2
    }

    #region EyeTracking 
    public enum pvrEyePoseStatus
    {
        kGazePointValid = (1 << 0),
        kGazeVectorValid = (1 << 1),
        kEyeOpennessValid = (1 << 2),
        kEyePupilDilationValid = (1 << 3),
        kEyePositionGuideValid = (1 << 4)
    };

    public enum TrackingMode
    {
        PVR_TRACKING_MODE_ROTATION = 0x1,
        PVR_TRACKING_MODE_POSITION = 0x2,
        PVR_TRACKING_MODE_EYE = 0x4
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct EyeTrackingData
    {
        public int leftEyePoseStatus;          //!< Bit field (pvrEyePoseStatus) indicating left eye pose status
        public int rightEyePoseStatus;         //!< Bit field (pvrEyePoseStatus) indicating right eye pose status
        public int combinedEyePoseStatus;      //!< Bit field (pvrEyePoseStatus) indicating combined eye pose status

        public Vector3 leftEyeGazePoint;        //!< Left Eye Gaze Point
        public Vector3 rightEyeGazePoint;       //!< Right Eye Gaze Point
        public Vector3 combinedEyeGazePoint;    //!< Combined Eye Gaze Point (HMD center-eye point)

        public Vector3 leftEyeGazeVector;       //!< Left Eye Gaze Point
        public Vector3 rightEyeGazeVector;      //!< Right Eye Gaze Point
        public Vector3 combinedEyeGazeVector;   //!< Comnbined Eye Gaze Vector (HMD center-eye point)

        public float leftEyeOpenness;            //!< Left eye value between 0.0 and 1.0 where 1.0 means fully open and 0.0 closed.
        public float rightEyeOpenness;           //!< Right eye value between 0.0 and 1.0 where 1.0 means fully open and 0.0 closed.

        public float leftEyePupilDilation;       //!< Left eye value in millimeters indicating the pupil dilation
        public float rightEyePupilDilation;      //!< Right eye value in millimeters indicating the pupil dilation

        public Vector3 leftEyePositionGuide;    //!< Position of the inner corner of the left eye in meters from the HMD center-eye coordinate system's origin.
        public Vector3 rightEyePositionGuide;   //!< Position of the inner corner of the right eye in meters from the HMD center-eye coordinate system's origin.
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public byte[] reserved;               //!< reserved
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct EyeTrackingGazeRay
    {
        /// <summary>
        /// Vector in world space with the gaze direction.
        /// </summary>
        public Vector3 Direction;

        /// <summary>
        /// IsValid is true when there is available gaze data.
        /// </summary>
        public bool IsValid;

        /// <summary>
        /// The middle of the eyes in world space.
        /// </summary>
        public Vector3 Origin;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct EyeDeviceInfo
    {
        public ViewFrustum targetFrustumLeft;
        public ViewFrustum targetFrustumRight;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ViewFrustum
    {
        public float left;           //!< Left Plane of Frustum
        public float right;          //!< Right Plane of Frustum
        public float top;            //!< Top Plane of Frustum
        public float bottom;         //!< Bottom Plane of Frustum

        public float near;           //!< Near Plane of Frustum
        public float far;            //!< Far Plane of Frustum (Arbitrary)
    }
    #endregion 

    [StructLayout(LayoutKind.Sequential)]
    public struct EyeSetting
    {
        public Transform eyelocalPosition;
        public Rect eyeRect;
        public float eyeFov;
        public float eyeAspect;
        public Matrix4x4 eyeProjectionMatrix;
        public Shader eyeShader;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Sensor
    {
#if ANDROID_DEVICE
        public const string LibFileName = "Pvr_UnitySDK";
#else
        public const string LibFileName = "Pvr_UnitySDK";
#endif


#if ANDROID_DEVICE
        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pvr_Enable6DofModule(bool enable);

        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pvr_OptionalResetSensor(int index, int resetRot, int resetPos);

        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pvr_Init(int index);

        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pvr_StartSensor(int index);

        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pvr_StopSensor(int index);

        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pvr_ResetSensor(int index);

        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pvr_ResetSensorAll(int index);

        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pvr_GetSensorState(int index, ref float x, ref float y, ref float z, ref float w, ref float px, ref float py, ref float pz);

        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pvr_GetMainSensorState(ref float x, ref float y, ref float z, ref float w, ref float px, ref float py, ref float pz, ref float vfov,ref float hfov, ref int viewNumber);

        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pvr_GetPsensorState();

        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pvr_GetHmdPSensorStatus();

        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pvr_GetSensorAcceleration(int index, ref float x, ref float y, ref float z);

        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pvr_GetSensorGyroscope(int index, ref float x, ref float y, ref float z);

        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pvr_GetSensorMagnet(int index, ref float x, ref float y, ref float z);

        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pvr_Get6DofSensorQualityStatus();

        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool Pvr_Get6DofSafePanelFlag();

        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Pvr_SetReinPosition(float x, float y, float z, int hand, bool valid);

        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool Pvr_SetTrackingOriginType(TrackingOrigin trackingOriginType);
#endif


        #region Public Funcation
        public static bool UPvr_Pvr_Get6DofSafePanelFlag()
        {
#if ANDROID_DEVICE
            return Pvr_Get6DofSafePanelFlag();
#endif
            return false;

        }
        public static int UPvr_Init(int index)
        {
#if ANDROID_DEVICE
            return Pvr_Init(index);
#endif
            return 0;
        }
        public static void UPvr_InitPsensor()
        {
            Pvr_InitPsensor();
        }
        public static int UPvr_GetPsensorState()
        {
            int platformType = -1;
#if ANDROID_DEVICE
            int enumindex = (int)GlobalIntConfigs.PLATFORM_TYPE;
            Render.UPvr_GetIntConfig(enumindex, ref platformType);
#endif
            if (platformType == 1)
            {
#if ANDROID_DEVICE
                return Pvr_GetPsensorState();
#else
                return 0;
#endif
            }
            else
            {
                int state = Pvr_GetAndroidPsensorState();
                if (state != 0 && state != -1)
                {
                    state = 1;
                }
                return state;
            }

        }

        public static int UPvr_GetPSensorStatus()
        {
#if ANDROID_DEVICE
             return Pvr_GetHmdPSensorStatus();
#endif
            return 0;

        }
        public static void UPvr_UnregisterPsensor()
        {
            Pvr_UnregisterPsensor();
        }
        public static int UPvr_StartSensor(int index)
        {
#if ANDROID_DEVICE
            return Pvr_StartSensor(index);
#else
            return 0;
#endif
        }
        public static int UPvr_StopSensor(int index)
        {
#if ANDROID_DEVICE
            return Pvr_StopSensor(index);
#else
            return 0;
#endif
        }
        public static int UPvr_ResetSensor(int index)
        {
            Pvr_UnitySDKManager.SDK.resetBasePos = new Vector3();
#if ANDROID_DEVICE
            return Pvr_ResetSensor(index);
#else
            return 0;
#endif
        }
        public static int UPvr_OptionalResetSensor(int index, int resetRot, int resetPos)
        {
#if ANDROID_DEVICE
            return Pvr_OptionalResetSensor(index, resetRot, resetPos);
#else
            return 0;
#endif
        }
        public static int UPvr_GetSensorState(int index, ref float x, ref float y, ref float z, ref float w, ref float px, ref float py, ref float pz)
        {
#if ANDROID_DEVICE
            return Pvr_GetSensorState(index, ref x, ref y, ref z, ref w, ref px, ref py, ref pz);
#else
            return 0;
#endif
        }
        public static int UPvr_GetMainSensorState(ref float x, ref float y, ref float z, ref float w, ref float px, ref float py, ref float pz, ref float vfov, ref float hfov, ref int viewNumber)
        {
#if ANDROID_DEVICE
            return Pvr_GetMainSensorState(ref x, ref y, ref z, ref w, ref px, ref py, ref pz, ref vfov,ref hfov, ref viewNumber);
#else
            return 0;
#endif
        }

        public static int UPvr_GetSensorAcceleration(int index, ref float x, ref float y, ref float z)
        {
#if ANDROID_DEVICE
            return Pvr_GetSensorAcceleration(index, ref x, ref y, ref z);
#else
            return 0;
#endif
        }

        public static int UPvr_GetSensorGyroscope(int index, ref float x, ref float y, ref float z)
        {
#if ANDROID_DEVICE
            return Pvr_GetSensorGyroscope(index, ref x, ref y, ref z);;
#else
            return 0;
#endif
        }

        public static int UPvr_GetSensorMagnet(int index, ref float x, ref float y, ref float z)
        {
#if ANDROID_DEVICE
            return Pvr_GetSensorMagnet(index, ref x, ref y, ref z);
#else
            return 0;
#endif
        }

        public static int UPvr_Get6DofSensorQualityStatus()
        {
#if ANDROID_DEVICE
            return Pvr_Get6DofSensorQualityStatus();
#else
            return 0;
#endif
        }

        public static int UPvr_Enable6DofModule(bool enable)
        {
#if ANDROID_DEVICE
            return    Pvr_Enable6DofModule(enable);
#endif
            return 0;
        }
        public static void Pvr_InitPsensor()
        {
#if ANDROID_DEVICE
            try
            {
                Pvr_UnitySDKAPI.System.UPvr_CallStaticMethod(Pvr_UnitySDKRender.javaSysActivityClass, "initPsensor",Pvr_UnitySDKManager.pvr_UnitySDKRender.activity);
            }
            catch (Exception e)
            {
                PLOG.E("Error :" + e.ToString());
            }
#endif
        }

        public static bool Pvr_IsHead6dofReset()
        {
            bool state = false;
#if ANDROID_DEVICE
            try
            {
                Pvr_UnitySDKAPI.System.UPvr_CallStaticMethod<bool>(ref state,Pvr_UnitySDKRender.javaSysActivityClass, "isHead6dofReset", Pvr_UnitySDKManager.pvr_UnitySDKRender.activity);
            }
            catch (Exception e)
            {
                PLOG.E("Error :" + e.ToString());
            }
#endif
            return state;
        }
        public static int Pvr_GetAndroidPsensorState()
        {
            int psensor = -1;
#if ANDROID_DEVICE
    
            try
            {
                Pvr_UnitySDKAPI.System.UPvr_CallStaticMethod<int>( ref psensor,Pvr_UnitySDKRender.javaSysActivityClass, "getPsensorState");
            }
            catch (Exception e)
            {
                PLOG.E("Error :" + e.ToString());
            }
#endif
            return psensor;
        }
        public static void Pvr_UnregisterPsensor()
        {
#if ANDROID_DEVICE
            try
            {
                Pvr_UnitySDKAPI.System.UPvr_CallStaticMethod(Pvr_UnitySDKRender.javaSysActivityClass, "unregisterListener");
            }
            catch (Exception e)
            {
                PLOG.E("Error :" + e.ToString());
            }
#endif
        }
        public static int UPvr_ResetSensorAll(int index)
        {
#if ANDROID_DEVICE
                return Pvr_ResetSensorAll(index);   
#endif
            return 0;
        }

        public static void UPvr_SetReinPosition(float x, float y, float z, int hand, bool valid)
        {
            PLOG.D("PvrLog UPvr_SetReinPosition" + x + y + z + hand + valid);
#if ANDROID_DEVICE
            Pvr_SetReinPosition(x,y,z,hand,valid);
#endif
        }

        public static bool UPvr_SetTrackingOriginType(TrackingOrigin trackingOriginType)
        {
            bool ret = true;
#if ANDROID_DEVICE
            ret = Pvr_SetTrackingOriginType(trackingOriginType);
#endif
            return ret;
        }
        #endregion

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Render
    {
#if ANDROID_DEVICE
        public const string LibFileName = "Pvr_UnitySDK";
#endif

#if ANDROID_DEVICE
        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
		private static extern void Pvr_ChangeScreenParameters(string model, int width, int height, double xppi, double yppi, double densityDpi );

		[DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int Pvr_SetRatio(float midH, float midV);

        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pvr_SetPupillaryPoint(bool enable);

        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Pvr_GetSupportHMDTypes();

        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Pvr_SetCurrentHMDType([MarshalAs(UnmanagedType.LPStr)]string type);

        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pvr_GetIntConfig(int configsenum, ref int res);

        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pvr_GetFloatConfig(int configsenum, ref float res);

        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Pvr_SetupLayerData(int layerIndex, int sideMask, int textureId, int textureType, int layerFlags);

        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Pvr_SetupLayerCoords(int layerIndex, int sideMask, float[] lowerLeft, float[] lowerRight, float[] upperLeft, float[] upperRight);

        // 2D Overlay
        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Pvr_SetOverlayModelViewMatrix(int overlayType, int texId, int eye, int layerIndex, bool isHeadLocked,
                                                                 float m0, float m1, float m2, float m3,
                                                                 float m4, float m5, float m6, float m7,
                                                                 float m8, float m9, float m10, float m11,
                                                                 float m12, float m13, float m14, float m15);

        // Foveation
        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Pvr_SetFoveationParameters(int textureId, int previousId,
                                                              float focalPointX, float focalPointY,
                                                              float foveationGainX, float foveationGainY,
                                                              float foveationArea, float foveationMinimum);

        // ColorSpace
        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Pvr_SetColorspaceType(int colorspaceType);

        // External Surface
        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Pvr_CreateLayerAndroidSurface(int layerType, int layerIndex);

        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Pvr_GetLayerAndroidSurface(int layerType, int layerIndex);

        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Pvr_SetMonoMode(bool openMono);
#endif


        #region Public Funcation

        public static void UPvr_ChangeScreenParameters(string model, int width, int height, double xppi, double yppi, double densityDpi)
		{
#if ANDROID_DEVICE
			Pvr_ChangeScreenParameters(model,  width,  height,  xppi,  yppi, densityDpi );
#endif
        }

        public static int UPvr_SetRatio(float midH, float midV)
        {
#if ANDROID_DEVICE
            return Pvr_SetRatio(midH, midV);
#endif
            return 0;
        }

        public static EFoveationLevel GetFoveatedRenderingLevel()
        {
#if ANDROID_DEVICE
            return Pvr_UnitySDKEyeManager.Instance.FoveationLevel;
#endif
            return EFoveationLevel.None;
        }

        public static void SetFoveatedRenderingLevel(EFoveationLevel level)
        {
#if ANDROID_DEVICE
            Pvr_UnitySDKEyeManager.Instance.FoveationLevel = level;
#endif
        }

        public static void GetFoveatedRenderingParameters(ref Vector2 ffrGainValue, ref float ffrAreaValue, ref float ffrMinimumValue)
        {
#if ANDROID_DEVICE
            Pvr_UnitySDKEye.GetFoveatedRenderingParameters(ref ffrGainValue, ref ffrAreaValue, ref ffrMinimumValue);
#endif
        }

        public static void SetFoveatedRenderingParameters(Vector2 ffrGainValue, float ffrAreaValue, float ffrMinimumValue)
        {
#if ANDROID_DEVICE
            Pvr_UnitySDKEye.SetFoveatedRenderingParameters(ffrGainValue, ffrAreaValue, ffrMinimumValue);
#endif
        }

        // Foveation
        public static void UPvr_SetFoveationParameters(int textureId, int previousId, float focalPointX, float focalPointY, float foveationGainX, float foveationGainY, float foveationArea, float foveationMinimum)
        {
#if ANDROID_DEVICE
            Pvr_SetFoveationParameters(textureId, previousId, focalPointX, focalPointY, foveationGainX, foveationGainY, foveationArea, foveationMinimum);
#endif
        }

        public static int UPvr_GetIntConfig(int configsenum, ref int res)
        {
#if ANDROID_DEVICE
            return Pvr_GetIntConfig(configsenum, ref res);
#else
            return 0;
#endif
        }

        public static int UPvr_GetFloatConfig(int configsenum, ref float res)
        {
#if ANDROID_DEVICE
            return Pvr_GetFloatConfig(configsenum, ref res);
#else
            return 0;
#endif
        }
        public static string UPvr_GetSupportHMDTypes()
        {
#if ANDROID_DEVICE
            IntPtr ptr = Pvr_GetSupportHMDTypes();
            if (ptr != IntPtr.Zero)
            {
                return Marshal.PtrToStringAnsi(ptr);
            }
#endif
            return null;

        }
        public static void UPvr_SetCurrentHMDType(string type)
        {
#if ANDROID_DEVICE
            Pvr_SetCurrentHMDType(type);
#endif
        }

        // StandTexture Overlay
        public static void UPvr_SetupLayerData(int layerIndex, int sideMask, int textureId, int textureType, int layerFlags)
        {
#if ANDROID_DEVICE
            Pvr_SetupLayerData(layerIndex, sideMask, textureId, textureType, layerFlags);
#endif
        }

        public static void UPvr_SetupLayerCoords(int layerIndex, int sideMask, float[] lowerLeft, float[] lowerRight, float[] upperLeft, float[] upperRight)
        {
#if ANDROID_DEVICE
             Pvr_SetupLayerCoords(layerIndex, sideMask, lowerLeft, lowerRight, upperLeft, upperRight);
#endif
        }

        public static void UPvr_SetOverlayModelViewMatrix(int overlayType, int texId, int eye, int layerIndex, bool isHeadLocked, Matrix4x4 MV)
        {
#if ANDROID_DEVICE
            Pvr_SetOverlayModelViewMatrix(overlayType, texId, eye, layerIndex, isHeadLocked,
            MV.m00, MV.m01, MV.m02, MV.m03,
            MV.m10, MV.m11, MV.m12, MV.m13,
            MV.m20, MV.m21, MV.m22, MV.m23,
            MV.m30, MV.m31, MV.m32, MV.m33);
#endif
        }

        public static void UPvr_SetColorspaceType(int colorspaceType)
        {
#if ANDROID_DEVICE
            Pvr_SetColorspaceType(colorspaceType);
#endif
        }

        public static IntPtr UPvr_CreateLayerAndroidSurface(int layerType, int layerIndex)
        {
#if ANDROID_DEVICE
            return Pvr_CreateLayerAndroidSurface(layerType, layerIndex);
#else
            return IntPtr.Zero;
#endif
        }

        public static IntPtr UPvr_GetLayerAndroidSurface(int layerType, int layerIndex)
        {
#if ANDROID_DEVICE
            return Pvr_GetLayerAndroidSurface(layerType, layerIndex);
#else
            return IntPtr.Zero;
#endif
        }

        public static void UPvr_SetMonoMode(bool openMono)
        {
#if ANDROID_DEVICE
            Pvr_SetMonoMode(openMono);
#endif
        }
        #endregion

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct System
    {
#if ANDROID_DEVICE
        public const string LibFileName = "Pvr_UnitySDK";
#else
        public const string LibFileName = "Pvr_UnitySDK";
#endif

        public const string UnitySDKVersion = "2.8.3.3";

#if ANDROID_DEVICE
		[DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void Pvr_SetInitActivity(IntPtr activity, IntPtr vrActivityClass);
        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Pvr_GetSDKVersion();   
		[DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pvr_GetHmdHardwareVersion();
        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Pvr_GetHmdFirmwareVersion();
        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Pvr_GetHmdSerialNumber();
		
		[DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PVR_GetHmdBatteryLevel();
        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PVR_GetHmdBatteryStatus();
        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        public static extern float PVR_GetHmdBatteryTemperature();
        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PVR_SetHmdAudioStatus(bool enable);
        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Pvr_GetEyeTrackingData(ref int leftEyePoseStatus, ref int rightEyePoseStatus, ref int combinedEyePoseStatus,
                                                         ref float leftEyeGazePointX, ref float leftEyeGazePointY, ref float leftEyeGazePointZ,
                                                         ref float rightEyeGazePointX, ref float rightEyeGazePointY, ref float rightEyeGazePointZ,
                                                         ref float combinedEyeGazePointX, ref float combinedEyeGazePointY, ref float combinedEyeGazePointZ,
                                                         ref float leftEyeGazeVectorX, ref float leftEyeGazeVectorY, ref float leftEyeGazeVectorZ,
                                                         ref float rightEyeGazeVectorX, ref float rightEyeGazeVectorY, ref float rightEyeGazeVectorZ,
                                                         ref float combinedEyeGazeVectorX, ref float combinedEyeGazeVectorY, ref float combinedEyeGazeVectorZ,
                                                         ref float leftEyeOpenness, ref float rightEyeOpenness,
                                                         ref float leftEyePupilDilation, ref float rightEyePupilDilation,
                                                         ref float leftEyePositionGuideX, ref float leftEyePositionGuideY, ref float leftEyePositionGuideZ,
                                                         ref float rightEyePositionGuideX, ref float rightEyePositionGuideY, ref float rightEyePositionGuideZ);

        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Pvr_SetTrackingMode(int trackingMode);
        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pvr_GetPvrHandnessExt();
        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Pvr_SetPvrHandnessExt(int value);
		[DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetRenderEventFunc();

        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]       
        public static extern void UnityEventData(int data);

#endif


#region Public Funcation
        public static bool UPvr_CallStaticMethod<T>(ref T result, UnityEngine.AndroidJavaClass jclass, string name, params object[] args)
        {
            try
            {
                result = jclass.CallStatic<T>(name, args);
                return true;
            }
            catch (AndroidJavaException e)
            {
                PLOG.E("Exception calling static method " + name + ": " + e);
                return false;
            }
        }

        public static bool UPvr_CallStaticMethod(UnityEngine.AndroidJavaObject jobj, string name, params object[] args)
        {
            try
            {
                jobj.CallStatic(name, args);
                return true;
            }
            catch (AndroidJavaException e)
            {
                PLOG.E("CallStaticMethod  Exception calling activity method " + name + ": " + e);
                return false;
            }
        }

        public static bool UPvr_CallMethod<T>(ref T result, UnityEngine.AndroidJavaObject jobj, string name, params object[] args)
        {
            try
            {
                result = jobj.Call<T>(name, args);
                return true;
            }
            catch (AndroidJavaException e)
            {
                PLOG.E("Exception calling activity method " + name + ": " + e);
                return false;
            }
        }

        public static bool UPvr_CallMethod(UnityEngine.AndroidJavaObject jobj, string name, params object[] args)
        {
            try
            {
                jobj.Call(name, args);
                return true;
            }
            catch (AndroidJavaException e)
            {
                PLOG.E(" Exception calling activity method " + name + ": " + e);
                return false;
            }
        }

        public static string UPvr_GetSDKVersion()
        {
#if ANDROID_DEVICE
            IntPtr ptr = Pvr_GetSDKVersion();
            if (ptr != IntPtr.Zero)
            {
                return Marshal.PtrToStringAnsi(ptr);
            }
#endif
            return "";
        }

        public static string UPvr_GetUnitySDKVersion()
        {
            return UnitySDKVersion;

        }
        public static string UPvr_GetDeviceMode()
        {
            string devicemode = "";
#if ANDROID_DEVICE
            devicemode = SystemInfo.deviceModel;
#endif
            return devicemode;
        }

        public static string UPvr_GetDeviceModel()
        {
            return SystemInfo.deviceModel;
        }
        public static string UPvr_GetDeviceSN()
        {
            string serialNum = "UNKONWN";
#if ANDROID_DEVICE
            System.UPvr_CallStaticMethod<string>(ref serialNum, Pvr_UnitySDKRender.javaSysActivityClass, "getDeviceSN");
#endif
            return serialNum;
        }


        public static AndroidJavaObject UPvr_GetCurrentActivity()
        {
            AndroidJavaObject currentActivity = null;
#if ANDROID_DEVICE
            UnityEngine.AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");          
#endif
            return currentActivity;
        }


        public static void UPvr_ShutDown()
        {
#if ANDROID_DEVICE
            System.UPvr_CallStaticMethod(Pvr_UnitySDKRender.javaSysActivityClass, "Pvr_ShutDown");
#endif
        }
        public static bool UPvr_SetMonoPresentation()
        {
            bool value = false;
#if ANDROID_DEVICE
           value =  System.UPvr_CallMethod(UPvr_GetCurrentActivity(), "Pvr_setMonoPresentation");
#endif
            return value;
        }

        public static bool UPvr_IsPresentationExisted()
        {
            bool value = false;
            bool result = false;
#if ANDROID_DEVICE
           value = System.UPvr_CallMethod<bool>(ref result, UPvr_GetCurrentActivity(), "Pvr_isPresentationExisted");
#endif
            return value && result;
        }

        public static bool UPvr_GetMainActivityPauseStatus()
        {
            bool ret = false;
            bool isPause = false;
#if ANDROID_DEVICE
            ret = System.UPvr_CallMethod<bool>(ref isPause, UPvr_GetCurrentActivity(), "Pvr_getMainActivityPauseStatus");
#endif
            return ret && isPause;
        }

        public static void UPvr_Reboot()
        {
#if ANDROID_DEVICE
            System.UPvr_CallStaticMethod( Pvr_UnitySDKRender.javaSysActivityClass, "Pvr_Reboot",UPvr_GetCurrentActivity());
#endif
        }

        public static void UPvr_Sleep()
        {
#if ANDROID_DEVICE
            System.UPvr_CallStaticMethod( Pvr_UnitySDKRender.javaSysActivityClass, "Pvr_Sleep");
#endif
        }

        public static bool UPvr_StartHomeKeyReceiver(string startreceivre)
        {
#if ANDROID_DEVICE
            try
            {
                if (Pvr_UnitySDKManager.pvr_UnitySDKRender !=null)
                {
					Pvr_UnitySDKAPI.System.UPvr_CallStaticMethod(Pvr_UnitySDKRender.javaVrActivityLongReceiver, "Pvr_StartReceiver", Pvr_UnitySDKManager.pvr_UnitySDKRender.activity, startreceivre);
                    PLOG.I("Start home key Receiver");
                    return true;
                }
              
            }
            catch (Exception e)
            {
                PLOG.E("Start home key  Receiver  Error :" + e.ToString());
                return false;
            }
#endif
            return true;
        }
        public static bool UPvr_StopHomeKeyReceiver()
        {
#if ANDROID_DEVICE
            try
            {
                if (Pvr_UnitySDKManager.pvr_UnitySDKRender !=null)
                {
					Pvr_UnitySDKAPI.System.UPvr_CallStaticMethod(Pvr_UnitySDKRender.javaVrActivityLongReceiver, "Pvr_StopReceiver", Pvr_UnitySDKManager.pvr_UnitySDKRender.activity);
                    PLOG.I("Stop home key Receiver");
                    return true;
                }
              
            }
            catch (Exception e)
            {
                PLOG.E("Stop home key Receiver Error :" + e.ToString());
                return false;
            }
#endif
            return true;
        }
        public static void UPvr_StartVRModel()
        {
#if ANDROID_DEVICE
             Pvr_UnitySDKAPI.System.UPvr_CallStaticMethod(Pvr_UnitySDKRender.javaVrActivityClass, "startVRModel");
#endif
        }
        public static void UPvr_RemovePlatformLogo()
        {
#if ANDROID_DEVICE
			Pvr_UnitySDKAPI.System.UPvr_CallStaticMethod(Pvr_UnitySDKRender.javaVrActivityClass, "removePlatformLogo");
#endif
        }
        public static void UPvr_ShowPlatformLogo()
        {
#if ANDROID_DEVICE
			Pvr_UnitySDKAPI.System.UPvr_CallStaticMethod(Pvr_UnitySDKRender.javaVrActivityClass, "showPlatformLogo");
#endif
        }

        public static bool UPvr_IsPicoActivity()
        {
            bool ret = false;
            bool isPause = false;
#if ANDROID_DEVICE
            ret = Pvr_UnitySDKAPI.System.UPvr_CallStaticMethod<bool>(ref isPause,Pvr_UnitySDKRender.javaVrActivityClass, "isPicoActivity", UPvr_GetCurrentActivity());
#endif
            return ret && isPause;
        }

        public static void UPvr_StopVRModel()
        {
#if ANDROID_DEVICE
             Pvr_UnitySDKAPI.System.UPvr_CallStaticMethod(Pvr_UnitySDKRender.javaVrActivityClass, "stopVRModel");
#endif
        }

        public static string UPvr_GetCountryCode()
        {
            string code = "";
#if ANDROID_DEVICE
             System.UPvr_CallStaticMethod<string>(ref code,Pvr_UnitySDKRender.javaVrActivityClass, "getCountryCode",UPvr_GetCurrentActivity());
#endif
            return code;
        }
#endregion

        public static void UPvr_UnityEventData(int data)
        {
#if ANDROID_DEVICE
             Pvr_UnitySDKAPI.System.UnityEventData(data);
#endif
        }

        public static bool UPvr_checkDevice(string packagename)
        {
            bool value = false;
#if ANDROID_DEVICE
             Pvr_UnitySDKAPI.System.UPvr_CallStaticMethod<bool>(ref value,Pvr_UnitySDKRender.javaVrActivityClass, "checkDevice", packagename,UPvr_GetCurrentActivity());
#endif
            return value;
        }

        public static int UPvr_GetHmdHardwareVersion()
        {
#if ANDROID_DEVICE
            return Pvr_GetHmdHardwareVersion();
#endif
            return 0;
        }
        public static string UPvr_GetHmdFirmwareVersion()
        {

#if ANDROID_DEVICE
            IntPtr ptr = Pvr_GetHmdFirmwareVersion();
            if (ptr != IntPtr.Zero)
            {
                return Marshal.PtrToStringAnsi(ptr);
            }
#endif
            return "";
        }
        public static string UPvr_GetHmdSerialNumber()
        {
#if ANDROID_DEVICE
            IntPtr ptr = Pvr_GetHmdSerialNumber();
            if (ptr != IntPtr.Zero)
            {
                return Marshal.PtrToStringAnsi(ptr);
            }
#endif
            return "";
        }
        public static int UPvr_GetHmdBatteryLevel()
        {
#if ANDROID_DEVICE
            return PVR_GetHmdBatteryLevel();
#endif
            return 0;
        }
        public static int UPvr_GetHmdBatteryStatus()
        {
#if ANDROID_DEVICE
            return PVR_GetHmdBatteryStatus();
#endif
            return 0;
        }

        public static float UPvr_GetHmdBatteryTemperature()
        {
#if ANDROID_DEVICE
            return PVR_GetHmdBatteryTemperature();
#endif
            return 0.0f;
        }
        public static int UPvr_SetHmdAudioStatus(bool enable)
        {
#if ANDROID_DEVICE
            return PVR_SetHmdAudioStatus(enable);
#endif
            return 0;
        }

        public static bool UPvr_setTrackingMode(int trackingMode)
        {
#if ANDROID_DEVICE
            return Pvr_SetTrackingMode(trackingMode);
#endif
            return false;
        }

        public static bool UPvr_getEyeTrackingData(ref EyeTrackingData trackingData)
        {
#if ANDROID_DEVICE
            trackingData.leftEyeGazeVector.z = -trackingData.leftEyeGazeVector.z;
            trackingData.rightEyeGazeVector.z = -trackingData.rightEyeGazeVector.z;
            trackingData.combinedEyeGazeVector.z = -trackingData.combinedEyeGazeVector.z;

            trackingData.leftEyeGazePoint.z = -trackingData.leftEyeGazePoint.z;
            trackingData.rightEyeGazePoint.z = -trackingData.rightEyeGazePoint.z;
            trackingData.combinedEyeGazePoint.z = -trackingData.combinedEyeGazePoint.z;

            bool result = Pvr_GetEyeTrackingData(
                ref trackingData.leftEyePoseStatus, ref trackingData.rightEyePoseStatus, ref trackingData.combinedEyePoseStatus,
                ref trackingData.leftEyeGazePoint.x, ref trackingData.leftEyeGazePoint.y, ref trackingData.leftEyeGazePoint.z,
                ref trackingData.rightEyeGazePoint.x, ref trackingData.rightEyeGazePoint.y, ref trackingData.rightEyeGazePoint.z,
                ref trackingData.combinedEyeGazePoint.x, ref trackingData.combinedEyeGazePoint.y, ref trackingData.combinedEyeGazePoint.z,
                ref trackingData.leftEyeGazeVector.x, ref trackingData.leftEyeGazeVector.y, ref trackingData.leftEyeGazeVector.z,
                ref trackingData.rightEyeGazeVector.x, ref trackingData.rightEyeGazeVector.y, ref trackingData.rightEyeGazeVector.z,
                ref trackingData.combinedEyeGazeVector.x, ref trackingData.combinedEyeGazeVector.y, ref trackingData.combinedEyeGazeVector.z,
                ref trackingData.leftEyeOpenness, ref trackingData.rightEyeOpenness,
                ref trackingData.leftEyePupilDilation, ref trackingData.rightEyePupilDilation,
                ref trackingData.leftEyePositionGuide.x, ref trackingData.leftEyePositionGuide.y, ref trackingData.leftEyePositionGuide.z,
                ref trackingData.rightEyePositionGuide.x, ref trackingData.rightEyePositionGuide.y, ref trackingData.rightEyePositionGuide.z
                );
            // Transform SVR to Unity by negating z-axis
            trackingData.leftEyeGazeVector.z = -trackingData.leftEyeGazeVector.z;
            trackingData.rightEyeGazeVector.z = -trackingData.rightEyeGazeVector.z;
            trackingData.combinedEyeGazeVector.z = -trackingData.combinedEyeGazeVector.z;

            trackingData.leftEyeGazePoint.z = -trackingData.leftEyeGazePoint.z;
            trackingData.rightEyeGazePoint.z = -trackingData.rightEyeGazePoint.z;
            trackingData.combinedEyeGazePoint.z = -trackingData.combinedEyeGazePoint.z;
            return result;            
#endif
            return false;
        }

        public static bool UPvr_getEyeTrackingGazeRay(ref EyeTrackingGazeRay gazeRay)
        {
#if ANDROID_DEVICE
            EyeTrackingData eyeTrackingData = new EyeTrackingData();
            bool result = Pvr_UnitySDKAPI.System.UPvr_getEyeTrackingData(ref eyeTrackingData);

            gazeRay.IsValid = (eyeTrackingData.combinedEyePoseStatus & (int)Pvr_UnitySDKAPI.pvrEyePoseStatus.kGazePointValid) != 0 && (eyeTrackingData.combinedEyePoseStatus & (int)Pvr_UnitySDKAPI.pvrEyePoseStatus.kGazeVectorValid) != 0;
            if(gazeRay.IsValid)
            {
                gazeRay.Direction = eyeTrackingData.combinedEyeGazeVector;
                gazeRay.Origin = eyeTrackingData.combinedEyeGazePoint;
                gazeRay.Origin = Pvr_UnitySDKManager.SDK.HeadPose.Matrix.MultiplyPoint(gazeRay.Origin);
                gazeRay.Direction = Pvr_UnitySDKManager.SDK.HeadPose.Matrix.MultiplyVector(gazeRay.Direction);
                return true;
            }

            return false;
#endif
            gazeRay.Origin = Vector3.zero;
            gazeRay.Direction = Vector3.forward;
            gazeRay.Origin = Pvr_UnitySDKManager.SDK.HeadPose.Matrix.MultiplyPoint(gazeRay.Origin);
            gazeRay.Direction = Pvr_UnitySDKManager.SDK.HeadPose.Matrix.MultiplyVector(gazeRay.Direction);
            return true;
        }

        public static bool UPvr_getEyeTrackingGazeRayWorld(ref EyeTrackingGazeRay gazeRay)
        {
            Transform target = Pvr_UnitySDKEyeManager.Instance.transform;
            Matrix4x4 mat = Matrix4x4.TRS(target.position, target.rotation, Vector3.one);
#if ANDROID_DEVICE
            EyeTrackingData eyeTrackingData = new EyeTrackingData();
            bool result = Pvr_UnitySDKAPI.System.UPvr_getEyeTrackingData(ref eyeTrackingData);

            gazeRay.IsValid = (eyeTrackingData.combinedEyePoseStatus & (int)Pvr_UnitySDKAPI.pvrEyePoseStatus.kGazePointValid) != 0 && (eyeTrackingData.combinedEyePoseStatus & (int)Pvr_UnitySDKAPI.pvrEyePoseStatus.kGazeVectorValid) != 0;
            if(gazeRay.IsValid)
            {
                gazeRay.Direction = eyeTrackingData.combinedEyeGazeVector;
                gazeRay.Origin = eyeTrackingData.combinedEyeGazePoint;
                gazeRay.Origin = mat.MultiplyPoint(gazeRay.Origin);
                gazeRay.Direction = mat.MultiplyVector(gazeRay.Direction);
                return true;
            }

            return false;
#endif
            gazeRay.IsValid = true;
            gazeRay.Origin = Vector3.zero;
            gazeRay.Direction = Vector3.forward;
            gazeRay.Origin = mat.MultiplyPoint(gazeRay.Origin);
            gazeRay.Direction = mat.MultiplyVector(gazeRay.Direction);
            return true;
        }

        public static Vector3 UPvr_getEyeTrackingPos()
        {
#if ANDROID_DEVICE
            return Pvr_UnitySDKEyeManager.Instance.GetEyeTrackingPos();
#endif
            return Vector3.zero;
        }

        public static int UPvr_GetPhoneScreenBrightness()
        {
            int level = 0;
#if ANDROID_DEVICE
             Pvr_UnitySDKAPI.System.UPvr_CallStaticMethod<int>(ref level,Pvr_UnitySDKRender.javaVrActivityClientClass, "Pvr_GetScreen_Brightness", UPvr_GetCurrentActivity());
#endif
            return level;
        }

        public static void UPvr_SetPhoneScreenBrightness(int level)
        {
#if ANDROID_DEVICE
             Pvr_UnitySDKAPI.System.UPvr_CallStaticMethod(Pvr_UnitySDKRender.javaVrActivityClientClass, "Pvr_setAPPScreen_Brightness", UPvr_GetCurrentActivity(),level);
#endif
        }

        public static bool UPvr_IsPicoDefaultActivity()
        {
            using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    string currentActivityClassName = jo.Call<string>("getLocalClassName");
                    if (currentActivityClassName == "com.unity3d.player.UnityPlayerNativeActivityPico")
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static int UPvr_GetPvrHandnessExt()
        {
#if ANDROID_DEVICE
            return Pvr_GetPvrHandnessExt();
#endif
            return 0;
        }

        public static void UPvr_SetPvrHandnessExt(int value)
        {
#if ANDROID_DEVICE
            Pvr_SetPvrHandnessExt(value);
#endif
        }

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BoundarySystem
    {
#if ANDROID_DEVICE
        public const string LibFileName = "Pvr_UnitySDK";
#else
        public const string LibFileName = "Pvr_UnitySDK";
#endif

#if ANDROID_DEVICE
        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        private static extern float Pvr_GetFloorHeight();

        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pvr_GetSeeThroughState();

        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool Pvr_GetFrameRateLimit();
        [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool Pvr_IsBoundaryEnable();
#endif

        public static float UPvr_GetFloorHeight()
        {
            float floorHeight = 0;
#if ANDROID_DEVICE
            floorHeight = Pvr_GetFloorHeight();
#endif
            return floorHeight;
        }

        /// <summary>
        /// 0 - no seethrough
        /// 1 - gradient seethrough
        /// 2 - total seethrough
        /// </summary>
        /// <returns></returns>
        public static int UPvr_GetSeeThroughState()
        {
            int state = 0;
#if ANDROID_DEVICE
            state = Pvr_GetSeeThroughState();
#endif
            return state;
        }

        public static bool UPvr_GetFrameRateLimit()
        {
            bool ret = false;
#if ANDROID_DEVICE
            ret = Pvr_GetFrameRateLimit();
#endif
            return ret;
        }

        public static bool UPvr_IsBoundaryEnable()
        {
            bool state = false;
#if ANDROID_DEVICE
            state = Pvr_IsBoundaryEnable();
#endif
            return state;
        }

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PlatformSettings
    {
        public static Pvr_UnitySDKPlatformSetting.simulationType UPvr_IsCurrentDeviceValid()
        {
            if (Pvr_UnitySDKPlatformSetting.Entitlementchecksimulation)
            {
                if (Pvr_UnitySDKPlatformSetting.Instance.deviceSN.Count <= 0)
                {
                    return Pvr_UnitySDKPlatformSetting.simulationType.Null;
                }
                else
                {
                    foreach (var t in Pvr_UnitySDKPlatformSetting.Instance.deviceSN)
                    {
                        if (System.UPvr_GetDeviceSN() == t)
                        {
                            return Pvr_UnitySDKPlatformSetting.simulationType.Valid;
                        }
                    }

                    return Pvr_UnitySDKPlatformSetting.simulationType.Invalid;
                }
            }
            else
            {
                return Pvr_UnitySDKPlatformSetting.simulationType.Invalid;
            }
        }

        public static bool UPvr_AppEntitlementCheck(string appid)
        {
            bool state = false;
#if ANDROID_DEVICE
            try
            {
                Pvr_UnitySDKAPI.System.UPvr_CallStaticMethod<bool>(ref state, Pvr_UnitySDKRender.javaVrActivityClass, "verifyAPP", System.UPvr_GetCurrentActivity(), appid,"");
            }
            catch (Exception e)
            {
                PLOG.E("Error :" + e.ToString());
            }
#endif
            Debug.Log("PvrLog UPvr_AppEntitlementCheck" + state);
            return state;
        }

        public static bool UPvr_KeyEntitlementCheck(string publicKey)
        {
            bool state = false;
#if ANDROID_DEVICE
            try
            {
                Pvr_UnitySDKAPI.System.UPvr_CallStaticMethod<bool>(ref state, Pvr_UnitySDKRender.javaVrActivityClass,"verifyAPP", System.UPvr_GetCurrentActivity(),"",publicKey);
            }
            catch (Exception e)
            {
                PLOG.E("Error :" + e.ToString());
            }
#endif
            Debug.Log("PvrLog UPvr_KeyEntitlementCheck" + state);
            return state;
        }
    }
}