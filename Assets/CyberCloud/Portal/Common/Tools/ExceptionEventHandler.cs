using UnityEngine;

public class ExceptionEventHandler
{
    public static void Init()
    {
        Application.logMessageReceived += LogCallback;
    }

    private static void LogCallback(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Assert || type == LogType.Exception)
        {
            // 此方法里不能使用log
            if (Application.platform == RuntimePlatform.Android)
            {
                string param = condition + System.Environment.NewLine + stackTrace + System.Environment.NewLine + type.ToString();
                // 在UnityActivit中实现ReportError方法
                PicoUnityActivity.CallObjectMethod("ReportError", param);
            }
        }
    }
}
