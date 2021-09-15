using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XMPPHelper
{
    private static XMPPHelper _instance = new XMPPHelper();

    public static XMPPHelper Instance
    {
        get {
            return _instance;
        }
    }

    private XMPPHelper()
    {   
        AndroidJavaObject context = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        _xmpphelper = new AndroidJavaObject("com.deepoon.xmpphelper.XMPPHelper", context);
    }

    AndroidJavaObject _xmpphelper;
    public delegate void OnMessage(string message);
    public class OnMessageListener : AndroidJavaProxy
    {
        OnMessage _handle;
        public OnMessageListener(OnMessage handler) : base("com.deepoon.xmpphelper.XMPPHelper$OnMessageListener")
        {
            _handle = handler;
        }

        void onMessage(string message)
        {
            _handle(message);
        }
    }

    public void SendBroadcast(string message , string toUser)
    {
        _xmpphelper.Call("sendBroadcast", message , toUser);
    }

    public void SetOnMessageListener(AndroidJavaProxy listener)
    {
        _xmpphelper.Call("setOnMessageListener", listener);
    }
}
