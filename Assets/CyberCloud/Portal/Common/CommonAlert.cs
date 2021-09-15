using UnityEngine;
using System.Collections;

public class CommonAlert : MonoBehaviour
{
    [SerializeField]
    UILabel mAllertTip;

    private const float mlimitTime = 2f;
    private float mlastTime = 0f;
    private bool isPersist = false;
    private static GameObject instance = null;
    private static string CurKey = string.Empty;

    void Update()
    {
        if (isPersist)
        {
            return;
        }
        mlastTime += Time.deltaTime;
        if (mlastTime > mlimitTime)
        {
            Clear();
        }
    }

    public static CommonAlert Show(string key, bool ispersist = false, Transform root = null, bool isShowBG = true)
    {
        if (instance != null && CurKey.Equals(key))
        {
            Debug.Log("already exist!");
            return null;
        }
        Clear();
        QuitAlert.Clear();
        GameObject go = Instantiate(Resources.Load("UI/CommonAlert")) as GameObject;
        instance = go;
        Transform tran = go.transform;
        CommonAlert alert = go.GetComponent<CommonAlert>(); ;
        if (root != null)
        {
            tran.parent = root;
        }
        tran.localPosition = new Vector3(0f, 0f, 3.0f);
        tran.localScale = 0.00278f * Vector3.one;
        //alert.SetPosition(Vector3.one, 1f);
        alert.SetAllertTip(key);
        alert.isPersist = ispersist;
        if(isShowBG)
        {
            BgTipsView.Show();
        }
        return alert;
    }

    public void SetPosition(Vector3 pos, float scale)
    {
        transform.localPosition = pos;
        transform.localScale = scale * Vector3.one;
    }

    public void SetAllertTip(string key)
    {
        mAllertTip.text = Localization.Get(key);
        CurKey = key;
        if (key.Equals("Home_NoNet"))
            mAllertTip.color = new Color(255,147,0);
    }

    public static void Clear()
    {
        if (instance != null)
        {
            Destroy(instance);
            CurKey = string.Empty;
        }
    }
}