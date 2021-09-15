using UnityEngine;
using System.Collections;

public class PlayerToast : MonoBehaviour {

    public UILabel mLabel;
    private Coroutine mLastCoroutine = null;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    public void Show(string value, float time = 3.0f)
    {
        if (mLastCoroutine != null)
        {
            StopCoroutine(mLastCoroutine);
            mLastCoroutine = null;
        }
      

        this.gameObject.SetActive(true);
        mLabel.text = value;

        //如果time < 0就让提示一直显示不消失
        if (time < 0)
        {
            return;
        }
        mLastCoroutine = StartCoroutine(DestoryToast(time));
    }

    IEnumerator DestoryToast(float time)
    {
        //Debug.Log( "DestoryToast" );
        yield return new WaitForSeconds(time);
        this.gameObject.SetActive(false);
    }
}
