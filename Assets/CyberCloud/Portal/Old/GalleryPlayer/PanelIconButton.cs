using UnityEngine;
using System.Collections;

public class PanelIconButton : MonoBehaviour {

    [HideInInspector]
    public Texture mDefaultTexture = null;
    public GameObject mBg;
    public GameObject mSelect;
    private string _mid;
    private int _listIndex;
    public Vector3 OriginPos;

    public string MID
    {
        get {
            return _mid;
        }
        set {
            _mid = value;
        }
    }

    public int ListIndex
    {
        get
        {
            return _listIndex;
        }
        set
        {
            _listIndex = value;
        }
    }


	// Use this for initialization
	void Start () {

        if (mDefaultTexture == null)
        {
            mDefaultTexture = this.gameObject.GetComponent<UITexture>().mainTexture;
        }
	}

    public void SetSelectActive(bool value)
    {
        mSelect.SetActive(value);
        if (_listIndex != GlobalPhotoData.Instance.mCurrentPhotoIndex)
        {
            mBg.SetActive(!value);
        }
    }

    // Update is called once per frame
    void Update () {
	
	}
}
