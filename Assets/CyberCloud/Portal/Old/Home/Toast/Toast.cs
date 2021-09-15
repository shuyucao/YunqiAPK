using UnityEngine;
using System.Collections;

public class Toast : MonoBehaviour   //, IMsgHandle
{
    //void Start()
    //{
    //    MsgManager.Instance.RegistMsg(MsgID.NetWorkIsSlow, this);
    //    MsgManager.Instance.RegistMsg(MsgID.NetWorkNotAvaillable, this);
    //}

    //public void HandleMessage(MsgID id, Bundle bundle)
    //{
    //    if (id == MsgID.NetWorkIsSlow)
    //    {
    //        // CommonAlert.Show("Home_Category_Failed", true);
    //    }
    //    else if (id == MsgID.NetWorkNotAvaillable)
    //    {
    //        CommonAlert.Show("Home_NoNet");
    //    }
    //}

    public void GetLowBattry_Activity(string value)
    {
        Debug.Log("no need show Battry！！");
        //if (value == "5")
        //{
        //    CommonAlert.Show("Home_Low_Battery_5");
        //}
        //else
        //{
        //    CommonAlert.Show("Home_Low_Battery");
        //    MachineState.IsBattryLower = true;
        //}
    }

    public void ShowLowAvaliableSize(string type)
    {
        Debug.Log("Jar : ShowLowAvaliableSize" + type);
        string Size_Type = type;
        if (Size_Type == "1")
        {
            CommonAlert.Show("PlayerScroll_Out_Storage");
        }
    }

    //获取网络状态
    public void GetNetworkStatus_Activity(string value)
    {
        if (value == "1")
        {
            CommonAlert.Clear();
        }
        else if (value == "0")
        {
            Debug.Log("*****no net!*****");
            CommonAlert.Show("Home_NoNet");
            MsgManager.Instance.SendMsg(MsgID.NetWorkNotAvaillable, null);
        }
    }
}