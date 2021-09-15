using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace com.cybercloud.tcpTool
{

    public class Address
    {

        public string IP;
        public int Port;
        public Address(string ip, int _port)
        {
            IP = ip;
            Port = _port;
        }
    }

    /// <summary>
    /// TCP接收端口15003
    /// 组播接收端口15002
    /// </summary>
    public class CyberCloudTcpCommSdk
    {

        //初始化结果
        public delegate void InitCompleteDel(int code, string desc);
        //组播发送结果
        public delegate void MulticastSendCompleteDel(int code, string desc);
        //tcp发送结果。
        public delegate void TcpSendCompleteDel(int code, string desc);
        //用于监听组播消息回调。Json为回调内容参考发送组播
        public delegate void MulticastReceiveListener(string json,string ip );
        //用于监听tcp消息回调。具体json内容参考上文中教师端与学生端的tcp接口定义
        public delegate void TcpReceiveListener(string json, string ip);
        //某一客户端断开连接
        public delegate void ClientDisConnectListener(string ip);
        //连接成功
        public delegate void ClinetConnectServerResult(bool result);


        //public delegate void UdpReceiveListener(string json);
        /// <summary>
        /// 初始化事件监听
        /// </summary>
        public event InitCompleteDel initCompleteDel;
        /// <summary>
        /// 发送组播事件监听
        /// </summary>
        public event MulticastSendCompleteDel multicastSendCompleteDel;
        /// <summary>
        /// tcp发送数据监听
        /// </summary>
        public event TcpSendCompleteDel tcpSendCompleteDel;
        /// <summary>
        /// 接收组播监听
        /// </summary>
        public event MulticastReceiveListener multicastReceiveListener;
        /// <summary>
        /// 接收tcp监听
        /// </summary>
        public event TcpReceiveListener tcpReceiveListener;
        /// <summary>
        /// 某一客户端从服务器断开连接
        /// </summary>
        public event ClientDisConnectListener clientDisConnectListener;
        // public event UdpReceiveListener udpReveiveListener;

        public event ClinetConnectServerResult clientConnnectResultListener;

        private Socket server;
        //  private Socket clientServer;
        private Thread createServerThread;
        private bool isListenClient;
        private bool isListenSever;
        public Dictionary<string, TcpClient> clientDic = new Dictionary<string, TcpClient>();
        public Dictionary<string, TcpClient> multiDic = new Dictionary<string, TcpClient>();

        private bool isClientReceive = false;
        private Thread clientReceiveThread;
        private MulticastFinder multiCastFinder;
        bool isInitSuccess = false;

        /// <summary>
        /// 服务端初始化
        /// </summary>
        /// <param name="_local"></param>
        /// <param name="multicast">组播监听端口，默认是15002   </param>
        /// <param name="type"> 使用端类型 可用值为0和1  0：表示当前处于学生端  1：表示当前处于教师端</param>
        public void Init(Address _local, Address multicast, string type)
        {
            isInitSuccess = true;
            if (type == "1")
            {
                Debug.Log("teacherlog>>  目前处于服务端  ");
                if (_local != null)
                {
                    server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                    server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, true);
                    server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    IPAddress ip = IPAddress.Parse(_local.IP);//创建IP地址,可以是本机的私网IP
                    EndPoint ep = new IPEndPoint(ip, _local.Port);//ip地址与端口号的组合，端口号大一些就
                    server.Bind(ep);//绑定EP
                                    //服务器开始监听
                    server.Listen(500);//设置最大连接数量为500
                    //开启服务器线程
                    createServerThread = new Thread(new ParameterizedThreadStart(CreateServer));
                    createServerThread.IsBackground = true;
                    isListenClient = true;
                    createServerThread.Start(_local);
                }
            }
            else
            {
                
                //将服务开启来
                Debug.Log("teacherlog>>  目前处于客户端");
                if (_local != null)
                {
                    try
                    {
                        server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        IPAddress ip = IPAddress.Parse(_local.IP);//创建IP地址,可以是本机的私网IP
                        EndPoint ep = new IPEndPoint(ip, _local.Port);//ip地址与端口号的组合，端口号大一些就
                        server.Bind(ep);//绑定EP
                                        //服务器开始监听
                        server.Listen(500);//设置最大连接数量为500

                        //在线程中监听客户端的连接
                        createServerThread = new Thread(new ParameterizedThreadStart(CreateServer));
                        createServerThread.IsBackground = true;
                        isListenClient = true;
                        createServerThread.Start(_local);

                    }
                    catch (Exception e)
                    {

                        GetInitListen(1, "TCP服务端建立失败 ： " + e.ToString());
                    }



                    // StartClientTcp(_local);
                }
            }
            if (multicast != null)
            {
                Debug.Log("teacherlog >>   开启组播TCP连接   ");
                //连接组播
                StartMultiCast(multicast);
            }

            if (isInitSuccess)
            {
                GetInitListen(0, "初始化成功");
                isInitSuccess = false;
            }

            //  initCompleteDel(isSuccess, des);
            return;
        }
        /// <summary>
        /// 初始化监听
        /// </summary>
        /// <param name="code"></param>
        /// <param name="desc"></param>
        public void GetInitListen(int code, string desc)
        {
            if (code == 1)
            {
                isInitSuccess = false;
            }

            if (initCompleteDel != null)
            {
                initCompleteDel(code, desc);
            }
        }
        /// <summary>
        /// 开启客户端TCP连接
        /// </summary>
        /// <param name="_local"></param>
        public void ConnectTcp(Address serverAddress)
        {
           // bool isConnectSuccess = true;
            try
            {

                Thread connectTcpThread = new Thread(ConnectTcpThread);
                connectTcpThread.IsBackground = true;
                connectTcpThread.Start(serverAddress);
            }
            catch (Exception e)
            {
                GetInitListen(1, "客户端TCP连接失败 ： " + e.ToString());
               // isConnectSuccess = false;
            }
          //  return isConnectSuccess;
        }


        public void ConnectTcpThread(object obj)
        {
            try
            {
                Address serverAddress = (Address)obj;
                Debug.Log("teacherlog ====================   serverAddress IP : " + serverAddress.IP + "端口号为 : " + serverAddress.Port);
                // //创建客户端
                Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                //client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, true);
                //client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                ////创建IP地址（IP地址和端口号是服务器端的，用来和服务器进行通信）
                IPAddress ip = IPAddress.Parse(serverAddress.IP);
                EndPoint ep = new IPEndPoint(ip, serverAddress.Port);
                client.Connect(ep);
                TcpClient multiClient = new TcpClient(client, this, serverAddress.IP);
                multiDic.Add(serverAddress.IP, multiClient);
                if (clientConnnectResultListener != null)
                {
                    clientConnnectResultListener(true);
                }
            }
            catch (Exception  e )
            {
                if (clientConnnectResultListener != null)
                {
                    clientConnnectResultListener(false );
                }
                GetInitListen(1, "客户端TCP连接失败 ： " + e.ToString());
                Debug.LogError("ucvr ConnectTcpThread error:"+e.StackTrace);
            }

        }

        /// <summary>
        /// 服务器开启
        /// </summary>
        /// <param name="_local"></param>
        public void CreateServer(object _local)
        {
            string des = string.Empty;
            try
            {
                Address local = (Address)_local;
                Debug.Log("teacherlog >>   成功开启TCP连接   >> 服务端");
                while (isListenClient)
                {
                    try
                    {
                        Socket client = server.Accept();//获取客户端的socket，用来与客户端通信
                        IPEndPoint clientipe = (IPEndPoint)client.RemoteEndPoint;
                        string clientIP = clientipe.Address.ToString();
                        Debug.Log("teacherlog ============================  一个客户端连接进来了  IP为 ： " + clientIP);

                        TcpClient connectClient = new TcpClient(client, this, clientIP);
                        if (!clientDic.ContainsKey(clientIP))
                        {
                            clientDic.Add(clientIP, connectClient);
                        }
                        else
                        {
                            clientDic[clientIP] = connectClient;
                        }
                    }
                    catch (Exception e )
                    {
                        Debug.Log("teacherlog 等待客户端连接 并接受出错  ：" + e.ToString());
                    }
        
                }
            }
            catch (System.Exception e)
            {
                Debug.Log("teacherlog >>   开启TCP连接失败    >> 服务端  " + e.ToString());
                return;

            }
        }
        public void ClientDisConnect(string ip)
        {

            Debug.Log("teacherlog      教师端是否包含 此IP " + ip + "  clientDic.ContainsKey(ip)  bool 值为 ：  " +  clientDic.ContainsKey(ip));

            if (clientDic.ContainsKey(ip))
            {
                try
                {
                    TcpClient tcpClient = clientDic[ip];
                    clientDic.Remove(ip);
                    //只有连接进来的客户端失去连接才会进行回调方法
                    if (clientDisConnectListener != null)
                    {
                        clientDisConnectListener(ip);
                    }
                    Debug.Log("teacherlog clientDic  当客户端断开连接时 移除字典成功");
                    tcpClient.UnInit();
                }
                catch (Exception e)
                {

                    Debug.Log("teacherlog clientDic   当客户端断开连接时 移除字典出错  错误信息  " + e.ToString());
                }
            }

            if (multiDic.ContainsKey(ip))
            {

                try
                {
                    //连接不需要进行回调

                    TcpClient client = multiDic[ip];
                    multiDic.Remove(ip);
                    Debug.Log("teacherlog   multiDic 当客户端断开连接时 移除字典成功");
                    client.UnInit();
                }
                catch (Exception e)
                {

                    Debug.Log("teacherlog multiDic   当客户端断开连接时 移除字典出错  错误信息  " + e.ToString());
                }

            }
        }
        /// <summary>
        /// 开启组播
        /// </summary>
        /// <param name="multicast"></param>
        public void StartMultiCast(Address multicast)
        {
            multiCastFinder = new MulticastFinder(this);
            multiCastFinder.StartMulticast(multicast);
        }

        /// <summary>
        /// 发送组播数据
        /// </summary>
        /// <param name="port"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        public int sendMulticast(string json)
        {
            multiCastFinder.SendBroadcastMessage(json);
            return 0;
        }

        /// <summary>
        /// 组播发送数据监听
        /// </summary>
        public void GetMultiListen(int code, string desc)
        {
            if (multicastSendCompleteDel != null)
            {
                multicastSendCompleteDel(code, desc);
            }
        }

        /// <summary>
        /// 组播接收到的数据
        /// </summary>
        /// <param name="json"></param>
        public void GetMultiReceiveListen(string json,string ip )
        {
            if (multicastReceiveListener != null)
            {
                multicastReceiveListener(json, ip);
            }
        }
        /// <summary>
        /// 客户端 服务器发送消息  通用
        /// </summary>
        /// <param name="Ip"></param>
        /// <param name="Port"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        public int SendTcp(string json, string Ip)
        {
    
            if (server != null && !string.IsNullOrEmpty(Ip))
            {
                if (clientDic.ContainsKey(Ip))
                {
                    clientDic[Ip].SendTcp(json);
                    return 0;
                }
            }
            return 0;
        }
        /// <summary>
        /// 教师端发送组播的返回数据
        /// </summary>
        /// <param name="json"></param>
        /// <param name="ip"></param>
        public void SendTcToServerMessage(string json, string ip)
        {
            if (multiDic.Count > 0)
            {
                if (multiDic.ContainsKey(ip))
                {
                    multiDic[ip].SendTcp(json);
                }
            }
        }



        /// <summary>
        /// 发送给所有的客户端
        /// </summary>
        /// <param name="json"></param>
        public void SendAllClient(string json)
        {
            Debug.Log("teacherlog   SendAllClient" + json);
            if (server != null)
            {
                foreach (var item in clientDic.Values)
                {
                    item.SendTcp(json);
                }
            }
        }
        /// <summary>
        /// tcp发送数据监听
        /// </summary>
        /// <param name="code"></param>
        /// <param name="desc"></param>
        public void GetTcpSendListen(int code, string desc)
        {
            if (tcpSendCompleteDel != null)
            {
                tcpSendCompleteDel(code, desc);
            }
        }

        /// <summary>
        /// TCP接收数据监听
        /// </summary>
        /// <param name="json"></param>
        public void GetTcpReceiveListen(string json, string ip)
        {
            if (tcpReceiveListener != null)
            {
                tcpReceiveListener(json, ip);
            }
        }
        /// <summary>
        /// 关闭socket连接
        /// </summary>
        /// <returns></returns>
        public int unInit()
        {
            isListenSever = false;
            //客户端关闭
            isClientReceive = false;
          
            
            //组播关闭
            try
            {
                if (multiCastFinder != null)
                {
                    Debug.Log("teacherlog   关闭组播 ");

                    multiCastFinder.Dispose();
                    multiCastFinder = null;
                }

            }
            catch (Exception e)
            {

                Debug.Log("teacherlog 结束组播报错 " + e.ToString());
            }

            try
            {
                foreach (var item in multiDic)
                {
                    Debug.Log("teacherlog   关闭 multiDic客户端  IP为  : " + item.Value.ip.ToString ());

                    item.Value.UnInit();
                }
            }
            catch (Exception e)
            {

                Debug.Log("teacherlog unInit() multiDic " + e.ToString());
            }
            try
            {
                //服务器关闭
                foreach (var item in clientDic)
                {
                    Debug.Log("teacherlog   关闭 clientDic客户端  IP为  : " + item.Value.ip.ToString());
                    item.Value.UnInit();
                }
            }
            catch (Exception e)
            {
                Debug.Log("teacherlog 结束客户端报错  " + e.ToString());
            }
            clientDic.Clear();
        

            try
            {
                if (server != null)
                {
                    Debug.Log("teacherlog   关闭 server");
                    server.Close();
                    server = null;
                }
            }
            catch (Exception e)
            {

                Debug.Log("teacherlog 结束server端报错  " + e.ToString());
            }
            isListenClient = false;
            if (createServerThread != null)
            {
                Debug.Log("teacherlog   关闭 createServerThread线程 ");
                createServerThread.Interrupt();
                createServerThread.Abort();
                createServerThread = null;
            }


            if (clientReceiveThread != null)
            {
      
                Debug.Log("teacherlog   关闭Server 线程   : ");
                clientReceiveThread.Interrupt();
                clientReceiveThread.Abort();
                clientReceiveThread = null;
            }
            return 0;
        }
    }


    /// <summary>
    /// TCP连接客户端
    /// </summary>
    public class TcpClient
    {

        public Socket clientSocket;
        public string ip;
        public CyberCloudTcpCommSdk tcpSdk;
        /// <summary>
        /// 是否持续接收消息
        /// </summary>
        private bool isReceive = false;
        private Thread TcpReceiveThread;
        public TcpClient(Socket client, CyberCloudTcpCommSdk _tcp, string _ip)
        {
            clientSocket = client;
            tcpSdk = _tcp;
            ip = _ip;

            TcpReceiveThread = new Thread(TcpReceive);
            isReceive = true;
            TcpReceiveThread.IsBackground = true;
            TcpReceiveThread.Start();
        }

        public void UnInit()
        {
            try
            {
                if (clientSocket != null)
                {
                    Debug.Log("teacherlog  关闭 客户端");
                    clientSocket.Close();
                }
                clientSocket = null;
            }
            catch (Exception e)
            {
                Debug.Log("teacherlog  TcpClient UnInit()" + e.ToString());
            }



            isReceive = false;
            if (TcpReceiveThread != null)
            {
                Debug.Log("teacherlog   TcpClient 停止线程");
                try
                {
                    TcpReceiveThread.Interrupt();
                    TcpReceiveThread.Abort();
                }
                catch (Exception e )
                {
                    Debug.Log("teacherlog   TcpClient 停止线程 报错  " +e .ToString ());
                }
          
            }
        }

        public void TcpReceive()
        {

            while (isReceive)
            {
                try
                {
                    if (clientSocket.Connected)
                    {
                        byte[] data = new byte[2048];
                        int length = clientSocket.Receive(data);

                        if (length == 0)
                        {
                           // Debug.Log("teacherlog  接受到的消息为空值  客户端已经断开连接 ：   ip为 ：   " + ip);
                            Debug.Log("teacherlog >> 客户端已经断开连接了  >> IP ： " + ip);
                            tcpSdk.ClientDisConnect(ip);
                            isReceive = false;
                            return;

                        }
                        string message = Encoding.UTF8.GetString(data, 0, length);
                        Debug.Log("teacherlog >> 接收到的消息为 >> " + message+">> 客户端是否连接 》》 " + clientSocket.Connected);
                        //执行TCP的监听
                        tcpSdk.GetTcpReceiveListen(message, ip);
                    }
                    else
                    {
                        Debug.Log("teacherlog >> 客户端已经断开连接了  >> IP ： " + ip);
                        tcpSdk.ClientDisConnect(ip);
                        isReceive = false;
                    }
                
                }
                catch (System.Exception e)
                {
                    Debug.Log("teacherlog >>   线程接收消息出错 :" + e.ToString()+ ";StackTrace:" + e.StackTrace);
                    if (clientSocket.Connected == false)
                    {
                        Debug.Log("teacherlog >>    客户端已经断开连接了");
                        tcpSdk.ClientDisConnect(ip);
                        isReceive = false;
                        return;
                    }
             

                }
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="ip"> 接收消息的IP地址</param>
        /// <param name="port">接收端口号 </param>
        /// <param name="json">发送数据</param>
        /// <returns></returns>
        public int SendTcp(string json)
        {
            try
            {
                Debug.Log("teacherlog >> 发送消息为 >> " + json);
                string message = json+ "\n";
                byte[] data = Encoding.UTF8.GetBytes(message);//转成能传送的byte类型的数据

                clientSocket.Send(data);
                tcpSdk.GetTcpSendListen(0, "发送成功 + IP ：" + ip + "message : " + message);
                Debug.Log("teacherlog SendTcp  发送组播返回数据成功： IP " + ip + "   message : " + message);
                return 0;
            }
            catch (Exception e)
            {
                tcpSdk.GetTcpSendListen(1, "发送失败 ： " + e.ToString());
                Debug.Log("teacherlog SendTcp  发送组播返回数据失败： IP " + ip + "   message : " + json + "  错误原因 ： " + e.ToString());
                return 1;
            }
        }
    }



    /// <summary>
    /// UDP组播
    /// </summary>
    public class MulticastFinder
    {

        private CyberCloudTcpCommSdk tcpSdk;

        /// <inheritdoc />
        public MulticastFinder(CyberCloudTcpCommSdk _tcpSdk)
        {
            tcpSdk = _tcpSdk;
            MulticastSocket = new Socket(AddressFamily.InterNetwork,
                SocketType.Dgram,
                ProtocolType.Udp);
            MulticastAddress = IPAddress.Parse("239.255.255.250");
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
        public int multiPort ;
        /// <summary>
        /// 启动组播
        /// </summary>
        public void StartMulticast(Address  address )
        {
            try
            {
                multiPort = address.Port;
                Debug.Log("teacherlog ------------------------- 开始绑定IP 和端口");
                // 如果首次绑定失败，那么将无法接收，但是可以发送
                TryBindSocket(address.Port);
                // 有多个 IP 时，指定本机的 IP 地址，此时可以接收到具体的内容
                var multicastOption = new MulticastOption(MulticastAddress, IPAddress.Parse(address.IP));
                MulticastSocket.SetSocketOption(SocketOptionLevel.IP,
                    SocketOptionName.AddMembership,
                    multicastOption);


                Debug.Log("teacherlog ------------------------- 组播的初始化完成");
            }
            catch (Exception e)
            {
                tcpSdk.GetInitListen(1, "组播开启失败");
                Debug.Log("teacherlog ------------------------- 组播的初始化出错+ " + e.ToString());
            }
            thr = new Thread(ReceiveBroadcastMessages);
            thr.IsBackground = true;
            thr.Start();
            // Task.Run(ReceiveBroadcastMessages);
        }

        /// <summary>
        /// 收到消息
        /// </summary>
        // public event EventHandler<string> ReceivedMessage;
        private void ReceiveBroadcastMessages()
        {
            // 接收需要绑定 MulticastPort 端口
            while (!_disposedValue)
            {
                try
                {
                    var bytes = new byte[MaxByteLength];
                    EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                  //  Debug.Log("teacherlog =========================== 组播开始接收数据");
                    var length = MulticastSocket.ReceiveFrom(bytes, ref remoteEndPoint);
                    string message = Encoding.UTF8.GetString(bytes, 0, length);
                    string[] str = remoteEndPoint.ToString().Split(':');
                    string IP = str[0];
                    IP = IP.Trim();
                    tcpSdk.GetMultiReceiveListen(message, IP);
                }
                catch (Exception e)
                {
                    Debug.Log("teacherlog >> 接收数据出错： " + e.ToString());
                    return;
                }
            }
        }

        /// <summary>
        /// 发送组播
        /// </summary>
        /// <param name="message"></param>
        public void SendBroadcastMessage(string message)
        {
            //这里走UDP发送数据
            try
            {
                IPEndPoint endPoint = new IPEndPoint(MulticastAddress, multiPort);
                byte[] byteList = Encoding.UTF8.GetBytes(message);
                MulticastSocket.SendTo(byteList, endPoint);
                tcpSdk.GetMultiListen(0, "组播发送成功 message ： " + message);
            }
            catch (Exception e)
            {
                Debug.Log("teacherlog >> 组播发送消息失败 >> " + e.ToString());
                tcpSdk.GetMultiListen(1, "组播发送失败 ： " + e.ToString());
            }
        }

        private IPAddress LocalIpAddress = IPAddress.Any;

        private Socket MulticastSocket;

        private void TryBindSocket(int port)
        {
            try
            {
                EndPoint localEndPoint = new IPEndPoint(LocalIpAddress, port);
                // Debug.Log("teacherlog ------------------------- 开始绑定IP 和端口  " + LocalIpAddress.ToString() + "   " + i.ToString());
                MulticastSocket.Bind(localEndPoint);
                return;
            }
            catch (SocketException e)
            {
                Debug.Log("teacherlog ------------------------- 绑定IP出错+——" + e.ToString());
            }

        }

        private const int MaxByteLength = 2048;

        #region IDisposable Support

        private bool _disposedValue = false; // 要检测冗余调用

        private void Dispose(bool disposing)
        {
            if (MulticastSocket != null)
            {
                MulticastSocket.Close();
            }
            //   ReceivedMessage = null;
            MulticastAddress = null;

            _disposedValue = true;

            if (thr != null)
            {
                thr.Interrupt();
                thr.Abort();
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

    }
}