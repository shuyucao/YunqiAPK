using System;
using System.Collections.Generic;
using Assets.CyberCloud.Scripts.Tools;
using com.cybercloud.tcpTool;
using Newtonsoft.Json;
namespace Assets.CyberCloud.Scripts
{
    public class LiveManager
    {
        MyMulticastFinder finder;//教室场景用于发送广播
        private CyberCloudTcpCommSdk tcpClient;//二期用于tcp通信
        GameAppControl gameAppControl;
        public void startConnect(GameAppControl g) {

            if (false)
            {
                String json = "{\"eventName\":\"exitApk\"}";       
                TcpData data = JsonConvert.DeserializeObject<TcpData>(json);
                MyTools.PrintDebugLogError("==============ucvr start startConnect==============="+ data.eventName+ ";json:"+ json);
                return;
            }


            gameAppControl = g;
            finder = new MyMulticastFinder();
            finder.StartMulticast();
            {//二期需要tcp建联连接
                //二期改用TCP通信
                tcpClient = new CyberCloudTcpCommSdk();
                //IPAddress addr = IPAddress.Parse(UdpReceiveService.getMyIp());
                Address serverAddress = new Address(UdpReceiveService.getMyIp(), MyMulticastFinder.TcpListenerPort);

                tcpClient.clientConnnectResultListener += clientConnnectResultListener;
                tcpClient.tcpReceiveListener += tcpReceiveListener;
                tcpClient.ConnectTcp(serverAddress);
                MyTools.PrintDebugLog("ucvr start tcpclient");
             
            }
            if (UdpReceiveService.IsUdpcRecvStart == false)
            {
                UdpReceiveService.udpReciveMessage = gameAppControl.udpReciveMessage;
                UdpReceiveService.StartReceive();
            }
        }

        //为了兼容一期通过组播和upd发送直播和退出指令，此处通过tcp连接状态如果连接失败使用组播和udp通信否则使用tcp通信
        private bool JSCJTcpEnble = false;
        /// <summary>
        /// tcp连接结果，如果连接成功将使用tcp直播通信否则使用广播和udp
        /// </summary>
        /// <param name="connect"></param>
        private void clientConnnectResultListener(Boolean connect)
        {
            if (connect)
            {
                MyTools.PrintDebugLog("ucvr JSCJ Tcp connect success");
                JSCJTcpEnble = true;
            }
            else
            {
                JSCJTcpEnble = false;
                MyTools.PrintDebugLogError("ucvr JSCJ Tcp connect failed");

            }
        }
        private void tcpReceiveListener(string json, string ip)
        {
            MyTools.PrintDebugLog("ucvr tcpReceiveListener json:" + json + ";ip:" + ip);
            if (json != null && json != "") {
                //Object obj = JsonConvert.DeserializeObject(json);
                TcpData data = JsonConvert.DeserializeObject<TcpData>(json);
                if (data.eventName == TcpEventName.ExitApk)
                {
                    gameAppControl.proExitAppByTeacher();
                }
                else if (data.eventName == TcpEventName.LiveAddrSendResult)
                {
                    if (data.value == 0)
                    {//发送成功
                        gameAppControl.liveAddrSendSuccess(data.deviceID);
                    }
                    else {//发送失败需要重复发送
                        sendMessageToListennerTeacher(data.deviceID, paraToTeacherOnStartLiveArr[data.deviceID],true);
                    }
                }
                else {
                    MyTools.PrintDebugLogError("ucvr tcpReceiveListener unknown");
                }
            }
        }
        private class ParaToTeacherOnStartLive{
            public string deviceID;
            public string liveAddr;
        }
       // private List<ParaToTeacherOnStartLive> paraToTeacherOnStartLiveArr;
        Dictionary<String, String> paraToTeacherOnStartLiveArr = new Dictionary<String, String>();
        public void sendMessageToListennerTeacher(string deviceID, string liveAddr, bool start)
        {
            string msg = "";
            if (GameAppControl.getGameRuning() == false&& start)
            {
                MyTools.PrintDebugLog("ucvr gamestop no need start" );
                return;
            }
         
            if (JSCJTcpEnble)
            {
            
                //{ "eventName":"CyberCloudGameLiveAddr","deviceID":"SN","liveAddr":ip: port: sessionID}
                CyberCloudGameLiveAddrData data = new CyberCloudGameLiveAddrData();
                if (start)
                {
                    data.eventName = "clientVideoStreaming";
                    paraToTeacherOnStartLiveArr[deviceID] = liveAddr;
                }
                else
                {
                    data.eventName = "stopLiveStream";
                    paraToTeacherOnStartLiveArr[deviceID] = "";
                }
                data.deviceID = deviceID;
                data.liveAddr = liveAddr;
                msg = JsonConvert.SerializeObject(data);
                tcpClient.SendTcToServerMessage(msg, UdpReceiveService.getMyIp());
                MyTools.PrintDebugLog("ucvr sendTcpMessageToListennerTeacher:" + msg);
            }
            else
            {
                if (start)
                    msg = "ClientVideoStreaming|" + deviceID + "|" + liveAddr;
                else
                    msg = "StopLiveStream|" + deviceID + "|" + liveAddr;
                MyTools.PrintDebugLog("ucvr sendBroadcastMessageToListennerTeacher:" + msg);
                finder.SendBroadcastMessage(msg);
            }

        }

        private class CyberCloudGameLiveAddrData {
            public string eventName = "CyberCloudGameLiveAddr";
            public string deviceID = "";
            public string liveAddr = "";
        }
        private class TcpData
        {
            public string eventName = "";
            public int value = 1;
            public string deviceID = "";
         
        }
        public class TcpEventName
        {
            public static string ExitApk = "exitApk";
            public static string LiveAddrSendResult = "liveAddrSendResult";
        }
    }
}
