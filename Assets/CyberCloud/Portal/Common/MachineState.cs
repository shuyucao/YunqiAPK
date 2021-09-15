public class MachineState
{
    private static bool mWifiAvailable = true;
    public static bool IsWifiAvailable
    {
        get
        {
            if (UnityEngine.Application.platform == UnityEngine.RuntimePlatform.Android)
            {
                int state = 0;
                PicoUnityActivity.CallObjectMethod<int>(ref state, "isNetworkAvailable");
                if (state == 0)
                {
                    mWifiAvailable = false;
                }
                else if (state == 1)
                {
                    mWifiAvailable = true;
                }
            }
            return mWifiAvailable;
        }
    }
    public static bool IsBattryLower { get; set; }
}