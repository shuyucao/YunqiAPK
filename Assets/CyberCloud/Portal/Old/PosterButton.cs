using UnityEngine;
using System.Collections;

public class PosterButton : MonoBehaviour
{
    [HideInInspector]
    public Texture mDefaultTexture = null;
    private GameObject mScrollPanel = null;
    public GameObject mBG;
    public Vector3 OriginPos;

    void Start()
    {
        //OriginPos = transform.localPosition;
        if (mDefaultTexture == null)
        {
            GameObject textureObject = this.gameObject.transform.Find("texture").gameObject;
            if (textureObject != null)
            {
                mDefaultTexture = textureObject.GetComponent<UITexture>().mainTexture;
            }
        }
        UIEventListener.Get(this.gameObject).onClick += OnButtonClick;
        UIEventListener.Get(this.gameObject).onHover += OnButtonHover;
        mScrollPanel = GameObject.Find("Scroll View").transform.GetChild(0).gameObject;
    }

    void OnButtonClick(GameObject obj)
    {
        Debug.Log("OnClick : " + obj.name);
        GlobalPhotoData.Instance.mCurrentPhotoIndex = int.Parse(obj.name.Split('_')[1]);
        if (mScrollPanel != null)
        {
            mScrollPanel.SendMessage("UpdateItemPick", this.gameObject, SendMessageOptions.DontRequireReceiver);
        }
        StartCoroutine(StartPlayer(null));
    }

    void OnButtonHover(GameObject obj, bool status)
    {
        mBG.SetActive(status);
        Vector3 pos = OriginPos;
        float x = Mathf.Sin(obj.transform.localRotation.eulerAngles.y * Mathf.Deg2Rad) * 3;
        obj.transform.localPosition = new Vector3(status ? pos.x - x : pos.x, pos.y, status ? pos.z - 3 : pos.z);
    }

    IEnumerator StartPlayer(string param)
    {
        Home.levelIndex = 2;
        Application.LoadLevel(0);
        yield return 0;
    }
}