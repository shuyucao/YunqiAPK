using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using CyberCloud.PortalSDK.Util;

public class MyMulticastFinder  {
    /// <inheritdoc />
    public MyMulticastFinder()
    {
        MulticastSocket = new Socket(AddressFamily.InterNetwork,
            SocketType.Dgram,
            ProtocolType.Udp);
        MulticastAddress = IPAddress.Parse("239.255.255.250");
    }

    /// <summary>
    /// 寻找局域网设备
    /// </summary>
    public void FindPeer( )
    {
        // 实际是反过来，让其他设备询问

        StartMulticast();

       // List<IPAddress> ipList = GetLocalIpList().ToList();
        //string message = string.Join(';', ipList);
      //  SendBroadcastMessage(message);
        // 先发送再获取消息，这样就不会收到自己发送的消息
       // ReceivedMessage += (s, e) => { Console.WriteLine($"找到 {e}"); };
    }

    /// <summary>
    /// 获取本地 IP 地址
    /// </summary>
    /// <returns></returns>
    private IEnumerable<IPAddress> GetLocalIpList()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                yield return ip;
            }
        }
    }

    /// <summary>
    /// 组播地址
    /// <para/>
    /// 224.0.0.0～224.0.0.255为预留的组播地址（永久组地址），地址224.0.0.0保留不做分配，其它地址供路由协议使用；
    /// <para/>
    /// 224.0.1.0～224.0.1.255是公用组播地址，可以用于Internet；
    /// <para/>
    /// 224.0.2.0～238.255.255.255为用户可用的组播地址（临时组地址），全网范围内有效；
    /// <para/>
    /// 239.0.0.0～239.255.255.255为本地管理组播地址，仅在特定的本地范围内有效。
    /// </summary>
    public IPAddress MulticastAddress { set; get; }
    public Thread thr;
    private const int MulticastPort = 15001;
    private const int SendCastPort = 15002;
    //15003	tcp	用于tcp消息的发送和监听
    public const int TcpListenerPort = 15003;
    public const int UdpSocketPort = 7976;
    /// <summary>
    /// 启动组播
    /// </summary>
    public void StartMulticast()
    {
        try
        {
            Debug.Log("ucvr ------------------------- 开始绑定IP 和端口");
            // 如果首次绑定失败，那么将无法接收，但是可以发送
            TryBindSocket();

			// Define a MulticastOption object specifying the multicast group 
			// address and the local IPAddress.
			// The multicast group address is the same as the address used by the server.
			// 有多个 IP 时，指定本机的 IP 地址，此时可以接收到具体的内容
			string ipv4 = IPManager.GetIP(ADDRESSFAM.IPv4);
			var multicastOption = new MulticastOption(MulticastAddress, IPAddress.Parse(ipv4));

            MulticastSocket.SetSocketOption(SocketOptionLevel.IP,
                SocketOptionName.AddMembership,
                multicastOption);
            Debug.Log("ucvr ------------------------- 组播的初始化完成");
        }
        catch (Exception e)
        {
            Debug.Log("ucvr ------------------------- 组播的初始化出错+ "+e.ToString ());
        }
		//目前不用接收数据
       // thr = new Thread(ReceiveBroadcastMessages);
        //thr.Start();
       // Task.Run(ReceiveBroadcastMessages);
    }

    /// <summary>
    /// 收到消息
    /// </summary>
   // public event EventHandler<string> ReceivedMessage;

    private void ReceiveBroadcastMessages()
    {
        // 接收需要绑定 MulticastPort 端口
    
        var bytes = new byte[MaxByteLength];
        EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

        try
        {
            while (!_disposedValue)
            {
                Debug.Log("ucvr =========================== 组播开始接收数据");

                var length = MulticastSocket.ReceiveFrom(bytes, ref remoteEndPoint);

                OnReceivedMessage(Encoding.UTF8.GetString(bytes, 0, length));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    /// <summary>
    /// 发送组播
    /// </summary>
    /// <param name="message"></param>
    public void SendBroadcastMessage(string message)
    {
        try
        {
            IPEndPoint endPoint = new IPEndPoint(MulticastAddress, SendCastPort);
            byte[] byteList = Encoding.UTF8.GetBytes(message);

            if (byteList.Length > MaxByteLength)
            {
                //throw new ArgumentException($"传入 message 转换为 byte 数组长度太长，不能超过{MaxByteLength}字节")
                //{
                //    Data =
                //        {
                //            { "message", message },
                //            { "byteList", byteList }
                //        }
                Debug.LogError("ucvr ============== 传入 message 转换为 byte 数组长度太长，不能超过{MaxByteLength}字节");
                //};
            }

            MulticastSocket.SendTo(byteList, endPoint);
        }
        catch (Exception e)
        {
            Console.WriteLine("\n" + e);
        }
    }

    private IPAddress LocalIpAddress  = IPAddress.Any;

    private Socket MulticastSocket;

    private void TryBindSocket()
    {
    
        for (var i = MulticastPort; i < 65530; i++)
        {
            try
            {
                EndPoint localEndPoint = new IPEndPoint(LocalIpAddress, i);
                Debug.Log("ucvr ------------------------- 开始绑定IP 和端口  " + LocalIpAddress.ToString() + "   " + i.ToString());
                MulticastSocket.Bind(localEndPoint);
                return;
            }
            catch (SocketException e)
            {
                Debug.Log("ucvr ------------------------- 绑定IP出错+——" + e.ToString());
            }
        }
    }

    private const int MaxByteLength = 2048;

    #region IDisposable Support

    private bool _disposedValue = false; // 要检测冗余调用

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
            }
            if (MulticastSocket != null)
            {
                MulticastSocket.Close();
            }
  

         //   ReceivedMessage = null;
            MulticastAddress = null;

            _disposedValue = true;
        }
    }

    // 添加此代码以正确实现可处置模式。
    public void Dispose()
    {
        // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion

    private void OnReceivedMessage(string e)
    {
        //ReceivedMessage?.Invoke(this, e);
        Debug.Log("ucvr ========================= + 接收到了 广播信息" + e.ToString());

      
      
    }
}


