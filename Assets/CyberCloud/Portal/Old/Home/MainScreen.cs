using UnityEngine;
using System.Collections;

public class MainScreen : WingScreenBase {

    //public LauncherButton userCenter;
    //public LauncherButton movies;
    //public LauncherButton games;
    //public GameObject toast;
    //private bool cancelClick = false;
    //private float clickTime;
    //void OnBack()
    //{
    //    if (!userCenter.istransition && !movies.istransition && !games.istransition && Visible)
    //    {
    //        if (cancelClick)
    //        {
    //            cancelClick = false;
    //            if (Time.time - clickTime < 2.0f)
    //            {
    //                WingScreenManager.Instance.PopScreen();
    //                PicoUnityActivity.CallObjectMethod("exitFromPico");
    //            }
    //        }
    //        else
    //        {
    //            clickTime = Time.time;
    //            cancelClick = true;
    //            WingToastManager.Instance.Show(Localization.Get("Launcher_Exit_message"));
    //        }
    //    }
    //}

    //public override void Show(object param)
    //{
    //    base.Show(param);
    //    Debug.Log(System.Convert.ToInt32(param));
    //    InputManager.OnBack += OnBack;
    //    int value = PlayerPrefs.GetInt("gaze");
    //    toast.SetActive(value == 0);
    //    if (value == 0) PlayerPrefs.SetInt("gaze", 1);
    //}
    //public override void Destroy()
    //{
    //    base.Destroy();
    //    InputManager.OnBack -= OnBack;
    //}

    //void OnDisable()
    //{
    //    toast.SetActive(false);
    //}
}
