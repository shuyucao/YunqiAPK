using System.Collections.Generic;
using UnityEngine;

public class UnityTools
{
    #region set active tools
    private static Vector3 mHidePos = new Vector3(0, 0, 100000);
    private static Dictionary<Transform, Vector3> mLocalPosDict = new Dictionary<Transform, Vector3>();

    public static void SetActive(Transform tran, bool isactive)
    {
        if (isactive)
        {
            Show(tran);
        }
        else
        {
            Hide(tran);
        }
    }

    public static bool IsActive(Transform tran)
    {
        if (tran != null && mLocalPosDict.ContainsKey(tran) && Mathf.Abs((tran.localPosition.z - mHidePos.z)) < float.Epsilon)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private static void Hide(Transform tran)
    {
        if (tran == null)
        {
            //Debug.LogError("cannot hide it ,the transform is null!");
            return;
        }
        if (mLocalPosDict.ContainsKey(tran))
        {
            if (!tran.localPosition.Equals(mHidePos))
            {
                mLocalPosDict.Remove(tran);
                mLocalPosDict.Add(tran, tran.localPosition);
            }
        }
        else
        {
            mLocalPosDict.Add(tran, tran.localPosition);
        }
        //Debug.Log("origin pos:"+tran.localPosition.z.ToString());
        tran.localPosition = mHidePos;
    }

    private static void Show(Transform tran)
    {
        if (tran == null)
        {
            Debug.LogError("cannot show it ,the transform is null!");
            return;
        }
        Vector3 pos;
        if (mLocalPosDict.TryGetValue(tran, out pos))
        {
            tran.localPosition = pos;
            //mLocalPosDict.Remove(tran);
            //Debug.Log("show origin pos:" + pos.z.ToString());
        }
        else
        {
            //tran.localPosition = Vector3.zero;
            //Debug.LogError("does not find the origin pos!");
        }
    }
    #endregion

    #region create go tools
    public static T CreateComptent<T>(GameObject origin, Transform root, Vector3 localpos, Quaternion roatation, Vector3 scale, string name)
    {
        if (origin == null)
        {
            Debug.LogError("the origin is null or not a gameobj");
            return default(T);
        }
        GameObject go = GameObject.Instantiate(origin);
        go.SetActive(true);
        ResetTran(go.transform, root, localpos, roatation, scale, name);

        return go.GetComponent<T>();
    }

    public static T CreateComptent<T>(GameObject origin, Transform root)
    {
        return CreateComptent<T>(origin, root, Vector3.zero, Quaternion.Euler(Vector3.zero), Vector3.one, origin == null ? "" : origin.name);
    }

    public static void ResetTran(Transform origin, Transform root)
    {
        ResetTran(origin, root, Vector3.zero, Quaternion.Euler(Vector3.zero), Vector3.one);
    }

    public static void ResetTran(Transform origin, Transform root, Vector3 localpos, Quaternion roatation, Vector3 scale)
    {
        origin.parent = root;
        origin.localPosition = localpos;
        origin.localRotation = roatation;
        origin.localScale = scale;
    }

    public static void ResetTran(Transform origin, Transform root, Vector3 localpos, Quaternion roatation, Vector3 scale, string name)
    {
        ResetTran(origin, root, localpos, roatation, scale);
        origin.gameObject.name = name;
    }

    public static void ResetTran(Transform origin, Vector3 localpos)
    {
        origin.localPosition = localpos;
    }
    #endregion

    #region camera set
    public static void SetCameraBlack(bool isblack)
    {
        Camera left, right;
        left = GameObject.Find("LeftEye").GetComponent<Camera>();
        right = GameObject.Find("RightEye").GetComponent<Camera>();
        //Camera.main.clearFlags = isblack ? CameraClearFlags.Color : CameraClearFlags.Skybox;//PUI双眼模式用不到
        left.clearFlags = isblack ? CameraClearFlags.Color : CameraClearFlags.Skybox;
        right.clearFlags = isblack ? CameraClearFlags.Color : CameraClearFlags.Skybox;
        left.backgroundColor = isblack ? Color.black : Color.blue;
        right.backgroundColor = isblack ? Color.black : Color.blue;
    }
    #endregion

    #region SetUV
    public static void SetMirrorUV(GameObject go)
    {
        var mf = go.GetComponent<MeshFilter>();
        if (mf == null)
        {
            return;
        }
        var uv = mf.mesh.uv;
        for (int i = 0; i < uv.Length; i++)
        {
            uv[i].y = 1 - uv[i].y;
        }
        go.GetComponent<MeshFilter>().mesh.uv = uv;
    }
    #endregion
}