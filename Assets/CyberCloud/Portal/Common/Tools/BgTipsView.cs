using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BgTipsView : MonoBehaviour {

    static List<BgTipsView> instanceList = new List<BgTipsView>();
    static Transform rootTran;
    static BgTipsView instance;


    public static void Show()
    {
        if (rootTran == null)
        {
              rootTran = FindObjectOfType<UICamera>().transform;
        }
        instance = null;
        if (instance == null)
        {
            if (instanceList.Count > 0)
            {
                instance = instanceList[0];
                instanceList.RemoveAt(0);
            }
            else
            {
                GameObject go = Resources.Load("UI/BlackBgPanel") as GameObject;
                GameObject obj = Instantiate(go);
                obj.transform.parent = rootTran;
                obj.gameObject.SetActive(false);
                instance = obj.GetComponent<BgTipsView>();
                //instance.gameObject.transform.localScale = Vector3.one;
                instance.gameObject.transform.localEulerAngles = Vector3.zero;
                instance.gameObject.transform.localPosition = new Vector3(0f, 0f, 1f);
            }

            instance.transform.localEulerAngles = Vector3.zero;
            //instance.transform.localPosition = Vector3.zero;
            instance.GetComponent<UIPanel>().alpha = 1.0f;
            instance.gameObject.SetActive(true);
            instance.StartCoroutine(instance.HideTips(2f));
            if (instance == null)
            {
                Debug.LogError("bgTipsView is null");
            }

        }

    }


    IEnumerator HideTips(float dua)
    {
        yield return new WaitForSeconds(dua);
        TweenAlpha.Begin(gameObject, 1f, 0f);
        StartCoroutine(AddInstanceList(1.01f));

    }
    IEnumerator AddInstanceList(float dua)
    {
        yield return new WaitForSeconds(dua);
        if (!instanceList.Contains(this))
        {
            gameObject.SetActive(false);
            instanceList.Add(this);
            instance = null;
        }
        if (instanceList.Count > 3)
        {
            for (int i = 0; i < instanceList.Count - 3; i++)
            {
                GameObject go = instanceList[0].gameObject;
                instanceList.RemoveAt(0);
                Destroy(go);
                i--;
            }
        }
    }
}
