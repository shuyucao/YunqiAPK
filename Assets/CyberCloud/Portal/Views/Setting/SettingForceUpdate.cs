using UnityEngine;
using System.Collections;

public class SettingForceUpdate : MonoBehaviour {
    [SerializeField]
    GameObject mUpdateOK;
    [SerializeField]
    GameObject mUpadateCancel;

    // Use this for initialization
    void Start () {
#if UNITY_EDITOR
#elif UNITY_ANDROID
#endif
        if (mUpdateOK != null)
            UIEventListener.Get(mUpdateOK).onClick = OnButtonClick;
        else
            Debug.Log("mUpdateOK is not found !!!");

        if (mUpadateCancel != null)
            UIEventListener.Get(mUpadateCancel).onClick = OnButtonClick;
        else
            Debug.Log("mUpadateCancel is not found !!!");
    }

    // Update is called once per frame
    void Update () {
	
	}

    void OnButtonClick(GameObject obj)
    {
        Debug.Log("OnButtonClick : " + obj.name);
        if (obj == mUpdateOK)
        {
            MsgManager.Instance.SendMsg(MsgID.SettingUpdating,null);
            GalleryTools.ShowLeftBar(false);
        }
        if (obj == mUpadateCancel)
        {
            Application.Quit();
        }
    }
}
