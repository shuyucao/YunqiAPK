public class LanguageManager
{
    public static string GetSystemLanguage()
    {
        string systemLanguage = "zh";
#if UNITY_ANDROID
        PicoUnityActivity.CallObjectMethod<string>(ref systemLanguage, "getLanguage");
#endif
        if (systemLanguage.Equals("zh"))
        {
            systemLanguage = "cn";
        }
        return systemLanguage;
    }
}