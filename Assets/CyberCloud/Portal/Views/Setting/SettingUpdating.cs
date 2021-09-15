using UnityEngine;
using System.Collections;

public class SettingUpdating : MonoBehaviour {
    [SerializeField]
    UILabel mTitle;
    [SerializeField]
    UILabel mNumber;
    int i = 0;
    // Use this for initialization
    void Start () {
        Debug.Log("SettingUpdating  Start  !!!!! ");
        GalleryActivity.Instance.StartUpdate();
        setLabel();
    }

    void setLabel()
    {
        mNumber.text = "0%";
    }

    public void RefreshLable(string str)
    {
        Debug.Log("RefreshLable   str  is "+ str);
        mNumber.text = str + "%";


    }

    public void InstallSilent(string str)
    {
        if (str.Equals("Error"))
        {
            Debug.Log("download failed！！！，response: " + str);

        }
        else if (str.Equals("Success"))
        {
            Debug.Log("download sucess response: " + str);
            mTitle.text = Localization.Get("Installing");
            GalleryActivity.Instance.InstallSilent();
        }
        else
        {
            Debug.LogError("Error！！！，response: " + str);
        }
    }

}
