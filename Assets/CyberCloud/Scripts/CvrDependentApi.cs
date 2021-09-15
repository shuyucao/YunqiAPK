using CyberCloud_UnitySDKAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
/// <summary>
/// 由于每个厂家sdk获取手柄数据 不一样 此处视博云需要对手柄接口进行一层封装
/// 三方sdk需要实现此接口用于获取手柄数据
/// </summary>
namespace CvrDependentApi
{
    public class ControllerManager
    {
        public static bool showController = true;
        /// <summary>
        /// 获取主手柄的索引,索引从0开始,pico手柄左手为0右手为1
        /// </summary>
        /// <returns></returns>
        public static int getMainControllerIndex()
        {
#if picoSdk
            return Pvr_UnitySDKAPI.Controller.UPvr_GetMainHandNess();
#else
            return 1;
#endif
            //return 0;
        }

        /// <summary>
        /// 打开或关闭手柄凝视点和射线功能,关闭后手柄按键依然响应
        /// 注：如在流化游戏中需要关闭终端手柄和射线的显示
        /// </summary>
        /// <param name="cursorStatus"></param>
        public static void closeOrOpenControllerRay(CursorStatus cursorStatus) {
            ////////////////三方实现代码///////////////////////
            if (cursorStatus == CursorStatus.openCursor)
            {
               // Debug.LogError("=++++++++++++++++++++++++++openCursor++++++++++++++++++++++++");
                showController = true;
            }
            if (cursorStatus == CursorStatus.closeCursor)
            {
               // Debug.LogError("======================closeCursor=========================");
                showController = false;
            }
        }

        /// <summary>
        /// 获取手柄是3dof还是6dof手柄
        /// </summary>
        /// <returns></returns>
        public static HeadBoxDofType getControlerDofType()
        {
            return HeadBoxDofType.Dof6;
        }
        /// <summary>
        /// 手柄碰撞检测，视博云的所有窗口对象会通过此接口检测本身的碰撞并处理
        /// </summary>
        /// <param name="hit">碰撞结果</param>
        /// <returns>是否发生碰撞</returns>
        public static bool getRaycastHit(out RaycastResult hit)
        {
            hit = new RaycastResult();//此处需要将RaycastResult传出
            RaycastHit hitobj = new RaycastHit();//此处需要将RaycastResult传出
           // Debug.LogError(" ucvr getRaycastHit true-------------------------------------");
            if (Pvr_RayManager.rayhit.transform != null)
            {
                hitobj = Pvr_RayManager.rayhit;
                hit.gameObject = hitobj.collider.gameObject;
                //Debug.LogError(" ucvr getRaycastHit true========================================");
                return true;
            }

            return false;
        }
       
    }
    /// <summary>
    /// 头盔外设相关
    /// </summary>
    public class HeadBox
    {
        /// <summary>
        /// 获取头盔是3dof还是6dof头盔
        /// </summary>
        /// <returns></returns>
        public static HeadBoxDofType getHmdDofType()
        {
            return HeadBoxDofType.Dof6;
        }

        private static GameObject _cursor;
        /// <summary>
        /// 打开或关闭头盔凝视点在界面上的显示
        /// 如果第三方使用的凝视点是sdk预设内提供的默认凝视点此处可以使用默认实现
        /// </summary>
        /// <param name="cursorStatus"></param>
        public static void closeOrOpenHeadGaze(CursorStatus cursorStatus)
        {
            //MyTools.PrintDebugLog("ucvr cannot find Cursor:"+ cursorStatus);
            if (_cursor == null)
                _cursor = GameObject.Find("Cursor");
            if (_cursor != null)
            {
                if (cursorStatus == CursorStatus.closeCursor)
                    _cursor.SetActive(false);
                else
                    _cursor.SetActive(true);
            }
            else
            {
                MyTools.PrintDebugLogError("ucvr cannot find Cursor=====================================================");
            }
        }
        /// <summary>
        /// 获取设备唯一编号
        /// 注：只要保证设备唯一即可 用于终端用户标识
        /// </summary>
        /// <returns></returns>
        public static string getDeviceSN()
        {
            string deviceSN = "";

#if UNITY_ANDROID && !UNITY_EDITOR
        
                 deviceSN = MyTools.getDeviceSN();
                
                if (deviceSN == null || deviceSN.Equals(""))
                {
                    deviceSN = "PA7210DGB7040082G";
                    Debug.LogError("ucvr can not get deviceSN");
                }
               Debug.Log("ucvr deviceSN:"+ deviceSN);
#else
            deviceSN = "002";
#endif
            return deviceSN;
        }
        /// <summary>
        /// 获取头盔按键的按下状态，这里要注意getKeyDown是个一次性的事件，只有在getKeyDown后的那一个frame里能够查询到此状态为true，后面就都为false
        /// 必须由三方进行功能实现
        /// </summary>
        /// <param name="key">手柄按键 详见CyberCloud_KeyCode定义</param>
        /// <returns></returns>
        public static bool getKeyDown(ControllerKeyCode key)
        {
            ////////////////三方实现代码///////////////////////
            //返回键 KeyCode.Escape
            //确认键 KeyCode.JoystickButton0
            //Home键   KeyCode.Home(系统占用)

            if (key == ControllerKeyCode.HmdOK)
            {
                bool h = Input.GetKeyDown(KeyCode.JoystickButton0);
                if (h)
                {
                    Debug.Log("ucvr HmdOK down1:" + h);
                }

                return h;
            }
            else if (key == ControllerKeyCode.HOME)
            {
                return Input.GetKeyDown(KeyCode.Home);
            }
            return false;
        
        }
        /// <summary>
        /// 获取头盔按键的弹起状态，这里要注意getKeyUp是个一次性的事件，只有在getKeyUp后的那一个frame里能够查询到此状态为true，后面就都为false
        /// </summary>
        /// <param name="key">手柄按键 详见CyberCloud_KeyCode定义</param>
        /// <returns></returns>
        public static bool getKeyUp(ControllerKeyCode key)
        {
            ////////////////三方实现代码///////////////////////
            if (key == ControllerKeyCode.HmdOK)
            {
                bool h = Input.GetKeyUp(KeyCode.JoystickButton0);
                if (h)
                {
                    Debug.Log("ucvr HmdOK up1:" + h);
                }

                return h;

            }
            else if (key == ControllerKeyCode.HOME)
            {
                return Input.GetKeyUp(KeyCode.Home);
            }
            return false;
        }
    }

 

}