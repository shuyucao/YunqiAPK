using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Assets.CyberCloud.Scripts.DeviceController
{
    public class DeviceInfo : MonoBehaviour
    {
       
       
        float[] fov = new float[4] { 1.213f, 1.213f, 1.213f, 1.213f };
        public static int unityDataEnable=0;
        void Awake()
        {
            Debug.Log("ucvr DeviceInfo Awake");


        }
        void Start() {
         
        }
        void Update()
        {
            if (unityDataEnable == 0)//判断是否开启unity数据
                return;

            //2更新头盔的姿态
            float[] dataHead = null;// Pvr_UnitySDKManager.SDK.headData;
            //string dataHeadStr= setDevicePose(0, dataHead);
                    
            //Debug.Log("ucvr dataHead:" + dataHeadStr);
            //3更新手柄連接状态和姿态
            for (int i = 0; i < 2; i++)
            {
               // ControllerState state = Pvr_UnitySDKAPI.Controller.UPvr_GetControllerState(i);
               // if (useTerminalFrmRtCtrl == ControllerState.Connected)
                {              
                    //setDevicePose(handindex, data);             
                  //  setControllerKeyEvent(i, key);
                }
           //     else
                {
             //       setControllerConnectionState(i, 0);
                }
            }


        }


        /**
         * 获取设备预测姿态位置接口
         * @param devIndex 设备ID：0表示HMD，1表示手柄1，2表示手柄2
         * @param deltaTime 预测时间,单位毫秒
         * @return
         *      获得deltaTime时间后的设备姿态和位置， 返回值为长度为7的float数组。
         *      数组的前四位(0-3)为姿态（Qx，Qy，Qz，Qw),
         *      数组的后三位(4-6)为位置（Px，Py，Pz）,
         *      返回数据符合右手坐标系
         */
        public string setDevicePose(int devIndex, float[] pose)
        {
            return MyTools.setDevicePose(devIndex,pose);
         
        }
        int keynum = 0;
        /**
         *  获取手柄获键值接口
         * @param devIndex 设备ID: 1表示手柄1， 2表示手柄2
         * @return
         *      返回结果具体请参见文档
         */
        public void setControllerKeyEvent(int devIndex, int[] key)
        {
            MyTools.setControllerKeyEvent(devIndex, key);
        }
        /**
         *  手柄连接状态
         * @param devIndex  设备ID：1表示手柄1，2表示手柄2
         * @return
         */
        public void setControllerConnectionState(int devIndex, int state)
        {
            MyTools.setControllerConnectionState(devIndex, state); ;
        }
  
    
        public void setHMDFov(float[] hmd)
        {
            MyTools.setHMDFov(hmd);
        }

       

    }
}
