using UnityEngine;
using System.Collections;

public class ChangeLanguage : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        GetSystemLanguage();
    }

    //void OnGUI()
    //{
    //    if (GUI.Button(new Rect(0, 0, 200, 200), "Change Language"))
    //    {
    //        Localization.language = Localization.language == "Chinese" ? "Japanese" : "Chinese";
    //    }
    //}

    void GetSystemLanguage()
    {
        string systemLanguage = "zh";
#if UNITY_ANDROID
        PicoUnityActivity.CallObjectMethod<string>(ref systemLanguage, "getLanguage");
#endif
        SetLanguage(systemLanguage);
    }

    public void Android_SendLanguageToUnity(string language)
    {
        Debug.Log("current android system language is " + language);
        SetLanguage(language);
    }

    void SetLanguage(string systemLanguage)
    {
        string language = "English";
        switch (systemLanguage)
        {
            case "zh":
                language = "Chinese";
                break;
            case "en":
                language = "English";
                break;
            case "ja":
                language = "Japanese";
                break;
            default:
                language = "Enlish";
                break;
        }
        Localization.language = language;
    }
}