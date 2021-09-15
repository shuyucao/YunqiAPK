using CvrDependentApi;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Assets.CyberCloud.Scripts.OpenApi;
#if PicoSDK
using Pvr_UnitySDKAPI;
#endif
/// <summary>
/// 对Cvrdependent的封装
/// </summary>
namespace CyberCloud_UnitySDKAPI
{
    public class ControllerManager
    {
        /// <summary>
        /// 获取按键的按下状态，这里要注意getKeyDown是个一次性的事件，只有在getKeyDown后的那一个frame里能够查询到此状态为true，后面就都为false
        /// 必须由三方进行功能实现
        /// </summary>
        /// <param name="hand">第一个手柄传0，获取第二个手柄传1</param>
        /// <param name="key">手柄按键 详见CyberCloud_KeyCode定义</param>
        /// <returns></returns>
        public static bool getKeyDown(int groupID, int hand, ControllerKeyCode key) {
            if (OpenApiImp.CyberCloudOpenApiEnable)
                return false;
            ////////////////三方实现代码///////////////////////
            bool tiger = getControllerKeyEvent(groupID, hand, key, true) == 1 ? true : false;//CvrDependentApi.ControllerManager.getKeyDown(hand,key);
            if (tiger) {
                MyTools.PrintDebugLog("ucvr getKeyDown " + key + "isdown");
            }
            return tiger;
        }
        /// <summary>
        /// 获取按键的弹起状态，这里要注意getKeyUp是个一次性的事件，只有在getKeyUp后的那一个frame里能够查询到此状态为true，后面就都为false
        /// </summary>
        /// <param name="hand">第一个手柄传0，获取第二个手柄传1</param>
        /// <param name="key">手柄按键 详见CyberCloud_KeyCode定义</param>
        /// <returns></returns>
        public static bool getKeyUp(int groupID, int hand, ControllerKeyCode key) {
            if (OpenApiImp.CyberCloudOpenApiEnable)
                return false;
            ////////////////三方实现代码///////////////////////
            bool tiger = getControllerKeyEvent(groupID, hand, key, false) == 1 ? true : false;// CvrDependentApi.ControllerManager.getKeyUp(hand,key);
            if (tiger)
            {
                MyTools.PrintDebugLog("ucvr getKeyDown " + key + "isUp");
            }
            return tiger;
        }
        public static bool getKeyDownDirection(int groupID, int hand, ControllerKeyCodePrivate key)
        {

            if (OpenApiImp.CyberCloudOpenApiEnable)
                return false;
            ////////////////三方实现代码///////////////////////
            return controllerKeyState(groupID, hand, key, true);
        }
        /// <summary>
        /// 获取按键的弹起状态，这里要注意getKeyUp是个一次性的事件，只有在getKeyUp后的那一个frame里能够查询到此状态为true，后面就都为false
        /// </summary>
        /// <param name="hand">第一个手柄传0，获取第二个手柄传1</param>
        /// <param name="key">手柄按键 详见CyberCloud_KeyCode定义</param>
        /// <returns></returns>
        public static bool getKeyUpDirection(int groupID, int hand, ControllerKeyCodePrivate key)
        {
            if (OpenApiImp.CyberCloudOpenApiEnable)
                return false;
            ////////////////三方实现代码///////////////////////
            return controllerKeyState(groupID, hand, key, false);
        }
        /// <summary>
        /// 手柄方向按键 弹起或按下的状态
        /// </summary>
        /// <param name="hand"></param>
        /// <param name="key"></param>
        /// <param name="keyDown"></param>
        /// <returns></returns>
        private static bool controllerKeyState(int groupID, int hand, ControllerKeyCodePrivate key, bool keyDown)
        {
            if (key == ControllerKeyCodePrivate.TOUCHPADUP || key == ControllerKeyCodePrivate.TOUCHPADDOWN || key == ControllerKeyCodePrivate.TOUCHPADLEFT || key == ControllerKeyCodePrivate.TOUCHPADRIGHT)
            {
                List<int> list = getControllerConnected();
                if (list == null || list.Count == 0)
                {
                    // MyTools.PrintDebugLog("ucvr controller list is null");
                    // 先判断通用设备是否被按下
                    if (commonAxis(key))
                        return true;
                } 
                else
                {

                    bool keyEnable = axisDirection(groupID, hand, key, keyDown);
                    // Debug.Log("ucvr hand:" + hand + ";key:" + key + ";keyEnable:" + keyEnable);
                    //先判断触摸是否弹起或按下在判断轴
                    if (CyberCloudConfig.currentType != CyberCloudConfig.DeviceTypes.PicoNeo2)
                    {
                        return keyEnable;
                    }
                    else {
                        //手柄未連接時不用判斷
                        if (CommonPlane.handlerList == null || !CommonPlane.handlerList.Contains(hand)) {
                            lastaAxisDirectionDownTime[hand] = 0;
                            lastaAxisDirectionDownTime[hand + 2] = 0;
                            return false;

                        }
                        if (keyEnable)
                        {
                            //上次按鍵也被按下時判斷一下時間超過0.8秒認為按鍵再次按下有效，否則上次沒有按下時返回按下狀態
                            if (lastaAxisDirectionDownTime[hand] > 0)
                            {
                                if (Time.deltaTime - lastaAxisDirectionDownTime[hand] > 0.8)
                                {
                                    lastaAxisDirectionDownTime[hand] = Time.deltaTime;
                                    lastaAxisDirectionDownTime[hand + 2] = (int)key;
                                    return true;
                                }
                            }
                            else
                            {
                                lastaAxisDirectionDownTime[hand] = Time.deltaTime;
                                lastaAxisDirectionDownTime[hand + 2] = (int)key;
                                return true;
                            }
                        }
                        else {//按鍵彈起後將上次按下的時間清除
                            if (lastaAxisDirectionDownTime[hand + 2] == (int)key)//本次沒有按下的按鍵和上次按下的按鍵是同一個按鍵，認為上次按鍵取消
                            {
                                lastaAxisDirectionDownTime[hand] = 0;
                                lastaAxisDirectionDownTime[hand + 2] = 0;
                            }
                        }
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// piconeo2上次按下的時間
        /// </summary>
        static float[] lastaAxisDirectionDownTime = new float[4] { 0, 0,0,0 };
        /// <summary>
        /// 未连接体感手柄时通用手柄方向判断
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static bool commonAxis(ControllerKeyCodePrivate key)
        {
            if (!Input.anyKeyDown)
                return false;

            if (key == ControllerKeyCodePrivate.TOUCHPADLEFT)
            {
                float h = Input.GetAxisRaw("Horizontal");
              
                if (h < -0.8)
                {
                    return true;
                }
            }
            else if (key == ControllerKeyCodePrivate.TOUCHPADRIGHT)
            {
                float h = Input.GetAxisRaw("Horizontal");
      
                if (h > 0.8)
                {
                    return true;
                }
            }
            else if (key == ControllerKeyCodePrivate.TOUCHPADUP)
            {
                float v = Input.GetAxisRaw("Vertical");
   
                if (v < -0.8)
                {
                    return true;
                }
            }
            else if (key == ControllerKeyCodePrivate.TOUCHPADDOWN)
            {
                float v = Input.GetAxisRaw("Vertical");
    
                if (v > 0.8)
                {
                    return true;
                }
            }
            return false;

        }
        /// <summary>
        /// 通过java接口获取手柄按键状态
        /// </summary>
        /// <param name="hand"></param>
        /// <param name="key"></param>
        /// <param name="keyDown">true表示获取按下 false表示获取弹起</param>
        /// <returns></returns>
        private static int getControllerKeyEvent(int groupID, int hand, ControllerKeyCode key, bool keyDown)
        {
  
            // 0	TouchPad x  touchPad X轴坐标值	0~255// 1	TouchPad y  touchPad Y轴坐标值	0~255
            // 2	Home Home 按键	0未按下，1按下
            // 3	App App按键	0未按下，1按下
            // 4	TouchpadClick Touch Pad按键	0未按下，1按下
            // 5	VolumeUp 音量加键	0未按下，1按下
            // 6	VolumeDown 音量减键	0未按下，1按下
            // 7	Trigger Tigger 触摸/按键	0~255，( 63~255 touch， 127~255 Click)
            int[] controllerKeys = MyTools.getControllerKeyEvent(hand + 1, groupID);
            int[] preControllerKeys = MyTools.getPreControllerKeyEvent(hand + 1);
            int currentFrameIsKeyDown = 0;
            int preFrameIsKeyDown = 0;
            //APP = 1,
            //TOUCHPAD = 2,//触摸板
            //HOME = 3,//头盔或手柄的home键
            //VOLUMEUP = 4,//头盔或手柄音量+
            //VOLUMEDOWN = 5,//头盔或手柄音量-
            //TRIGGER = 10,//手柄扳机键
            try
            {

                if (key == ControllerKeyCode.APP) {
                    currentFrameIsKeyDown = controllerKeys[3];
                    preFrameIsKeyDown = preControllerKeys[3];
                }
                else if (key == ControllerKeyCode.TOUCHPAD) {
        
                    currentFrameIsKeyDown = controllerKeys[4];
                    preFrameIsKeyDown = preControllerKeys[4];
                 
                }
                else if (key == ControllerKeyCode.HOME)
                {

                    currentFrameIsKeyDown = controllerKeys[2];
                    preFrameIsKeyDown = preControllerKeys[2];

                }
                else if (key == ControllerKeyCode.VOLUMEUP)
                {

                    currentFrameIsKeyDown = controllerKeys[5];
                    preFrameIsKeyDown = preControllerKeys[5];
           
                }
                else if (key == ControllerKeyCode.VOLUMEDOWN)
                {
                    currentFrameIsKeyDown = controllerKeys[6];
                    preFrameIsKeyDown = preControllerKeys[6];
                }
                else if (key == ControllerKeyCode.TRIGGER)
                {
                    currentFrameIsKeyDown = controllerKeys[7] > 127 ? 1 : 0;
                    preFrameIsKeyDown = preControllerKeys[7] > 127 ? 1 : 0;
                }
            }
            catch (Exception e)
            {
                MyTools.PrintDebugLogError("ucvr getControllerKeyEvent error:" + e.Message);
            }

            if (keyDown)
            {
                return currentFrameIsKeyDown;
            }
            else
            {
                if (preFrameIsKeyDown == 1 && currentFrameIsKeyDown == 0)
                {
                    return 1;
                }
                else
                    return 0;
            }
        }
        /// <summary>
        /// 将java接口获取的轴数据转换为0到1
        /// </summary>
        /// <param name="hand"></param>
        /// <returns></returns>
        private static Vector2 getTouchPosition(int groupID, int hand) {
            Vector2 v2 = new Vector2();
            int[] controllerKeys =MyTools.getPreControllerKeyEvent(hand+1);//轴数据取的是按下的数据
            v2.x = controllerKeys[0]/255f;
            v2.y = controllerKeys[1]/255f;
            return v2;
        }
        //根据轴数据判断方向是否被按下
        private static bool axisDirection(int groupID,int hand, ControllerKeyCodePrivate key, bool keyDown)
        {
            float centerPosition = 0.5f;//255/2;
            float bigValue = 0.75f;//255*3/4
            float litterValue = 0.25f;//255*1/4

            //neo2的軸比較特殊不方便用按下操作方向改成觸摸判斷方向
            if (CyberCloudConfig.currentType != CyberCloudConfig.DeviceTypes.PicoNeo2)
            {
                //判断是否有需要获取的按下或抬起的按键状态
                if (keyDown)
                {
                    if (getKeyDown(groupID, hand, ControllerKeyCode.TOUCHPAD) == false)
                        return false;
                }
                else
                {
                    if (getKeyUp(groupID, hand, ControllerKeyCode.TOUCHPAD) == false)
                        return false;
                }
            }
            else {

                if (getKeyUp(groupID, hand, ControllerKeyCode.TOUCHPAD)) {//按键弹起时充值按键
                    lastaAxisDirectionDownTime[hand] = 0;
                    lastaAxisDirectionDownTime[hand+2] = 0;
                }
                    
            }
            //获取轴数据
            Vector2 v2 = getTouchPosition(groupID,hand);
            if (v2.x == 0 && v2.y == 0) {//为了和piconeo和htc统一，未触摸状态是0，
                                         // Debug.Log("ucv");
                return false;
            }
           


            float Aisx_x = Mathf.Abs(v2.x - centerPosition);
            float  Aisx_y = Mathf.Abs(v2.y - centerPosition);

            //Debug.Log("ucvr hand:" + hand + ";v2x:" + v2.x + ";v2y:" + v2.y + ";Aisx_y:" + Aisx_y + ";bigValue:" + bigValue);
   
            //判断是否按下方向键
            if (Aisx_y > litterValue || Aisx_x > litterValue)
            {

               
                //纵向大于横向

                if (Aisx_y > Aisx_x)
                {
                    return yAixEnable(v2, key, litterValue, bigValue);
                }
                else if (Aisx_y < Aisx_x)
                {
                  
                    return xAixEnable(v2, key, litterValue, bigValue);
                }
                else
                {//两个值相等
                    bool e = yAixEnable(v2, key, litterValue, bigValue);
                    if (e == false)
                        e = xAixEnable(v2, key, litterValue, bigValue);
                    return e;
                }

            }
          
            return false;
        }
        //上下轴方向判断
        private static bool yAixEnable(Vector2 v2, ControllerKeyCodePrivate key, float litterValue, float bigValue)
        {
            if (key == ControllerKeyCodePrivate.TOUCHPADUP)
            {
                if (v2.y > bigValue)
                {
                    return true;
                }
                else
                    return false;
            }
            else if (key == ControllerKeyCodePrivate.TOUCHPADDOWN)
            {
               
                if (v2.y < litterValue)
                {
                    return true;
                }
                else
                    return false;
            }
            return false;
        }
        //左右轴方向判断
        private static bool xAixEnable(Vector2 v2, ControllerKeyCodePrivate key, float litterValue, float bigValue)
        {
            if (key == ControllerKeyCodePrivate.TOUCHPADLEFT)
            {
                if (v2.x < litterValue)
                {
                    return true;
                }
                else
                    return false;
            }
            else if (key == ControllerKeyCodePrivate.TOUCHPADRIGHT)
            {
                if (v2.x > bigValue)
                {
                    return true;
                }
                else
                    return false;
            }
            return false;

        }



        
       
        /// <summary>
        /// 获取主手柄的索引,索引从0开始
        /// </summary>
        /// <returns></returns>
        public static int getMainControllerIndex()
        {
   
            return CvrDependentApi.ControllerManager.getMainControllerIndex();
        }

        /// <summary>
        /// 打开或关闭手柄凝视点和射线功能,关闭后手柄按键依然响应
        /// 注：如在流化游戏中需要关闭终端手柄和射线的显示
        /// </summary>
        /// <param name="cursorStatus"></param>
        public static void closeOrOpenControllerRay(CursorStatus cursorStatus) {
            ////////////////三方实现代码///////////////////////
            if (OpenApiImp.CyberCloudOpenApiEnable)
                return ;
            CvrDependentApi.ControllerManager.closeOrOpenControllerRay(cursorStatus);
        }

        /// <summary>
        /// 获取手柄是3dof还是6dof手柄
        /// pico目前不支持此接口
        /// 如果不确定默认返回3dof
        /// </summary>
        /// <returns></returns>
        public static HeadBoxDofType getControlerDofType()
        {
            return CvrDependentApi.ControllerManager.getControlerDofType();
        }
        /// <summary>
        /// 手柄碰撞检测，视博云的所有窗口对象会通过此接口检测本身的碰撞并处理
        /// </summary>
        /// <param name="hit">碰撞结果</param>
        /// <returns>是否发生碰撞</returns>
        public static bool getRaycastHit(out RaycastResult hit)
        {
        
            return CvrDependentApi.ControllerManager.getRaycastHit(out hit);
        }
        /// <summary>
        /// 和RaycastResult任选其一（使用者pico2）
        /// </summary>
        /// <param name="hitobj"></param>
        /// <returns></returns>
        public static bool getRaycastHit(out RaycastHit hitobj)
        {
            if (OpenApiImp.CyberCloudOpenApiEnable)
            {
                hitobj = new RaycastHit();
                return false;
            }
            hitobj = new RaycastHit();//此处需要将RaycastResult传出
       
            return false;
        }
        static int mUpdateFrame = -1;
        static List<int> controllerConnectedlist = null;
        /// <summary>
        /// 获取连接的手柄索引
        /// </summary>
        /// <returns>索引数组 手柄索引从0开始，无手柄连接时返回null</returns>
        public static List<int> getControllerConnected()
        {
            if (Time.frameCount == mUpdateFrame)
            {//同一帧不再重复调用 节省cpu消耗
                return controllerConnectedlist;
            }
            else
                mUpdateFrame = Time.frameCount;

            // Debug.LogError(" ucvr ggetControllerConnected" );
            if (false)
            {
                List<int> l = new List<int>();
                l.Add(0);
                return l;
            }
              List<int> list = null;
            //Debug.Log(" ucvr getRaycastHit start");
#if PicoSDK
            ControllerState state1 = Pvr_UnitySDKAPI.Controller.UPvr_GetControllerState(0);
          
            ControllerState state2 = Pvr_UnitySDKAPI.Controller.UPvr_GetControllerState(1);
            if (state1== ControllerState.Connected || state2== ControllerState.Connected)
            {
                //Debug.LogError(" ucvr getRaycastHit state1:"+ state1+ ";state2:"+ state2);
                list = new List<int>();
                if (state1 == ControllerState.Connected)
                    list.Add(0);
                if (state2 == ControllerState.Connected)
                    list.Add(1);
            }
            else
            {

            }
             controllerConnectedlist = list;
            return list;
#else

            controllerConnectedlist = list;
            return list;
#endif

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
       
            return CvrDependentApi.HeadBox.getHmdDofType() ;
        }
        /// <summary>
        /// 打开或关闭头盔凝视点在界面上的显示
        /// 如果第三方使用的凝视点是sdk预设内提供的默认凝视点此处可以使用默认实现
        /// </summary>
        /// <param name="cursorStatus"></param>
        public static void closeOrOpenHeadGaze(CursorStatus cursorStatus)
        {

            if (OpenApiImp.CyberCloudOpenApiEnable)
                return;
            CvrDependentApi.HeadBox.closeOrOpenHeadGaze(cursorStatus);
        }
        /// <summary>
        /// 获取设备唯一编号
        /// 注：只要保证设备唯一即可 用于终端用户标识
        /// </summary>
        /// <returns></returns>
        public static string getDeviceSN()
        {
            return CvrDependentApi.HeadBox.getDeviceSN();
            //return "CyberClloudTest";//所有测试id都使用这个
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
            return CvrDependentApi.HeadBox.getKeyDown(key); ;
        }
        /// <summary>
        /// 获取头盔按键的弹起状态，这里要注意getKeyUp是个一次性的事件，只有在getKeyUp后的那一个frame里能够查询到此状态为true，后面就都为false
        /// </summary>
        /// <param name="key">手柄按键 详见CyberCloud_KeyCode定义</param>
        /// <returns></returns>
        public static bool getKeyUp(ControllerKeyCode key)
        {
            ////////////////三方实现代码///////////////////////
            return CvrDependentApi.HeadBox.getKeyUp(key); ;
        }
    }
    public enum CursorStatus
    {
        closeCursor = 1,
        openCursor = 2
    }
    public enum HeadBoxDofType
    {
        Dof3 = 1,//3dof头盔
        Dof6 = 2//6dof头盔类型
    }
    /// <summary>
    /// 手柄或头盔按键值
    /// </summary>
    public enum ControllerKeyCode
    {
        APP = 1,
        TOUCHPAD = 2,//触摸板
        HOME = 3,//头盔或手柄的home键
        VOLUMEUP = 4,//头盔或手柄音量+
        VOLUMEDOWN = 5,//头盔或手柄音量-
        TRIGGER=10,//手柄扳机键
   
        HmdOK = 11//头盔确认键
    }
    /// <summary>
    /// 触摸板私有按键定义
    /// </summary>
    public enum ControllerKeyCodePrivate
    {       
        TOUCHPADUP = 6,//触摸板上
        TOUCHPADDOWN = 7,//
        TOUCHPADLEFT = 8,//
        TOUCHPADRIGHT = 9,//
    }

}