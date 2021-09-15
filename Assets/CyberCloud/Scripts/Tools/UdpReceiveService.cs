using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using CyberCloud.PortalSDK.Util;
using Newtonsoft.Json;

namespace Assets.CyberCloud.Scripts.Tools
{
    
    /// <summary>
    /// 接受
    /// </summary>
    public class UdpReceiveService
    {
        public delegate void UDPReciveMessage(String message);
        public static UDPReciveMessage udpReciveMessage;
        /// <summary>
        /// 用于UDP发送的网络服务类
        /// </summary>
        static UdpClient udpcRecv = null;

        static IPEndPoint localIpep = null;

        /// <summary>
        /// 开关：在监听UDP报文阶段为true，否则为false
        /// </summary>
        public static bool IsUdpcRecvStart = false;
        /// <summary>
        /// 线程：不断监听UDP报文
        /// </summary>
        static Thread thrRecv;

        public static void StartReceive()
        {
        

            try
            {
                if (!IsUdpcRecvStart) // 未监听的情况，开始监听
                {
                    // 有多个 IP 时，指定本机的 IP 地址，此时可以接收到具体的内容
                    string ipv4 = getMyIp();// IPManager.GetIP(ADDRESSFAM.IPv4);
                    localIpep = new IPEndPoint(IPAddress.Parse(ipv4), MyMulticastFinder.UdpSocketPort); // 本机IP和监听端口号
                    udpcRecv = new UdpClient(localIpep);
                    thrRecv = new Thread(ReceiveMessage);
                    thrRecv.Start();
                    IsUdpcRecvStart = true;
                    Debug.Log("ucvr UDP监听器已成功启动: my ip is :" + ipv4);
                }
            }
            catch(Exception e) {
                Debug.LogError("ucvr StartReceive error:"+e.StackTrace);
            }
        }
        public static string getMyIp() {
            //if(true)
           //     return "192.168.24.133";
            IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());   //Dns.GetHostName()获取本机名Dns.GetHostAddresses()根据本机名获取ip地址组
            foreach (IPAddress ip in ips)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    Debug.Log("ucvr ipv4:" + ip.ToString());
                    return ip.ToString();  //ipv4
                   
                }

                else if (ip.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    //   return ip.ToString(); //ipv6
                    Debug.Log("ucvr ipv6:"+ip.ToString());
                }


            }
            return "127.0.0.1";
        }
        public static void StopReceive()
        {
            Debug.Log("ucv StopReceive");
            try { 
                if (IsUdpcRecvStart)
                {
                    //thrRecv.Abort(); // 必须先关闭这个线程，否则会异常
                    udpcRecv.Close();
                    IsUdpcRecvStart = false;
                    Debug.Log("ucvr UDP监听器已成功关闭");
                }
            }
            catch (Exception e)
            {
                Debug.Log("ucvr StopReceive error:" + e.StackTrace);
            }
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="obj"></param>
        private static void ReceiveMessage(object obj)
        {
            while (IsUdpcRecvStart)
            {
                try
                {
                    localIpep = new IPEndPoint(IPAddress.Any, MyMulticastFinder.UdpSocketPort); // 本机IP和监听端口号;
                    byte[] bytRecv = udpcRecv.Receive(ref localIpep);
                    Debug.Log("ucvr ReceiveMessage 1");
                    if (bytRecv != null&& bytRecv.Length>0)
                    {

                        string message = Encoding.UTF8.GetString(bytRecv, 0, bytRecv.Length);



                        Debug.Log(string.Format("ucvr {0}[{1}]", localIpep, message));
                        if (udpReciveMessage != null)
                        {
                            udpReciveMessage(message);
                        }
                    }
                    else {
                        Debug.Log("ucvr ReceiveMessage null" );
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log("ucvr ReceiveMessage error:" + ex.StackTrace);
                    break;
                }
            }
            Debug.Log("ucvr ReceiveMessage over");
        }

    }
}
