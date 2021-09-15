using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

static class WingUIExtensions {

    public static GameObject GetNGUIComponentByID(this GameObject root, string id)
        {
            string[] arr = id.Split(':');
            string path = string.Empty;
            if (arr[0] == root.name)
            {
                if (arr.Length == 1)
                    return root;
                path = string.Join("/", arr, 1, arr.Length - 1);
                Transform tt = root.transform.Find(path);
                if (tt != null)
                {
                    return tt.gameObject;
                }
                return null;
            }
            else
            {
                path = string.Join("/", arr);
            }
            Transform t = getChildByPath(root.transform, path);
            if (t != null)
                return t.gameObject;
            return null;
        }

    private static Transform getChildByPath(Transform t, string path)
    {
        Transform tt = t.Find(path);
        if (tt != null)
        {
            return tt;
        }
        int len = t.childCount;
        for (int i = 0; i < len; i++)
        {
            Transform ct = t.GetChild(i);
            tt = ct.Find(path);
            if (tt != null)
            {
                return tt;
            }
            tt = getChildByPath(ct, path);
            if (tt != null)
                return tt;
        }
        return null;
    }

    private static bool checkPathMatch(Transform t, string path)
    {
        List<string> idList = new List<string>(path.Split(':'));
        idList.Reverse();
        int len = idList.Count;
        Transform tmpTran = t;
        for (int i = 0; i < len; i++)
        {
            if (tmpTran.gameObject.name != idList[i])
                return false;
            tmpTran = tmpTran.parent;
        }
        return true;
    }

    public static GameObject FindActiveObjectByID(this GameObject root, string id)
        {
            if (!root.activeInHierarchy)
                return null;

            string[] pathIDs = id.Split(':');
            if (pathIDs[0] == root.name)
            {
                if (pathIDs.Length == 1)
                {
                    return root.activeSelf ? root : null;
                }

                string[] findPathIDs = new string[pathIDs.Length - 1];
                Array.Copy(pathIDs, 1, findPathIDs, 0, pathIDs.Length - 1);

                return FindActiveChild(root.transform, findPathIDs);
            }
            else
            {
                return FindActiveChild(root.transform, pathIDs);
            }
        }

    private static GameObject FindActiveChild(Transform root, string[] pathIDs, int currentIndex = 0)
    {
        int childCount = root.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = root.GetChild(i);
            if (child.gameObject.activeSelf)
            {
                int searchIndex = currentIndex;
                if (child.name == pathIDs[currentIndex])
                {
                    if (currentIndex == pathIDs.Length - 1)
                    {
                        return child.gameObject;
                    }
                    searchIndex = currentIndex + 1;
                }

                GameObject gameObject = FindActiveChild(child, pathIDs, searchIndex);
                if (gameObject != null)
                {
                    return gameObject;
                }
            }
        }
        return null;
    }

    public static void Reset(this Transform transform)
    {
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        transform.localPosition = Vector3.zero;
    }
}
