using UnityEngine;
using System.Collections;

public class QuitAlert : MonoBehaviour
{
    private System.Action ConfirmCallBack = null;
    private static QuitAlert _instance = null;
    private static GameObject _instanceGameObj = null;
    private const float mlimitTime = 2f;
    private float mlastTime = 0f;

    void Update()
    {
        mlastTime += Time.deltaTime;
        if (mlastTime > mlimitTime)
        {
            Clear();
        }
    }

    public static void Show(System.Action callback)
    {
        if (_instance == null)
        {
            CommonAlert.Clear();
            GameObject go = Instantiate(Resources.Load("UI/QuitAlert")) as GameObject;
            Transform tran = go.transform;
            _instanceGameObj = go;
            //tran.parent = root;
            _instance = go.GetComponent<QuitAlert>();
            _instance.ConfirmCallBack = callback;
            _instance.transform.localPosition = new Vector3(0f,0f,2.95f);
            _instance.transform.localScale = 0.00278f * Vector3.one;
            BgTipsView.Show();
            //SetPosition(Vector3.one,1f);
        }
        else if (_instance.ConfirmCallBack != null)
        {
            _instance.ConfirmCallBack();
            QuitAlert.Clear();
        }
    }

    //public static void SetPosition(Vector3 pos, float scale)
    //{
    //    if (_instance != null)
    //    {
    //        _instance.transform.localPosition = pos;
    //        _instance.transform.localScale = scale * Vector3.one;
    //    }
    //}

    public void Reset()
    {
        mlastTime = 0f;
    }

    public static void Clear()
    {
        if (_instanceGameObj != null)
        {
            _instance = null;
            Destroy(_instanceGameObj);
        }
    }
}
