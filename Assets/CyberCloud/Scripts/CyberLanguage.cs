using UnityEngine;
using System.Collections;

public class CyberLanguage : MonoBehaviour {

	public static CyberLanguage Instance { get; private set; }
	
	public Font MicrosoftYH;
	public Font Arial;

	void Awake ()
	{
		if (Instance != null && Instance != this) {
			Destroy (gameObject);
		}
		Instance = this;
	}

	// Use this for initialization
	void Start (){
		GetSystemLanguage ();
	}
	
	//ucvr 国际化设置语言
	void GetSystemLanguage (){
	string systemLanguage = CyberCloudConfig.cvrLanguage;// 
                                                         //演示时不希望看到范围光圈
                                                         // if ( CyberCloudConfig.cvrScreen== CyberCloudConfig.CVRScreen.YanShi)
                                                         //     Pvr_UnitySDKManager.SDK.CustomRange = 120;//安全提示演示环境修改成120
        MyTools.PrintDebugLog("ucvr  set mLanguage=" + systemLanguage+ ";Application.systemLanguage:"+ Application.systemLanguage);
        Localization.language = systemLanguage;
	}
	
    //public void Android_SendLanguageToUnity (string language){
    //    Debug.Log ("current android system language is " + language);
    //    SetLanguage (language);
    //}
	
    //void SetLanguage (string systemLanguage){
    //    string language = "English";
    //    switch (systemLanguage) {
    //    case "zh":
    //        language = "Chinese";
    //        break;
    //    case "en":
    //        language = "English";
    //        break;
    //    case "ja":
    //        language = "Japanese";
    //        break;
    //    case "fr":
    //        language = "French";
    //        break;
    //    case "de":
    //        language = "German";
    //        break;
    //    case "it":
    //        language = "Italian";
    //        break;
    //    case "es":
    //        language = "Spanish";
    //        break;
    //    default:
    //        language = "Enlish";
    //        break;
    //    }
    //    Localization.language = language;
    //}


}
