using UnityEngine;
using System.Collections;

/// <summary>
/// Pico input Control.
/// Create By Oscar.
/// Date:2015-07-02
/// </summary>
public class PicoInput
{
    public static bool isScrrenLock = false;

    public static void LockScreen()
    {
        //isScrrenLock = !isScrrenLock;
        //string headPath = "CardboardMain/Head";
        //GameObject head = GameObject.Find (headPath);
        //if (head != null) {
        //    CardboardHead cardboardHead=head.GetComponent<CardboardHead>();
        //    cardboardHead.trackPosition=!isScrrenLock;
        //    cardboardHead.trackRotation=!isScrrenLock;
        //    isScrrenLock=isScrrenLock;
        //}
    }

    public static void Recenter()
    {
        //Cardboard.SDK.Recenter ();
        //PicoVRManager.SDK.ResetHead();
        //PicoVRManager.SDK.ResetFalconBoxSensor();
        if (CyberCloudConfig.currentType == CyberCloudConfig.DeviceTypes.Pico|| CyberCloudConfig.currentType == CyberCloudConfig.DeviceTypes.PicoNeo2)
        {
            ControllerTool.Reflection("Recenter", null);
        }

    }

    public static void  HideCrosshair()
    {
        setCrossHairActive (false);

    }
    public static void ShowCrosshair ()
    {
        setCrossHairActive (true);
    }

    static void setCrossHairActive(bool isActive)
    {
        //string crossHairPath = "CardboardMain/Head/Main Camera/PicoCross";
        //string uiCameraPath = "CardboardMain/Head/Main Camera";

        //GameObject crossHairObj = GameObject.Find(crossHairPath);

        //if (crossHairObj != null)
        //{
        //    crossHairObj.GetComponent<Renderer>().enabled = isActive;
        //    PicoCross picoCross = crossHairObj.GetComponent<PicoCross>();
        //    picoCross.enabled = isActive;
        //}

        //GameObject uiCameraObj = GameObject.Find(uiCameraPath);
        //if (uiCameraObj != null)
        //{
        //    UICamera uiCamera = uiCameraObj.GetComponent<UICamera>();
        //    uiCamera.enabled = isActive;
        //}
    }

    public static bool GetKeyDown (PicoKeyCode keyCode)
    {
        bool result = false;
        switch (keyCode) {
        case PicoKeyCode.SUBMIT:
            result = Input.GetButtonDown (InputUtil.Submit);
            break;
        case PicoKeyCode.CANCEL:
            result = Input.GetButtonDown (InputUtil.Cancel);
            break;
        case PicoKeyCode.RECENTER:
            result = Input.GetButtonDown (InputUtil.Recenter);
            break;
        case PicoKeyCode.UP:
            break;
        case PicoKeyCode.DOWN:
            break;
        case PicoKeyCode.RIGHT:
            break;
        case PicoKeyCode.LEFT:
            break;
        case PicoKeyCode.LOCKSCREEN:
            result = Input.GetButtonDown (InputUtil.LockScreen);
            break;
        default:
            break;
        }
        return result;
    }

    public static bool GetKeyUp (PicoKeyCode keyCode)
    {
        bool result = false;
        switch (keyCode) {
        case PicoKeyCode.SUBMIT:
            result = Input.GetButtonUp (InputUtil.Submit);
            break;
        case PicoKeyCode.CANCEL:
            result = Input.GetButtonUp (InputUtil.Cancel);
            break;
        case PicoKeyCode.RECENTER:
            result = Input.GetButtonUp (InputUtil.Recenter);
            break;
        case PicoKeyCode.UP:
            break;
        case PicoKeyCode.DOWN:
            break;
        case PicoKeyCode.RIGHT:
            break;
        case PicoKeyCode.LEFT:
            break;
        case PicoKeyCode.LOCKSCREEN:
            result = Input.GetButtonUp (InputUtil.LockScreen);
            break;
        case PicoKeyCode.JUMP:
            result = Input.GetButtonUp(InputUtil.Jump);
            break;
            default:
            break;
        }
        return result;
    }

    public static float GetAxis(PicoDirection direction)
    {
        float result = 0.0f;
        switch (direction) {
        case PicoDirection.Horizontal:
            float resultHJ = Input.GetAxis(InputUtil.Horizontal);
            float resultHB = 0;
//#if UNITY_EDITOR
//#else
//            PicoUnityActivity.CallObjectMethod(ref resultHB, "GetAxis","Horizontal");
//#endif
            if(resultHJ != 0.0f){
                result = resultHJ;
            }else{
                result = resultHB;
            }
            break;
        case PicoDirection.Vertical:
            float resultVJ = Input.GetAxis(InputUtil.Vertical);
            float resultVB = 0;
//#if UNITY_EDITOR
//#else
//            PicoUnityActivity.CallObjectMethod(ref resultVB, "GetAxis","Vertical");
//#endif
            if(resultVJ != 0.0f){
                result = resultVJ;
            }else{
                result = resultVB;
            }
            break;
        default:
            break;
        }
        return result;
    }
}

public enum PicoKeyCode
{
    SUBMIT,
    CANCEL,
    RECENTER,
    UP,
    DOWN,
    LEFT,
    RIGHT,
    LOCKSCREEN,
    JUMP
}

public enum PicoDirection
{
    Horizontal,
    Vertical
}
