using UnityEngine;
using System.Collections;
using System;

public class WingToastManager : Singleton<WingToastManager>
{
    public void Show(string value, float time = 3.0f)
    {
        if (string.IsNullOrEmpty(value)) {

            Debug.LogError("value can't be null or empty");
            return;
        }
        GameObject Root = GameObject.Find("UI Root");
        if (Root == null) {

            Debug.LogError("UI Root not exist");
            return;
        }
        GameObject toast = Root.GetNGUIComponentByID("ToastScreen");
        if (toast != null)
        {
            ToastScreen screen = toast.GetComponent<ToastScreen>();
            screen.Show(value, time);
        }
        else {

            //string path = "Prefabs/ToastScreen";
            string path = "UI/ToastScreen";
            GameObject prefab = (GameObject)Resources.Load(path, typeof( GameObject ))  as GameObject;
            if (prefab == null)
            {
                throw new System.ArgumentException("toast no found at path ");
            }
            toast = (GameObject)GameObject.Instantiate(prefab);
            toast.transform.parent = Root.transform;
            toast.SetActive(true);
            toast.transform.Reset();
            toast.name = "ToastScreen";
            ToastScreen screen = toast.GetComponent<ToastScreen>();
            screen.Show(value, time);
        }
    }
    public void Hide()
    {
        GameObject Root = GameObject.Find("UI Root");
        if (Root == null)
        {
            Debug.LogError("UI Root not exist");
            return;
        }
        GameObject toast = Root.GetNGUIComponentByID("ToastScreen");
        if (toast != null)
        {
            toast.SetActive(false);
        }
    }

    public void ShowConfirmScreen(string value)
    {
        if (string.IsNullOrEmpty(value))
        {

            Debug.LogError("value can't be null or empty");
            return;
        }
        GameObject Root = GameObject.Find("UI Root");
        if (Root == null)
        {
            Debug.LogError("UI Root not exist");
            return;
        }
        GameObject toast = Root.GetNGUIComponentByID("ConfirmScreen");
        if (toast != null)
        {
            ConfirmScreen screen = toast.GetComponent<ConfirmScreen>();
            screen.Show(value);
        }
        else
        {
            string path = "UI/ConfirmScreen";
            GameObject prefab = (GameObject)Resources.Load(path, typeof(GameObject)) as GameObject;
            if (prefab == null)
            {
                throw new System.ArgumentException("toast no found at path ");
            }
            toast = (GameObject)GameObject.Instantiate(prefab);
            toast.transform.parent = Root.transform;
            toast.SetActive(true);
            toast.transform.Reset();
            toast.name = "ConfirmScreen";
            ConfirmScreen screen = toast.GetComponent<ConfirmScreen>();
            screen.Show(value);
        }
    }

    ///// <summary>
    ///// Shows add favorite ok and increase score value toast.
    ///// </summary>
    ///// <param name="number">Number. the increase score</param>
    //public void ShowFavoriteIncreaseScore(int number)
    //{
    //    string showText = Constant.TOAST_ADD_FAVORITE_OK + ",+" + number;
    //    Show(showText);
    //}

    //public void Show(int msgType)
    //{
    //    string loadingTextStr = null;

    //    switch (msgType)
    //    {
    //        case Constant.MSG_NONETWORK:
    //            loadingTextStr = Constant.TOAST_NO_NETWORK;
    //            break;
    //        case Constant.MSG_NO_MOREDATA:
    //            loadingTextStr = Constant.TOAST_NO_MOREDATA;
    //            break;
    //        case Constant.MSG_NO_LOGIN:
    //            loadingTextStr = Constant.TOAST_NO_LOGIN;
    //            break;
    //        case Constant.MSG_NO_HISTORYVIDEO:
    //            loadingTextStr = Constant.TOAST_NO_HISTORY_VIDEO;
    //            break;
    //        case Constant.MSG_NO_COLLECTVIDEO:
    //            loadingTextStr = Constant.TOAST_NO_COLLECT_VIDEO;
    //            break;
    //        case Constant.MSG_NO_DOWNLOADVIDEO:
    //            loadingTextStr = Constant.TOAST_NO_CACHE_VIDEO;
    //            break;
    //        case Constant.MSG_NO_VIDEO_IN_SERVER:
    //            loadingTextStr = Constant.TOAST_NO_VIDEO;
    //            break;
    //        case Constant.MSG_PAUSE_JSON_ERROR:
    //            loadingTextStr = Constant.TOAST_PAUSE_JSON_ERROR;
    //            break;
    //        case Constant.MSG_NO_LOCALVIDEO:
    //            loadingTextStr = Constant.TOAST_NO_LOCALVIDEO;
    //            break;
    //        case Constant.MSG_IS_IN_FIRSTPAGE:
    //            loadingTextStr = Constant.TOAST_Current_is_FisrstPage;
    //            break;
    //        case Constant.MSG_ADD_FAVORITE_OK:
    //            loadingTextStr = Constant.TOAST_ADD_FAVORITE_OK;
    //            break;
    //        case Constant.MSG_ADD_FAVORITE_FAILED:
    //            loadingTextStr = Constant.TOAST_ADD_FAVORITE_FAILED;
    //            break;
    //        case Constant.MSG_ADD_DOWNLOAD_OK:
    //            loadingTextStr = Constant.TOAST_ADD_DOWNLOAD_OK;
    //            break;
    //        case Constant.MSG_ADD_DOWNLOAD_FAILED:
    //            loadingTextStr = Constant.TOAST_ADD_DOWNLOAD_FAILED;
    //            break;
    //        case Constant.MSG_DEL_FAVORITE_OK:
    //            loadingTextStr = Constant.TOAST_DEL_FAVORITE_OK;
    //            break;
    //        case Constant.MSG_DEL_FAVORITE_FAILED:
    //            loadingTextStr = Constant.TOAST_DEL_FAVORITE_FAILED;
    //            break;
    //        case Constant.MSG_FILE_NOT_EXIST:
    //            loadingTextStr = Constant.TOAST_FILE_NOT_EXIST;
    //            break;
    //        case Constant.MSG_CONNECT_TIMEOUT:
    //            loadingTextStr = Constant.TOAST_CONNECT_TIMEOUT;
    //            break;
    //        case Constant.MSG_TIMEOUT:
    //            loadingTextStr = Constant.TOAST_TIMEOUT;
    //            break;
    //        default:
    //            loadingTextStr = Constant.TOAST_UNKNOWN_ERROR;
    //            Debug.LogError("Unknown msg type: " + msgType);
    //            break;
    //    }

    //    Show(loadingTextStr);
    //}
}
