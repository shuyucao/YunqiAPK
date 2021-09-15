using CyberCloud_UnitySDKAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
/// <summary>
/// 对CyberCloud_UnitySDKAPI内部接口进一步的封装
/// </summary>
public class ControllerTool {

    private static bool getDeviceEnable(int deviceIndex) {
        List<int> deviceList = ControllerManager.getControllerConnected();
        if (deviceList != null ) {
            return deviceList.Contains(deviceIndex);
        }
        return false;
    }
    /// <summary>
    /// 判断对应键值的按键是否按下
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public static bool getControllerBtDown(CyberCloud_UnitySDKAPI.ControllerKeyCode code,int index=-1)
    {
        int groupID = UnityEngine.Random.Range(0, int.MaxValue);
        bool pressed = false;
        if (index == -1)
        {
            if (getDeviceEnable(0))
                pressed = ControllerManager.getKeyDown(groupID, 0, code);
            if (pressed == false)
            {
                if (getDeviceEnable(1))
                    pressed = ControllerManager.getKeyDown(groupID, 1, code);
            }
        }
        else {
            if(getDeviceEnable(index))
                pressed = ControllerManager.getKeyDown(groupID, index, code);
        }
    
        return pressed;
    }
    /// <summary>
    /// 判断手柄按键是否抬起
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public static bool getControllerBtUp(ControllerKeyCode code, int index = -1)
    {
        int groupID = UnityEngine.Random.Range(0, int.MaxValue);
        // bool unpressed = CyberCloud_UnitySDKAPI.ControllerManager.GetKeyUp(InputController.GetInstance().Hand, CyberCloud_UnitySDKAPI.ControllerKeyCode.TOUCHPAD);
        bool unpressed = false;
        if (index == -1)
        {
            if (getDeviceEnable(0))
                unpressed = ControllerManager.getKeyUp(groupID,0, code);
            if (unpressed == false)
            {
                if (getDeviceEnable(1))
                    unpressed = ControllerManager.getKeyUp(groupID,1, code);
            }
        }
        else {
            if (getDeviceEnable(index))
                unpressed = ControllerManager.getKeyUp(groupID,index, code);
        }
        return unpressed;
    }



    public static bool getControllerBtDownDirection(CyberCloud_UnitySDKAPI.ControllerKeyCodePrivate code)
    {
        int groupID = UnityEngine.Random.Range(0, int.MaxValue);
        //bool pressed = CyberCloud_UnitySDKAPI.ControllerManager.GetKeyDown(InputController.GetInstance().Hand, CyberCloud_UnitySDKAPI.ControllerKeyCode.TOUCHPAD);
        bool pressed = false;
        if (getDeviceEnable(0))
            pressed = ControllerManager.getKeyDownDirection(groupID,0, code);
        if (pressed == false)
        {
            if (getDeviceEnable(1))
                pressed = ControllerManager.getKeyDownDirection(groupID,1, code);
        }
      
        return pressed;
    }
    /// <summary>
    /// 判断手柄按键是否抬起
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public static bool getControllerBtUpDirection(ControllerKeyCodePrivate code)
    {
        int groupID =  UnityEngine.Random.Range(0, int.MaxValue);
        // bool unpressed = CyberCloud_UnitySDKAPI.ControllerManager.GetKeyUp(InputController.GetInstance().Hand, CyberCloud_UnitySDKAPI.ControllerKeyCode.TOUCHPAD);
        bool unpressed = false;
        if (getDeviceEnable(0))
            unpressed = ControllerManager.getKeyUpDirection(groupID,0, code);
        if (unpressed == false)
        {
            if (getDeviceEnable(1))
                unpressed = ControllerManager.getKeyUpDirection(groupID,1, code);
        }
        return unpressed;
    }

    /// <summary>
    /// 获取是3dof还是6dof 外设
    /// </summary>
    /// <returns></returns>
    public static HeadBoxDofType getDofType() {
        if (CyberCloudConfig.currentType == CyberCloudConfig.DeviceTypes.Pico|| CyberCloudConfig.currentType == CyberCloudConfig.DeviceTypes.PicoNeo2 || CyberCloudConfig.currentType == CyberCloudConfig.DeviceTypes.DaPeng)
        {//pico无法判断手柄连接的是3dof还是6dof所以只需要判断头盔是否支持6dof，大鹏连接nolo时只支持6dof所以都只需要判断头盔是6dof即可并且有手柄连接就认为是6dof
            HeadBoxDofType typeHead = HeadBox.getHmdDofType();
            if (typeHead == HeadBoxDofType.Dof6)
            {
 
                return HeadBoxDofType.Dof6;
            }
            return HeadBoxDofType.Dof3;
        }
        else {
            HeadBoxDofType type = HeadBox.getHmdDofType();
            Debug.Log("ucvr getDofType:"+type);
            return type;
        }
           
    }
    /// <summary>
    /// 打开或关闭任何有效的控制器的
    /// </summary>
    public static void CloseOrOpenAnyEnableContrlorCursor(CursorStatus cursorStatus) {
        if (CursorStatus.closeCursor == cursorStatus)
        {
            HeadBox.closeOrOpenHeadGaze(CyberCloud_UnitySDKAPI.CursorStatus.closeCursor);
            ControllerManager.closeOrOpenControllerRay(CyberCloud_UnitySDKAPI.CursorStatus.closeCursor);
        }
        else
        {
            if (CommonPlane.handlerList != null)
            {

                HeadBox.closeOrOpenHeadGaze(CyberCloud_UnitySDKAPI.CursorStatus.closeCursor);
                ControllerManager.closeOrOpenControllerRay(CyberCloud_UnitySDKAPI.CursorStatus.openCursor);
                MyTools.PrintDebugLog("ucvr open controller");
            }
            else {
                HeadBox.closeOrOpenHeadGaze(CyberCloud_UnitySDKAPI.CursorStatus.openCursor);
                ControllerManager.closeOrOpenControllerRay(CyberCloud_UnitySDKAPI.CursorStatus.closeCursor);
            }
        }

    }
    /// <summary>
    /// 反射调用 静态方法
    /// 
    /// </summary>
    /// <param name="methodName"></param>
    /// <param name="parm"></param>
    public static object Reflection(string methodName,object[] parm,string className= "CyberCloud_UnitySDKAPI.HeadBox")
    {
        Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
        Type type = assembly.GetType(className);     //命名空间名 + 类名
        MethodInfo method = type.GetMethod(methodName);
        if (method != null)
        {
            object obj = method.Invoke(null, parm); //第一个参数忽略
            return obj;
        }
        else {
            MyTools.PrintDebugLogError("ucvr className not found "+ methodName);
            return null;
        }

    }
}
